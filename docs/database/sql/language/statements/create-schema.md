# CREATE SCHEMA (Cohesion SQL)

Creates a persisted SQL namespace in the session database, including an empty namespace.

> **Status:** Implemented. Supports the documented subset.

## Syntax

```syntaxsql
CREATE SCHEMA [ IF NOT EXISTS ] schema_name [ ; ]
```

## Remarks

Names are unqualified and compared without case; the catalog retains the declaration's spelling.
Unqualified table names use `dbo`. To create a table elsewhere, first create its namespace and
then qualify the table name. An unqualified REFERENCES target uses the child table's namespace.

Every new database seeds `dbo`. Code-first provisioning remains pinned to `dbo`; creating another
namespace does not change that target. A SQL namespace is distinct from a compiled .NET schema
and its ownership identity. Namespaces survive restart even when empty.

IF NOT EXISTS accepts an existing namespace without publishing a change. Successful schema DDL
returns affected count zero. DDL is self-committing and is refused in explicit transactions.
Storage format 25 persists namespace records of kind 15, introduced in format 21. Every earlier format is refused at open;
there is no upgrade path.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| COHSQLE029 | Namespace already exists without IF NOT EXISTS | 42P06 |
| COHSQLE028 | Table/index DDL names a namespace that does not exist | 3F000 |
| COHSQLE021 | CREATE targets INFORMATION_SCHEMA or COHESION_SCHEMA, including existence guards | 25006 |
| COHDBL001 | AUTHORIZATION, with or without an explicit name, or nested CREATE/GRANT schema elements | — |
| COHSQLT003 | DDL inside an explicit transaction | — |

Reads and DML against a missing relation retain COHSQLE014 (42P01).
DROP TABLE IF EXISTS missing.t remains a no-op. See [Diagnostics](../diagnostics.md).

## Example

On a new database:

```sql
CREATE SCHEMA sales;
CREATE TABLE Sales.t (id INT);
INSERT INTO sales.t VALUES (7);
SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA ORDER BY SCHEMA_NAME;
```

The query returns `dbo`, then `sales`. `SELECT id FROM SALES.t;` returns `7`.

## See also

[DROP SCHEMA](drop-schema.md) · [CREATE TABLE](create-table.md) · [INFORMATION_SCHEMA](../system-views/information-schema.md) · [Statements](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Ddl.Schemas.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSchemaExecutionTests.cs`.
