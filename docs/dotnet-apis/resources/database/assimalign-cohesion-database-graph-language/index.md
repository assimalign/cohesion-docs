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
- **[`GqlQueryExpression`](gql-query-expression.md)** — Documented public type.
- **[`GqlQueryParser`](gql-query-parser.md)** — Documented public type.
- **[`GqlQueryStatement`](gql-query-statement.md)** — Documented public type.

This package parses the executable Cohesion subset of ISO/IEC 39075 GQL into an AST. It depends on
`Database.Language` for the lexer, parser lifecycle, analyzer pipeline, and diagnostic contracts.
The Graph engine consumes the AST for planning and execution in the session's bound database.

See the [source-backed usage examples](examples/index.md).

The supported surface is finite `MATCH` paths, scalar comparison/conjunction filters, variable or
property projection, `INSERT`, `CREATE` as an insertion compatibility extension, and restricted or
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
