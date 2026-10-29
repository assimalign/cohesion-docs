# UNION, EXCEPT, INTERSECT and OTHERWISE

Combines read-only results with set/multiset operations or a lazy whole-result fallback.

## Syntax

```syntaxsql
query { UNION [ ALL | DISTINCT ] | EXCEPT [ ALL | DISTINCT ]
      | INTERSECT [ ALL | DISTINCT ] | OTHERWISE } query
( composite_query )
composite_query NEXT next_segment
```

## Remarks

UNION and UNION DISTINCT remove duplicate rows; UNION ALL preserves every occurrence. Chains
associate left. UNION, EXCEPT, INTERSECT and OTHERWISE have equal precedence. Parentheses group
complete operands as a Cohesion extension. Bare UNION and
UNION DISTINCT can mix, but mixing either with UNION ALL anywhere in one composite tree,
including a parenthesized group or across other operators, reports CG00016. EXCEPT/INTERSECT
default to DISTINCT; each may independently use ALL or DISTINCT. Otherwise mixed operators are
allowed. OTHERWISE takes no quantifier. These operator words remain contextual in binding and
scalar positions.

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

### EXCEPT and INTERSECT

The operators compare complete tuples with the same structural equality as UNION, including
equal NULLs and compatible numeric representations. GQL has no MISSING value.

| Operator | Copies of a row with left count m and right count n |
| --- | --- |
| EXCEPT / EXCEPT DISTINCT | 1 when m > 0 and n = 0; otherwise 0 |
| EXCEPT ALL | max(m - n, 0) |
| INTERSECT / INTERSECT DISTINCT | 1 when m > 0 and n > 0; otherwise 0 |
| INTERSECT ALL | min(m, n) |

Both read the left operand, then the right, and validate runtime shapes on both even if the
result is provably empty. Eligible occurrences come from the start of the left result, keeping
their original values and relative order. DISTINCT keeps the first representative. No partial
result publishes on a runtime failure.

### OTHERWISE

OTHERWISE returns the **entire left result** when it has any row, preserving duplicates and NULL
rows. Only a zero-row left result opens the right execution cursor. A left error propagates,
including an error after earlier left rows; it never triggers fallback. Both branches still
participate in static name, parameter, shape, metadata and wire validation before execution.
This whole-result fallback is Cohesion-specific.

### Precedence and paging

`A UNION B INTERSECT C` means `(A UNION B) INTERSECT C`, and
`A OTHERWISE B UNION ALL C` means `(A OTHERWISE B) UNION ALL C`. Parentheses change the grouping.

[ORDER BY](order-by.md), [OFFSET/SKIP and LIMIT](limit-offset.md) belong to each operand's RETURN.
A tail after the final RETURN affects only that operand. A suffix after a grouped composite is
CK00001; there is no global composite ordering or page clause.

### NEXT

Every composite operator binds more tightly than [NEXT](next.md). Each NEXT segment can itself be a composite,
whose operands all receive an isolated view of the preceding segment's complete projected
binding table. A NEXT inside a parenthesized operand stays inside that operand. Later segments
consume the combined projected bindings; named paths retain their kind, and quoted names carry
their decoded binding names. The final composite also reconciles types and nullability.

### Snapshot, budgets and EXPLAIN

All operands share one statement snapshot, cancellation and work/memory ceilings. UNION distinct
state uses the shared memory grant (CG05030); ordinary operand/output retention and work retain
CG05020/CG05004. EXCEPT/INTERSECT retain left rows and structural hash counts, charged with
their execution frames to CG05032. Each input hash visit and retained-row output visit spends a
work unit in addition to ordinary operand work. A binary operation has expected O(m+n) work and
O(m) retained state; DISTINCT retains O(distinct left rows). Nested operators own separate grants.
In a mixed tree, every parent hash visit or forwarded row spends work at each visited level,
including discarded UNION duplicates and UNION ALL/OTHERWISE forwarding. Forwarding and ordinary
operand frames retain CG05020; UNION frames retain CG05030. UNION ALL drains operand
cursors without a distinct table or intermediate per-operand result set. Final results materialize
under the existing API contract. Failure and cancellation dispose all cursors and reservations.

All operands are planned and statically admitted for wire delivery before execution, even empty
ones. Composite values retain graph.composite-values negotiation (CG07016), and final entity
lists retain CG05001 until #1662. No format or protocol version changes.
[EXPLAIN](../statements/explain.md) preserves binary grouping with Union/UnionAll,
Except/ExceptAll, Intersect/IntersectAll and Otherwise nodes and
every operand's complete plan, including correlated subplans, without executing data or callbacks.

## Diagnostics

These codes have no assigned SQLSTATE.

| Code | Meaning and trigger |
| --- | --- |
| CG00015 | Missing composite operand, including ALL/DISTINCT after OTHERWISE |
| CG00016 | Mixed UNION distinct and UNION ALL modes in one composite tree |
| CG01020 | Operand column names, order or count differ |
| CG02017 | Proven or runtime result shapes are incompatible |
| CG05029 | Modifying operand or stage in a composite pipeline |
| CG05030 | UNION duplicate-elimination state exceeds the shared memory grant |
| CG05031 | Internal statement lookup unexpectedly lacks its key; original KeyNotFoundException is retained |
| CG05032 | EXCEPT/INTERSECT retained frames, rows or hash state exceed the shared memory grant |
| CG05020 | Other retained statement memory exceeds its grant |
| CG05004 | Shared statement work ceiling exceeded |
| CK00001 | Global ordering/page suffix after a grouped composite |

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

Each following statement also needs no stored graph data:

```sql
FOR x IN [1,1,1,2,3,NULL,NULL] RETURN x
EXCEPT ALL FOR x IN [1,1,2,2,NULL] RETURN x;
FOR x IN [1,1,1,2,3,NULL,NULL] RETURN x
INTERSECT ALL FOR x IN [1,1,2,2,NULL] RETURN x;
RETURN 1 AS x UNION RETURN 2 AS x INTERSECT RETURN 2 AS x;
RETURN 1 AS x UNION (RETURN 2 AS x INTERSECT RETURN 2 AS x);
FOR x IN [] RETURN x OTHERWISE FOR x IN [7,7] RETURN x;
FOR x IN [1,1,NULL] RETURN x OTHERWISE RETURN 9 AS x;
```

Results: `1, 3, NULL`; `1, 1, 2, NULL`; `2`; `1, 2`; `7, 7`; and `1, 1, NULL`.
Replacing ALL with DISTINCT in the first two queries returns `3` and `1, 2, NULL` respectively.

## See also

[Clauses](index.md) · [RETURN](return.md) · [NEXT](next.md) · [GqlCompositeQuery](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph-language/gql-composite-query.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphUnionTests.cs`, `GraphUnionExecutionTests.cs` and `GraphUnionSecondPassTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Execution/GraphPlanExecutor.Composite.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlCompositeQueryParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphSetQueryTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlSetQueryParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphSetQueryWireTests.cs`.
