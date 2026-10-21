# Assimalign.Cohesion.Database.Documents design

This page describes the boundaries and implementation shape of `Assimalign.Cohesion.Database.Documents`.

> **Status:** Partial.

## Model component boundaries

Catalog, Storage and the empty Security satellite are now part of `Assimalign.Cohesion.Database.Documents`.
Public Catalog and Storage types retain their namespaces and members; they ship in the model
assembly and NuGet package. Replace satellite references with the model package and recompile.
Assembly-qualified type names change, with no compatibility assembly or type forwarders.

The direction is engine → Catalog → Storage → shared kernel. Public component types live in
`src/Catalog/` and `src/Storage/`; internal types live in `src/Internal/Catalog/`
and `src/Internal/Storage/`. Engine persistence strategies and catalog-aware codecs belong
in `src/Internal/Engine/Storage/`. Transaction record-space adapters belong to Storage.

Roslyn architecture tests check actual compiler inputs and resolved symbols: Storage cannot use
Catalog or engine types; Catalog cannot use engine types. Storage permits shared Storage,
Transactions and Indexing plus the BCL reference pack. Catalog additionally permits the
area root and Types. Protocol, Language, Schema and Connections are disallowed in both constrained
layers; engine code has no external assembly restriction.

Wire messages, codecs and transfer values now belong to [Documents.Protocol](../assimalign-cohesion-database-documents-protocol/index.md).
Engine and client reference those same contracts. Moved wire types change namespace as well as
assembly identity; update their imports. Wire and disk formats are unchanged.

## String comparison and collation (#1025)

Documents remains ordinal-only. OQL string predicates, ordering, grouping, and distinct comparisons
use case-sensitive .NET ordinal (UTF-16 code-unit) order. String index keys encode that same order,
including supplementary characters; they do not inherit SQL's UTF-8 collation transforms. Collection
names, document IDs, JSON property names, and prefix listings are also ordinal and case-sensitive.
Administrative database lookup remains ordinal-ignore-case. SQL database defaults, column
collations, and expression `COLLATE` have no effect on this model. Configurable document collation
is deferred; stored text retains its original form.

## Intent and composition

Documents combines the Blob engine's lifecycle, session transaction, worker, and stamped chunk
discipline with SQL's parse, logical planning, physical planning, and execution split. `Storage`,
journaling, paging, locks, MVCC, and B+Tree algorithms belong to the shared kernel. No kernel
contract or existing public interface was widened. The model adds internal implementations of the
frozen document contracts.

Every OQL statement flows through these stages. Catalog metadata informs query access-path selection
and index-DDL planning; `SELECT`, `CREATE INDEX`, and `DROP INDEX` all reach the same plan
executor rather than an index-management side channel.

```mermaid
flowchart TD
    Text["OQL text"] --> Parse["OqlQueryParser and profile"]
    Parse --> Ast["OqlQueryStatement and diagnostics"]
    Ast --> Logical["Query semantics or index-DDL validation"]
    Logical --> Physical["DocumentPlanner access path or catalog operation"]
    Catalog["Collection and index catalog"] --> Physical
    Physical --> Execute["DocumentPlanExecutor"]
    Data["Snapshot-visible documents"] --> Execute
    Execute --> Result["Query rows or DDL command result"]
```

| Component or assembly | Responsibility |
| --- | --- |
| `Database.Documents` | Engine, bound sessions, CRUD, OQL query/index-DDL plans and execution, server and builder extension |
| `Database.Documents.Language` | Profile, parser, AST, stable diagnostic locations |
| Catalog component in `Database.Documents` | Versioned collection/document/index metadata and transactional index maintenance |
| Storage component in `Database.Documents` | Explicit JSON validation, stamped metadata/chunk records, kernel record-space adapter |
| `Database` and child roots | Shared contracts, transaction coordinator, lock manager, WAL, pages, B+Tree |

The engine references Documents.Language, Documents.Protocol and the Database root. Its Catalog
component uses Documents Storage and shared indexing/transactions, never engine types. There are no Hosting or
ApplicationModel references.

## Sessions and authority

Explicit commit and rollback observe cancellation only before their end starts, leaving the
transaction usable when canceled then. A started commit ignores caller cancellation and returns
the real append/durability outcome. Automatic commit takes over without the caller token after
execution completes. Execution and lock waits remain cancellable; a dead wire connection can
still lose the result. See [the shared boundary](../assimalign-cohesion-database-transactions/transaction-coordinator.md#commit-and-rollback).

The database's direct collection methods run automatic transactions. Methods called through
`session.Database` use that session's active transaction. OQL statements execute through the session
and therefore use its active transaction or an automatic statement transaction. Collection CRUD
always takes a session; a collection rejects sessions from another database. A handle obtained
through a session remains bound to that specific session and fails once it closes. Collection names
are database-local, case-sensitive names; OQL has one collection source and no database
qualification or server commands. The inherited `IDatabase.Engine` lifecycle reference remains the
frozen root API; executing OQL or CRUD never interprets it as session authority over other
databases.

Each statement uses one `ITransactionContext`. Automatic operations commit on success and roll back
on failure. Explicit transaction operations leave commit to the caller; a failing operation rolls
back the whole explicit transaction. Snapshot isolation fixes the read horizon at transaction start.
ReadCommitted captures one statement snapshot and pins its retention horizon for the statement.
Serializable is rejected rather than silently weakened.

The shared lock manager serializes writers per logical database. Before changing a document,
collection, or index, the operation compares its snapshot with the latest state under that lock. An
intervening change raises `DatabaseTransactionAbortedException`. Reads stay snapshot based. Catalog
and index writes use the same logical context as content chunks; rollback and crash recovery cannot
publish a partial document.

Collections created by these APIs have `DatabaseObjectOwner.Adhoc`. A collection directly marked
`Schema` refuses `DROP COLLECTION`, `CREATE INDEX`, and `DROP INDEX`, with
`DatabaseObjectLockedException` naming the collection, owning schema, and requested operation.
`Document` contents remain mutable. There is no compiled-schema provisioning authority in this engine.

## OQL query and DDL semantics

### SQL++ scalar functions

The parser, profile and evaluator share [OqlFunctions](../assimalign-cohesion-database-documents-language/oql-functions.md),
with 34 scalar functions alongside META and five aggregates. See the [scalar reference](../../../../database/document/language/functions/scalar-functions.md)
for signatures, examples and NULL/MISSING rules. Calls validate every argument/context before reads;
lazy conditionals evaluate only through their selected argument. Functions are not index seek bounds.
String indexing counts code points. Constructed strings/work buffers cap at 1,048,576 UTF-16 units.
ROUND uses exact digits once, including at negative scales. SQRT and fractional POWER calculate in
double and convert through round-trip text to Decimal. Function domain failures use COHDOC019
(no assigned SQLSTATE); overflow/string limits use COHDOC007 (22003).

Each explicit transaction captures one UTC clock instant, reused by NOW_STR/NOW_MILLIS across
statements; autocommit captures one per statement. Strings use yyyy-MM-ddTHH:mm:ss.fffZ.
COUNT, LENGTH, POSITION, ARRAY_LENGTH and epoch-millisecond calls use Int64/long, including
empty grouped metadata and wire results. Mixed numeric columns promote to Decimal; heterogeneous
columns normalize numeric cells to Decimal for embedded/client agreement.

### Catalog introspection

Collections are already discoverable through `GetCollectionsAsync` and `GetCollectionAsync`. OQL
adds only the catalog facts those handles do not expose, through two virtual document collections:

| Source | `Document` properties | Cardinality |
| --- | --- | --- |
| `COHESION_SCHEMA.INDEXES` | `COLLECTION_CATALOG`, `COLLECTION_NAME`, `INDEX_NAME`, `PATH`, `PATHS`, `IS_UNIQUE` | One document per visible definition; PATHS is the ordered array, PATH its first path and IS_UNIQUE the actual flag |
| `COHESION_SCHEMA.OBJECT_OWNERSHIP` | `COLLECTION_CATALOG`, `COLLECTION_NAME`, `OBJECT_TYPE`, `OBJECT_NAME`, `OWNER`, `OWNING_SCHEMA` | One document per visible collection; `OBJECT_TYPE` is `COLLECTION` |

`COLLECTION_CATALOG` is the bound database name. `OWNER` preserves SQL's `Adhoc` and `Schema`
values; `OWNING_SCHEMA` is the schema name or JSON null. Documents enforces ownership on the
collection: index DDL checks that collection's owner. The index metadata has no independent
ownership, so the ownership source reports collections and does not invent an index owner. `Index`
paths retain the catalog's canonical document-path spelling; physical generation IDs and B+Tree
pages remain internal. There is no collection schema or inferred document-field catalog.

The sources are document shaped: `SELECT *` returns one JSON document per entry, and ordinary
projection, aliases, parameters, filtering, grouping, aggregates, and ordering use the same
evaluator as stored documents. `For` example:

```sql
SELECT INDEX_NAME, PATH FROM COHESION_SCHEMA.INDEXES
WHERE COLLECTION_NAME = 'items' ORDER BY INDEX_NAME

SELECT OBJECT_NAME, OWNER, OWNING_SCHEMA
FROM COHESION_SCHEMA.OBJECT_OWNERSHIP
```

This follows the existing choice to put document index DDL in OQL and keeps the frozen database
interface unchanged. The system-source names are case-insensitive; their JSON property names are
case-sensitive, as with all document paths. Only the reserved `COHESION_SCHEMA` qualifier is
accepted: it names a source in the session's database, never another database. Ordinary
`other.items` remains invalid. Quoting the entire system-source name also identifies the same
reserved source.

Every query computes documents directly from the same transaction catalog snapshot used by the
statement. Snapshot transactions retain their read horizon, while own uncommitted catalog writes are
visible. Subsequent statements observe committed catalog changes according to their isolation level.
No metadata copy, content record, or synthetic collection is stored, and existing collection listing
stays unchanged.

The sources are read-only. `CREATE INDEX` and `DROP INDEX` targeting either source, and
`CreateCollectionAsync` or `DropCollectionAsync` using either reserved name, throw
`DatabaseException` with `System collection '<canonical source>' is read-only.` before ordinary
catalog lookup or mutation with COHDOC004. OQL INSERT, UPSERT, UPDATE and DELETE execute for stored collections; system targets use COHDOC004. Virtual sources do not provide
mutable `IDocumentCollection` handles.

### Stored document queries

The supported clause matrix lives in the language package's
[DESIGN.md](../assimalign-cohesion-database-documents-language/design.md) . SELECT, FROM, WHERE,
GROUP BY, HAVING, ORDER BY, LIMIT/OFFSET, INSERT, UPSERT, UPDATE, DELETE, EXPLAIN, collection and index DDL execute. Nested queries
and database administration statements are rejected with
`COHDBL001`. The parser advertises only clauses this executor supports. AST diagnostics are checked
both for text requests and directly constructed requests.

Projection supports whole documents, nested field paths, zero-based array element paths, literals,
parameters, arithmetic, and aggregates. `SELECT *` returns one `document` column containing the
entire JSON value. Objects and arrays remain `JsonElement` values; scalars become null, Boolean,
decimal, or string. Mixed-type columns advertise `DatabaseType.Null` as unknown rather than guessing
a schema. An explicit alias resolves duplicate projected column names; otherwise they fail. No POCO
reflection, schema inference, or runtime code generation is involved. Query JSON parsing uses the
same 128-level nesting limit as storage validation and index extraction, so every accepted document
can be queried at its stored depth.

SELECT supports ALL/DISTINCT and VALUE/RAW through [OqlSelectExpression](../assimalign-cohesion-database-documents-language/oql-select-expression.md).
VALUE/RAW produces one bare `value` column and normalizes a root MISSING to NULL before DISTINCT,
projected ordering and completion counting. Named projections omit MISSING object members; array
elements become JSON null. DISTINCT uses grouping's structural collation, retains the first value,
and runs after projection and before ordering. Ordering outside the projection after alias rewriting
is `COHDOC014` (no SQLSTATE). SELECT constructors allow 256 result levels, including nested
parameters/stored values and a named wrapper; exceeding this is `COHDOC015` at the constructor
(no SQLSTATE). INSERT retains its 128-level JSON limit and `COHDOC008`.

The internal DocumentRowWriter maps every result to one JSON value on every transport: named lists
become objects, VALUE/RAW stays bare, mixed star becomes a `document` member and lone star preserves
stored bytes through DISTINCT and ordering. Bare GetBytes(0) returns JSON bytes, named strings
unquoted UTF-8 and named NULL/MISSING empty memory. See [SELECT](../../../../database/document/language/statements/select.md).

Absent fields, incompatible traversal, navigation through NULL and out-of-range indices produce
MISSING. NULL and MISSING have separate grouping and presence tests. WHERE/HAVING use the common
SQL++ truth conversion, retaining nonzero numbers and nonempty values as TRUE. Equality compares scalar values and structurally
compares arrays/objects. Range predicates compare only values of the same scalar kind. Arithmetic
requires decimal operands; invalid numeric input and division by zero fail explicitly.

COUNT(*) counts input rows, including expanded UNNEST rows; expression aggregates skip both NULL and MISSING. COUNT(expression) counts present nonnull values. SUM and AVG require numeric
nonnull inputs. SUM, AVG, MIN, and MAX over no nonnull inputs return null; COUNT returns zero.
Ungrouped aggregate queries yield a single group even for an empty source. Outside aggregate calls,
grouped projection, HAVING, and ORDER BY expressions must be a group key or an expression built from
group keys and constants. `Group`-key binding compares expression structure, preserves literal scalar
types, and treats qualified and unqualified paths to the same iteration variable as equivalent.
`Group` values compare structurally.

Results are deterministic. A scan and an index seek both establish ordinal document-ID order before
filtering and projection. GROUP BY uses the total order MISSING, NULL, Boolean, decimal, ordinal string,
array, object. Arrays compare element by element; objects compare property names in ordinal order
and their values. ORDER BY accepts source expressions or a standalone explicit projection alias;
alias names take precedence over source fields for that standalone form. It compares its expressions
and preserves the established order for ties. This baseline makes identical data and queries return
identical sequences across access paths. Query results are materialized and own their JSON values;
they do not retain a transaction or borrowed storage memory after execution.

## Result paging

[OqlSelectExpression](../assimalign-cohesion-database-documents-language/oql-select-expression.md)
carries Limit and Offset. Planning checks nonnegative Int64 counts before collection lookup,
including EXPLAIN/direct AST calls. Invalid counts, including NULL, use OQL0007; missing parameters
retain COHDOC005. Paging runs after HAVING, DISTINCT, ORDER BY and column-type inference. Empty
pages retain metadata, query AffectedCount stays -1, and INSERT/UPSERT SELECT consumes only the page.

## `Index` planning and writes

`CREATE [UNIQUE] INDEX [IF NOT EXISTS] <index-name> ON <collection> (<path>, ...)` and
`DROP INDEX [IF EXISTS] <index-name> ON <collection>`
are OQL statements. `DocumentPlanner` binds them to catalog-operation plans and
`DocumentPlanExecutor` executes those plans under the statement's `ITransactionContext`. This
replaces the former extension-member entry point and leaves the frozen `IDocumentDatabase` member
list unchanged; there is no runtime switch on internal database implementations.

The create path uses the same segment grammar as a WHERE path, including nested object fields, array
subscripts, and bracket-string property names. Planning converts those segments to the catalog's
lossless canonical path without changing their case or treating a property name's punctuation as
structure.

The executor takes the logical database's exclusive writer lock, enforces schema ownership using the
specific `CREATE INDEX` or `DROP INDEX` operation name, and delegates the transactional catalog/tree
work to Documents.Catalog. The catalog owns index definitions and maintains shared B+Trees during
every Put/`Delete` and collection drop. `Index` creation populates existing documents in its
transaction; queries never lazily build trees. Dropping an index removes its visible definition in
the same transaction, so subsequent physical plans stop selecting it while older snapshots retain
their defined visibility.

Successful index DDL returns a command `DatabaseResult` with `Success` status and an affected count of
zero; index definition changes are not document-row mutations.

The physical planner uses applicable equality/range predicates on indexed paths, including parameter
values, reversed operands, and conjunctive bounds. Choice ranks longest leading equality prefix,
then a range on the next path, then UNIQUE, then ordinal index name. A non-leading-only bound scans. The executor reapplies the full predicate to candidates, preserving mixed-shape
semantics. See the [catalog design](catalog-design.md) for
composite key collation, NULL/MISSING rules, visibility filtering and restart recovery. Index
records use v2; v1 indexes are refused before recovery writes with a recreate remedy and no upgrade.
UNIQUE keys containing NULL/MISSING do not conflict. Duplicate valued scalar keys use
DocumentConstraintViolationException/COHDOC020 (23505); array/object components use COHDOC021.
Direct path counts outside 1..32 use COHDOC022. Constraints are immediate per row; failed explicit
transactions require RollbackAsync. Conditional CREATE keeps any same-named definition; conditional
DROP ignores an absent index. Snapshot conflicts precede no-ops; an existing schema-owned index
still refuses DROP. See [CREATE INDEX](../../../../database/document/language/statements/create-index.md).

## Data mutation and serialization semantics

OQL INSERT provides insert-only VALUES/SELECT writes with duplicate-key diagnostics. The collection API
provides deterministic mutation semantics: Put replaces the complete JSON value by ordinal identity,
`Delete` removes that identity, and each operation either completes in its transaction or rolls back.
Put captures caller memory before awaiting and returns a new version. An optional expected version
must match an existing visible document; mismatch fails. Versions use the durable kernel sequence
allocator, are strictly increasing for successful writes, and may have gaps. `Delete`/reinsert and
restart never reuse an old version.

[OQL DELETE](../../../../database/document/language/statements/delete.md) chooses the same scan/seek
as a SELECT filter, reapplies its full predicate and collects matching snapshot identities before
the first write. META remains a residual filter. The shared delete primitive checks writer and
snapshot conflicts, tombstones content and removes catalog/index entries in one transaction.
Rollback restores both documents and index visibility; older snapshots can still read deleted
documents. A later conflict rolls back earlier deletions; failure in an explicit transaction aborts
all its work. No WHERE deletes every document while retaining the collection and index definitions.
Schema ownership permits content deletion, system sources use COHDOC004 and absent targets COHDOC002.
Success reports deleted count with no Document frames. No format or protocol version changes.

Nested objects, arrays, and scalar roots are accepted. JSON validation and exact byte-preservation
rules are specified in the
[storage format](storage-design.md) . Adding/removing
fields and changing a scalar's type are supported complete-document replacements. They need no
compiled schema migration and update indexes in the same transaction. The new shape can change
predicate membership and projection types by the explicit mixed-shape rules above.

## Partial updates

[OQL UPDATE](../../../../database/document/language/statements/update.md) freezes candidates and every
SET value from the statement snapshot before writing. It reapplies the full filter after any index
seek. SET/UNSET changes content, versions and indexes atomically; failure rolls back the statement or
aborts the entire explicit transaction. Every match gets a higher version, including an unchanged
match. Completion reports the matched count without Document frames. EXPLAIN adds an Update root
and scan/seek child. No storage or protocol version changes.

Untouched property names and values retain their original UTF-8 bytes. Whitespace around rewritten
container members can normalize; surviving members keep order and new members append in target
order. Missing objects can be created for SET. Every index requires an existing in-range array,
including removal paths beneath missing members. UNSET of an absent member is a no-op; UNSET cannot
end in an index. SET-to-MISSING removes object members or writes null to existing array elements.
COHDOC016 identifies invalid targets/shapes, COHDOC017 normalized duplicate/prefix overlaps and
COHDOC018 invalid stored JSON or a patched depth beyond 128. No SQLSTATE is assigned to these codes.

## Lifecycle and durability

`DocumentDatabaseEngine.Create` starts four engine-owned workers: checkpoint, write-ahead-log flush,
dirty-page write-back, and MVCC version purge. `Workers` exposes them through the existing engine
contract. The observable state is Running, Faulted after an unexpected worker exception, and
Disposed after close. Disposal is idempotent: stop/join workers, dispose coordinators (rolling back
open transactions), then durably flush and close each storage file set. Close errors are aggregated
after attempting every database.

File-backed databases have `document.dat`, `document.log`, and `document.bak` under one validated
database-name directory. `Open` performs kernel WAL replay with its checkpoint deferred, scrubs
uncommitted record writers and index changes, and then completes the recovery checkpoint. Both
synchronous and grouped durability acknowledge commits only after the journal is durable.
Memory-backed databases use the identical storage/transaction implementation over in-memory streams.

## Limits and verification

The current engine materializes query inputs/results and whole JSON values in managed memory. Chunk
persistence handles documents larger than a page but does not promise a bounded heap independent of
document/query size. `Database`-wide writer locking is conservative; there is no query-cost statistics
model, join, subquery or external sort. Document server/client and hosting composition execute;
replication and model-specific security remain outside this surface.

Co-located tests cover nested/mixed JSON, expected-version writes, explicit commit and rollback,
both isolation levels, cross-database/session guards, direct-marked schema ownership,
indexed-versus-scanned queries, and file reopen. `Storage`/catalog tests exercise crash images with
committed and abandoned writes and index recovery. All serialization and activation are static BCL
calls compatible with trimming and NativeAOT.

The frozen lock manager does not cancel queued requests on transaction rollback. Documents rechecks
the context after a writer grant and releases any grant to an ended transaction. A caller
cancellation token cancels a pending wait promptly; without cancellation, an ended operation fails
when the earlier writer releases. Operation completion and abort are serialized to avoid duplicate
logical rollback.

The engine's durability setting configures the storage's physical commit gate. The current
transaction coordinator flushes logical document commits synchronously in both settings; grouped
logical commit batching is not claimed. The WAL flush worker remains the engine-owned implementation
of the shared storage flush duty.

## Document wire family and server

[DocumentProtocol](../assimalign-cohesion-database-documents-protocol/document-protocol.md) binds messages 64–72 to one Documents endpoint.
[DocumentDatabaseServer](document-database-server.md) serves OQL and document/collection commands;
[DocumentClient](../assimalign-cohesion-database-documents-client/document-client.md) rents authenticated
connections. Shared protocol 2.1 negotiates startup; Documents retains Failure for structured errors.

ResultHeader precedes Document objects (whole source values or projections with omitted MISSING),
then Complete with two Int64 fields: result count and affected count. ResultRow 68 is retired.
Command 69 carries CRUD/collection operations, StoredDocument 70 and Collection 71 carry responses,
and Failure 72 is terminal without Complete. Malformed UTF-8 is rejected. See the codec pages for
payload validation and [OQL diagnostics](../../../../database/document/language/diagnostics.md) for
COHDOC001–022, including collection ownership, duplicate keys, LIKE errors, DISTINCT ordering,
SELECT constructor depth, UPDATE target/overlap/stored-JSON failures and scalar domain errors.

The formatter retains one encoded frame, with backpressure supplied by TCP. InMemory can queue
output if its reader stops. Queries and client results remain materialized; explicit wire
transactions remain unsupported.

## Phase 29: deferred hosting composition

The owner-approved `Database` hosting composition
(`cohesion/docs/programs/DATABASE_HOSTING_DESIGN.md`) is implemented as
`AddDocuments((context, engine) => ...)` on `IDatabaseApplicationBuilder`. This replaces
`AddDocumentDatabase`. The model callback runs during application `Build` and receives an
`IDocumentDatabaseEngineBuilder`. It configures the complete option set, including
`FileSystemPath? RootPath`, durability, storage strategy, identity and worker intervals; it neither
binds configuration nor accesses a service container. Retained builder options and factories reject
mutation after the first engine `Build` attempt.

`AddWorker` and `AddServer` take factories whose engine argument exists before the factory runs. The
engine schedules custom workers through the common `IDatabaseEngineWorker.Run` contract; this is the
concrete generic consumer that earns `IDatabaseEngineBuilder`. There are no additional strongly
typed factory overloads: a model-specific factory can cast its argument, while ordinary workers
remain portable across models. The engine owns successful factory products and cleans them up on
subsequent construction failure. Nested servers must front that exact engine. The application
snapshots each engine's Servers for start/stop; disposing the engine disposes its servers and custom
workers.

`DocumentDatabaseEngine.Create(options)` remains the standalone entry point. `Application` factory
registrations are application-owned; instance registrations remain caller-owned, including their
nested components. All four named database operations now take `DatabaseName`, with the existing
implicit string conversion preserving ordinary literal call sites. Empty/default names are rejected.

The explicit requirement for StorageStrategy supersedes the draft's statement that this model lacks
a storage injection parameter. `IDocumentStorageStrategy` provides create/open/drop, existence and
discovery using the existing `DocumentStorage` product. It overrides RootPath without allocating
default files; returned storage is engine-owned and the strategy itself is borrowed. Durability is
supplied explicitly, and opening must defer checkpointing until engine recovery. Default file/memory
selection remains unchanged.

`DocumentDatabaseEngine.CreateBuilder()` exposes the model builder for the concrete hosting
builder's `AddEngine(name, build => ...)` overload. The consumer assigns resolved
configuration/service values, registers nested server/worker factories, and returns `Build()`; the
model package still never sees DI. There is no generic production orchestration over
`IDatabaseEngineBuilder`; the base contract supports model-agnostic worker composition,
demonstrated by tests exercising the public factory through that base interface.

## Declared dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Documents.Protocol` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Execution` | `CohesionSharedSource` |
| `Assimalign.Cohesion.Database` | `CohesionSharedSource` |
| `Assimalign.Cohesion.Database` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Documents.Language` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Transactions` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Indexing` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |

[Assembly overview](index.md) · [Examples](examples/index.md)

## UPSERT and generated JSON bytes

[UPSERT](../../../../database/document/language/statements/upsert.md) shares INSERT validation and
the Put write primitive. Inputs materialize before writes; repeated keys fail COHDOC012 with
`Document key 'a' appears more than once in one statement.` Whole-document versions and indexes
change atomically. Shared validation messages name INSERT or UPSERT as written.

One RFC 8259 encoder supplies generated JSON bytes for INSERT, UPSERT, constructors, named
projections and bare VALUE/RAW rows across in-process, InMemory and TCP paths. Unicode, HTML
characters and U+2028/U+2029 remain literal UTF-8. Quotes, backslashes and controls escape.
Raw stored/lone-star bytes retain their exact representation. See [JSON bytes](../../../../database/document/language/statements/select.md#json-bytes).

## Array bindings and predicates

[UNNEST/LEFT UNNEST](../../../../database/document/language/clauses/unnest.md) expands arrays before
WHERE and preserves document-ID/array order. LEFT synthesizes MISSING for empty/non-array inputs;
star constructs owned objects keyed by binding names. Quantifiers use inner lexical scopes and
true/not-true element results; collection MISSING propagates and NULL/non-array gives NULL.
Array IN retains the separate NULL-left rule, including NULL IN [] = NULL.

FROM/UNNEST names are unique, reserving the unaliased source name (OQL0010 in text, COHDOC023
for direct SELECT AST duplicates/empty names). Quantifiers may shadow outer names. META on
an array/quantifier variable is COHDOC001. EXPLAIN adds Unnest/LeftUnnest and SystemCollectionScan
without changing the shared eight columns. Catalog, storage and wire layouts are unchanged;
the profile executes 31 of 32 declared clauses.

## Database files

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| `COHDBD003` | Missing journal/backup, incomplete creation or unrecoverable files during create/open; preserve ambiguous files and restore the complete file set, or drop/recreate only a known incomplete create as directed | — |
| `COHDBS003` | Detected storage corruption; DatabaseCorruptionException carries database, optional page and original cause; refused opens also retain COHDBD003 | — |

Reads, statements, transactions and create/open translate storage corruption, including coded and
aggregate causes. Damaged reads fail instead of omitting documents; legitimate reclamation retains
absence semantics. The existing wire execution-failure path carries the code and leaves the
connection usable. See [DatabaseCorruptionException](../assimalign-cohesion-database/database-corruption-exception.md).

This is a database lifecycle diagnostic carried by DatabaseException.Diagnostics, rather than a
query evaluation error. A leftover directory without its data file remains DatabaseNotFoundException
with the same diagnostic. Missing sidecars are never recreated. Transient open I/O failures ask for
a retry once resolved and carry no restore/drop advice. No SQLSTATE is assigned to COHDBD003.

## Database creation, open and drop

File-backed Documents databases require their original document.dat, document.log and document.bak.
Create and open refuse a missing journal or backup with COHDBD003 in DatabaseException.Diagnostics;
neither operation recreates a sidecar, even beside an empty, cleanly closed database. Preserve an
ambiguous file set and restore the complete backup: a lost journal can contain acknowledged commits.

A leftover directory without its data file is DatabaseNotFoundException on open, retaining COHDBD003.
A data file no larger than one page is an incomplete create, since successful creation flushes two
pages. These known incomplete cases direct DropDatabaseAsync followed by CreateDatabaseAsync;
ordinary create does not adopt them. Drop removes document.dat before deleting the remaining directory.

A partial final data page goes through WAL recovery. An uncommitted torn extension at the tail is
tolerated and overwritten by the next allocation, preserving committed content. Unrecoverable
corruption uses DatabaseCorruptionException / COHDBS003 with COHDBD003 retained as an additional
diagnostic and verified-backup recovery advice. Transient storage I/O, including a full
pinned buffer pool, instead asks callers to retry OpenDatabaseAsync after resolving the condition,
without drop or backup advice. The shared storage format is 4; this does not change Document record formats.

### Lifecycle API

```csharp
public new ValueTask<DocumentDatabase> CreateDatabaseAsync(DatabaseName name, CancellationToken cancellationToken = default);
public new ValueTask<DocumentDatabase> OpenDatabaseAsync(DatabaseName name, CancellationToken cancellationToken = default);
public ValueTask DropDatabaseAsync(DatabaseName name, CancellationToken cancellationToken = default);
```

Disposed engines throw ObjectDisposedException; cancellation throws OperationCanceledException.
Invalid names use DatabaseName validation, and an existing completed database rejects create.
The file-set code is an Error diagnostic with no assigned SQLSTATE. Declared Build also refuses
missing sidecars without writing replacements; declared databases cannot be dropped.

On a fresh in-memory engine:

```csharp
await using var engine = DocumentDatabaseEngine.Create("sales-engine", new());
var database = await engine.CreateDatabaseAsync("sales");
await database.DisposeAsync();
var reopened = await engine.OpenDatabaseAsync("SALES"); // reopens the same logical database
await engine.DropDatabaseAsync("sales");
```

See [database lifecycle](../../../../database/document/index.md) and the DocumentDatabaseNameTests
file-set refusal, crash recovery and transient-open cases.

## Engine failure boundaries

Content chunk failures return from storage to the engine's failure handler. It orders the
operation completion gate, transaction end gate and coordinator apply gate, so exactly one abort
runs even when failure races rollback or session disposal. Earlier chunks and statements are
undone together. An explicit session refuses commands with COHDBD001 until the caller rolls back.

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| `COHDBD004` | Storage operation or open I/O failure, including I/O inside catalog/index-registration wrappers; original cause retained | — |
| `COHDBD005` | Catalog/index failure; additional model diagnostic for attached-index corruption | — |
| `COHDOC024` | CREATE INDEX finds an existing name without IF NOT EXISTS | — |
| `COHDOC025` | DROP INDEX finds no index without IF EXISTS | — |
| `COHDOC026` | Encoded composite index key exceeds 1,024 bytes during build or mutation | — |

Child-root storage, catalog and index failures cross the engine boundary as DatabaseException
with Error diagnostics and the original InnerException. Explicit and automatic transaction begin
use the same translator. Wrapped I/O keeps COHDBD004. Attached-index corruption instead uses
DatabaseCorruptionException, leading with COHDBS003 and retaining COHDBI002 and COHDBD005.
Existing offline/unconfirmed-commit classifications remain in force. An I/O open failure releases
acquired resources, publishes no database and permits retry after resolution.

A failed operation aborts an explicit transaction; roll it back before further work. In autocommit
the next statement can run immediately. COHDBD001 is in Diagnostics with an ordinary embedded
message. The wire adds its prefix and returns ExecutionFailure with diagnostics, preserving the
connection. Index-name/key-size user failures retain their distinct COHDOC024–026 diagnostics.

### Cleanup diagnostics

After a successful automatic commit, snapshot-cleanup failure preserves the committed result
and releases the session hold. `OutOfMemoryException` still propagates. Cleanup failure before
commit success or inside an explicit transaction still fails the operation.

| EventSource | Event ID | Name | Level | Payload |
| --- | --- | --- | --- | --- |
| `Assimalign.Cohesion.Database.Documents` | 13 | `StatementCleanupFailed` | Warning | `database`, `exceptionType` |

The warning omits the exception message because it may contain document input. The committed
write survives reopen; do not retry a write reported as successful because of this warning.

## Terminal errors and drain

Terminal shared Error delivery starts one ShutdownDrainTimeout deadline before the write. The
server writes and flushes the complete frame, completes connection Output, then consumes peer input
until peer close, TerminalDrainLinger or the outer deadline. The default drain budget is 30 seconds;
linger defaults to two seconds and starts after output completion. Capacity refusals use the same
sequence without creating a session, and are registered before I/O so Stop joins their cleanup.
Soft stop allows its terminal notice to drain, including a stop racing the final Ready flush.

Hard abort or outer expiry aborts blocked transport I/O. Teardown joins pending I/O and detaches
callbacks before disposing token sources, the transport and engine session. Every resource release
is attempted even when an earlier disposal fails. Final cleanup is joined without another timeout:
a transport/execution that ignores abort can delay Stop. ShutdownDrainTimeout bounds graceful work;
it does not abandon resources still owned by pending I/O. An unread idle pooled peer releases its
server slot after linger rather than occupying the whole outer budget.

Both ShutdownDrainTimeout and TerminalDrainLinger must be greater than zero and at most
4,294,967,294 milliseconds. Infinite values are refused with ArgumentOutOfRangeException at server
construction. Setters store values and creation snapshots them. A disposed input reader during
drain counts as transport closure. TLS terminal delivery remains a known gap (#1086): completing
the TLS output does not complete its inner TCP output, so a fatal frame can still be lost.

See [DocumentDatabaseServerOptions](document-database-server-options.md) for signatures and configuration.

### Terminal drain diagnostics

Provider `Assimalign.Cohesion.Database.Documents` reports terminal-session deadline aborts:

| Id | Event | Level | Payload |
| --- | --- | --- | --- |
| 14 | TerminalDrainTimedOut | Warning | sessionId, drainTimeoutMilliseconds, suppressedCount |

Warnings are limited to one per second per provider; the next emitted warning carries the number
suppressed. The process-wide total-terminal-drain-timeouts polling counter counts every terminal
session deadline that wins the abort transition, even with events disabled or warnings suppressed.
Normal peer-close linger expiry and a server hard abort do not emit this event. Capacity refusals
have no session and do not increment this session counter. This warning adds no diagnostic code
or SQLSTATE.

## Sources

- **Terminal delivery** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentServerDrainTests.cs`, `DocumentServerRejectionDrainTests.cs`, `DocumentServerRejectionSetupTests.cs` and `DocumentTerminalDrainEventTests.cs` in the same directory.

- **Conformance evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentCommitCancellationTests.cs`.
- **Conformance evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentOperationOutcomeTests.cs`.

- **Boundary and index failures** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/ChildRootFailureWireTests.cs`, `IndexFailureTranslationTests.cs` and `StorageOpenFailureTests.cs` in the same directory.

- **Current project boundaries** — `cohesion/resources/Database/README.md`.
- **Build guard** — `cohesion/resources/Database/Directory.Build.targets`.

- **Lifecycle implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/DocumentDatabaseEngine.cs`.
- **File-set and recovery cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentDatabaseNameTests.cs`.
- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Assimalign.Cohesion.Database.Documents.csproj`.

## Session execution (#1576, 2026-10-16)

`DocumentDatabaseSession` derives from `DatabaseLanguageSession<DocumentQueryRequest>` for typed and text
execution. `DocumentQueryRequest` owns its typed `Statement` and optional `Parameters` over
the memberless `DatabaseRequest` root. Results use the `DatabaseResult` family.
