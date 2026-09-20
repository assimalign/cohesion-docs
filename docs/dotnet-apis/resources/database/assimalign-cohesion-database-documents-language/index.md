# Assimalign.Cohesion.Database.Documents.Language

This package parses the Documents engine's executable OQL surface into typed statement and expression trees.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

This package parses the Documents engine's executable OQL surface into typed statement and
expression trees. It provides `OqlLanguageProfile`, stable `OqlClauses` names, `OqlQueryParser`,
`OqlQueryStatement`, `OqlSelectExpression`, `OqlCreateIndexExpression`, `OqlDropIndexExpression`
, and expression nodes for projection, filtering, grouping, aggregates, ordering, nested properties,
and array access.

See the [source-backed usage examples](examples/index.md).

The package references only the shared `Assimalign.Cohesion.Database.Language` package. The
Documents engine supplies catalog binding, planning, and execution. Queries and index DDL address
one collection in the session's database. `CREATE INDEX` and `DROP INDEX` use the same
parse-plan-execute path as `SELECT`; document data mutations use the existing collection API.
`DEFINE`, `ELEMENT`, `FLATTEN`, and subqueries are reserved but unsupported and produce
`COHDBL001`.

See [DESIGN.md](design.md) for the supported-clause matrix, statement/expression grammar,
diagnostics, scope restrictions, and extension rules. Co-located Shouldly tests provide the parser
and lexer conformance corpora.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Language` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/Assimalign.Cohesion.Database.Documents.Language.csproj`.
