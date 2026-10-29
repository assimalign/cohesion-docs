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

- **`view_name`** — One of the nine relation names below.
- **`select_list`** — Metadata columns or expressions; `*` uses the listed column order.
- **Query clauses** — This abbreviated diagram uses the ordinary supported
  [SELECT](../statements/select.md) grammar.

## Relations

| Relation | Columns in SELECT * order | Rows |
| --- | --- | --- |
| `SCHEMATA` | `CATALOG_NAME`, `SCHEMA_NAME`, `SCHEMA_OWNER`, `DEFAULT_CHARACTER_SET_CATALOG`, `DEFAULT_CHARACTER_SET_SCHEMA`, `DEFAULT_CHARACTER_SET_NAME`, `SQL_PATH` | Persisted namespaces, including empty namespaces and dbo |
| `TABLES` | `TABLE_CATALOG`, `TABLE_SCHEMA`, `TABLE_NAME`, `TABLE_TYPE` | Stored tables (BASE TABLE) and persisted views (VIEW) |
| `COLUMNS` | `TABLE_CATALOG`, `TABLE_SCHEMA`, `TABLE_NAME`, `COLUMN_NAME`, `ORDINAL_POSITION`, `COLUMN_DEFAULT`, `IS_NULLABLE`, `DATA_TYPE`, `CHARACTER_MAXIMUM_LENGTH`, `CHARACTER_OCTET_LENGTH`, `NUMERIC_PRECISION`, `NUMERIC_PRECISION_RADIX`, `NUMERIC_SCALE`, `DATETIME_PRECISION`, `IS_IDENTITY`, `IDENTITY_GENERATION`, `IDENTITY_START`, `IDENTITY_INCREMENT`, `IDENTITY_MAXIMUM`, `IDENTITY_MINIMUM`, `IDENTITY_CYCLE` | One row per stored-table or frozen view column |
| `TABLE_CONSTRAINTS` | `CONSTRAINT_CATALOG`, `CONSTRAINT_SCHEMA`, `CONSTRAINT_NAME`, `TABLE_CATALOG`, `TABLE_SCHEMA`, `TABLE_NAME`, `CONSTRAINT_TYPE`, `IS_DEFERRABLE`, `INITIALLY_DEFERRED` | Primary keys, unique indexes/constraints, foreign keys, and explicit checks |
| `KEY_COLUMN_USAGE` | `CONSTRAINT_CATALOG`, `CONSTRAINT_SCHEMA`, `CONSTRAINT_NAME`, `TABLE_CATALOG`, `TABLE_SCHEMA`, `TABLE_NAME`, `COLUMN_NAME`, `ORDINAL_POSITION` | One row per primary, unique, or foreign-key column, in key order |
| `REFERENTIAL_CONSTRAINTS` | `CONSTRAINT_CATALOG`, `CONSTRAINT_SCHEMA`, `CONSTRAINT_NAME`, `UNIQUE_CONSTRAINT_CATALOG`, `UNIQUE_CONSTRAINT_SCHEMA`, `UNIQUE_CONSTRAINT_NAME`, `MATCH_OPTION`, `UPDATE_RULE`, `DELETE_RULE` | Foreign keys and their referenced key identities |
| `VIEWS` | `TABLE_CATALOG`, `TABLE_SCHEMA`, `TABLE_NAME`, `VIEW_DEFINITION`, `CHECK_OPTION`, `IS_UPDATABLE` | Canonical persisted view definitions; CHECK_OPTION is NONE and IS_UPDATABLE is NO |
| `CHECK_CONSTRAINTS` | `CONSTRAINT_CATALOG`, `CONSTRAINT_SCHEMA`, `CONSTRAINT_NAME`, `CHECK_CLAUSE` | Persisted explicit check expressions |
| `SEQUENCES` | `SEQUENCE_CATALOG`, `SEQUENCE_SCHEMA`, `SEQUENCE_NAME`, `DATA_TYPE`, `NUMERIC_PRECISION`, `NUMERIC_PRECISION_RADIX`, `NUMERIC_SCALE`, `START_VALUE`, `MINIMUM_VALUE`, `MAXIMUM_VALUE`, `INCREMENT`, `CYCLE_OPTION` | Standalone and identity-owned sequence definitions |

## Remarks

SCHEMATA reads the captured namespace list independently of tables. CATALOG_NAME and SCHEMA_NAME
are nonnull String columns; the other five columns are nullable String and currently NULL.
SQL principals, default character sets and SQL paths are not implemented. On a new database,
`SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA;` returns `dbo`.
[CREATE SCHEMA](../statements/create-schema.md) adds rows and [DROP SCHEMA](../statements/drop-schema.md)
removes them; dropping the last table leaves its namespace visible.

These are a minimum viable product (MVP) subset of International Organization for Standardization
(ISO) 9075-11 layouts. No Cohesion-only columns are appended. Virtual relations themselves are not
stored table records in `TABLES`.

Names, descriptions, expressions, and yes/no columns have shared type `String`.
Ordinals, lengths, precision, radix, and scale have type `Int64`; applicable values are
nonnegative and ordinals start at one. All non-null catalog-name fields name the current database.
`IS_NULLABLE` uses `YES`/`NO`; both deferral fields are `NO`.
`MATCH_OPTION` is `NONE`. UPDATE_RULE and DELETE_RULE each report their stored action:
`RESTRICT`, `CASCADE`, `NO ACTION`, `SET NULL` or `SET DEFAULT`. Each omitted SQL action reports
`NO ACTION`. UPDATE_RULE describes the executable [ON UPDATE action](../constraints.md#on-update-declarations).

`COLUMN_DEFAULT` is canonical SQL, with doubled quotes for a string default, or null if absent.
Resolved temporal/binary defaults use typed prefixes and X hex, including defaults declared as
plain strings. Canonical DATA_TYPE names parse in DDL and CAST.
Inapplicable or unknown metadata is null. `CHARACTER_OCTET_LENGTH` is null because the catalog has
no character-set byte bound.

The seven [identity](../clauses/identity.md) fields are appended after DATETIME_PRECISION,
preserving existing ordinals. IS_IDENTITY is `YES`/`NO`; IDENTITY_GENERATION is `ALWAYS` or
`BY DEFAULT`; IDENTITY_CYCLE is `YES` or `NO` from the owned sequence. Identity option fields are nullable String values, including
invariant integer text for START/INCREMENT/MAXIMUM/MINIMUM. Bounds and options reflect the effective owned sequence,
including custom options. Nonidentity columns have NULL option fields; identity COLUMN_DEFAULT is NULL
and IS_NULLABLE is `NO`.

Canonical `DATA_TYPE` values are `BOOLEAN`, `TINYINT`, `SMALLINT`, `INTEGER`,
`BIGINT`, `REAL`, `DOUBLE PRECISION`, `NUMERIC`, `CHARACTER`, `CHARACTER VARYING`,
`BINARY VARYING`, `DATE`, `TIME`, `TIMESTAMP`, `TIMESTAMP WITH TIME ZONE`,
`INTERVAL`, `UUID`, `JSON`, and `JSONB`. Alias spelling is not retained;
type parameters appear in the separate metadata fields where available.

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
persisted read-only views. Their COLUMNS rows use frozen type/length/precision/scale without binding
the body; IS_NULLABLE is always YES and COLUMN_DEFAULT is NULL. CASE, COALESCE and set branches
keep facets only when all types/domains/modifiers agree; NULLIF keeps its first argument's facets.
Metadata enumeration still works when a recoverable body bind failure prevents reading a view.

## Examples

### Sequences

SEQUENCES reads the immutable catalog image used by other system relations. DATA_TYPE is
SMALLINT, INTEGER or BIGINT; precision is 16, 32 or 64, radix 2 and scale 0. START_VALUE,
MINIMUM_VALUE, MAXIMUM_VALUE and INCREMENT are invariant integer text; CYCLE_OPTION is YES/NO.
It exposes definitions, not session currval or the allocator's live cursor.

After `CREATE SEQUENCE s AS SMALLINT START 3 INCREMENT 2 MINVALUE 1 MAXVALUE 99 CACHE 4 CYCLE;`:

```sql
SELECT SEQUENCE_SCHEMA,SEQUENCE_NAME,DATA_TYPE,START_VALUE,INCREMENT,CYCLE_OPTION
FROM INFORMATION_SCHEMA.SEQUENCES WHERE SEQUENCE_NAME='s';
```

Returns `('dbo','s','SMALLINT','3','2','YES')`. See [CREATE SEQUENCE](../statements/create-sequence.md).

Identity-owned generators are listed deliberately, unlike PostgreSQL. Their generated names
(`table_column_seq`, with collision suffixes) support currval lookup. Prepared and abandoned
sequences remain hidden. TINYINT identity sequences have numeric precision 8, radix 2 and scale 0.

### Tables and columns

This dialect example inspects an existing `orders` table:

```sql
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'orders'
ORDER BY ORDINAL_POSITION;
```

It returns no rows if that table is absent.

For a SMALLINT identity declared with `(INCREMENT BY -1)`:

```sql
SELECT IS_IDENTITY, IDENTITY_GENERATION, IDENTITY_START, IDENTITY_INCREMENT,
       IDENTITY_MAXIMUM, IDENTITY_MINIMUM, IDENTITY_CYCLE
FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'descending_ids' AND COLUMN_NAME = 'id';
```

After creating descending_ids with that declaration, the query returns
`(YES, ALWAYS, -1, -1, -1, -32768, NO)`, all as text.

## See also

[System views](index.md) · [COHESION_SCHEMA](cohesion-schema.md) · [Data types](../data-types/index.md)

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
