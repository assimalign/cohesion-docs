# Examples

Examples for `Assimalign.Cohesion.Database.Sql.Language` are derived from source documentation and code.

> **Status:** Partial.

The examples below retain real usage from project documentation, templates, fixtures, or tests. Test
examples require their original test project’s dependencies and supporting objects.

- **[Sql Token Lexer Test](sql-token-lexer-test.md)** — This example exercises `Assimalign.Cohesion.Database.Sql.Language` through its co-located test source.
- **[Sql Delete Parser Tests](sql-delete-parser-tests.md)** — This example exercises `Assimalign.Cohesion.Database.Sql.Language` through its co-located test source.
- **[Parse a query](parse-query.md)** — `Parse` query text into the language-specific statement representation.

[Assembly overview](../index.md) · [Design](../design.md)

## T1 API examples

[SqlLiteralText](../sql-literal-text.md) reads strict typed contents;
[SqlTypeNames](../sql-type-names.md) resolves multi-word/national-character aliases.
[SqlDefaultExpression](../sql-default-expression.md) and [SqlExplainExpression](../sql-explain-expression.md)
show the landed AST forms.

[SqlSelectExpression](../sql-select-expression.md), [SqlFromItem](../sql-from-item.md) and
[SqlJoinClause](../sql-join-clause.md) inspect comma grouping and LEFT/CROSS/USING shapes from
SqlJoinShapeTests. The [JOIN language examples](../../../../../database/sql/language/clauses/join.md#examples)
show executed values and merged types from SqlNWayJoinTests.

[SqlRelationReference](../sql-relation-reference.md#example) inspects named versus derived inputs;
[SqlDerivedTableReference](../sql-derived-table-reference.md#example) reads query bodies and column
renames from the parser. The [DML snapshot example](../../../../../database/sql/language/expressions/subqueries.md#update-and-delete-snapshots)
gives UPDATE/DELETE results from SqlDmlSubqueryTests.

See [SqlIdentityDefinition](../sql-identity-definition.md#example) for parser facets and
[IDENTITY](../../../../../database/sql/language/clauses/identity.md#examples) for executed generation.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/OVERVIEW.md`.
