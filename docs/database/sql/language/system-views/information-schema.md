# INFORMATION_SCHEMA (Cohesion SQL)

Exposes the measured subset of standard table, column, and constraint metadata.

> **Status:** Partial. The listed virtual relations execute; complete standard metadata layouts are not claimed.

## Syntax

```syntaxsql
SELECT select_list
FROM INFORMATION_SCHEMA.view_name
[ WHERE search_condition ]
[ ORDER BY order_by_item [ , ...n ] ]
[ ; ]
```

## Arguments

- **`view_name`** — One of the six relation names below.
- **`select_list`** — Metadata columns or expressions; `*` uses the listed column order.
- **Query clauses** — This abbreviated diagram uses the ordinary supported
  [SELECT](../statements/select.md) grammar.

## Relations

| Relation | Columns in SELECT * order | Rows |
| --- | --- | --- |
| `TABLES` | `TABLE_CATALOG`, `TABLE_SCHEMA`, `TABLE_NAME`, `TABLE_TYPE` | Stored tables; type is `BASE TABLE` |
| `COLUMNS` | `TABLE_CATALOG`, `TABLE_SCHEMA`, `TABLE_NAME`, `COLUMN_NAME`, `ORDINAL_POSITION`, `COLUMN_DEFAULT`, `IS_NULLABLE`, `DATA_TYPE`, `CHARACTER_MAXIMUM_LENGTH`, `CHARACTER_OCTET_LENGTH`, `NUMERIC_PRECISION`, `NUMERIC_PRECISION_RADIX`, `NUMERIC_SCALE`, `DATETIME_PRECISION` | One row per stored column |
| `TABLE_CONSTRAINTS` | `CONSTRAINT_CATALOG`, `CONSTRAINT_SCHEMA`, `CONSTRAINT_NAME`, `TABLE_CATALOG`, `TABLE_SCHEMA`, `TABLE_NAME`, `CONSTRAINT_TYPE`, `IS_DEFERRABLE`, `INITIALLY_DEFERRED` | Primary keys, unique indexes/constraints, foreign keys, and explicit checks |
| `KEY_COLUMN_USAGE` | `CONSTRAINT_CATALOG`, `CONSTRAINT_SCHEMA`, `CONSTRAINT_NAME`, `TABLE_CATALOG`, `TABLE_SCHEMA`, `TABLE_NAME`, `COLUMN_NAME`, `ORDINAL_POSITION` | One row per primary, unique, or foreign-key column, in key order |
| `REFERENTIAL_CONSTRAINTS` | `CONSTRAINT_CATALOG`, `CONSTRAINT_SCHEMA`, `CONSTRAINT_NAME`, `UNIQUE_CONSTRAINT_CATALOG`, `UNIQUE_CONSTRAINT_SCHEMA`, `UNIQUE_CONSTRAINT_NAME`, `MATCH_OPTION`, `UPDATE_RULE`, `DELETE_RULE` | Foreign keys and their referenced key identities |
| `CHECK_CONSTRAINTS` | `CONSTRAINT_CATALOG`, `CONSTRAINT_SCHEMA`, `CONSTRAINT_NAME`, `CHECK_CLAUSE` | Persisted explicit check expressions |

## Remarks

These are a minimum viable product (MVP) subset of International Organization for Standardization
(ISO) 9075-11 layouts. No Cohesion-only columns are appended. Virtual relations themselves are not
stored table records in `TABLES`.

Names, descriptions, expressions, and yes/no columns have shared type `String`.
Ordinals, lengths, precision, radix, and scale have type `Int64`; applicable values are
nonnegative and ordinals start at one. All non-null catalog-name fields name the current database.
`IS_NULLABLE` uses `YES`/`NO`; both deferral fields are `NO`.
`MATCH_OPTION` is `NONE`, `UPDATE_RULE` is `RESTRICT`, and `DELETE_RULE`
is `RESTRICT` or `CASCADE`.

`COLUMN_DEFAULT` is SQL literal text, with doubled quotes for a string default, or null if absent.
Inapplicable or unknown metadata is null. `CHARACTER_OCTET_LENGTH` is null because the catalog has
no character-set byte bound.

Canonical `DATA_TYPE` values are `BOOLEAN`, `TINYINT`, `SMALLINT`, `INTEGER`,
`BIGINT`, `REAL`, `DOUBLE PRECISION`, `NUMERIC`, `CHARACTER VARYING`,
`BINARY VARYING`, `DATE`, `TIME`, `TIMESTAMP`, `TIMESTAMP WITH TIME ZONE`,
`INTERVAL`, `UUID`, `JSON`, and `JSONB`. Alias spelling is not retained;
type parameters appear in the separate metadata fields where available.

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

This dialect example inspects an existing `orders` table:

```sql
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'orders'
ORDER BY ORDINAL_POSITION;
```

It returns no rows if that table is absent.

## See also

[System views](index.md) · [COHESION_SCHEMA](cohesion-schema.md) · [Data types](../data-types/index.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Catalog snapshots and query behavior** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`
- **Persistence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Catalog/docs/DESIGN.md`
- **Execution tests** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSystemViewTests.cs`
- **Wire tests** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSystemViewProtocolTests.cs`

