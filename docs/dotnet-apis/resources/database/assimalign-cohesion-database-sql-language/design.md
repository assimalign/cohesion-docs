# Assimalign.Cohesion.Database.Sql.Language design

This page describes the boundaries and implementation shape of `Assimalign.Cohesion.Database.Sql.Language`.

> **Status:** Partial.

The SQL front-end (area architecture: resources/`Database`/DESIGN.md
(`cohesion/docs/resources/Database/DESIGN.md`) `3.3). The dialect is deliberately fundamental —
basic SQL, declared precisely — and structured to be extended without churning what exists
(open/closed).

## Design intent

`Parse` the declared dialect into a stable, fully-typed AST that planners, catalogs, tooling, and the
SDK schema compiler can rely on. "Declared" is the operative word: DIALECT.md
(`cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`) is a
contract, not aspiration — the parser, the matrix, and the conformance corpus change together, and
anything outside the matrix fails loudly instead of half-parsing. `SqlLanguageProfile.Instance` is
the executable contract: it owns the SQL keyword/function tables and opts into only the clauses the
parser implements today. Recognized clauses outside that set produce the shared `COHDBL001`
model-specific diagnostic; `SQL0002` remains the unknown-command diagnostic.

## Why-this-not-that decisions

- **Recursive descent, hand-written** — over a parser generator. The dialect is
  small, error tolerance matters more than grammar elegance, the AST is the
  product, and AOT rules out runtime-generated parsers. Keyword dispatch at the
  statement level and a precedence ladder for expressions make extension points
  obvious: a new statement kind is a new branch + partial file; a new operator is
  a new rung.
- **Error-tolerant, total parsing.** Malformed input yields a statement with
  diagnostics — never an exception. Hostile inputs are part of the conformance
  corpus. This is what lets tooling (editors, the schema compiler) reuse the
  parser on incomplete text.
- **Sealed AST nodes with internal constructors.** The parser is the only
  producer; consumers pattern-match. Extending the AST is a kernel change, which
  keeps downstream planners honest (no third-party node types appearing mid-plan).
- **Positions are offsets; line/column is presentation.** Nodes carry absolute
  character offsets. Mapping offsets to line/column belongs to the tool holding
  the source text (Roslyn's model) — carrying line numbers per node would bloat
  every node for a consumer that rarely needs them. The raw statement text is
  stamped once on the root (`SqlQueryExpression.Text`) after parsing.
- **String literal nodes carry the value, not the lexeme** — quotes stripped,
  doubled quotes unescaped — because every consumer (executor, planner, schema
  compiler) wants the value, and exactly one component (the parser) knows the
  escaping rules.
- **Delimited identifier nodes carry the identifier value.** `Name` consumption
  strips the surrounding double quotes for tables, schemas, aliases, columns,
  indexes and constraints. Tokens retain their quoted-identifier classification,
  so a quoted reserved word cannot become a keyword or literal. Keyword dispatch,
  literal parsing and diagnostics continue to consume raw token text. Embedded
  double-quote escapes are outside the current lexer subset.
- **Type names resolve through one table.** `SqlTypeNames` is the single
  SQL-name → `DatabaseType` mapping (with the `DECIMAL(p[,s])`
  argument-is-precision rule); catalogs and the schema compiler must not grow
  their own copies.
- **Recognized-but-unsupported tokens stay in the lexer tables.** Recursive `WITH`
  and window functions are lexed so diagnostics can
  say "unsupported" precisely rather than mis-parsing them as identifiers. Gateable
  names live in `SqlClauses`; supported names are present in `SqlLanguageProfile`,
  while excluded clauses such as recursive CTEs, windows,
  and unsupported join forms remain absent. Views and DML RETURNING execute. Non-recursive WITH, FETCH and
  lexical SELECT TOP execute; PERCENT, WITH TIES and DML TOP retain capability diagnostics. `ParseCore` scans a copy of its configured
  lexer before recursive descent and attaches the first rejected clause to the
  statement, so analyzers observe `COHDBL001` with the token location and the `SQL`
  surface name. An actually unknown leading command still receives `SQL0002`, even
  when a later recognized token is also unsupported.

## Namespace note

Types live in `Assimalign.Cohesion.Database.Sql.Language`, matching the assembly name (the repo
rule). Earlier scaffolding used `...Database.Language.Sql`; the rename happened before external
consumers existed.

## Non-goals (current dialect)

Recursive/nested CTEs, window functions, `ON UPDATE`, `MERGE`, RETURNING subqueries/OLD/NEW/INTO, savepoints,
isolation-level syntax, and cost-hint syntax. Each is an additive dialect extension when its engine
feature lands.

## Namespaces and query bodies

[SqlCreateSchemaExpression](sql-create-schema-expression.md) and [SqlDropSchemaExpression](sql-drop-schema-expression.md)
carry namespace guards and dependency policy. AUTHORIZATION/nested elements are COHDBL001.
The engine seeds and protects dbo, persists namespaces as kind 15 records in current format 25, and keeps
code-first provisioning in dbo. [Schema diagnostics](../../../../database/sql/language/diagnostics.md)
include COHSQLE028–031 and their source SQLSTATE categories.

[SqlQueryBodyExpression](sql-query-body-expression.md) now owns CommonTables and result tails;
SELECT and binary [set operations](sql-set-operation-expression.md) inherit it. CTE, derived,
scalar/IN/EXISTS and INSERT query-source members use this base. Parser precedence is INTERSECT
before left-associative UNION/EXCEPT. Combined ORDER BY accepts output names/ordinals only;
CORRESPONDING remains contextual COHDBL001 and changes no persisted keyword quoting.

## Transactions and B7 extension points

`SqlTransactionExpression` identifies `BEGIN`, `COMMIT`, and `ROLLBACK` through
`SqlQueryCommandType`. BEGIN/COMMIT/ROLLBACK also accept WORK; START TRANSACTION maps to BEGIN and bare END to COMMIT.
START/WORK remain contextual. END takes no suffix; unsupported suffixes are SQL0003. The executor joins
subsequent statements to the session's active scope, and disconnect aborts that transaction. The
language adds no concurrency machinery. Malformed suffixes produce `SQL0003` instead of silently
treating `ROLLBACK TO` as a complete rollback.

B7 anticipates **`SAVEPOINT <name>`**, **`ROLLBACK TO [SAVEPOINT] <name>`**, **`RELEASE [SAVEPOINT]
<name>`**, and **`SET TRANSACTION ISOLATION LEVEL <level>`**. Those forms are not implemented by B2.
The session owns a stack of transaction scopes with exactly one root entry today, plus a defaulted
value of the existing transaction coordinator's `IsolationLevel`. Savepoints can add scope markers
and isolation syntax can set that value without replacing the session representation.

## `Constraint` normalization and persistence

`Column` and table declarations produce `SqlConstraintDefinition` nodes with the same shape: optional
name, ordered key columns, foreign-key target and deletion action, or a check predicate.
`SqlCreateTableExpression.Constraints` contains all constraints; column definitions also retain
their own constraints so `ALTER TABLE ADD COLUMN` carries the same information. The executor
consumes the normalized table list once. `CHECK` retains both its expression tree and exact
predicate source; the catalog persists source and reparses it after restart without reflection or a
second expression codec. `New` constraint syntax follows the DDL parser's diagnostic recovery
discipline and emits `SQL0003` on malformed input.

**`UNIQUE` is a unique index, not a new compiled-schema constraint kind.** The syntax still has a
`Unique` AST kind so it can preserve the declaration and name, but execution lowers it to the same
unique catalog index used by `CREATE UNIQUE INDEX` and `CompiledSchemaIndex(IsUnique: true)`. This
keeps compiled schemas, SQL DDL, persistence, and concurrent uniqueness enforcement on one path.
Foreign keys and checks remain constraint catalog records. ON DELETE accepts CASCADE, RESTRICT,
NO ACTION (the SQL default), SET NULL and SET DEFAULT. ON UPDATE stays outside the profile
and reports `COHDBL001`. See [constraint semantics](../../../../database/sql/language/constraints.md).

## T1 literal, type and request forms

[SqlSelectExpression.From](sql-select-expression.md) contains comma-separated [SqlFromItem](sql-from-item.md)
join trees. Each [SqlJoinClause](sql-join-clause.md) carries ON or ordered UsingColumns; INNER,
LEFT and CROSS execute, while RIGHT/FULL, NATURAL and qualified wildcards stay
excluded. Table members use [SqlRelationReference](sql-relation-reference.md), with named-table and
[derived-query](sql-derived-table-reference.md) leaves. A derived alias is required and its optional
column list must rename the full output width. SELECT and UPDATE/DELETE WHERE accept correlated
and uncorrelated subqueries; SET remains row-local. Local names shadow outer scopes. A real derived
sibling reference requires LATERAL; enclosing-query references execute. [WITH](sql-common-table-expression.md)
definitions bind in declaration order, with schema-less names and stored-table fallback for self or
later-sibling names. Every definition binds; reachable definitions materialize once before writes.
The renderer and persisted definitions use this canonical shape. Current SQL storage format 25
quotes newly executable function names and BOTH/LEADING/TRAILING when used as identifiers in
persisted CHECKs, defaults and function bodies. It follows format 17 (default/missing-value split,
table extension 6) and format 16 (SEARCH/MATERIALIZED quoting). Older formats are refused without
an upgrade reader; compiled schema version remains 7.

[SqlLiteralText](sql-literal-text.md) is the strict public grammar for temporal/day-time interval
and hexadecimal text, reused by CAST. Typed literals and literal DEFAULTs carry canonical text;
malformed contents are SQL0008 and year-month intervals COHDBL001. [SqlTypeNames](sql-type-names.md)
resolves contextual multi-word/national-character and SQL Server lexical aliases without reserving
new words. TIME WITH TIME ZONE is recognized but rejected with SQL0005.

SqlDefaultExpression is valid only as a whole VALUES item or SET value. SqlInsertExpression exposes
IsDefaultValues. SqlExplainExpression wraps data statements for planning without execution. The
profile contains only functions with executors; recognized unsupported calls use COHDBL001.
Parameter names share SQL/OQL/GQL's ordinal bare-name rules (COHSQLE023 for collisions).

[SqlColumnDefinition.Identity](sql-column-definition.md) exposes [SqlIdentityDefinition](sql-identity-definition.md):
mode, signed start and increment. SqlClauses.Identity is the `IDENTITY` capability name. CREATE and
ADD accept GENERATED ALWAYS/BY DEFAULT or the SQL Server alias. Identity implies NOT NULL;
explicit NULL with NOT NULL/PRIMARY KEY/IDENTITY is SQL0003 in either order. SERIAL-family type
and domain names are reserved unsupported, suggesting GENERATED BY DEFAULT AS IDENTITY.

[SQL diagnostics](../../../../database/sql/language/diagnostics.md) lists SQL0008, COHSQLE001–036,
transaction codes and source SQLSTATE categories; binding spans survive in DatabaseException.
Database creation/open additionally uses COHSQLC001 for incomplete or ambiguous file sets, with no
assigned SQLSTATE. See [creation diagnostics](../../../../database/sql/language/diagnostics.md#database-creation-and-open).

## AOT posture

Hand-written parser over the zero-allocation `TokenLexer` ref struct; no reflection, no grammar
codegen.

## Declared dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Language` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |

[Assembly overview](index.md) · [Examples](examples/index.md)

## ALTER actions and missing values

[SqlAlterTableExpression](sql-alter-table-expression.md) exposes Table and Action; its new action
leaves carry table/column rename names, SET DEFAULT expressions or a column name for DROP DEFAULT
and SET/DROP NOT NULL. They have internal constructors and read-only properties. The engine owns
validation, ownership checks and atomic publication. RENAME/TO are contextual identifiers.
Renames carry CHECK/FK/index references without changing row/index identity. Defaults use INSERT
assignment compatibility; Stable/Volatile calls execute per row on the transaction clock. The catalog separates
insert defaults from frozen ADD COLUMN missing values in table extension 6 (format 17); the current
format is 25 for distinct predicates, Boolean tests and LIKE ESCAPE, following format 24 for NULL
placement, format 23 for views and format 22 for STRING_AGG/LISTAGG identifier quoting, after
format 21 for persisted namespaces and format 20 for referential actions. See [ALTER TABLE](../../../../database/sql/language/statements/alter-table.md).

Format 19 quotes PERCENT and TIES in persisted SQL. Format 20 adds persisted SET NULL,
SET DEFAULT and NO ACTION delete actions. Format 21 introduced namespace records of kind 15. Format 22 quotes STRING_AGG and LISTAGG
identifiers in persisted SQL. Format 23 introduced catalog views and format 24 explicit NULL
placement. Current format 25 adds distinct predicates, Boolean tests and LIKE ESCAPE.
Every earlier format is refused at open; there is no
upgrade path. Compiled schema format remains version 7.

## Aggregate call modifiers

[SqlFunctionCallExpression](sql-function-call-expression.md) carries Quantifier, OrderBy and
HasWithinGroupOrdering. The catalog decides aggregate identity; scalar modifiers use COHSQLE032,
as do DISTINCT ordering keys absent from arguments (no assigned SQLSTATE). Rendering, depth,
structural equality and persisted validation include order keys. LISTAGG WITHIN GROUP uses the
same ordered input shape; explicit ALL renders but executes like Default. STRING_AGG and LISTAGG
join the executable function profile; no named clause is added. Format 22 introduced quoting for their
names as identifiers in persisted SQL; compiled schema version stays 7. There is no upgrade reader.

## Views, NULL placement and RETURNING

[CREATE](sql-create-view-expression.md)/[DROP VIEW](sql-drop-view-expression.md) use typed nodes.
Bodies use the existing query-body grammar. The engine persists expanded bound inputs and output
aliases remain output references. [SqlOrderByColumn](sql-order-by-column.md) independently retains
Default/First/Last placement; the renderer emits only written modifiers. Format 24 introduced
persisted NULL modifiers, following format 23's view records. Current format 25 adds the predicates below.

DML nodes retain optional Returning SELECT lists. OUTPUT, OLD/NEW, INTO and RETURNING WITH (...)
are contextual refusals; ordinary identifier quoting is unchanged. Subqueries and windows remain
excluded there, while aggregates are refused by the planner with COHSQLE017.

## Predicate nodes and precedence

[SqlDistinctExpression](sql-distinct-expression.md), [SqlBooleanTestExpression](sql-boolean-test-expression.md)
and [SqlTruthValue](sql-truth-value.md) represent NULL-safe distinct predicates and truth tests.
[SqlLikeExpression.Escape](sql-like-expression.md) retains an optional scalar child. Depth,
rendering and every child walk include all operands. Constructors remain internal.

The parser follows PostgreSQL's IS/comparison/predicate/ESCAPE ladder, with concatenation below
addition and unary signs above COLLATE. Non-associative chains report SQL0003 at their second
operator; completed IN/NOT IN lists or subqueries may continue through another predicate.
Canonical rendering preserves conservative comparison/IS grouping and LIKE operand parentheses.
DEFAULT's restricted grammar includes comparisons/distinct predicates while leaving following
constraints unconsumed. Contextual ILIKE/SIMILAR TO and postfix ISNULL/NOTNULL are COHDBL001.
These expressions add no named clause or reserved identifier spelling. Format 25 gates the
persisted grammar and changed precedence; earlier files have no upgrade reader.

## Conflict clause shape

[SqlInsertExpression.OnConflict](sql-insert-expression.md) retains an optional
[SqlOnConflictClause](sql-on-conflict-clause.md) with target kind, inference columns or constraint
name, action, assignments, WHERE and the parser-supplied EXCLUDED relation. The
[target](sql-on-conflict-target-kind.md) and [action](sql-on-conflict-action.md) enums keep that
shape explicit. SqlClauses.OnConflict enables the new named clause, bringing execution to 51 of
57 declared clauses. Existing keywords/identifier quoting and format 25 are unchanged.

Malformed forms retain syntax diagnostics; unsupported inference/action subqueries and MySQL/SQLite
alternatives receive COHDBL001. Binding owns unique-index selection (COHSQLE035, 42P10) and repeated
effects use COHSQLE036 (21000). See [ON CONFLICT](../../../../database/sql/language/clauses/on-conflict.md)
for EXCLUDED scope, constraints, isolation and retries.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Assimalign.Cohesion.Database.Sql.Language.csproj`.
