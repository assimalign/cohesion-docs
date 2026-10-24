# WITH

Projects an intermediate binding table using the Cypher compatibility spelling.

> **Status:** Implemented. Includes DISTINCT, grouping, ordering, paging and WHERE.

## Syntax

```syntaxsql
WITH [ ALL | DISTINCT ] { * | expression [ AS alias ] [ , ...n ] }
[ GROUP BY { grouping_name_or_expression [ , ...n ] | () } ]
[ ORDER BY sort_item [ , ...n ] ]
[ { OFFSET | SKIP } count ] [ LIMIT count ]
[ WHERE predicate ]
```

## Scope and stage order

WITH uses RETURN's grouping, DISTINCT, ordering and count rules, then applies WHERE **after
ORDER BY, OFFSET and LIMIT**. Only aliased items and bare-variable projections introduce names
for subsequent stages. Unaliased computed items are accepted but cannot be referenced afterward.
Star carries all current bindings in scope order, including LET/FOR values. Duplicate carried
names use decoded variable names, so grouping/quoting does not make a second binding distinct.
Entity kinds, optionality, possible composite values and Int64 COUNT metadata survive the boundary.

Without DISTINCT or aggregation, WITH ORDER BY and WHERE can see projected aliases and incoming
bindings; aliases shadow incoming names. With DISTINCT or aggregation, WHERE sees only projected
bindings. An aggregate call in WHERE is invalid even when repeated from the projection; use its
alias. ORDER BY retains its grouped/projected-expression rules. Every projection stage has its
own validation scope; a previous stage's aliases cannot validate a later stage.

NULL properties and expression results follow ordinary GQL rules; WHERE keeps only TRUE.
WITH does not finish a query and cannot be followed immediately by NEXT. Continue with another
stage and eventually RETURN or a write. Each stage materializes eagerly in source order.

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| CG05001 | Hidden/unbound name, aggregate in WHERE or invalid grouping/stage shape |
| CG01015 | Duplicate carried binding, including under EXPLAIN and on empty input |
| CG02013 | Evaluated non-Boolean predicate or invalid paging count |
| CG00002 | WITH followed immediately by NEXT or malformed clause order |

These codes have no assigned SQLSTATE. See [Diagnostics](../diagnostics.md).

## Examples

```sql
FOR x IN [3,1,2] WITH x ORDER BY x OFFSET 1 LIMIT 1 WHERE x > 1 RETURN x;
```

Result: `2`. Replacing OFFSET 1 LIMIT 1 with LIMIT 1 returns no rows: paging selects `1`, then
WHERE removes it.

```sql
LET x = 1 WITH x AS y WHERE x = 1 RETURN y;
```

Result: `1`. Adding DISTINCT makes the reference to incoming `x` in WHERE fail with CG05001;
`y` remains visible. `FOR x IN [1,1.0,2] WITH DISTINCT x RETURN x;` returns `1`, `2`.

## See also

[RETURN](return.md) · [NEXT](next.md) · [GROUP BY](group-by.md) · [ORDER BY](order-by.md) · [Clauses](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Planning/GraphPlanner.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Execution/GraphPlanExecutor.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlLinearCompositionTests.cs`.
