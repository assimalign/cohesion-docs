# UNION and UNION ALL

Combines read-only query results with or without duplicate removal.

## Syntax

```syntaxsql
query { UNION [ DISTINCT ] | UNION ALL } query
( composite_query )
composite_query NEXT next_segment
```

## Remarks

UNION and UNION DISTINCT remove duplicate rows; UNION ALL preserves every occurrence. Chains
associate left. Parentheses group complete operands as a Cohesion extension. Bare UNION and
UNION DISTINCT can mix, but mixing either with UNION ALL anywhere in one composite tree,
including a parenthesized group, reports CG00016. EXCEPT, INTERSECT and OTHERWISE remain
CK00001 until GQ-Q2.

Every operand must be read-only and return the same case-sensitive column names, count and
order. A composite pipeline containing a modifying stage is refused before any cursor opens
(CG05029). Each operand has an isolated lexical scope; names introduced in one do not escape
to another. WITH stays within its operand. Missing names and parameters still validate in every
operand, including empty or unreachable inputs.

### Columns and types

Column names come from the first operand. Proven incompatible result shapes fail planning;
unknown property kinds are checked as rows arrive, before results publish (CG02017).
Numeric kinds are compatible and NULL is kind-neutral. Lists/maps keep the heterogeneous
collection-value contract; proven incompatible nested entity kinds are refused. GQL has no MISSING.

Metadata reconciles every operand, including values later removed as duplicates. Integral kinds
widen to the largest kind; floating/integral mixtures use Float64 and decimal/integral mixtures
use Decimal. Decimal/floating mixtures and dynamic kinds use DatabaseType.Null (unknown).
Actual values retain their CLR types; metadata reconciliation does not coerce cells.

### Duplicate removal and ordering

UNION compares complete tuples using RETURN DISTINCT equality: NULLs group together, numeric
representations compare by exact represented value, strings use ordinal equality, lists preserve
element order, maps compare entries, and entities/paths compare their identities and path order.
It retains the first structural occurrence, including that representative's runtime values.
Operands are visited in source order. Use explicit ordering where order is part of the application contract.

[ORDER BY](order-by.md), [OFFSET/SKIP and LIMIT](limit-offset.md) belong to each operand's RETURN.
A tail after the final RETURN affects only that operand. A suffix after a grouped composite is
CK00001; there is no global composite ordering or page clause.

### NEXT

UNION binds more tightly than [NEXT](next.md). Each NEXT segment can itself be a composite,
whose operands all receive an isolated view of the preceding segment's complete projected
binding table. A NEXT inside a parenthesized operand stays inside that operand. Later segments
consume the combined projected bindings; named paths retain their kind, and quoted names carry
their decoded binding names. The final composite also reconciles types and nullability.

### Snapshot, budgets and EXPLAIN

All operands share one statement snapshot, cancellation and work/memory ceilings. UNION distinct
state uses the shared memory grant (CG05030); ordinary operand/output retention and work retain
CG05020/CG05004. Each submitted composite row spends one work unit. UNION ALL drains operand
cursors without a distinct table or intermediate per-operand result set. Final results materialize
under the existing API contract. Failure and cancellation dispose all cursors and reservations.

All operands are planned and statically admitted for wire delivery before execution, even empty
ones. Composite values retain graph.composite-values negotiation (CG07016), and final entity
lists retain CG05001 until #1662. No format or protocol version changes.
[EXPLAIN](../statements/explain.md) preserves binary grouping with Union/UnionAll nodes and
every operand's complete plan, including correlated subplans, without executing data or callbacks.

## Diagnostics

These codes have no assigned SQLSTATE.

| Code | Meaning and trigger |
| --- | --- |
| CG00015 | Missing composite query operand |
| CG00016 | Mixed UNION distinct and UNION ALL modes in one composite tree |
| CG01020 | Operand column names, order or count differ |
| CG02017 | Proven or runtime result shapes are incompatible |
| CG05029 | Modifying operand or stage in a composite pipeline |
| CG05030 | UNION duplicate-elimination state exceeds the shared memory grant |
| CG05031 | Internal statement lookup unexpectedly lacks its key; original KeyNotFoundException is retained |
| CG05020 | Other retained statement memory exceeds its grant |
| CG05004 | Shared statement work ceiling exceeded |
| CK00001 | EXCEPT, INTERSECT, OTHERWISE or a global ordering/page suffix |

## Examples

Each statement runs independently without stored graph data:

```sql
RETURN 1 AS x UNION DISTINCT RETURN 1.0 AS x;
RETURN 1 AS x UNION ALL RETURN 1 AS x;
FOR x IN [3,1,2] RETURN x ORDER BY x SKIP 1 LIMIT 1
UNION ALL FOR x IN [9,8] RETURN x ORDER BY x LIMIT 1;
RETURN 1 AS x UNION ALL RETURN 2 AS x NEXT RETURN x*10 AS x;
```

The results are one row containing Int64 `1` (the first representative), two rows containing `1`,
rows `2` then `8`, and rows `10` then `20`, respectively.

## See also

[Clauses](index.md) · [RETURN](return.md) · [NEXT](next.md) · [GqlCompositeQuery](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph-language/gql-composite-query.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphUnionTests.cs`, `GraphUnionExecutionTests.cs` and `GraphUnionSecondPassTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Execution/GraphPlanExecutor.Composite.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlCompositeQueryParserTests.cs`.
