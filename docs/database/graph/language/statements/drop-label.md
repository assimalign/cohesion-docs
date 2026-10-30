# DROP LABEL

Drops an empty label definition and its child rules and indexes. This is a Cohesion extension.

> **Status:** Implemented.

## Syntax

```syntaxsql
DROP LABEL label_name [ IF EXISTS ]
```

## Remarks

The name uses ordinal, case-sensitive identity. A real drop affects 1; IF EXISTS suppresses
an absent label and affects 0. Existing nodes with the label prevent the drop (CG03003).
An absent unguarded label uses CG01002. Schema-owned definitions are protected.

The [autocommit policy and DDL limits](create-index.md#transactions-and-limits) apply,
including owner-posting validation. CG04025 aborts explicit transactions, even for conditional
no-ops. Invalid limits use CG08021; exhausted work/memory budgets use CG05004/CG05020.
These codes have no assigned SQLSTATE. See [Diagnostics](../diagnostics.md).

## Example

Execute separately in a fresh database:

```sql
CREATE LABEL Empty
DROP LABEL Empty
DROP LABEL Empty IF EXISTS
SHOW LABELS
```

Affected counts are 1, 1, 0. SHOW returns no rows.

## See also

[CREATE LABEL](create-label.md) · [SHOW](show.md) · [Statements](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphSchema.Ddl.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphSchemaDdlTests.cs`.
