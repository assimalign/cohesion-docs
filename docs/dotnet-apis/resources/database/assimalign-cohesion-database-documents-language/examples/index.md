# Examples

Examples for `Assimalign.Cohesion.Database.Documents.Language` are derived from source documentation and code.

> **Status:** Partial.

The examples below retain real usage from project documentation, templates, fixtures, or tests. Test
examples require their original test project’s dependencies and supporting objects.

- **[Oql Introspection Parser Tests](oql-introspection-parser-tests.md)** — This example exercises `Assimalign.Cohesion.Database.Documents.Language` through its co-located test source.
- **[Oql Index Ddl Parser Tests](oql-index-ddl-parser-tests.md)** — This example exercises `Assimalign.Cohesion.Database.Documents.Language` through its co-located test source.
- **[Parse a query](parse-query.md)** — `Parse` query text into the language-specific statement representation.

[Assembly overview](../index.md) · [Design](../design.md)

## T1 API examples

The public AST pages show constructors for [IN](../oql-in-expression.md),
[BETWEEN](../oql-between-expression.md), [LIKE](../oql-like-expression.md), [CASE](../oql-case-expression.md),
[MISSING](../oql-missing-expression.md), [INSERT](../oql-insert-expression.md) and
[collection DDL](../oql-create-collection-expression.md). Execution examples and results appear in
[OQL](../../../../../database/document/language/index.md).

[OqlSelectExpression](../oql-select-expression.md#example) inspects DISTINCT VALUE/RAW flags;
[OqlArrayExpression](../oql-array-expression.md#example) inspects nested constructor elements.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/OVERVIEW.md`.
