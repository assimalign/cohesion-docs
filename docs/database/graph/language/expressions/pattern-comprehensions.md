# Pattern comprehensions

Collects a projection for every match of a correlated path pattern into one ordered list per outer row.

## Syntax

```syntaxsql
[ [ path_variable = ] pattern [ WHERE predicate ] | projection ]
```

The outer square brackets and projection bar are literal syntax. The pattern must contain a
relationship chain; the projection is required. Fixed chains and the existing ISO and Cypher
[relationship quantifiers](../patterns.md#quantified-paths) are accepted, including abbreviated
connectors such as `->{1,2}`, `->+` and `->*`. A zero-hop quantifier still counts as a chain.
Explicit path modes, selectors and parenthesized quantified path groups are outside this form.
Map comprehensions and allReduce are outside the completed #1736 collection-expression work.

## Remarks

Matches retain MATCH traversal order and duplicates. Only TRUE passes WHERE; FALSE and NULL
discard a match without evaluating its projection. NULL projections remain elements. Zero
matches, including a correlated NULL node or relationship, return `[]`. GQL has no MISSING value.

Existing pattern names correlate with enclosing bindings. New node and relationship names stay
local. The optional path variable is always local and can shadow an outer name; it names the
complete matched path inside the predicate and projection. Inner declarations do not escape.
Names, entity kinds and known predicate types validate even for empty matches and EXPLAIN.

The expression works in RETURN, WHERE, ORDER BY, scalar SET values, aggregate operands, nested
pattern/list comprehensions, list predicates and reduce. For example, an outer
`sum(size([(a)->(b) | b.k]))` can aggregate list lengths. An aggregate over the inner matches,
such as `[(a)->(b) | count(*)]`, is refused. Stored properties remain scalar-only; use a scalar
result such as size rather than assigning the list itself.

Each evaluation collects all inner matches on the statement's operation, snapshot, update
overlay, parameters and shared work/memory budgets. Cancellation and failures dispose the inner
cursor and release temporary storage; statement failures roll back writes. EXPLAIN shows
CollectApply, correlated names, the projection and inner plan.

## Results and wire capability

In-process lists can contain nodes, relationships and paths. Scalar and nested list/map results
cross Execute with protocol 2.1 `graph.composite-values`; absent negotiation gives CG07016.
Final entity-valued lists remain CG05001 until #1662, including empty or FALSE-filtered lists
and entities nested in maps. Scalar projections from entity lists and intermediate FOR expansion
can still produce permitted results. See [value limits and wire capability](list-values.md#limits-and-wire-capability).

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CG00014 | Pattern comprehension lacks a nonempty path or projection | — |
| CG00002 | Malformed syntax, including an empty projection operand | — |
| CG05028 | Malformed pattern-comprehension AST, aggregate projection or missing collection plan | — |
| CG01019 | Proven incompatible correlated entity kind | — |
| CG02016 | Runtime correlated argument has an incompatible entity kind | — |
| CG02013 | Non-Boolean/non-NULL predicate or constructed-value limit | — |
| CG05001 | Unbound name or final entity-valued wire result | — |
| CG07016 | Possible composite wire result without negotiation | — |
| CG05004 | Shared statement work budget exhausted | — |
| CG05020 | Shared statement memory budget exhausted | — |
| CK00001 | Excluded path prefix or list-valued stored property | — |

## Examples

Execute separately in a fresh database:

```sql
INSERT (a:N {k:1})-[:T]->(b:N {k:2}), (a)-[:T]->(b),
       (b)-[:T]->(:N {k:3}), (:N {k:4});
MATCH (a:N) RETURN a.k, [(a)-[:T]->(b) | b.k] ORDER BY a.k;
LET a=NULL RETURN [(a)->(b) | b.k];
RETURN [p=(:N {k:1})-[:T]->(b) | size(nodes(p))];
RETURN size([p=(a)-[:T*0..0]->(b) | size(nodes(p))]);
```

The MATCH returns `(1,[2,2])`, `(2,[3])`, `(3,[])`, `(4,[])`. The following queries return
`[]`, `[2,2]` and `4`, respectively. The last query has one zero-hop path per node.

## See also

[Expressions](index.md) · [EXISTS](exists.md) · [List comprehensions](list-comprehensions.md) · [GqlPatternComprehensionExpression](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph-language/gql-pattern-comprehension-expression.md)

## Sources

- **Execution and scope** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphPatternComprehensionTests.cs`.
- **Wire admission** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphPatternComprehensionWireTests.cs`.
- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlPatternComprehensionParserTests.cs`.
