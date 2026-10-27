# Assimalign.Cohesion.Database.Documents.Language

This package parses the Documents engine's executable OQL surface into typed statement and expression trees.

> **Status:** Partial.

## Reference pages

- **[OqlClauses](oql-clauses.md)** — Transaction grammar and session-control metadata.
- **[OqlTransactionIsolation](oql-transaction-isolation.md)** — Transaction grammar and session-control metadata.

- **[OqlTransactionCommand](oql-transaction-command.md)** — Transaction grammar and session-control metadata.
- **[OqlTransactionExpression](oql-transaction-expression.md)** — Transaction grammar and session-control metadata.

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[OqlFunctions](oql-functions.md)** — Immutable executable vocabulary and case-insensitive lookup.
- **[OqlFunction](oql-function.md)** — Arity, JSON kinds and static result metadata.
- **[OqlCallExpression](oql-call-expression.md)** — Scalar and aggregate call arguments.
- **[OqlCreateCollectionExpression](oql-create-collection-expression.md)** — AST members and execution contract.
- **[OqlDropCollectionExpression](oql-drop-collection-expression.md)** — AST members and execution contract.
- **[OqlUpsertExpression](oql-upsert-expression.md)** — Whole-document replacement from VALUES or SELECT.
- **[OqlInsertExpression](oql-insert-expression.md)** — AST members and execution contract.
- **[OqlReturningExpression](oql-returning-expression.md)** — Ordered affected-image projections and RAW/VALUE.
- **[OqlUpdateExpression](oql-update-expression.md)** — Partial updates, removals and filtering.
- **[OqlUpdateAssignment](oql-update-assignment.md)** — SET path and pre-update value.
- **[OqlDeleteExpression](oql-delete-expression.md)** — Target, alias and optional predicate.
- **[OqlMetaExpression](oql-meta-expression.md)** — AST members and execution contract.
- **[OqlObjectExpression](oql-object-expression.md)** — AST members and execution contract.
- **[OqlArrayExpression](oql-array-expression.md)** — Ordered constructor and MISSING elements.
- **[OqlComprehensionExpression](oql-comprehension-expression.md)** — Scoped ARRAY/FIRST mapping and optional WHEN.
- **[OqlCreateIndexExpression](oql-create-index-expression.md)** — Ordered paths, UNIQUE and conditional creation.
- **[OqlDropIndexExpression](oql-drop-index-expression.md)** — Conditional removal.
- **[OqlSelectExpression](oql-select-expression.md)** — DISTINCT, VALUE/RAW and LIMIT/OFFSET operands.
- **[OqlInExpression](oql-in-expression.md)** — AST members and execution contract.
- **[OqlBetweenExpression](oql-between-expression.md)** — AST members and execution contract.
- **[OqlLikeExpression](oql-like-expression.md)** — AST members and execution contract.
- **[OqlCaseExpression](oql-case-expression.md)** — AST members and execution contract.
- **[OqlMissingExpression](oql-missing-expression.md)** — AST members and execution contract.
- **[OqlLiteralExpression](oql-literal-expression.md)** — AST members and execution contract.
- **[OqlExplainExpression](oql-explain-expression.md)** — SELECT/INSERT/UPDATE/DELETE wrapper.
- **[OqlUnnestExpression](oql-unnest-expression.md)** — Ordered INNER/LEFT array binding.
- **[OqlQuantifiedExpression](oql-quantified-expression.md)** — Scoped ANY/SOME/EVERY predicate.

This package parses the Documents engine's executable OQL surface into typed statement and
expression trees. It provides `OqlLanguageProfile`, stable `OqlClauses` names, `OqlQueryParser`,
`OqlQueryStatement`, `OqlSelectExpression`, `OqlCreateIndexExpression`, `OqlDropIndexExpression`
, and expression nodes for projection, filtering, grouping, aggregates, ordering, nested properties,
and array access.

See the [source-backed usage examples](examples/index.md).

The package references only the shared `Assimalign.Cohesion.Database.Language` package. The
Documents engine supplies catalog binding, planning, and execution. Queries and index DDL address
one collection in the session's database. `CREATE INDEX` and `DROP INDEX` use the same
parse-plan-execute path as `SELECT`; INSERT, UPSERT, UPDATE, DELETE, collection DDL and EXPLAIN also execute. UPSERT replaces whole documents.
OQL follows the SQL++ MISSING contract. DEFINE/ELEMENT/FLATTEN are ordinary field names;
their old statement/call positions and subqueries report CK00001.

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
