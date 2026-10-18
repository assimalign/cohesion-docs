# Examples

Examples for `Assimalign.Cohesion.Database.Graph.Language` are derived from source documentation and code.

> **Status:** Partial.

The examples below retain real usage from project documentation, templates, fixtures, or tests. Test
examples require their original test project’s dependencies and supporting objects.

- **[Gql Capability Diagnostic Tests](gql-capability-diagnostic-tests.md)** — This example exercises `Assimalign.Cohesion.Database.Graph.Language` through its co-located test source.
- **[Gql Catalog Parser Tests](gql-catalog-parser-tests.md)** — This example exercises `Assimalign.Cohesion.Database.Graph.Language` through its co-located test source.
- **[Parse a query](parse-query.md)** — `Parse` query text into the language-specific statement representation.

[Assembly overview](../index.md) · [Design](../design.md)

## RETURN API examples

[GqlProjection](../gql-projection.md) executes a source-free computed expression;
[GqlQueryExpression](../gql-query-expression.md) inspects standalone RETURN DISTINCT and EXPLAIN.
[GqlVariableExpression](../gql-variable-expression.md) reads a bare binding and
[GqlSetQuantifier](../gql-set-quantifier.md) constructs DISTINCT output. These examples follow
GqlReturnParserTests and GraphReturnTests. The [language examples](../../../../../database/graph/language/clauses/return.md#examples)
give values and wire result limits.

See [GqlMatchClause](../gql-match-clause.md#example) for repeated and optional matching clauses,
and [MATCH](../../../../../database/graph/language/statements/match.md#optional-and-repeated-matching)
for their results and the bound-anchor measurement.

[GqlSortItem](../gql-sort-item.md#example) and its enum pages inspect parsed ORDER BY keys.
The [ordering fixture](../../../../../database/graph/language/clauses/order-by.md#examples)
shows NULL placement, aliases, SKIP and LIMIT with exact result sequences.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/OVERVIEW.md`.

## Conditional and composite values

[GqlCaseExpression](../gql-case-expression.md) and [GqlCaseAlternative](../gql-case-alternative.md)
show ordered simple/searched CASE construction. [COALESCE](../gql-coalesce-expression.md) and
[NULLIF](../gql-null-if-expression.md) retain case-abbreviation nodes. [Map](../gql-map-expression.md),
[list](../gql-list-expression.md), [subscript](../gql-subscript-expression.md) and
[member](../gql-member-expression.md) examples follow GraphValueTests and GqlValueParserTests.
