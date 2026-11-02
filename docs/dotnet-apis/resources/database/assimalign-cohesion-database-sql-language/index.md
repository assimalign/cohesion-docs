# Assimalign.Cohesion.Database.Sql.Language

- **[SqlCreateSequenceExpression](sql-create-sequence-expression.md)** — Sequence name, guard and options.
- **[SqlAlterSequenceExpression](sql-alter-sequence-expression.md)** — Option changes and restart.
- **[SqlDropSequenceExpression](sql-drop-sequence-expression.md)** — Atomic name list and policy.
- **[SqlNextValueExpression](sql-next-value-expression.md)** — Per-row standard sequence value.
- **[SqlSequenceOption](sql-sequence-option.md)** — Written option name, integer and type spelling.
- **[SqlGroupingSetExpression](sql-grouping-set-expression.md)** — Empty/composite sets, GROUPING SETS, ROLLUP and CUBE.
- **[SqlGroupingSetKind](sql-grouping-set-kind.md)** — Grouping expansion operation.

The SQL language package parses query text, declares dialect capabilities, and resolves SQL type names.

> **Status:** Partial.

## Reference pages

- **[SqlCreateIndexExpression](sql-create-index-expression.md)** — Per-column direction, NULL placement and partial predicates.

- **[SqlTransactionExpression](sql-transaction-expression.md)** — Control command and ordered isolation modes.
- **[SqlTransactionIsolationLevel](sql-transaction-isolation-level.md)** — SQL spellings and effective kernel levels.

- **[SqlWindowExpression](sql-window-expression.md)** — Function and OVER syntax.
- **[SqlWindowSpecification](sql-window-specification.md)** — Written partition/order, frame and inherited name.
- **[SqlWindowFrame](sql-window-frame.md)** — Explicit ROWS/RANGE/GROUPS mode and boundaries.
- **[SqlWindowExclusion](sql-window-exclusion.md)** — Members removed from the selected frame.
- **[SqlWindowNullTreatment](sql-window-null-treatment.md)** — Navigation and frame-value NULL policy.
- **[SqlWindowFrameBound](sql-window-frame-bound.md)** — Boundary kind and optional row offset.
- **[SqlNamedWindow](sql-named-window.md)** — Ordered WINDOW definitions.

- **[SqlAlterIdentityAction](sql-alter-identity-action.md)** — ADD/DROP, mode and sequence-option changes.
- **[SqlIdentityOverride](sql-identity-override.md)** — INSERT and MERGE assignment policy.

- **[SqlMergeExpression](sql-merge-expression.md)** — Target/source association, ordered actions and RETURNING.
- **[SqlMergeWhenClause](sql-merge-when-clause.md)** — Match kind, operation, values and retained clause span.
- **[SqlCreateTableAsExpression](sql-create-table-as-expression.md)** — Destination, inferred query and data policy.
- **[SqlTruncateExpression](sql-truncate-expression.md)** — Immutable target list and effective policies.
- **[SqlTruncateTable](sql-truncate-table.md)** — One target and its ONLY modifier.

- **[SqlJoinedTableReference](sql-joined-table-reference.md)** — Unaliased grouped joins and component scopes.

- **[SqlCreateSchemaExpression](sql-create-schema-expression.md)** — Namespace name and existence guard.
- **[SqlDropSchemaExpression](sql-drop-schema-expression.md)** — RESTRICT/CASCADE namespace removal.
- **[SqlQueryBodyExpression](sql-query-body-expression.md)** — Shared SELECT/set body and tail.
- **[SqlSetOperationExpression](sql-set-operation-expression.md)** — Left/right operands and ALL flag.
- **[SqlSetOperator](sql-set-operator.md)** — UNION, EXCEPT and INTERSECT identities.
- **[SqlSubqueryExpression](sql-subquery-expression.md)** — SELECT/set bodies in scalar positions.
- **[SqlInExpression](sql-in-expression.md)** — Value lists or SELECT/set membership.
- **[SqlExistsExpression](sql-exists-expression.md)** — Row existence over SELECT/set bodies.
- **[SqlQuantifiedComparisonExpression](sql-quantified-comparison-expression.md)** — Scalar comparison over a query body.
- **[SqlComparisonQuantifier](sql-comparison-quantifier.md)** — ANY, SOME or ALL combination.
- **[SqlDistinctExpression](sql-distinct-expression.md)** — NULL-safe IS [NOT] DISTINCT FROM.
- **[SqlBooleanTestExpression](sql-boolean-test-expression.md)** — IS [NOT] TRUE/FALSE/UNKNOWN.
- **[SqlTruthValue](sql-truth-value.md)** — Truth-test selection.
- **[SqlLikeExpression](sql-like-expression.md)** — Pattern, optional ESCAPE and negation.

- **[SqlLanguageProfile](sql-language-profile.md)** — Public contract and usage.
- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[SqlFunctionCallExpression](sql-function-call-expression.md)** — Aggregate FILTER, quantifier and input ordering.
- **[SqlSetQuantifier](sql-set-quantifier.md)** — Default, ALL and DISTINCT call inputs.
- **[SqlLiteralText](sql-literal-text.md)** — Public contract and usage.
- **[SqlExplainExpression](sql-explain-expression.md)** — Parsed shape and diagnostics.
- **[SqlDefaultExpression](sql-default-expression.md)** — Parsed shape and diagnostics.
- **[SqlInsertExpression](sql-insert-expression.md)** — Parsed shape and diagnostics.
- **[SqlOnConflictClause](sql-on-conflict-clause.md)** — Unique arbiter, action, assignments, filter and EXCLUDED.
- **[SqlOnConflictTargetKind](sql-on-conflict-target-kind.md)** — All indexes, column-set or named-index arbitration.
- **[SqlOnConflictAction](sql-on-conflict-action.md)** — DO NOTHING or DO UPDATE.
- **[SqlCommonTableExpression](sql-common-table-expression.md)** — WITH definitions, output columns and recursive-scope metadata.
- **[SqlUpdateExpression](sql-update-expression.md)** — Target, assignments, predicate and WITH list.
- **[SqlDeleteExpression](sql-delete-expression.md)** — Target, predicate and WITH list.
- **[SqlLimitSyntaxKind](sql-limit-syntax-kind.md)** — LIMIT, FETCH and lexical TOP families.
- **[SqlConstraintDefinition](sql-constraint-definition.md)** — Normalized constraints and all five delete actions.
- **[SqlSelectExpression](sql-select-expression.md)** — SELECT clauses and canonical FROM trees.
- **[SqlFromItem](sql-from-item.md)** — One comma-separated relation and its joins.
- **[SqlRelationReference and SqlTableReference](sql-relation-reference.md)** — Named and derived relation base and named-table leaf.
- **[SqlDerivedTableReference](sql-derived-table-reference.md)** — Aliased SELECT and optional output renames.
- **[SqlJoinClause](sql-join-clause.md)** — Join kind, ON predicate and ordered USING names.
- **[SqlAlterColumnTypeAction](sql-alter-column-type-action.md)** — Target type/collation and old-row USING conversion.
- **[SqlAlterTableExpression](sql-alter-table-expression.md)** — Table and one typed ALTER action.
- **[SqlAlterRenameTableAction](sql-alter-rename-table-action.md)** — Parsed ALTER payload and execution contract.
- **[SqlAlterRenameColumnAction](sql-alter-rename-column-action.md)** — Parsed ALTER payload and execution contract.
- **[SqlAlterSetDefaultAction](sql-alter-set-default-action.md)** — Parsed ALTER payload and execution contract.
- **[SqlAlterDropDefaultAction](sql-alter-drop-default-action.md)** — Parsed ALTER payload and execution contract.
- **[SqlAlterSetNotNullAction](sql-alter-set-not-null-action.md)** — Parsed ALTER payload and execution contract.
- **[SqlAlterDropNotNullAction](sql-alter-drop-not-null-action.md)** — Parsed ALTER payload and execution contract.
- **[SqlColumnDefinition](sql-column-definition.md)** — Parsed shape and diagnostics.
- **[SqlIdentityDefinition](sql-identity-definition.md)** — ALWAYS/BY DEFAULT mode and start/increment.
- **[SqlLiteralExpression](sql-literal-expression.md)** — Parsed shape and diagnostics.
- **[SqlTypeNames](sql-type-names.md)** — Alias and modifier resolution.
- **[SqlLiteralType](sql-literal-type.md)** — Values and usage.

- **[SqlCreateViewExpression](sql-create-view-expression.md)** — Parsed view or ordering contract.

- **[SqlDropViewExpression](sql-drop-view-expression.md)** — Parsed view or ordering contract.

- **[SqlOrderByColumn](sql-order-by-column.md)** — Parsed view or ordering contract.

- **[SqlNullPlacement](sql-null-placement.md)** — Parsed view or ordering contract.

The SQL language front-end of the Cohesion Data Platform: a recursive-descent parser
(`SqlQueryParser`) producing a full clause-level AST over the shared lexer infrastructure
(`Database.Language`), the declared dialect contract (DIALECT.md
(`cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`)), and the
type-name translation table (`SqlTypeNames`) binding SQL DDL/CAST type names to the shared type
system (`Database.Types`).

The [paging syntax kind](sql-limit-syntax-kind.md) preserves LIMIT/FETCH/TOP spelling.
[SqlConstraintDefinition](sql-constraint-definition.md) retains NO ACTION, SET NULL and SET DEFAULT
alongside CASCADE/RESTRICT; omitted SQL actions are NO ACTION.

## Scope

- **Parser** — `SELECT` (joins, grouping, ordering, limits, subqueries), `INSERT`
  (multi-row, `INSERT ... SELECT`, ON CONFLICT and RETURNING), `UPDATE`, `DELETE`, `CREATE/ALTER/DROP TABLE`,
  and a full expression grammar (precedence, `CASE`, `CAST`, predicates,
  parameters). Error-tolerant: malformed input yields diagnostics, never
  exceptions.
- **AST** — sealed statement/expression node families under
  `SqlQueryStatement`/`SqlQueryExpression`; the raw statement text is stamped on
  the root after parsing.
- **Dialect contract** — DIALECT.md (`cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`) is the authoritative supported /
  recognized / rejected matrix. `SqlLanguageProfile.Instance` carries its lexical
  tables and implemented clause set; recognized clauses outside that set report
  `CK00001` rather than an undifferentiated parse failure.
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
