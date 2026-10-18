# DROP SCHEMA (Cohesion SQL)

Removes a persisted SQL namespace, optionally removing its dependent objects atomically.

> **Status:** Implemented for the documented subset.

## Syntax

```syntaxsql
DROP SCHEMA [ IF EXISTS ] schema_name [ RESTRICT | CASCADE ] [ ; ]
```

## Remarks

RESTRICT is the default and refuses a namespace containing tables. CASCADE drops its contained
tables and indexes in one catalog publication, including identity state. If a table outside the
namespace has an FK into it, CASCADE removes that FK and preserves the outside table and its data.
Ownership checks cover the namespace, contained objects and affected outside tables. A refusal,
conflict or cancellation before publication leaves the complete catalog intact.

IF EXISTS accepts an absent namespace without publishing a change. It does not bypass protection
of `dbo`, reserved virtual namespaces or schema-owned objects. SQL cannot drop `dbo`, because
code-first provisioning requires it. DDL is self-committing, returns affected count zero and is
refused in an explicit transaction. Dropping a table does not drop its namespace.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| COHSQLE028 | Namespace absent without IF EXISTS | 3F000 |
| COHSQLE030 | RESTRICT finds contained objects; message offers CASCADE | 2BP01 |
| COHSQLE031 | DROP targets dbo, including IF EXISTS and CASCADE | — |
| COHSQLE021 | DROP targets INFORMATION_SCHEMA or COHESION_SCHEMA | 25006 |
| COHSQLE013 | Namespace or dependency set changed while DROP waited; retry the statement | — |
| COHSQLT003 | DDL inside an explicit transaction | — |

Schema ownership refusals use DatabaseObjectLockedException. See [Diagnostics](../diagnostics.md).

## Example

```sql
CREATE SCHEMA sales;
CREATE TABLE sales.parent (id INT PRIMARY KEY);
CREATE TABLE external_child (id INT REFERENCES sales.parent(id));
INSERT INTO sales.parent VALUES (1);
INSERT INTO external_child VALUES (1);
DROP SCHEMA sales CASCADE;
SELECT id FROM external_child;
```

The query returns `1`: the outside table survives and its FK is removed. SCHEMATA no longer lists
sales, and sales.parent is absent. Replacing CASCADE with RESTRICT fails with COHSQLE030 and
preserves every object.

## See also

[CREATE SCHEMA](create-schema.md) · [DROP TABLE](drop-table.md) · [Constraints](../constraints.md) · [Statements](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/SqlPlanExecutor.Schemas.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSchemaExecutionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSchemaExecutionTests.Recovery.cs`.
