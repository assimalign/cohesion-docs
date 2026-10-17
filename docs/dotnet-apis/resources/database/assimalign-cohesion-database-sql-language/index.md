# Assimalign.Cohesion.Database.Sql.Language

The SQL language package parses query text, declares dialect capabilities, and resolves SQL type names.

> **Status:** Partial.

## Reference pages

- **[SqlLanguageProfile](sql-language-profile.md)** — Public contract and usage.
- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[SqlLiteralText](sql-literal-text.md)** — Public contract and usage.
- **[SqlExplainExpression](sql-explain-expression.md)** — Parsed shape and diagnostics.
- **[SqlDefaultExpression](sql-default-expression.md)** — Parsed shape and diagnostics.
- **[SqlInsertExpression](sql-insert-expression.md)** — Parsed shape and diagnostics.
- **[SqlSelectExpression](sql-select-expression.md)** — SELECT clauses and canonical FROM trees.
- **[SqlFromItem](sql-from-item.md)** — One comma-separated relation and its joins.
- **[SqlJoinClause](sql-join-clause.md)** — Join kind, ON predicate and ordered USING names.
- **[SqlColumnDefinition](sql-column-definition.md)** — Parsed shape and diagnostics.
- **[SqlLiteralExpression](sql-literal-expression.md)** — Parsed shape and diagnostics.
- **[SqlTypeNames](sql-type-names.md)** — Alias and modifier resolution.
- **[SqlLiteralType](sql-literal-type.md)** — Values and usage.

The SQL language front-end of the Cohesion Data Platform: a recursive-descent parser
(`SqlQueryParser`) producing a full clause-level AST over the shared lexer infrastructure
(`Database.Language`), the declared dialect contract (DIALECT.md
(`cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`)), and the
type-name translation table (`SqlTypeNames`) binding SQL DDL/CAST type names to the shared type
system (`Database.Types`).

## Scope

- **Parser** — `SELECT` (joins, grouping, ordering, limits, subqueries), `INSERT`
  (multi-row, `INSERT ... SELECT`), `UPDATE`, `DELETE`, `CREATE/ALTER/DROP TABLE`,
  and a full expression grammar (precedence, `CASE`, `CAST`, predicates,
  parameters). Error-tolerant: malformed input yields diagnostics, never
  exceptions.
- **AST** — sealed statement/expression node families under
  `SqlQueryStatement`/`SqlQueryExpression`; the raw statement text is stamped on
  the root after parsing.
- **Dialect contract** — DIALECT.md (`cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`) is the authoritative supported /
  recognized / rejected matrix. `SqlLanguageProfile.Instance` carries its lexical
  tables and implemented clause set; recognized clauses outside that set report
  `COHDBL001` rather than an undifferentiated parse failure.
- **Types and builtins** — `SqlTypeNames` resolves declared type names (with
  length/precision/scale) to `DatabaseType` identities; builtin function names are
  declared in the lexer tables and the dialect doc.

## Dependencies

`Database.Language` (lexer, parser base, diagnostics) and `Database.Types` (type identities).
Consumed by `Database.Sql` (the engine) and, later, the SQL catalog/planner and SDK schema compiler.

## Usage

See the [source-backed usage examples](examples/index.md).

See [DESIGN.md](design.md) for the parser's shape and the decisions behind it.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Language` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Assimalign.Cohesion.Database.Sql.Language.csproj`.
