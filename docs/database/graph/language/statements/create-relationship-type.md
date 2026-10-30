# CREATE RELATIONSHIP TYPE

Creates a relationship-type definition without inserting an edge. This is a Cohesion extension.

> **Status:** Implemented.

## Syntax

```syntaxsql
CREATE RELATIONSHIP TYPE type_name [ IF NOT EXISTS ]
```

## Remarks

The name is required and uses ordinal, case-sensitive identity. A real create affects 1;
IF NOT EXISTS suppresses an existing relationship type and affects 0. It declares no property
rules. Constraint creation can also discover an unseen relationship type automatically.
Label and relationship-type names occupy separate namespaces.

The [autocommit policy and DDL limits](create-index.md#transactions-and-limits) apply.
An existing unguarded name uses CG03003; CG04025 aborts explicit transactions, even for
conditional no-ops. Invalid limits use CG08021; exhausted work/memory budgets use CG05004/CG05020.
These codes have no assigned SQLSTATE. See [Diagnostics](../diagnostics.md).

## Example

Execute separately in a fresh database:

```sql
CREATE RELATIONSHIP TYPE KNOWS
CREATE RELATIONSHIP TYPE KNOWS IF NOT EXISTS
SHOW RELATIONSHIP TYPES
```

Affected counts are 1 then 0. SHOW returns the session database, a generated type GUID and
`KNOWS`; no relationship is inserted.

## See also

[DROP RELATIONSHIP TYPE](drop-relationship-type.md) · [CREATE CONSTRAINT](create-constraint.md) · [Statements](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphSchema.Ddl.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlSchemaDdlParserTests.cs`.
