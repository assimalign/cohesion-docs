# Assimalign.Cohesion.Database.Graph.Language

This package parses the executable Cohesion subset of ISO/IEC 39075 GQL into an AST.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`GqlClauses`](gql-clauses.md)** — Documented public type.
- **[`GqlLanguageProfile`](gql-language-profile.md)** — Documented public type.
- **[`GqlPathPattern`](gql-path-pattern.md)** — Documented public type.
- **[`GqlProjection`](gql-projection.md)** — Documented public type.
- **[GqlVariableExpression](gql-variable-expression.md)** — Bare node, relationship or path binding.
- **[GqlSetQuantifier](gql-set-quantifier.md)** — ALL or DISTINCT result tuples.
- **[`GqlQueryExpression`](gql-query-expression.md)** — Documented public type.
- **[GqlSortItem](gql-sort-item.md)** — Sort expression, direction and NULL placement.
- **[GqlSortDirection](gql-sort-direction.md)** — Ascending or descending comparison.
- **[GqlNullOrdering](gql-null-ordering.md)** — Default or explicit NULL placement.
- **[GqlMatchClause](gql-match-clause.md)** — Ordered MATCH paths, predicate and optional flag.
- **[`GqlQueryParser`](gql-query-parser.md)** — Documented public type.
- **[`GqlQueryStatement`](gql-query-statement.md)** — Documented public type.
- **[GqlParameterExpression](gql-parameter-expression.md)** — Scalar AST and validation.
- **[GqlUnaryExpression](gql-unary-expression.md)** — Scalar AST and validation.
- **[GqlIsNullExpression](gql-is-null-expression.md)** — Scalar AST and validation.
- **[GqlTruthTestExpression](gql-truth-test-expression.md)** — Scalar AST and validation.
- **[GqlCaseAlternative](gql-case-alternative.md)** — Value expression shape and execution.
- **[GqlCaseExpression](gql-case-expression.md)** — Value expression shape and execution.
- **[GqlCoalesceExpression](gql-coalesce-expression.md)** — Value expression shape and execution.
- **[GqlNullIfExpression](gql-null-if-expression.md)** — Value expression shape and execution.
- **[GqlMapExpression](gql-map-expression.md)** — Value expression shape and execution.
- **[GqlSubscriptExpression](gql-subscript-expression.md)** — Value expression shape and execution.
- **[GqlMemberExpression](gql-member-expression.md)** — Value expression shape and execution.
- **[GqlListExpression](gql-list-expression.md)** — Scalar AST and validation.
- **[GqlInExpression](gql-in-expression.md)** — Scalar AST and validation.
- **[GqlStringPredicateExpression](gql-string-predicate-expression.md)** — Scalar AST and validation.
- **[GqlLogicalExpression](gql-logical-expression.md)** — Scalar AST and validation.
- **[GqlLogicalOperator](gql-logical-operator.md)** — Values and usage.
- **[GqlStringPredicateOperator](gql-string-predicate-operator.md)** — Values and usage.

This package parses the executable Cohesion subset of ISO/IEC 39075 GQL into an AST. It depends on
`Database.Language` for the lexer, parser lifecycle, analyzer pipeline, and diagnostic contracts.
The Graph engine consumes the AST for planning and execution in the session's bound database.

See the [source-backed usage examples](examples/index.md).

The supported surface is finite `MATCH`/`OPTIONAL MATCH` paths in one or several clauses, scalar Boolean/arithmetic filters with parameters,
RETURN expressions/aliases, CASE/COALESCE/NULLIF, list/map values with subscripts/member access,
DISTINCT and star, standalone RETURN, ORDER BY, OFFSET/SKIP and LIMIT, `INSERT`, `CREATE` as an insertion compatibility extension, and restricted or
cascading deletion. Unsupported features produce `COHDBL001`; malformed supported syntax produces
stable `GQL` diagnostics. The parser does not throw for malformed query text.

`Read` [DESIGN.md](design.md) for the standard decision, exact clause matrix, AST model, bounds,
diagnostic codes, and the conformance corpus's relationship to ISO GQL.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Language` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/Assimalign.Cohesion.Database.Graph.Language.csproj`.
