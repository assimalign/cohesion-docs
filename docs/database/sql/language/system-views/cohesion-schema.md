# COHESION_SCHEMA (Cohesion SQL)

Exposes Cohesion-specific index, ownership, function and type metadata.

> **Status:** Partial. The listed virtual relations execute; complete standard metadata layouts are not claimed.

## Syntax

```syntaxsql
SELECT select_list
FROM COHESION_SCHEMA.view_name
[ WHERE search_condition ]
[ ORDER BY order_by_item [ , ...n ] ]
[ ; ]
```

## Arguments

- **`view_name`** — `INDEXES`, `OBJECT_OWNERSHIP`, `FUNCTIONS`, `TYPES` or `DOMAINS`.
- **`select_list`** — Metadata columns or expressions; `*` uses the listed column order.
- **Query clauses** — This abbreviated diagram uses the ordinary supported
  [SELECT](../statements/select.md) grammar.

## Relations

| Relation | Columns in SELECT * order | Rows |
| --- | --- | --- |
| `INDEXES` | `TABLE_CATALOG`, `TABLE_SCHEMA`, `TABLE_NAME`, `INDEX_NAME`, `COLUMN_NAME`, `ORDINAL_POSITION`, `IS_UNIQUE`, `IS_PRIMARY_KEY` | One row per ordered index-key column |
| `OBJECT_OWNERSHIP` | `TABLE_CATALOG`, `TABLE_SCHEMA`, `TABLE_NAME`, `OBJECT_TYPE`, `OBJECT_NAME`, `OWNER`, `OWNING_SCHEMA` | One row per table or index |
| `FUNCTIONS` | `FUNCTION_NAME`, `FUNCTION_KIND`, `PARAMETER_TYPES`, `PARAMETER_COUNT`, `RETURN_TYPE`, `VOLATILITY`, `NULL_BEHAVIOR`, `IS_BUILT_IN`, `IS_SQL_DEFINED` | One row per overload or special form |
| `TYPES` | `TYPE_NAME`, `BASE_TYPE`, `IS_DOMAIN`, `IS_SQL_DEFINED` | Native types/domains and SQL-defined domains |
| `DOMAINS` | `DOMAIN_NAME`, `BASE_TYPE`, `IS_SQL_DEFINED` | Native and SQL-defined domains |

## Remarks

`INDEXES` is an explicit Cohesion extension because the standard information schema does not
supply a general index relation. It exposes key-column order rather than physical root pages.
`ORDINAL_POSITION` is one-based `Int64`; names and flag values are `String`.
`IS_UNIQUE` and `IS_PRIMARY_KEY` contain `YES` or `NO`.

`OBJECT_TYPE` is `TABLE` or `INDEX`. `OWNER` is `Adhoc` or `Schema`.
`OWNING_SCHEMA` is the compiled schema name for schema-owned objects and null for ad-hoc objects;
it is distinct from the SQL namespace in `TABLE_SCHEMA`. All non-null catalog names identify the
session's current database.

Ordinary sessions can change rows in schema-owned tables but cannot use session DDL to alter/drop
those tables or drop schema-owned indexes. Owning-schema deployment controls those changes.

System relations are virtual, read-only projections of the current database catalog. Names are
case-insensitive and must include their schema. Supported SELECT projection, aliases, parameters,
filtering, ordering, distinct results, aggregation, and pagination work over the wire as well.
Supported N-way INNER/LEFT/CROSS and comma joins accept stored or system inputs, including USING.

A snapshot-isolation transaction retains its catalog capture from transaction begin.
Auto-commit and `ReadCommitted` statements capture at first system-relation binding. Each capture includes
tables and indexes together, preventing partially observed DDL. Fresh captures after a table drop
contain none of that table's related metadata. These catalog snapshots differ from the ordinary
table-definition binding rule described under [ALTER TABLE](../statements/alter-table.md).

Data manipulation and table/index DDL targeting the named relations fail with
`System view '<SCHEMA>.<VIEW>' is read-only.`, including colliding creates and
`IF [NOT] EXISTS` forms. The wire error is `ExecutionFailure`.
[CREATE VIEW](../statements/create-view.md) and [DROP VIEW](../statements/drop-view.md) manage
persisted read-only views.

All five extension relations join through the same binder, including the function/type/domain
relations. SQL-defined entries read the statement catalog image; native registrations are frozen
at engine build.

FUNCTIONS includes the executable T1 scalar overloads. Stable clocks and Volatile GEN_RANDOM_UUID
have their declared volatility; nextval, currval, setval and lastval are Volatile BIGINT overloads.
COALESCE, NULLIF, GROUPING, GROUPING_ID, CASE, CAST and EXTRACT appear as SPECIAL FORM
rows with type/count/volatility/NULL-behavior fields NULL. This distinguishes evaluator forms from
catalog functions. Renames expose the new table/column names without changing generated index names.

[GROUPING/GROUPING_ID](../functions/grouping.md) have no independently invocable scalar callback.
See [sequence functions](../functions/sequences.md) for session observations and allocation effects.

## Examples

This example adapts the conformance index setup:

```sql
CREATE TABLE t (id INT PRIMARY KEY, name TEXT, age INT);
CREATE INDEX ix_age ON t(age);
SELECT INDEX_NAME, COLUMN_NAME, ORDINAL_POSITION
FROM COHESION_SCHEMA.INDEXES
WHERE TABLE_NAME = 't'
ORDER BY INDEX_NAME, ORDINAL_POSITION;
```

The result includes `ix_age` and the table's primary-key enforcement index.

OBJECT_OWNERSHIP also includes [persisted views](../statements/create-view.md): session-created
views are Adhoc and declared-script views use Schema ownership.

## See also

[System views](index.md) · [INFORMATION_SCHEMA](information-schema.md) · [CREATE INDEX](../statements/create-index.md)

## Joining and snapshots

These read-only relations join with each other or with stored tables through the supported
[N-way INNER/LEFT/CROSS and comma joins](../clauses/join.md). Every system relation in a statement, including
subqueries and INSERT SELECT sources, reads one immutable catalog image. Snapshot transactions
use their BEGIN-time image; automatic and ReadCommitted statements capture it at first system
relation binding. Mutations report `CS04021` (SQLSTATE 25006).

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Catalog snapshots and query behavior** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`
- **Persistence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`
- **Execution tests** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSystemViewTests.cs`
- **Wire tests** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSystemViewProtocolTests.cs`
