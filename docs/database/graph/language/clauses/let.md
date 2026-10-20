# LET

Adds value bindings to each incoming row in a GQL stage pipeline.

> **Status:** Implemented.

## Syntax

```syntaxsql
LET variable = expression [ , variable = expression ...n ]
```

## Remarks

Assignments evaluate left to right; later assignments can read earlier ones. LET retains incoming
bindings and adds new names. A statement starts with one empty row, so LET can stand alone before
RETURN. Values may be scalar, NULL, lists, maps or entities. GQL has no separate MISSING value;
missing properties yield NULL. A NULL assignment retains its row.

Names cannot rebind an existing variable (COHDBG003). Unknown inputs use COHDBG001. Member access
on a known scalar fails planning with COHDBG003; an invalid runtime container uses COHDBG013.
These codes have no assigned SQLSTATE. Aggregates belong to RETURN/WITH, not LET.

## Example

```sql
LET x = 1, y = x + 1 RETURN y;
```

Result: one row containing `2`. `LET x = NULL RETURN x;` returns one NULL row.

## See also

[FILTER](filter.md) · [FOR/UNWIND](for.md) · [WITH](with.md) · [NEXT](next.md) · [Clauses](index.md) · [Diagnostics](../diagnostics.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Execution/GraphPlanExecutor.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlLinearCompositionTests.cs`.
