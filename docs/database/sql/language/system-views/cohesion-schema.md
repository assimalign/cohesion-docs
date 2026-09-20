# COHESION_SCHEMA (Cohesion SQL)

Exposes Cohesion-specific index-key and object-ownership metadata.

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

- **`view_name`** — `INDEXES` or `OBJECT_OWNERSHIP`.
- **`select_list`** — Metadata columns or expressions; `*` uses the listed column order.
- **Query clauses** — This abbreviated diagram uses the ordinary supported
  [SELECT](../statements/select.md) grammar.

## Relations

| Relation | Columns in SELECT * order | Rows |
| --- | --- | --- |
| `INDEXES` | `TABLE_CATALOG`, `TABLE_SCHEMA`, `TABLE_NAME`, `INDEX_NAME`, `COLUMN_NAME`, `ORDINAL_POSITION`, `IS_UNIQUE`, `IS_PRIMARY_KEY` | One row per ordered index-key column |
| `OBJECT_OWNERSHIP` | `TABLE_CATALOG`, `TABLE_SCHEMA`, `TABLE_NAME`, `OBJECT_TYPE`, `OBJECT_NAME`, `OWNER`, `OWNING_SCHEMA` | One row per table or index |

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
Joins involving these relations remain excluded.

A snapshot-isolation transaction retains its catalog capture from transaction begin.
Auto-commit and `ReadCommitted` statements capture at statement start. Each capture includes
tables and indexes together, preventing partially observed DDL. Fresh captures after a table drop
contain none of that table's related metadata. These catalog snapshots differ from the ordinary
table-definition binding rule described under [ALTER TABLE](../statements/alter-table.md).

Data manipulation and table/index DDL targeting the named relations fail with
`System view '<SCHEMA>.<VIEW>' is read-only.`, including colliding creates and
`IF [NOT] EXISTS` forms. The wire error is `ExecutionFailure`.
User-defined `CREATE VIEW` and `DROP VIEW` remain unsupported.

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

## See also

[System views](index.md) · [INFORMATION_SCHEMA](information-schema.md) · [CREATE INDEX](../statements/create-index.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Catalog snapshots and query behavior** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`
- **Persistence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/docs/DESIGN.md`
- **Execution tests** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSystemViewTests.cs`
- **Wire tests** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSystemViewProtocolTests.cs`

