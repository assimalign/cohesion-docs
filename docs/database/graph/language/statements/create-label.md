# CREATE LABEL

Creates a label definition without inserting a node. This is a Cohesion extension.

> **Status:** Implemented.

## Syntax

```syntaxsql
CREATE LABEL label_name [ IF NOT EXISTS ]
```

## Remarks

The name is required and uses ordinal, case-sensitive identity. A real create affects 1;
IF NOT EXISTS suppresses an existing label and affects 0. It declares no property rules.
Index and constraint creation can also discover an unseen label automatically.

The [autocommit policy and DDL limits](create-index.md#transactions-and-limits) apply.
An existing unguarded name uses CG03003; explicit-transaction DDL uses CG04025 and aborts
the transaction. Invalid limits use CG08021; exhausted work/memory budgets use CG05004/CG05020.
These codes have no assigned SQLSTATE. See [Diagnostics](../diagnostics.md).

## Example

Execute separately in a fresh database:

```sql
CREATE LABEL Empty
CREATE LABEL Empty IF NOT EXISTS
SHOW LABELS
MATCH (n:Empty) RETURN count(n) AS total
```

Affected counts are 1 then 0. SHOW returns the session database, a generated label GUID and
`Empty`. The count query returns one row, `0`.

## See also

[DROP LABEL](drop-label.md) · [CREATE CONSTRAINT](create-constraint.md) · [Statements](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphSchema.Ddl.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlSchemaDdlParserTests.cs`.
