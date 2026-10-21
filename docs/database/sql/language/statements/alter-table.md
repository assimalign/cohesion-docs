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
with `COHSQLE042` / SQLSTATE 54000 before the new column, constraint or index is published;
the old schema and rows remain usable.

One statement accepts one action. SET SCHEMA, RENAME CONSTRAINT, PostgreSQL TYPE/SET DATA TYPE,
TYPE ... USING and comma-separated action lists are outside this subset (COHDBL001).

### Renames

RENAME preserves object IDs, physical ordinals, row versions and index trees. Primary-key and
index column lists, local/incoming/self-referencing foreign keys and CHECK definitions change
in one catalog transaction. Generated index and constraint names stay unchanged. CHECK rewriting
changes bound identifier tokens only, preserving strings, function/type names and collations;
it handles persisted definitions created under a raised parser nesting limit. System views expose
the new names immediately. Queries using old names fail normal table/column binding.

A name collision, including renaming an object to itself, is COHSQLE013. A rename cannot overwrite
a system relation (COHSQLE021, 25006). After ownership checks, a table rename whose target is retained
by a declared script claim is COHSQLD002 naming that script.

### Defaults and nullability

SET DEFAULT changes later omitted-column and whole-item DEFAULT assignments. Function result types
must satisfy INSERT assignment compatibility (COHSQLE013 naming the column); literals must convert
within the destination bounds before publication. Immutable expressions fold at declaration; Stable
and Volatile calls execute per assigned row using the transaction-start clock. Defaults never run at table open.
DROP DEFAULT on a domain column re-inherits the domain default. SET DEFAULT NULL preserves an
explicit NULL override when hiding a domain default. On a NOT NULL column without a usable default,
later omitted values fail COHSQLE024 (23502).

Neither SET nor DROP DEFAULT changes old rows or their frozen missing values. ADD COLUMN captures
that missing value once, including NULL when no default exists; it survives default changes and
restart. Explicit stored NULL stays NULL. A later validation-only type narrowing checks the frozen
literal under the new bounds even when no live rows remain; an unconvertible value refuses publication.

SET NOT NULL scans current live rows, including synthesized missing fields, under the table lock.
A NULL refuses the change and leaves metadata intact. DROP NOT NULL allows later NULL writes but
refuses primary-key columns (SQL0003). SET/DROP DEFAULT and DROP NOT NULL refuse identity columns
(COHSQLE025). Even an already satisfied facet publishes a replacement definition.

### Added columns and publication

ADD CONSTRAINT rejects repeated PRIMARY KEY, UNIQUE or FOREIGN KEY columns, including the
referenced list, with `COHSQLE019`. ADD CONSTRAINT or ADD COLUMN cannot declare a second primary
key over an existing one; repeated PRIMARY KEY clauses on a single added column also fail with
that code. Validation uses case-insensitive column identity and leaves the catalog unchanged.
New CHECKs in either action must qualify columns with this table and schema, or leave them
unqualified; another qualifier reports located `COHSQLE015`. See [Constraints](../constraints.md).

ADD COLUMN admits expression defaults. A nonvolatile expression freezes once for old rows;
a volatile expression is refused on live rows with `COHSQLE027` and admitted on an empty table. Old rows resolve a missing trailing
field from persisted catalog metadata; their bytes and multi-version concurrency control (MVCC)
stamps are unchanged. An explicitly stored `NULL` remains `NULL`. A later update writes the full row.

| Addition | Existing rows | Later insert omitting the column |
| --- | --- | --- |
| Nullable, no default or `DEFAULT NULL` | `NULL` | `NULL` |
| Immutable default | Folded value | Validated declaration value |
| Stable default | One frozen addition-time value | Per-row value using transaction-start clock |
| Volatile default | Refused with `COHSQLE027` when live rows exist | Per-row value if added to an empty table |
| `NOT NULL`, non-null default | Default value | Default value |
| `NOT NULL`, no non-null default | Rejected if currently committed rows exist | Must supply a value if addition succeeds on an empty table |

The emptiness check uses current committed rows. Historical versions visible only to an older
snapshot can retain a missing `NULL` field after a default-free addition to a currently empty table.

ADD [IDENTITY](../clauses/identity.md) requires no live rows, including when old deleted versions
remain. A populated-table ADD fails `COHSQLE025`. Full-definition ALTER preserves identity only
with its existing type, NOT NULL and no DEFAULT; type/nullability/default or generation changes
fail `COHSQLE025`. An explicit NULL conflicting with NOT NULL, PRIMARY KEY or IDENTITY reports
`SQL0003`; ALTER also rejects making an existing primary-key column explicitly NULL.

ADD COLUMN defaults must fit destination bounds. Invalid defaults, invalid conversions, and out-of-range
values fail before schema publication. The complete definition is published atomically; a failed
addition leaves schema, rows, and catalog unchanged. Defaults survive catalog reopen.

Already bound plans retain their immutable definition. Statements planned after the change use
the new definition even when an older row snapshot selects earlier versions. Ordinary table schemas
are not pinned to `BEGIN`; system-view catalog snapshots have their own contract.

DDL is self-committing and fails inside explicit transactions with `COHSQLT003`. Session alteration
of schema-owned tables is refused. DROP COLUMN changes catalog metadata without rewriting rows;
physical ordinals and dropped-column metadata keep existing row values aligned.

Table and dependent-definition changes publish atomically. DML waiting behind a changed definition
must retry; after a table rename it reports the old table missing. SELECT planning retries binding
when catalog publication races it. SQL storage format 25 is current: format 17 introduced this
insert-default/frozen-missing-value split with table extension 6; format 18 separately changes
canonical function-name quoting. Compiled schema version remains 7. Older SQL formats are refused.

### View dependencies

[Persisted views](create-view.md) block table rename and drop/rename of columns they read.
ALTER COLUMN also refuses changes to those columns' type, length, precision or scale with
COHSQLD002. Default and nullability changes retain their usual validation.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE where supplied |
| --- | --- | --- |
| `SQL0003` | Malformed action or DROP NOT NULL on a primary-key column | — |
| `COHDBL001` | Excluded action/list or subquery in DEFAULT | — |
| `COHSQLE013` | Rename collision, invalid default form or incompatible function result type | — |
| `COHSQLE014` / `COHSQLE015` | Missing table / missing column | — |
| `COHSQLE015` | New CHECK names another table or schema | — |
| `COHSQLE019` | Repeated key-list column or second PRIMARY KEY declaration, including ADD COLUMN | — |
| `COHSQLE021` | System relation target or attempted rename over a system relation | 25006 |
| `COHSQLE024` | NOT NULL validation finds NULL | 23502 |
| `COHSQLE025` | Protected identity facet change | — |
| `COHSQLE027` | Volatile ADD COLUMN default requires a rewrite because live rows exist | — |
| `COHSQLE037` | Existing/defaulted rows violate a new PRIMARY KEY, UNIQUE constraint or unique index | 23505 |
| `COHSQLE038` | Existing/defaulted rows violate a CHECK or domain check | 23514 |
| `COHSQLE039` | Existing/defaulted rows lack a referenced foreign-key parent | 23503 |
| `COHSQLD002` | Declared script retains a dependent target for a table rename | — |
| `COHSQLT003` | DDL inside an explicit transaction; the transaction remains usable | — |

Conversion and length failures retain their [normal codes](../diagnostics.md). `COHSQLE027` has no assigned SQLSTATE.

## Examples

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
requests fails with COHSQLE019 and leaves keyed unchanged:

```sql
ALTER TABLE keyed ADD COLUMN other INT PRIMARY KEY;
ALTER TABLE keyed ADD CONSTRAINT uq_bad UNIQUE(id, ID);
```

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
placement. Current format 25 adds distinct predicates, Boolean tests and LIKE ESCAPE.
Every earlier format is refused at open; there is no
upgrade path. Compiled schema format remains version 7.

## See also

[CREATE TABLE](create-table.md) · [DEFAULT](../clauses/default.md) · [Constraints](../constraints.md) · [System views](../system-views/index.md)

[SqlAlterTableExpression](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-sql-language/sql-alter-table-expression.md) · [Statements](index.md) · [Language (SQL)](../index.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAddColumnExecutionTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`
- **Renames and facets** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAlterTableExecutionTests.cs` and `SqlAlterTableRecoveryTests.cs` in the same directory.

