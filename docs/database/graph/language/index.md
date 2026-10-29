# Language (GQL)

GQL provides finite graph matching, scalar filtering, insertion, property/label updates, deletion, and catalog discovery.

> **Status:** Partial. Executes a bounded ISO GQL subset with Cohesion extensions, including outer path modes.

This reference describes the executable Cohesion subset of ISO/IEC 39075:2024.
`INSERT` is the graph-insertion verb; `CREATE` is a compatibility extension and `SHOW` is a
Cohesion catalog extension. Every statement evaluates in the session's bound logical database.

## Reference

- **[Syntax conventions](syntax-conventions.md)** — diagrams, names, literals, and comments.
- **[Statements](statements/index.md)** — queries, mutations, and catalog introspection.
- **[Clauses](clauses/index.md)** — filtering, grouping, projection, composite queries, ordering and paging.
- **[Functions](functions/index.md)** — scalar signatures, transaction clocks, aggregates and NULL behavior.
- **[Patterns](patterns.md)** — nodes, relationships, maps, directions, and execution bounds.
- **[Path selectors](path-selectors.md)** — ANY/ALL/SHORTEST execution, modes, prefilters and budgets.
- **[Expressions](expressions/index.md)** — operators, CASE/COALESCE/NULLIF, lists, maps and property access.
- **[EXISTS and NOT EXISTS](expressions/exists.md)** — correlated braced patterns with local inner bindings.
- **[Diagnostics](diagnostics.md)** — syntax, capability, planning, and execution errors.
- **[Unsupported features](unsupported.md)** — recognized constructs outside the profile.

## Supported-clause matrix

| Construct | Support | Scope |
|---|---|---|
| [`MATCH`, `OPTIONAL MATCH`](statements/match.md) | Supported | Fixed/quantified paths; repeated clauses join shared variables and optional clauses NULL-extend unmatched rows |
| `WHERE` | Supported | Boolean/arithmetic expressions, NULL/truth tests, IN and string predicates |
| [EXISTS and NOT EXISTS](expressions/exists.md) | Supported | Correlated braced pattern blocks; Boolean result and isolated inner variables |
| [Pattern comprehensions](expressions/pattern-comprehensions.md) | Supported | Correlated ordered lists; duplicates and NULL projections retained |
| [LET](clauses/let.md), [FILTER](clauses/filter.md) | Supported | Sequential value bindings and TRUE-only stage filtering |
| [FOR / UNWIND](clauses/for.md) | Supported | Streaming list expansion; NULL gives no rows; proven entity kinds survive |
| [WITH](clauses/with.md), [RETURN … NEXT](clauses/next.md) | Supported | Intermediate projection, scoped carry-forward and one statement transaction |
| [UNION, EXCEPT, INTERSECT and OTHERWISE](clauses/union.md) | Supported subset | Read-only operands, matching columns, set/multiset counts, lazy fallback, operand-local paging and NEXT composition |
| `RETURN` | Supported | Standalone or after MATCH/insertion; expressions, aliases, ALL/DISTINCT and named-binding star |
| [ORDER BY](clauses/order-by.md) | Supported | Expression keys, ASC/ASCENDING/DESC/DESCENDING, NULLS FIRST/LAST and stable ties |
| [OFFSET/SKIP and LIMIT](clauses/limit-offset.md) | Supported | Output paging after RETURN; offset precedes limit |
| [SET](clauses/set.md), [REMOVE](clauses/remove.md) | Supported | Scalar property replacement/merge and node label changes with affected counts |
| `INSERT` | Supported | Node/path insertion with row-evaluated scalar properties, optionally after matching or value stages |
| [MERGE](statements/merge.md) | Supported | Whole-pattern match/create, bound-node reuse and ordered ON CREATE/ON MATCH SET |
| `CREATE` | Supported | Insertion compatibility extension |
| `DELETE` | Supported | Bound variables; connected-node deletion is restricted |
| `DETACH DELETE` | Supported | Bound variables; incident relationships cascade |
| `SHOW` | Supported | Five database-scoped metadata subjects |
| Parameters | Supported | Scalar/list/map values in expressions; stored pattern properties stay scalar-only |
| [CASE](expressions/case.md), [COALESCE](expressions/coalesce.md), [NULLIF](expressions/nullif.md) | Supported | Lazy conditional values; simple CASE accepts ISO when-operand lists |
| [Lists](expressions/list-values.md), [maps](expressions/map-values.md) and [access](expressions/property-access.md) | Supported | Transient nested values, with negotiated graph.composite-values over protocol 2.1 |
| [List comprehensions](expressions/list-comprehensions.md) and [slices](expressions/list-slices.md) | Supported | Scoped filtering/projection and bounded ranges; stored properties remain scalar |
| [reduce](expressions/reduce.md) and [list predicates](expressions/list-predicates.md) | Supported | Ordered folds and all/any/none/single with lexical bindings and three-valued tests |
| EXPLAIN | Supported | Chosen data plan without execution |
| [Path selectors](path-selectors.md) and inline WHERE | Supported | Per-pair execution under work/memory budgets; counted ANY remains refused |
| [Scalar functions](functions/scalar-functions.md) and [CAST](expressions/cast.md) | Supported | Plan-time signatures, strict NULLs, code-point substring and transaction-start clocks |
| [Aggregates](functions/aggregate-functions.md) | Supported | COUNT, SUM, AVG, MIN, MAX, COLLECT_LIST/collect; implicit and [ISO GROUP BY](clauses/group-by.md) |
| [Quantified paths](patterns.md#quantified-paths) | Supported | ISO edge/subpath and Cypher lengths, outer WALK/TRAIL/SIMPLE/ACYCLIC (default TRAIL), list-valued groups |

The keyword table is larger than the executable clause list. Recognition does not imply support.
The parser conformance corpus checks the subset's syntax and abstract syntax tree; it is not an
ISO certification suite.

[EXPLAIN](statements/explain.md) describes a chosen plan. [Parameters](expressions/parameters.md)
share the SQL/OQL binding contract.

[Functions](functions/index.md) documents aggregates, exact numeric results and NULL behavior.
[GROUP BY](clauses/group-by.md) covers implicit keys, RETURN aliases and GROUP BY ().

The collection-expression work in #1736 is complete: list comprehensions, slices, reduce,
list predicates and pattern comprehensions execute. Final entity lists retain the #1662 wire gate.

## See also

- **[Graph](../index.md)** — engine, storage, and delivery status.

## Streaming and budgets

Reads stream under [memory/work ceilings](../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph/graph-database-engine-options.md). Unordered LIMIT can stop upstream work; ordered finite pages use bounded top-N. Mutation targets and final results retain statement memory accounting.

## Transaction control

[START TRANSACTION, COMMIT and ROLLBACK](statements/transactions.md) execute as standalone ISO text statements, including over Execute.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlLanguageProfile.cs`.
