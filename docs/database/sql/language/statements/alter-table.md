# ALTER TABLE (Cohesion SQL)

Changes columns, defaults and nullability, renames tables or columns, and adds or removes constraints.

> **Status:** Partial. The documented subset is measured against the live SQL engine.

## Syntax

```syntaxsql
ALTER TABLE [ schema_name . ] table_name
{
    ADD [ COLUMN ] column_definition
  | DROP [ COLUMN ] column_name
  | ALTER [ COLUMN ] column_definition
  | ALTER [ COLUMN ] column_name [ SET DATA ] TYPE data_type
      [ COLLATE collation_name ] [ USING expression ]
  | ADD table_constraint
  | DROP CONSTRAINT constraint_name
  | RENAME TO new_table_name
  | RENAME [ COLUMN ] column_name TO new_column_name
  | ALTER [ COLUMN ] column_name SET DEFAULT default_expression
  | ALTER [ COLUMN ] column_name DROP DEFAULT
  | ALTER [ COLUMN ] column_name SET NOT NULL
  | ALTER [ COLUMN ] column_name DROP NOT NULL
}
[ ; ]
```

## Arguments

- **`table_name`** — The stored table being changed.
- **`column_definition`** — A column definition from [CREATE TABLE](create-table.md#syntax).
- **`table_constraint`** — A supported table constraint from [CREATE TABLE](create-table.md#syntax).
- **`column_name` or `constraint_name`** — The existing object to change or remove.
- **`new_table_name` or `new_column_name`** — An unqualified name; table renames stay in the same schema.
- **`default_expression`** — A column-free scalar expression, including literals, arithmetic, CAST and function calls.
  See [DEFAULT](../clauses/default.md) for volatility and ADD COLUMN rules.

## Remarks

Adding PRIMARY KEY/UNIQUE constraints or columns that build an index checks the
[1,024-byte encoded key limit](../diagnostics.md#row-and-index-key-limits). Oversized keys fail
with `CS05042` / SQLSTATE 54000 before the new column, constraint or index is published;
the old schema and rows remain usable.

One statement accepts one action. SET SCHEMA, RENAME CONSTRAINT and comma-separated action
lists are outside this subset (CK00001).

### Identity changes

ALTER COLUMN supports ADD GENERATED, SET GENERATED, SET sequence options, RESTART and DROP
IDENTITY [IF EXISTS]. ADD GENERATED needs NOT NULL and preserves existing values. Adding a
new identity column instead backfills populated storage before atomic publication. See
[identity syntax, diagnostics and examples](../clauses/identity.md#alter-and-backfill).

### ALTER COLUMN TYPE

Changing any column read by a [partial-index predicate](create-index.md#partial-indexes), including
a widening change, refuses with CS03083 (0A000). Drop the index before TYPE and recreate it
afterward. Dropping a key or predicate column removes its index; table and column renames rewrite
predicate references along with the other dependent metadata.

`ALTER TABLE t ALTER [COLUMN] c [SET DATA] TYPE type [COLLATE collation] [USING expression]`
changes the type while preserving nullability, keys and constraints. The target declaration
selects its collation; omitted COLLATE uses the database default. USING sees every column of the
**old row**, including qualified references to t and synthesized missing fields. It cannot read
another table or contain subqueries, aggregates or windows. Parameters and scalar expressions
use the ordinary expression rules. Without USING, the old column supplies the value.

Both the old value and a USING result require an assignment or implicit registry cast to the
target. An explicit-only pair such as TEXT to INT requires `USING CAST(c AS INT)`; `USING c`
alone does not authorize it, even on an empty table (CS02064, 42804). A bare string literal
retains unknown-input coercion. Character modifiers use assignment semantics: excess trailing
spaces can trim to a narrower VARCHAR, but other overflow refuses. A written character CAST
inside USING can explicitly permit truncation. SQL has NULL and no MISSING value; NOT NULL still
validates the converted value.

USING does not transform the default. The original default expression must independently
assignment-cast to the target, even on an empty table. Drop an incompatible default before TYPE,
then SET DEFAULT afterward. Numeric/Boolean defaults keep their expression identity; an unknown
string default keeps its resolved old type. Immutable defaults validate eagerly and persist as
canonical literals. Stable/Volatile defaults retain their text and pinned calls and evaluate at
INSERT time with the new assignment coercion. Repeated changes do not accumulate explicit CASTs.
A column with no default inherits the target domain's default, which is validated too.

Existing CHECK definitions are rebound against changed operand types and rechecked over converted
rows; domain checks and NOT NULL also revalidate. A rewriting change rebuilds every index and
checks uniqueness under the target collation. New key collisions refuse publication. Incoming,
outgoing and self foreign keys involving c must retain identical underlying storage type identities
(INT and BIGINT differ), compatible character collations and valid final keys. Only affected FK
neighbors are locked and revalidated. A view depending on c blocks TYPE with CS03302, even for a
same-type USING expression; a view reading only other columns does not block it.

Integer identity columns can change within the integer family through explicit TYPE, retaining
the cursor, START and INCREMENT. Domain/non-integer targets refuse with CS03025. START and
allocated counter values must fit the new range; unissued reservation tails are trimmed before
publication. Reopen retains the existing policy of skipping unused reserved values.

Without USING, same-storage, same-scale, same-domain and same-collation changes that only widen
length/precision or make them unbounded take the binary-compatible no-rewrite path. Heap and
index trees remain. CHAR width changes can repad values and therefore rewrite. Narrowing uses
the copier and assignment semantics.

Other changes copy live rows into a dense replacement and atomically publish the definition,
indexes and catalog generation. Execution runs **inline under the table's exclusive lock**, also
locking affected adjacent FK tables exclusively. Populated-table handoff to the provisioning
worker arrives in batch 7. Conversion, constraint failure or cancellation before publication
keeps the old table and indexes current. Old read-only snapshots retain the old owner; stale
writers retry their transaction with CS04006. Cancellation after publication cannot undo it.
LockTimeout/cancellation retain the ordinary DDL cleanup rules; successful TYPE affects zero rows.

### Full-definition ALTER COLUMN

The older `ALTER COLUMN c type [NULL | NOT NULL] [DEFAULT expression]` form describes the full
column definition rather than an old-row USING conversion. It retains the validation-only facet
path when storage type, scale, domain and collation stay unchanged. Tightening length/precision
and every CHAR width change validate existing values; a change needing repadding is refused on
that path. Storage-type, scale, domain and explicit collation changes share the rewrite machinery.
It refuses CS05013 if the resulting default differs, or if the collation changes without an
explicit COLLATE, even when rewriting. Use SET/DROP DEFAULT for default changes. Full-definition
identity-type changes remain refused; use explicit TYPE for integer identity conversion.

### Renames

RENAME preserves object IDs, physical ordinals, row versions and index trees. Primary-key and
index column lists, local/incoming/self-referencing foreign keys and CHECK definitions change
in one catalog transaction. Generated index and constraint names stay unchanged. CHECK rewriting
changes bound identifier tokens only, preserving strings, function/type names and collations;
it handles persisted definitions created under a raised parser nesting limit. System views expose
the new names immediately. Queries using old names fail normal table/column binding.

A name collision, including renaming an object to itself, is CS05013. A rename cannot overwrite
a system relation (CS04021, 25006). After ownership checks, a table rename whose target is retained
by a declared script claim is CS03302 naming that script.

### Defaults and nullability

SET DEFAULT changes later omitted-column and whole-item DEFAULT assignments. Function result types
must satisfy INSERT assignment compatibility (CS05013 naming the column); literals must convert
within the destination bounds before publication. Immutable expressions fold at declaration; Stable
and Volatile calls execute per assigned row using the transaction-start clock. Defaults never run at table open.
DROP DEFAULT on a domain column re-inherits the domain default. SET DEFAULT NULL preserves an
explicit NULL override when hiding a domain default. On a NOT NULL column without a usable default,
later omitted values fail CS03024 (23502).

Neither SET nor DROP DEFAULT changes old rows or their frozen missing values. ADD COLUMN captures
that missing value once, including NULL when no default exists; it survives default changes and
restart. Explicit stored NULL stays NULL. A later validation-only type narrowing checks the frozen
literal under the new bounds even when no live rows remain; an unconvertible value refuses publication.

SET NOT NULL scans current live rows, including synthesized missing fields, under the table lock.
A NULL refuses the change and leaves metadata intact. DROP NOT NULL allows later NULL writes but
refuses primary-key columns (CS00003). SET/DROP DEFAULT and DROP NOT NULL refuse identity columns
(CS03025). Even an already satisfied facet publishes a replacement definition.

### Added columns and publication

ADD CONSTRAINT and ADD COLUMN validation stream current live rows under the table's Exclusive
lock, retaining only the current decoded row instead of materializing the table. Referenced
parent tables are also locked for foreign-key validation. Provisioning progress can require a
preliminary count pass. Validation memory does not grow with the table's row count; this does
not make the ALTER an online operation or remove the work of scanning existing rows.

PRIMARY KEY/UNIQUE backing indexes validate the completed tree in bounded key batches before
publication. Only current live keys participate in duplicate checks; historical entries remain
available to older snapshots. A duplicate reports CS03037 (23505). Construction checks the
encoded key limit first, so CS05042 (54000) can precede a duplicate encountered earlier in
storage order. A validation failure or cancellation before publication removes unpublished
indexes and leaves the old schema, constraints and rows intact; another request can retry.
See [unique-index builds](create-index.md#building-over-existing-data).

ADD CONSTRAINT rejects repeated PRIMARY KEY, UNIQUE or FOREIGN KEY columns, including the
referenced list, with `CS01019`. ADD CONSTRAINT or ADD COLUMN cannot declare a second primary
key over an existing one; repeated PRIMARY KEY clauses on a single added column also fail with
that code. Validation uses case-insensitive column identity and leaves the catalog unchanged.
New CHECKs in either action must qualify columns with this table and schema, or leave them
unqualified; another qualifier reports located `CS01015`. See [Constraints](../constraints.md).

ADD COLUMN admits expression defaults. A nonvolatile expression freezes once for old rows;
a volatile expression is refused on live rows with `CS03027` and admitted on an empty table. Old rows resolve a missing trailing
field from persisted catalog metadata; their bytes and multi-version concurrency control (MVCC)
stamps are unchanged. An explicitly stored `NULL` remains `NULL`. A later update writes the full row.

| Addition | Existing rows | Later insert omitting the column |
| --- | --- | --- |
| Nullable, no default or `DEFAULT NULL` | `NULL` | `NULL` |
| Immutable default | Folded value | Validated declaration value |
| Stable default | One frozen addition-time value | Per-row value using transaction-start clock |
| Volatile default | Refused with `CS03027` when live rows exist | Per-row value if added to an empty table |
| `NOT NULL`, non-null default | Default value | Default value |
| `NOT NULL`, no non-null default | Rejected if currently committed rows exist | Must supply a value if addition succeeds on an empty table |

The emptiness check uses current committed rows. Historical versions visible only to an older
snapshot can retain a missing `NULL` field after a default-free addition to a currently empty table.

ADD [IDENTITY](../clauses/identity.md#alter-and-backfill) backfills live rows through an atomic
storage rewrite. ADD GENERATED on an existing NOT NULL column preserves its values. Full-definition ALTER preserves identity only
with its existing type, NOT NULL and no DEFAULT; type/nullability/default or generation changes
fail `CS03025`. An explicit NULL conflicting with NOT NULL, PRIMARY KEY or IDENTITY reports
`CS00003`; ALTER also rejects making an existing primary-key column explicitly NULL.

ADD COLUMN defaults must fit destination bounds. Invalid defaults, invalid conversions, and out-of-range
values fail before schema publication. The complete definition is published atomically; a failed
addition leaves schema, rows, and catalog unchanged. Defaults survive catalog reopen.

Already bound plans retain their immutable definition. Statements planned after the change use
the new definition even when an older row snapshot selects earlier versions. Ordinary table schemas
are not pinned to `BEGIN`; system-view catalog snapshots have their own contract.

DDL is self-committing and fails inside explicit transactions with `CS04003`. Session alteration
of schema-owned tables is refused. DROP COLUMN changes catalog metadata without rewriting rows;
physical ordinals and dropped-column metadata keep existing row values aligned.

To reclaim dropped ordinals, use the C# maintenance operation
[SqlDatabase.RewriteTableAsync](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/sql-database.md).
It copies live rows into a fresh dense layout, rebuilds indexes and publishes atomically. Old
read-only snapshots retain the original layout until they end; writers bound before publication
abort retryably with CS04006. This adds no ALTER syntax and keeps SQL format 31.

Table and dependent-definition changes publish atomically. DML waiting behind a changed definition
must retry; after a table rename it reports the old table missing. SELECT planning retries binding
when catalog publication races it. SQL storage format 31 is current: format 17 introduced this
insert-default/frozen-missing-value split with table extension 6; format 18 separately changes
canonical function-name quoting. Compiled schema version is 10. Older SQL formats are refused.

### View dependencies

[Persisted views](create-view.md) block table rename and drop/rename of columns they read.
ALTER COLUMN also refuses changes to those columns' type, length, precision or scale with
CS03302. Explicit TYPE refuses even a same-type USING conversion. Default and nullability
changes retain their usual validation.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE where supplied |
| --- | --- | --- |
| `CS00003` | Malformed action or DROP NOT NULL on a primary-key column | — |
| `CK00001` | Excluded action/list, subquery in DEFAULT/USING, or aggregate/window in USING | — |
| `CS05013` | Rename collision, invalid default, incompatible function result, or changed full-definition default/implicit collation | — |
| `CS02064` | Old value, USING result or dependent default lacks an assignment/implicit cast to the target | 42804 |
| `CS02002` | Numeric range/precision failure during conversion/default re-cast | 22003 |
| `CS02022` | Invalid non-temporal conversion text | 22P02 |
| `CS02047` / `CS02048` / `CS02062` | Temporal format / field range / interval overflow during conversion | 22007 / 22008 / 22015 |
| `CS01014` / `CS01015` | Missing bound table / column | 42P01 / 42703 |
| `CS01015` | New CHECK names another table or schema | — |
| `CS01019` | Repeated key-list column or second PRIMARY KEY declaration, including ADD COLUMN | — |
| `CS04021` | System relation target or attempted rename over a system relation | 25006 |
| `CS03024` | NOT NULL validation finds NULL | 23502 |
| `CS03025` | Protected identity facet change | — |
| `CS03027` | Volatile ADD COLUMN default requires a rewrite because live rows exist | — |
| `CS03037` | Existing/defaulted rows violate a new PRIMARY KEY, UNIQUE constraint or unique index | 23505 |
| `CS03038` | Existing/defaulted rows violate a CHECK or domain check | 23514 |
| `CS03039` | Existing/defaulted rows lack a referenced foreign-key parent | 23503 |
| `CS05042` | An existing/defaulted index key exceeds 1,024 encoded bytes during a backing build | 54000 |
| `CS03049` / `CS03061` | Rename target table / column already exists | 42P07 / 42701 |
| `CS03050` / `CS01052` | Duplicate constraint name / missing constraint or other catalog object | 42710 / 42704 |
| `CS01051` | Table is absent during execution | 42P01 |
| `CS03053` | Invalid table definition, including dropping its last column | 42P16 |
| `CS03054` | FOREIGN KEY column list or referenced unique-key requirement fails | 42830 |
| `CS02055` / `CS02063` | FOREIGN KEY type / collation mismatch | 42804 / 42P21 |
| `CS03056` | Constraint or index depends on the object being changed | 2BP01 |
| `CS05057` | Validation-only full-definition change would rewrite values, or target index collation is unsupported | 0A000 |
| `CS04044` | DDL lock wait exceeds LockTimeout | 55P03 |
| `CS04006` | Writer needs a retired table layout after publication; retry the transaction | — |
| `CS03059` | CHECK cannot be stored as a deterministic row definition | 42P17 |
| `CS04064` | Bound table was replaced during a DDL wait; retry the statement | 40001 |
| `CS03066` | Compiled-schema ownership refuses session DDL | 42501 |
| `CS03302` | Declared script or view retains a dependent target | 2BP01 |
| `CS04003` | DDL inside an explicit transaction; the transaction remains usable | 25001 |

Conversion and length failures retain their [normal codes](../diagnostics.md). `CS03027` has no assigned SQLSTATE.

## Examples

### Convert using the old row

Adapted from SqlAlterColumnTypeTests; execute each request separately:

```sql
CREATE TABLE retype (id INT PRIMARY KEY, v TEXT NOT NULL, extra INT DEFAULT 7);
INSERT INTO retype VALUES (1,'10',7),(2,'20',7);
CREATE UNIQUE INDEX by_v ON retype(v);
ALTER TABLE retype ALTER COLUMN v SET DATA TYPE BIGINT
    USING CAST(retype.v AS BIGINT) + id + extra;
SELECT id,v FROM retype ORDER BY id;
```

The result is `(1,18)` and `(2,29)`; by_v now indexes BIGINT values. A conversion, CHECK or
uniqueness failure would retain the original table and values.

```sql
CREATE TABLE default_retype (v TEXT DEFAULT '7');
ALTER TABLE default_retype ALTER v TYPE INT USING CAST(v AS INT);
```

The ALTER fails CS02064 (42804): USING converts rows, but the TEXT default lacks an assignment
cast to INT. DROP DEFAULT, repeat TYPE, then SET DEFAULT 7 to complete that change.

```sql
CREATE DOMAIN positive_default AS INT DEFAULT 42 CHECK(VALUE>0);
CREATE TABLE domain_retype (v INT);
ALTER TABLE domain_retype ALTER v TYPE positive_default;
INSERT INTO domain_retype DEFAULT VALUES;
SELECT v FROM domain_retype;
```

This returns 42, inherited from the target domain.

Starting from the [conformance fixture](select.md#a-create-the-conformance-fixture):

```sql
ALTER TABLE t ADD COLUMN extra INT DEFAULT 7;
SELECT id, name, age, extra FROM t ORDER BY id;
INSERT INTO t (id, name, age) VALUES (4, 'new', 1);
ALTER TABLE t ADD CONSTRAINT positive CHECK(extra > 0);
ALTER TABLE t DROP CONSTRAINT positive;
ALTER TABLE t DROP COLUMN extra;
```

All three original rows and the omitted-column insert read `7` for `extra` before it is dropped.
Each statement is a separate request.

With `CREATE TABLE keyed (id INT PRIMARY KEY);` already executed, each of these separate
requests fails with CS01019 and leaves keyed unchanged:

```sql
ALTER TABLE keyed ADD COLUMN other INT PRIMARY KEY;
ALTER TABLE keyed ADD CONSTRAINT uq_bad UNIQUE(id, ID);
```

### Retry a refused constraint

Execute each statement separately in a fresh database:

```sql
CREATE TABLE existing_keys (id INT);
INSERT INTO existing_keys VALUES (1), (1), (2);
ALTER TABLE existing_keys ADD CONSTRAINT uq_id UNIQUE(id);
SELECT id FROM existing_keys ORDER BY id;
DELETE FROM existing_keys WHERE id = 1;
ALTER TABLE existing_keys ADD CONSTRAINT uq_id UNIQUE(id);
```

The first ALTER fails with CS03037 (23505) and publishes no constraint or index. SELECT still
returns 1, 1, 2. DELETE removes both 1 rows, and the retried ALTER succeeds with affected count
zero. A later duplicate 2 is refused. NULL key components still compare equal for uniqueness.

### Change an insert default without changing old missing fields

Execute each statement separately:

```sql
CREATE TABLE alterations (id INT PRIMARY KEY);
INSERT INTO alterations VALUES (1);
ALTER TABLE alterations ADD extra INT DEFAULT 7;
ALTER TABLE alterations ALTER COLUMN extra SET DEFAULT 9;
INSERT INTO alterations (id) VALUES (2);
ALTER TABLE alterations ALTER extra DROP DEFAULT;
INSERT INTO alterations (id) VALUES (3);
SELECT id, extra FROM alterations ORDER BY id;
ALTER TABLE alterations RENAME TO renamed;
ALTER TABLE renamed RENAME COLUMN extra TO score;
SELECT id, score FROM renamed ORDER BY id;
```

Both SELECTs return (1, 7), (2, 9), (3, NULL). The first row keeps its frozen value after DROP DEFAULT
and restart. The table primary key survives both renames.

```sql
UPDATE renamed SET score = 4 WHERE id = 3;
ALTER TABLE renamed ALTER score SET NOT NULL;
ALTER TABLE renamed ALTER score DROP NOT NULL;
```

The UPDATE affects one row; each ALTER affects zero rows and changes the nullability facet.

Format 19 quotes PERCENT and TIES in persisted SQL. Format 20 adds persisted SET NULL,
SET DEFAULT and NO ACTION delete actions. Format 21 introduced namespace records of kind 15. Format 22 quotes STRING_AGG and LISTAGG
identifiers in persisted SQL. Format 23 introduced catalog views and format 24 explicit NULL
placement. Format 25 added distinct predicates, Boolean tests and LIKE ESCAPE.
Format 26 fenced temporal DEFAULT and frozen missing-value text.
Format 29 added owned-sequence metadata. Format 30 added ON UPDATE action metadata in table extension 8.
Current format 31 adds partial-index predicate metadata in index extension 2 and retains TYPE default assignment coercion.
Every earlier format is refused at open; there is no
upgrade path. Compiled schema format remains version 10.

## See also

[CREATE TABLE](create-table.md) · [DEFAULT](../clauses/default.md) · [Constraints](../constraints.md) · [System views](../system-views/index.md)

[SqlAlterColumnTypeAction](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-sql-language/sql-alter-column-type-action.md) · [SqlAlterTableExpression](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-sql-language/sql-alter-table-expression.md) · [Statements](index.md) · [Language (SQL)](../index.md)

## Sources

- **TYPE and review cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAlterColumnTypeTests.cs`, `SqlAlterColumnTypeReviewTests.cs` and `SqlAlterColumnTypeReviewTests.SecondPass.cs`.
- **Bounded validation and retry** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDdlMemoryTests.cs` and `SqlUniqueIndexBuildTests.cs` in the same directory.
- **Conformance evidence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlProgramLimitReviewTests.cs`.

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAddColumnExecutionTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`
- **Renames and facets** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAlterTableExecutionTests.cs` and `SqlAlterTableRecoveryTests.cs` in the same directory.

