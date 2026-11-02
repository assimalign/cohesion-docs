# Sql catalog design

This page describes the catalog component inside `Assimalign.Cohesion.Database.Sql`.

> **Status:** Partial.

The SQL model's schema authority (area architecture: resources/`Database`/DESIGN.md
(`cohesion/docs/resources/Database/DESIGN.md`) `3.3). The catalog answers exactly two questions for
the planner: *what objects exist* (with stable identities) and *what shape are they* — and it must
answer identically after any crash.

Namespace: `Assimalign.Cohesion.Database.Sql.Catalog`. Assembly: `Assimalign.Cohesion.Database.Sql.dll`. NuGet package: `Assimalign.Cohesion.Database.Sql`.

This catalog component ships inside the model package. Keep its namespace
imports and replace the former satellite package reference with `Assimalign.Cohesion.Database.Sql`; recompile consumers.
The model enforces engine → Catalog → Storage → shared kernel through architecture tests.

## `Collation` metadata (#1025)

`SqlCatalogColumn.Collation` is an optional string-column override. Null inherits
`ISqlCatalog.DefaultCollation`, which is Binary when no default record exists. The default is
persisted in a dedicated kind-7 catalog record and captured in immutable statement snapshots.

`DefaultCollation` is read-only on the contract. The default is established when the catalog is
opened — `SqlCatalog.Open(storage, defaultCollation)` — and is fixed for the lifetime of the
database, because every index key on a column that inherited it is encoded through that collation's
byte transform. Opening an already-populated catalog under a different default is rejected;
reopening under the same one, or with no default supplied, keeps the persisted value.

Making this creation-time state rather than a mutator is deliberate: a setter on the contract would
tell every implementer the value is changeable and then guard that promise at runtime, which is the
kind of one-off bridging API the repo's abstraction rule exists to keep off interfaces.

Table metadata extension version 2 appends one collation identifier per column after the existing
constraints. Version-1 and pre-extension records still read with null column overrides and the
Binary database fallback. `Column` additions, drops, and restart preserve explicit overrides. An index
inherits its key columns' effective collations; the SQL engine verifies index eligibility and uses
those same transforms for uniqueness enforcement.

## Why-this-not-that decisions

- **A dedicated catalog storage file set** — not catalog rows mixed into the data
  file. The current row facade exposes one record space per storage instance;
  sharing it would put metadata records inside user table scans. A second
  `SqlStorage` (by convention `<database>.catalog`) buys complete isolation while
  reusing the exact same durability machinery — no bespoke metadata persistence.
  When the data file gains per-object record spaces, folding the catalog back in
  becomes an option, not a requirement.
- **Self-committing DDL.** Every catalog mutation runs in its own storage
  transaction and is durable (WAL commit record) when the call returns. DDL inside
  a user DML transaction is deliberately unsupported in the MVP: the in-memory
  cache updates on commit, and half-visible schema changes are a correctness trap
  (historically mishandled by real engines). The engine serializes DDL per
  database.
- **The shared tuple codec as the record format.** Catalog records are encoded
  with `DatabaseKeyWriter`/`DatabaseKeyReader` (#854): self-describing, exact
  round-trips, one codec maintained in one place. Ordering (the codec's other
  property) is irrelevant here — reuse beats a second bespoke format.
- **One record per table.** Columns and the primary key fold into the table's
  record: schema changes rewrite one record (in place when it fits, relocating —
  delete + insert — when it grows). Per-column records would buy nothing at this
  scale and cost multi-record consistency.
- **`Object` identities are catalog-assigned `ulong`s** persisted with a counter
  record, monotonic across reopen (the loader also raises the counter past every
  loaded table, so a torn counter update can never recycle an id). Data rows,
  index registrations, and lock resources key off these ids.
- **`Index` directory persistence lives here** — the index manager stays a physical
  component (`Database.Indexing`'s documented split): the catalog stores the
  exported `BTreeIndexRegistration` set and hands it back for re-attachment on
  open. `Root` page ids drift on splits; the engine re-saves at its persistence
  points (checkpoint/shutdown).
- **`Index` descriptions are schema metadata, one record per index (kind 5)** —
  `SqlCatalogIndex`: name (unique per table, case-insensitive), owning table
  object id, ordered key columns, uniqueness. The description is deliberately
  separate from the physical registration: the description is stable while root
  page ids drift, and the planner needs columns/uniqueness the registration
  doesn't carry. **Description and registration writes are atomic** —
  `CreateIndexAsync`/`DropIndexAsync` take the registration set and persist both
  records in one self-committing transaction, because a crash must never leave a
  description promising an index no tree backs (an unenforced UNIQUE) or a
  registration re-attaching a tree no statement can reach. `DropTableAsync`
  removes the table's descriptions and registrations the same way. Guards:
  dropping an indexed column is rejected (entries key on its values); index
  columns must exist at creation.
- **The record-space format version lives here** (kind-4 record,
  `RecordSpaceFormatVersion`): data rows are not self-describing across layout
  changes — a stamped (MVCC, version ≥ 2) record and an unstamped (version 1)
  record cannot be told apart record-by-record, and version 2 vs 3 (shared page
  stream vs per-object page chains) is a page-placement property no record
  carries — so the database-grain marker is catalog metadata, read by the
  engine at open to enforce format 30. Earlier or newer markers, including format 26, are
  refused without an upgrade reader. An absent marker does not authorize migration; only
  proven interrupted creation can complete under the engine's creation rules.
- **The applied compiled-schema state lives here** (kind-6 records). The catalog
  stores the lowercase content hash together with the complete canonical schema
  document. Documents are strict UTF-8 and chunked into bounded records; replacing
  all old chunks with all new chunks is one self-committing storage transaction,
  so reopen observes either complete state. The SQL provisioner verifies both the
  document/hash pair and the live table/index catalog before treating a repeated
  apply as a no-op; the marker is never an authority over detected drift.

## `Object` ownership

Table and secondary-index records persist `DatabaseObjectOwner` and the owning compiled schema's
name. `SqlCatalogTable.Schema` is strictly the SQL namespace (for example, `dbo`);
`SqlCatalogTable.OwningSchema` is the compiled schema that provisioned the object and is null for
ad-hoc objects. `SqlCatalogIndex` exposes the same ownership value as `OwningSchema`, distinct from
its table's SQL namespace. Ordinary catalog creation defaults to `Adhoc`; the SQL provisioner's
`SqlCatalog.ReserveTableAsync` /`SqlCatalog.PublishTableAsync` path stamps `Schema` ownership and
`OwningSchema` atomically with the first table record. `Index` creation already accepts a complete
description and persists the same metadata. `Column` add/drop replacements retain the table's
ownership unchanged.

The ownership fields are an appended tuple suffix. Older records without the suffix load as `Adhoc`
, preserving their existing mutability rather than guessing an owner from a database-wide schema
marker. Invalid owner/name combinations are rejected. The tuple encoding is positional and never
persisted CLR property identifiers; renaming the ownership property to `OwningSchema` therefore does
not change the on-disk format, and catalogs written by the contract-freeze build load without a
compatibility alias. The schema hash/document record remains unchanged and continues to support
drift detection independently of per-object ownership.

The engine enforces the live-session DDL lock; the catalog remains the durable metadata component
used by sanctioned schema application as well. Ownership metadata is accepted by the
staged-publication statics on `SqlCatalog`; authorization to change an existing schema-owned object
remains an engine/session decision. `ISqlCatalog` itself has no new member. The older
direct-creation helpers remain internal and are used only by catalog tests.

## `Constraint` persistence

Table records now append a versioned constraint extension after ownership: version `1`, constraint
count, and each immutable foreign-key/check definition. A reference stores its ordered local and
target columns, target SQL namespace/table, and `RESTRICT`, `CASCADE`, `NO ACTION`, `SET NULL` or `SET DEFAULT` delete action. A check
stores its SQL expression. Records ending after the original primary keys or ownership suffix still
load with no constraints. Unknown extension versions and malformed definitions fail closed. `Column`
changes retain constraints, and add/drop constraint rewrites use the same WAL-backed,
self-committing record replacement as existing catalog metadata.

`UNIQUE` uses `SqlCatalogIndex.IsUnique`, matching `CompiledSchemaIndex.IsUnique`; it does not
acquire a competing foreign-key/check constraint kind. Creation of a table with unique declarations
reserves and durably advances its object identity, then the engine commits the empty index trees
before publishing the table, constraints, index descriptions, and registrations in one catalog
transaction. A crash before publication leaves no visible table with missing enforcement.
Replacement publication similarly commits newly added columns/constraints and their new indexes
together. `SqlCatalog.ReserveTableAsync(ISqlCatalog, ...)` and
`SqlCatalog.PublishTableAsync(ISqlCatalog, ...)` expose this composition lifecycle as
`public static` methods that downcast to the internal implementation - the same bridge shape as the
existing `CreateTableAsync` /`AddConstraintAsync` helpers, and for the same reason: the lifecycle is
a capability of *this* catalog, not a contract every `ISqlCatalog` implementation must honour. A
reservation persists only the identity counter and does not lock the name; callers serialize DDL and
durably build enforcing indexes before publishing. `New` publications reject zero or unallocated
identities so they cannot bypass the durable identity counter. Replacement publication retains
existing index descriptions while adding the supplied new descriptions.
`SqlCatalog.DropConstraintAsync(ISqlCatalog, ...)` owns removal of persisted foreign-key/check
metadata, alongside the existing table, column, and index mutations on the interface.

`Index` records have their own version-`1` trailing extension carrying `IsPrimaryKey`. This
identifies the physical index enforcing primary-key metadata, allowing schema reconciliation to
distinguish it from a separately declared unique index on the same columns. Older index records have
no marker and load as ordinary indexes.

## Physical table replacement

[RewriteTableAsync](sql-database.md) reserves a fresh owner, copies live rows with dense physical
ordinals and rebuilds indexes before atomic catalog publication. Dropped ordinals stay associated
with the old owner for pinned snapshots. Logical schema, identity allocation, ownership and
dependent view identities survive. Prepared and retired owners use durable kind-14 reclamation
intents; recovery selects a complete old or replacement layout/index set and maintenance reclaims
abandoned storage. This reuses format 30 and the TRUNCATE replacement contract.

## Single source for SQL system views (C1)

The engine's `INFORMATION_SCHEMA` and `COHESION_SCHEMA` relations project this catalog's table,
column, key, index, constraint, and ownership descriptions. Those descriptions remain the single
source of truth: system views are computed when queried and never inserted as stored catalog tables
or copied into a second metadata store. `SqlCatalogTable` describes stored objects only and gains no
virtual/system flag. SQL view names, columns, binding, and row projection belong to the SQL engine,
not to this persistence library.

`SqlCatalog.CaptureSnapshot(ISqlCatalog)` returns an `ISqlCatalogSnapshot` containing an atomic
capture of tables, index descriptions, and default collation under the catalog's metadata lock.
`ISqlCatalogSnapshot` is public because it is the return type of a public static method; the
implementation stays internal; callers can retain the read-only capture without holding a storage
handle or disposing it. Table columns, primary-key columns, and index key-column names are copied
into read-only collections when descriptions are created; callers cannot mutate retained input lists
to alter a published table or an already captured directory. The SQL session captures it at
transaction begin for snapshot isolation and at statement start for `ReadCommitted` or auto-commit.
The engine derives every view row and referenced constraint from that capture, preventing an
enumeration from mixing metadata before and after a DDL publication. Captures expose existing
catalog descriptions, with no SQL view binding, query execution, or mutable persistence capability.
These `SqlCatalog` statics replace the shipped-to-shipped friend grant without widening
`ISqlCatalog`: consistent reads and staged publication are capabilities of this catalog
implementation, and putting them on the interface would make every future implementation owe four
more members. After a table drop, fresh snapshots contain none of its table, index, constraint,
column, or ownership metadata. See the SQL engine's
[virtual relation design](design.md#virtual-system-relations-c1)
for the query surface and the two deliberately non-standard extension views.

## Identity reservations

[SqlCatalogColumn.Identity](sql-catalog-column.md) retains immutable generation metadata in table
extension 8. Each identity owns a shared kind-18 sequence, with owning table id, column name and
identity flag; kind-19 reservations carry its durable endpoint and called state. Kind 16 is retired.
[AllocateIdentity](sql-catalog.md) delegates to this allocator and reserves up to 32 values before
returning any. Rollback/reopen can leave gaps; CYCLE or restart can deliberately repeat values.

Owned sequences publish with table creation, identity addition/removal, rename and storage
replacement. The generated table_column_seq name receives numeric collision suffixes and stays
unchanged on rename. Catalog load validates ownership in both directions, defaults and reservations
without cleanup writes; reclamation removes abandoned owner records later. INFORMATION_SCHEMA lists
identity sequences deliberately. See [identity behavior](../../../../database/sql/language/clauses/identity.md).

## Standalone sequences

Catalog sequence types and entry points remain internal. [SQL sequence DDL](../../../../database/sql/language/statements/create-sequence.md)
and [session functions](../../../../database/sql/language/functions/sequences.md) now execute.
Immutable definitions store object identity, schema/name and resolved
SMALLINT/INTEGER/BIGINT type, START, INCREMENT, MINVALUE, MAXVALUE, CACHE and CYCLE.
Defaults are BIGINT, increment 1, cache 1 and no cycle. Ascending defaults span 1 through the
type maximum; descending defaults span the type minimum through -1. START defaults to the
directional bound. Zero increment, nonpositive cache, unordered/out-of-type bounds or a
start/restart/current value outside the bounds use CS03069 (22023).

Sequences share the case-insensitive table/view relation namespace and the catalog object-id
counter; ids are never reused. Publication rechecks collisions, including CTAS and table rename.
Sequence creation against any occupied relation, or another relation colliding with an existing
sequence, uses CS03070 (42P07) and names the existing kind. Table-on-table collisions retain
CS03049 (42P07). An absent sequence namespace uses CS03071 (3F000). Schema RESTRICT counts
sequences; CASCADE removes definitions and counters atomically in the schema transaction.

Format 28 introduced kind 18 definitions and kind 19 reservations. Format 29 added owning
table id, column name and identity ownership to kind 18; kind 19 stores object id, durable endpoint
and a called flag. Retired kind 16 is never decoded. A false flag represents an
explicit restart before first use. Definitions exceeding 8,092 bytes use CS03072 (54000) before
identity reservation/publication. Invalid/truncated definitions, duplicate/orphaned counters,
namespace/name/id collisions or out-of-range endpoints use CS06061 (XX001) on load.
SqlCatalogSnapshot captures the internal sequence directory for INFORMATION_SCHEMA.SEQUENCES;
it adds no public sequence-list API. The relation uses the ordinary immutable catalog snapshot.

Create/alter/restart/drop atomically commit metadata and cursor changes under the catalog monitor.
Ordinary alter validates and retains the last fetched value, discards its unused reservation,
then continues with the new increment. Explicit restart may deliberately repeat values and
does not change START metadata. Dropped/replaced incarnation handles use CS04067 (40001).
See [shared reservations](storage-design.md#identity-and-sequence-reservations).

## CREATE TABLE AS publication

[CTAS](../../../../database/sql/language/statements/create-table.md#create-table-as) reserves a
hidden table identity with a durable kind-14 abandonment intent, using the same prepared-owner
reclamation mechanism as TRUNCATE. One builder transaction writes all rows before catalog
publication removes the intent and exposes the table. Recovery retains either the whole table or
an abandoned owner to reclaim, with no partial table. A lost name race uses CS03049 (42P07), or a
zero-row no-op with IF NOT EXISTS. The protocol reuses SQL format 30.

## Error model

TRUNCATE preserves catalog definitions/ownership while publishing fresh storage owners for the
complete table set. Retired rows/indexes and their view bindings stay available to older snapshots;
reclamation waits for their transaction and statement pins. CONTINUE moves the exact volatile
identity cursor/block; RESTART resumes at the declared START. See
[TRUNCATE](../../../../database/sql/language/statements/truncate.md).

Its RESTRICT closure refusal uses CS03067 (0A000); concurrent closure/identity changes use retryable
CS04064 (40001). Explicit-transaction admission uses CS04066 (25001) and retains prior work.

`SqlCatalogException : DatabaseException` for catalog violations (duplicate or missing
tables/columns, primary-key drops, malformed persisted records).

Catalog DDL guards now carry stable diagnostics separately from Message: duplicate table/index
CS03049 (42P07), duplicate constraint/non-function catalog definition CS03050 (42710), missing
table CS01051 (42P01), missing object CS01052 (42704), duplicate column CS03061 (42701)
and stale rename CS04064 (40001). Size/generation exhaustion uses CS05042 (54000).
Session execution attaches the current statement span without changing the concrete catalog
exception type. Direct catalog calls have no statement text. See the [complete code reference](../../../../database/sql/language/diagnostics.md).

## Non-goals

- **Sequence permissions** — Identity integration, SERIAL, OWNED BY and sequence defaults execute; permissions remain future work, #177.
- **Multi-statement DDL atomicity (see** — self-committing DDL above). The migration
  layer compensates completed reversible statements on failure; an MVCC bracket
  spanning catalog and data DDL requires a future catalog batch-transaction seam.

## AOT posture

Value/data classes plus the shared codec — no reflection, no serialization libraries.


## Renames and frozen missing values

[SqlCatalog.RenameTableAsync and RenameColumnAsync](sql-catalog.md#rename-members) require the
current immutable table and canonical CHECK text supplied by the engine. One transaction carries
incoming/self foreign keys, primary keys and index column lists, preserving IDs, ordinals and trees.
Generated names stay unchanged. Stale definitions/collisions or incomplete CHECK metadata fail closed.

[SqlCatalogColumn.MissingValueLiteral](sql-catalog-column.md) is captured by ADD COLUMN and is
independent of DefaultLiteral for future inserts. Table extension 6 introduced one optional frozen
literal per live column after identity facets. Format 17 introduced this split; format 18 separately
changes persisted canonical function-name quoting. SET/DROP DEFAULT cannot change old missing reads.

## Current format boundary

SQL namespaces are independent persisted [SqlCatalogNamespace](sql-catalog-namespace.md) records
of kind 15. [SqlCatalog](sql-catalog.md) exposes namespace create/drop and receipt overloads;
[SqlCatalogSnapshot.Namespaces](sql-catalog-snapshot.md) includes empty namespaces. Publication
captures namespaces with tables/indexes, and CASCADE removes contents and incoming FKs atomically
while preserving outside tables. The SQL executor owns DDL locks and ownership/protection checks.
SQL database creation seeds dbo; low-level catalog open does not mutate or seed records.

The integration branch uses SQL row-storage format 30 and compiled schema version 9.
Format 17 introduced the insert-default/frozen-missing-value split (table extension 6); format 18
separately changes quoting for newly executable function names in persisted canonical SQL.
The formats preserve resolved shared type identity/modifiers across contextual multi-word and
SQL Server lexical aliases. Typed temporal, interval and binary defaults use canonical literal
text. These unreleased formats have no older-format compatibility reader; use this current
boundary when interpreting historical layout notes above.

Format 19 quotes PERCENT and TIES in persisted SQL. Format 20 adds persisted SET NULL,
SET DEFAULT and NO ACTION delete actions. Format 21 introduced namespace records of kind 15. Format 22 quotes STRING_AGG and LISTAGG
identifiers in persisted SQL. Format 23 introduced catalog views and format 24 explicit NULL
placement. Format 25 added distinct predicates, Boolean tests and LIKE ESCAPE.
Format 26 fenced temporal DEFAULT and frozen missing-value text.
Format 29 added owned-sequence metadata. Current format 30 adds ON UPDATE action metadata
in table extension 8 and retains TYPE default assignment coercion.
Every earlier format is refused at open; there is no
upgrade path. Compiled schema format remains version 9.

## Persisted views

Format 29 added owned-sequence metadata. Current format 30 adds ON UPDATE action metadata
in table extension 8 and retains TYPE default assignment coercion. Format 29 and
every earlier catalog are refused before definitions bind, carrying CS06041; they are never
silently rewritten. Format 26 introduced the temporal DEFAULT/frozen missing-value fence.
Compiled documents must be v9. See the [format and rebuild procedure](../../../../database/sql/index.md#sql-format-30-and-schema-v9).

[SqlCatalogView](sql-catalog-view.md) stores canonical bodies, nullable frozen columns and resolved
dependencies in kind 17 records introduced by format 23. Current format is 30. Publication checks
collisions, live dependencies, cycles and append-only compatible replacement atomically; records
larger than 8,092 bytes use CS03033. DROP RESTRICT protects dependents; CASCADE removes dependent
views. Table/function/domain/column changes independently protect view dependencies with CS03302.
Snapshots capture immutable view definitions alongside the rest of the catalog.

## Owned sequence diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS03078 | ADD GENERATED requires NOT NULL | 55000 |
| CS03079 | Identity required by ALTER is absent | 55000 |
| CS03080 | Sequence required by identity or column default | 2BP01 |
| CS03081 | Invalid OWNED BY target or identity ownership change | 55000 |

Allocator exhaustion uses CS02002 (22003); disappearing identity or unplanned live referential
default sequence grants use CS04067 (40001). Retry replans before defaults or row mutation.

## Sources

- **Sequence definitions, namespace and validation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Catalog/SqlCatalog.Sequences.cs`, `src/Internal/Catalog/SqlSequenceOptions.cs` and `tests/Catalog/SqlCatalogTests.Sequences.cs`.

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Assimalign.Cohesion.Database.Sql.csproj`.
