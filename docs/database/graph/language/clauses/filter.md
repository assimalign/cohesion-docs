# FILTER

Filters the current binding table independently of a MATCH clause.

> **Status:** Implemented.

## Syntax

```syntaxsql
FILTER [ WHERE ] predicate
```

## Remarks

Only TRUE retains a row; FALSE and NULL/UNKNOWN remove it. Non-Boolean evaluated values use
COHDBG013. FILTER sees the current stage's bindings, including LET/FOR values. Unlike an OPTIONAL
MATCH WHERE predicate, FILTER runs after the optional stage has already produced its NULL-extended
rows. It can therefore remove those rows. GQL has NULL and no separate MISSING value.

Unknown names, missing parameters and aggregate calls in FILTER use COHDBG001; the codes have
no assigned SQLSTATE. Ordinary expression errors retain their [diagnostics](../diagnostics.md).

## Example

```sql
FOR x IN [1,2,3] FILTER WHERE x > 1 RETURN x;
```

Result: `2`, `3`. `LET x = 1 FILTER UNKNOWN RETURN x;` returns no rows.

## See also

[WHERE](where.md) · [LET](let.md) · [FOR/UNWIND](for.md) · [Clauses](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Execution/GraphPlanExecutor.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlLinearCompositionTests.cs`.
