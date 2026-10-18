# Assimalign.Cohesion.Database.Documents.Language

This package parses the Documents engine's executable OQL surface into typed statement and expression trees.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[OqlCreateCollectionExpression](oql-create-collection-expression.md)** — AST members and execution contract.
- **[OqlDropCollectionExpression](oql-drop-collection-expression.md)** — AST members and execution contract.
- **[OqlInsertExpression](oql-insert-expression.md)** — AST members and execution contract.
- **[OqlDeleteExpression](oql-delete-expression.md)** — Target, alias and optional predicate.
- **[OqlMetaExpression](oql-meta-expression.md)** — AST members and execution contract.
- **[OqlObjectExpression](oql-object-expression.md)** — AST members and execution contract.
- **[OqlArrayExpression](oql-array-expression.md)** — Ordered constructor and MISSING elements.
- **[OqlSelectExpression](oql-select-expression.md)** — DISTINCT and VALUE/RAW result flags.
- **[OqlInExpression](oql-in-expression.md)** — AST members and execution contract.
- **[OqlBetweenExpression](oql-between-expression.md)** — AST members and execution contract.
- **[OqlLikeExpression](oql-like-expression.md)** — AST members and execution contract.
- **[OqlCaseExpression](oql-case-expression.md)** — AST members and execution contract.
- **[OqlMissingExpression](oql-missing-expression.md)** — AST members and execution contract.
- **[OqlLiteralExpression](oql-literal-expression.md)** — AST members and execution contract.
- **[OqlExplainExpression](oql-explain-expression.md)** — SELECT/INSERT/DELETE wrapper.

This package parses the Documents engine's executable OQL surface into typed statement and
expression trees. It provides `OqlLanguageProfile`, stable `OqlClauses` names, `OqlQueryParser`,
`OqlQueryStatement`, `OqlSelectExpression`, `OqlCreateIndexExpression`, `OqlDropIndexExpression`
, and expression nodes for projection, filtering, grouping, aggregates, ordering, nested properties,
and array access.

See the [source-backed usage examples](examples/index.md).

The package references only the shared `Assimalign.Cohesion.Database.Language` package. The
Documents engine supplies catalog binding, planning, and execution. Queries and index DDL address
one collection in the session's database. `CREATE INDEX` and `DROP INDEX` use the same
parse-plan-execute path as `SELECT`; INSERT, DELETE, collection DDL and EXPLAIN also execute. Replacement uses collection APIs.
OQL follows the SQL++ MISSING contract. DEFINE/ELEMENT/FLATTEN are ordinary field names;
their old statement/call positions and subqueries report COHDBL001.

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
