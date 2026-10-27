# Assimalign.Cohesion.Database.Graph.Language

- **[GqlSetStage](gql-set-stage.md)** — Ordered graph update AST.
- **[GqlRemoveStage](gql-remove-stage.md)** — Ordered graph update AST.
- **[GqlSetItem](gql-set-item.md)** — Ordered graph update AST.
- **[GqlRemoveItem](gql-remove-item.md)** — Ordered graph update AST.
- **[GqlSetPropertyItem](gql-set-property-item.md)** — Ordered graph update AST.
- **[GqlSetPropertiesItem](gql-set-properties-item.md)** — Ordered graph update AST.
- **[GqlSetLabelsItem](gql-set-labels-item.md)** — Ordered graph update AST.
- **[GqlRemovePropertyItem](gql-remove-property-item.md)** — Ordered graph update AST.
- **[GqlRemoveLabelsItem](gql-remove-labels-item.md)** — Ordered graph update AST.

This package parses the executable Cohesion subset of ISO/IEC 39075 GQL into an AST.

> **Status:** Partial.

## Reference pages

- **[GqlPathSelector](gql-path-selector.md)** — Selector kind/count and rendering; binding and EXPLAIN only.
- **[GqlNamedFunctionCallExpression](gql-named-function-call-expression.md)** — Engine-resolved application function calls.

- **[GraphFunctionName](graph-function-name.md)** — Generated function identifier and validation.
- **[GqlPathMode](gql-path-mode.md)** — Outer path identity-reuse policies.

- **[GqlStage](gql-stage.md)** — Source-ordered stage family and source-ordered execution.
- **[GqlMatchStage](gql-match-stage.md)** — MATCH/OPTIONAL MATCH over incoming rows.
- **[GqlLetStage](gql-let-stage.md)** — Sequential value bindings.
- **[GqlFilterStage](gql-filter-stage.md)** — TRUE-only stage filtering.
- **[GqlForStage](gql-for-stage.md)** — FOR/UNWIND expansion and entity kinds.
- **[GqlInsertStage](gql-insert-stage.md)** — Per-row INSERT/CREATE patterns.
- **[GqlDeleteStage](gql-delete-stage.md)** — Bound deletion and scope removal.
- **[GqlReturnStage](gql-return-stage.md)** — RETURN/WITH projection and per-stage tails.
- **[GqlNextStage](gql-next-stage.md)** — Segment boundary.
- **[GqlQuerySegment](gql-query-segment.md)** — Stages between NEXT boundaries.
- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`GqlClauses`](gql-clauses.md)** — Documented public type.
- **[`GqlLanguageProfile`](gql-language-profile.md)** — Documented public type.
- **[GqlQuantifier](gql-quantifier.md)** — Quantified path AST contract.
- **[GqlPathElement](gql-path-element.md)** — Quantified path AST contract.
- **[GqlPathNode](gql-path-node.md)** — Quantified path AST contract.
- **[GqlPathRelationship](gql-path-relationship.md)** — Quantified path AST contract.
- **[GqlParenthesizedPathPattern](gql-parenthesized-path-pattern.md)** — Quantified path AST contract.
- **[GqlRelationshipPattern](gql-relationship-pattern.md)** — Quantified path AST contract.
- **[`GqlPathPattern`](gql-path-pattern.md)** — Documented public type.
- **[`GqlProjection`](gql-projection.md)** — Documented public type.
- **[GqlFunctionCallExpression](gql-function-call-expression.md)** — Scalar identity, arguments and niladic clocks.
- **[GqlScalarFunction](gql-scalar-function.md)** — Executable scalar operations and aliases.
- **[GqlCastExpression](gql-cast-expression.md)** — Explicit conversion operand/target.
- **[GqlCastType](gql-cast-type.md)** — Supported scalar conversion identities.
- **[GqlAggregateExpression](gql-aggregate-expression.md)** — Aggregate operand, function and DISTINCT flag.
- **[GqlAggregateFunction](gql-aggregate-function.md)** — Six operations, including CollectList/collect.
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

The supported surface is fixed/quantified `MATCH`/`OPTIONAL MATCH` paths in one or several clauses, scalar Boolean/arithmetic filters with parameters,
RETURN expressions/aliases, scalar functions/CAST, CASE/COALESCE/NULLIF, list/map values with subscripts/member access,
DISTINCT and star, standalone RETURN, implicit/ISO GROUP BY, aggregates, ORDER BY, OFFSET/SKIP and LIMIT, `INSERT`, `CREATE` as an insertion compatibility extension, and restricted or
cascading deletion. Unsupported features produce `CK00001`; malformed supported syntax produces
stable `GQL` diagnostics. The parser does not throw for malformed query text.

`Read` [DESIGN.md](design.md) for the standard decision, exact clause matrix, AST model, bounds,
diagnostic codes, and the conformance corpus's relationship to ISO GQL.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Language` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Transaction AST

[GqlTransactionCommand](gql-transaction-command.md) identifies parsed transaction controls on GqlQueryExpression.TransactionCommand.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/Assimalign.Cohesion.Database.Graph.Language.csproj`.
