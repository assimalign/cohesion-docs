# Assimalign.Cohesion.Database.Sql.Language design

This page describes the boundaries and implementation shape of `Assimalign.Cohesion.Database.Sql.Language`.

> **Status:** Partial.

The SQL front-end (area architecture: resources/`Database`/DESIGN.md
(`cohesion/docs/resources/Database/DESIGN.md`) `3.3). The dialect is deliberately fundamental —
basic SQL, declared precisely — and structured to be extended without churning what exists
(open/closed).

## ALTER COLUMN TYPE

[SqlAlterColumnTypeAction](sql-alter-column-type-action.md) retains a target SqlColumnDefinition
and optional original-row UsingExpression. The renderer overload canonicalizes TYPE, COLLATE,
USING and escaped domain names. Execution requires assignment/implicit casts for the old value,
USING result and independent default (CS02064 / 42804). Compatible widenings avoid rewriting;
other changes use an inline exclusive-lock rewrite with CHECK/domain, FK and index validation.
Full-definition default/implicit collation changes retain CS05013 (no SQLSTATE). Dependent views
retain CS03302 (2BP01), invalid identity conversion retains CS03025. See
[ALTER TABLE](../../../../database/sql/language/statements/alter-table.md#alter-column-type).

## Design intent

[SqlTruncateExpression](sql-truncate-expression.md) stores an immutable ordered target list and
effective RestartIdentity/Cascade flags; [SqlTruncateTable](sql-truncate-table.md) retains each
table's ONLY modifier. TABLE is optional; CONTINUE IDENTITY and RESTRICT are defaults. Canonical
rendering supplies TABLE and both effective policies. SqlQueryCommandType.Truncate is appended
as value 12; Begin/Commit/Rollback keep 9/10/11. SqlClauses.Truncate is `TRUNCATE`.
The [statement reference](../../../../database/sql/language/statements/truncate.md) describes
CS03067 (0A000, incomplete RESTRICT closure), CS04066 (25001, explicit transaction with earlier
work retained), and retryable CS04064 (40001, concurrent closure change).
[SqlCreateTableAsExpression](sql-create-table-as-expression.md) represents inferred-table creation,
including a complete optional name list and WITH DATA/NO DATA policy. It shares ordinary query
parsing and view output inference; execution uses atomic publication in SQL format 29.

CTAS output-name failures use CS03068 (42601); explicit transactions use CS04003 (25001).
[SqlMergeExpression](sql-merge-expression.md) and [SqlMergeWhenClause](sql-merge-when-clause.md)
retain ordered actions and clause spans for execution and EXPLAIN. A later clause after an unconditional
clause of the same match kind uses CS05071; merge_action() outside MERGE RETURNING uses CS01053.
Neither has an assigned SQLSTATE. MERGE executes; CS05036 (21000) refuses a second selected
modification, including after rechecks, and CS05067 (53200) refuses retained
state above the statement grant before writes. Action conditions, values and read subqueries
are lazy; RETURNING subqueries remain CK00001. ReadCommitted locks the latest target and reevaluates
ON and ordered WHEN clauses after concurrent commits; Snapshot uses retryable CS04064 (40001).
Mixed-action foreign keys see final rows and direct/referential overlaps compose without CS05036.
Statement-level uniqueness accepts key swaps that PostgreSQL's immediate checks reject and
reuses keys freed by MERGE deletes. Unique failures precede final FK and referential SET-action
constraint failures. See [MERGE](../../../../database/sql/language/statements/merge.md).

`Parse` the declared dialect into a stable, fully-typed AST that planners, catalogs, tooling, and the
SDK schema compiler can rely on. "Declared" is the operative word: DIALECT.md
(`cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`) is a
contract, not aspiration — the parser, the matrix, and the conformance corpus change together, and
anything outside the matrix fails loudly instead of half-parsing. `SqlLanguageProfile.Instance` is
the executable contract: it owns the SQL keyword/function tables and opts into only the clauses the
parser implements today. Recognized clauses outside that set produce the shared `CK00001`
model-specific diagnostic; `CS00002` remains the unknown-command diagnostic.

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
  literal parsing and diagnostics continue to consume raw token text. Doubled double quotes
  decode to one embedded quote, and expression rendering escapes them the same way.
  Column-reference spans include the complete raw token for persisted-view binding edits.
  An escaped final quote without a closing delimiter reports CS00003. Earlier stored view text
  may use adjacent quoted tokens for a column and alias; `"a""x"` now names one identifier.
  Recreate affected views with whitespace or AS; see the
  [view compatibility note](../../../../database/sql/language/statements/create-view.md#delimited-identifiers).
- **Type names resolve through one table.** `SqlTypeNames` is the single
  SQL-name → `DatabaseType` mapping (with the `DECIMAL(p[,s])`
  argument-is-precision rule); catalogs and the schema compiler must not grow
  their own copies.
- **Recognized-but-unsupported tokens stay in the lexer tables.** Row-locking tails
  and window functions are lexed so diagnostics can
  say "unsupported" precisely rather than mis-parsing them as identifiers. Gateable
  names live in `SqlClauses`; supported names are present in `SqlLanguageProfile`,
  while excluded clauses such as NATURAL remain absent. Ranking/navigation windows execute.
  RECURSIVE is advertised for binding only; recursive execution remains CK00001. Views and DML RETURNING execute. Non-recursive WITH, FETCH and
  lexical SELECT TOP execute; PERCENT, WITH TIES and DML TOP retain capability diagnostics. `ParseCore` scans a copy of its configured
  lexer before recursive descent and attaches the first rejected clause to the
  statement, so analyzers observe `CK00001` with the token location and the `SQL`
  surface name. An actually unknown leading command still receives `CS00002`, even
  when a later recognized token is also unsupported.

## SELECT evaluation

The grammar and public AST are unchanged by bounded selection. Unordered non-DISTINCT limits stop
projection/HAVING at the qualifying window; table/system scans also stop reading. Fixed-collation
ORDER BY with a non-DISTINCT limit retains OFFSET + LIMIT candidates with stable input ties.
Independent volatile outputs keep full sorting and sorted-order projection before pagination;
ordinary nonvolatile projections evaluate before each input's keys. DISTINCT and upstream
aggregation/join/set materialization remain. These are engine rules, documented with examples in
[SELECT](../../../../database/sql/language/statements/select.md#bounded-result-selection) and
[ORDER BY](../../../../database/sql/language/clauses/order-by.md#evaluation-order).

## Result encoding diagnostics

Result encoding uses CS05045 for uncoded preparation failures and header/row size refusals
(SQLSTATE 54000 for size refusals). Writes prepare before commit and roll back only the failed
statement; reads encode during delivery and may terminate partial results. The connection and
explicit transaction remain usable. The 16 MiB limit applies per header/row, with no aggregate cap.
Parser and AST shapes are unchanged. See [result diagnostics](../../../../database/sql/language/diagnostics.md#result-encoding-and-delivery).

## Namespace note

Types live in `Assimalign.Cohesion.Database.Sql.Language`, matching the assembly name (the repo
rule). Earlier scaffolding used `...Database.Language.Sql`; the rename happened before external
consumers existed.

## Non-goals (current dialect)

Boolean positions use the existing AST. The engine requires Boolean search conditions in WHERE,
HAVING, JOIN ON, CHECK/domain CHECK, searched CASE WHEN and ON CONFLICT WHERE, with CS02003
(42804) for non-Boolean types. Valid Boolean string literals coerce only in Boolean positions;
every CASE/COALESCE alternative there must qualify. See [search conditions](../../../../database/sql/language/clauses/where.md#boolean-search-conditions).

Every SELECT may omit FROM and operate on one implicit row, including top-level and nested
queries, views, derived tables and set operands. A wildcard still needs a relation (CS05013).
The grammar and AST are unchanged; see [SELECT](../../../../database/sql/language/statements/select.md#c-select-without-from).

Recursive execution, nested CTEs, explicit window frames and aggregate windows, RETURNING subqueries/OLD/NEW/INTO, savepoints,
isolation-level syntax, and cost-hint syntax. Each is an additive dialect extension when its engine
feature lands.

## Namespaces and query bodies

[SqlCreateSchemaExpression](sql-create-schema-expression.md) and [SqlDropSchemaExpression](sql-drop-schema-expression.md)
carry namespace guards and dependency policy. AUTHORIZATION/nested elements are CK00001.
The engine seeds and protects dbo, persists namespaces as kind 15 records in current format 29, and keeps
code-first provisioning in dbo. [Schema diagnostics](../../../../database/sql/language/diagnostics.md)
include CS01028, CS03029, CS03030 and CS03031 and their source SQLSTATE categories.

[SqlQueryBodyExpression](sql-query-body-expression.md) now owns CommonTables and result tails;
SELECT and binary [set operations](sql-set-operation-expression.md) inherit it. CTE, derived,
scalar/IN/EXISTS and INSERT query-source members use this base. Parser precedence is INTERSECT
before left-associative UNION/EXCEPT. Combined ORDER BY accepts output names/ordinals only;
CORRESPONDING remains contextual CK00001 and changes no persisted keyword quoting.

## Transactions and B7 extension points

`SqlTransactionExpression` identifies `BEGIN`, `COMMIT`, and `ROLLBACK` through
`SqlQueryCommandType`. BEGIN/COMMIT/ROLLBACK also accept WORK; START TRANSACTION maps to BEGIN and bare END to COMMIT.
START/WORK remain contextual. END takes no suffix; unsupported suffixes are CS00003. The executor joins
subsequent statements to the session's active scope, and disconnect aborts that transaction. The
language adds no concurrency machinery. Malformed suffixes produce `CS00003` instead of silently
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
discipline and emits `CS00003` on malformed input.

**`UNIQUE` is a unique index, not a new compiled-schema constraint kind.** The syntax still has a
`Unique` AST kind so it can preserve the declaration and name, but execution lowers it to the same
unique catalog index used by `CREATE UNIQUE INDEX` and `CompiledSchemaIndex(IsUnique: true)`. This
keeps compiled schemas, SQL DDL, persistence, and concurrent uniqueness enforcement on one path.
Foreign keys and checks remain constraint catalog records. ON DELETE accepts CASCADE, RESTRICT,
NO ACTION (the SQL default), SET NULL and SET DEFAULT. ON UPDATE accepts the same declarations;
changing-action execution retains CK00001 until batch 13. See [constraint semantics](../../../../database/sql/language/constraints.md).

## T1 literal, type and request forms

[SqlSelectExpression.From](sql-select-expression.md) contains comma-separated [SqlFromItem](sql-from-item.md)
join trees. Each [SqlJoinClause](sql-join-clause.md) carries ON or ordered UsingColumns; INNER,
LEFT, RIGHT, FULL and CROSS execute, while NATURAL and qualified wildcards stay
excluded. Table members use [SqlRelationReference](sql-relation-reference.md), with named-table and
[derived-query](sql-derived-table-reference.md) leaves. A derived alias is required and its optional
column list must rename the full output width. SELECT and UPDATE/DELETE WHERE accept correlated
and uncorrelated subqueries; SET remains row-local. Local names shadow outer scopes. A real derived
sibling reference requires LATERAL; enclosing-query references execute. [WITH](sql-common-table-expression.md)
definitions without RECURSIVE bind in declaration order, with schema-less names and stored-table
fallback for self or later-sibling names. RECURSIVE lists use dependency order; recursive execution
is refused with CK00001 after binding. Every definition binds; reachable definitions materialize once before writes.
The renderer and persisted definitions use this canonical shape. Current SQL storage format 29
quotes newly executable function names and BOTH/LEADING/TRAILING when used as identifiers in
persisted CHECKs, defaults and function bodies. It follows format 17 (default/missing-value split,
table extension 6) and format 16 (SEARCH/MATERIALIZED quoting). Older formats are refused without
an upgrade reader; compiled schema version remains 8.

RIGHT preserves written column slots and makes the whole left subtree nullable; USING reads the
coerced right value and its collation. [SqlJoinedTableReference](sql-joined-table-reference.md)
retains unaliased parenthesized groups, including derived-first groups. Aliased groups lower to
derived SELECTs and hide component names. EXPLAIN names RIGHT-to-LEFT normalization.
FULL uses common-typed COALESCE for merged USING keys and makes both qualified inputs nullable.
A merged key is nonnullable only when both original inputs were nonnullable. FULL requires a safe
mandatory hash equality and proven non-throwing complete ON evaluation; CS05070 refuses other
conditions before reads (no SQLSTATE). EXPLAIN describes FullHashJoin and unmatched=both.
Direct RIGHT, FULL or unaliased grouped sources in joined DML fail CS05069 before reads/writes (no
SQLSTATE); a derived SELECT containing RIGHT or FULL executes. See
[JOIN](../../../../database/sql/language/clauses/join.md).

[SqlLiteralText](sql-literal-text.md) is the strict public grammar for temporal/day-time interval
and hexadecimal text, reused by CAST. Typed literals and literal DEFAULTs carry canonical text;
malformed contents are CS00008 and year-month intervals CK00001. [SqlTypeNames](sql-type-names.md)
resolves contextual multi-word/national-character and SQL Server lexical aliases without reserving
new words. TIME WITH TIME ZONE is recognized but rejected with CS00005.

SqlDefaultExpression is valid only as a whole VALUES item or SET value. SqlInsertExpression exposes
IsDefaultValues. SqlExplainExpression wraps data statements for planning without execution. The
profile contains only functions with executors; recognized unsupported calls use CK00001.
Parameter names share SQL/OQL/GQL's ordinal bare-name rules (CS01023 for collisions).

[SqlColumnDefinition.Identity](sql-column-definition.md) exposes [SqlIdentityDefinition](sql-identity-definition.md):
mode, signed start and increment. SqlClauses.Identity is the `IDENTITY` capability name. CREATE and
ADD accept GENERATED ALWAYS/BY DEFAULT or the SQL Server alias. Identity implies NOT NULL;
explicit NULL with NOT NULL/PRIMARY KEY/IDENTITY is CS00003 in either order. SERIAL-family column
aliases mean BY DEFAULT identity; serial domain names remain CK00001.

[SQL diagnostics](../../../../database/sql/language/diagnostics.md) lists CS00008, SQL execution codes,
transaction codes and source SQLSTATE categories; binding spans survive in DatabaseException.
CS05042 / SQLSTATE 54000 is the engine's pre-apply encoded row/index-key size refusal;
see [size limits](../../../../database/sql/language/diagnostics.md#row-and-index-key-limits).
Database creation/open additionally uses CS03401 for incomplete or ambiguous file sets, with no
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
format is 29; format 25 introduced distinct predicates, Boolean tests and LIKE ESCAPE after format 24 for NULL
placement, format 23 for views and format 22 for STRING_AGG/LISTAGG identifier quoting, after
format 21 for persisted namespaces and format 20 for referential actions. See [ALTER TABLE](../../../../database/sql/language/statements/alter-table.md).

Format 19 quotes PERCENT and TIES in persisted SQL. Format 20 adds persisted SET NULL,
SET DEFAULT and NO ACTION delete actions. Format 21 introduced namespace records of kind 15. Format 22 quotes STRING_AGG and LISTAGG
identifiers in persisted SQL. Format 23 introduced catalog views and format 24 explicit NULL
placement. Format 25 added distinct predicates, Boolean tests and LIKE ESCAPE.
Format 26 fenced temporal DEFAULT and frozen missing-value text.
Current format 29 adds owned-sequence metadata and retains TYPE default assignment coercion
in table extension 7.
Every earlier format is refused at open; there is no
upgrade path. Compiled schema format remains version 8.

## Aggregate call modifiers

[SqlFunctionCallExpression](sql-function-call-expression.md) carries Filter, Quantifier, OrderBy and
HasWithinGroupOrdering. The catalog decides aggregate identity; scalar modifiers use CS05032,
as do DISTINCT ordering keys absent from arguments (no assigned SQLSTATE). Rendering, depth,
structural equality and persisted validation include order keys. LISTAGG WITHIN GROUP uses the
same ordered input shape; explicit ALL renders but executes like Default. STRING_AGG and LISTAGG
join the executable function profile; no named clause is added. Format 22 introduced quoting for their
names as identifiers in persisted SQL; compiled schema version stays 8. There is no upgrade reader.

FILTER retains its condition in depth, source extent, rendering, child walks, structural equality
and aggregate-level correlation. The catalog rejects scalar-call FILTER with CS05072 (42809),
same-scope condition aggregates with CS05073 (42803), and non-Boolean conditions with CS02003
(42804). Child queries bind independently; aggregate windows retain CK00001. SqlClauses.Filter adds one
named clause and nine Boolean/statistical function names join the executable profile: 74 of 76
clauses including the two exclusions. See [FILTER](../../../../database/sql/language/clauses/filter.md).

## Views, NULL placement and RETURNING

[CREATE](sql-create-view-expression.md)/[DROP VIEW](sql-drop-view-expression.md) use typed nodes.
Bodies use the existing query-body grammar. The engine persists expanded bound inputs and output
aliases remain output references. [SqlOrderByColumn](sql-order-by-column.md) independently retains
Default/First/Last placement; the renderer emits only written modifiers. Format 24 introduced
persisted NULL modifiers, following format 23's view records. Format 25 introduced the predicates below; current format 29 also fences temporal default text.

DML nodes retain optional Returning SELECT lists. OUTPUT, OLD/NEW, INTO and RETURNING WITH (...)
are contextual refusals; ordinary identifier quoting is unchanged. RETURNING subqueries retain
CK00001, windows use CS05076 (42P20), and aggregates use CS05017.

## Predicate nodes and precedence

[SqlDistinctExpression](sql-distinct-expression.md), [SqlBooleanTestExpression](sql-boolean-test-expression.md)
and [SqlTruthValue](sql-truth-value.md) represent NULL-safe distinct predicates and truth tests.
[SqlLikeExpression.Escape](sql-like-expression.md) retains an optional scalar child. Depth,
rendering and every child walk include all operands. Constructors remain internal.

The parser follows PostgreSQL's IS/comparison/predicate/ESCAPE ladder, with concatenation below
addition and unary signs above COLLATE. Non-associative chains report CS00003 at their second
operator; completed IN/NOT IN lists or subqueries may continue through another predicate.
Canonical rendering preserves conservative comparison/IS grouping and LIKE operand parentheses.
DEFAULT's restricted grammar includes comparisons/distinct predicates while leaving following
constraints unconsumed. Contextual ILIKE/SIMILAR TO and postfix ISNULL/NOTNULL are CK00001.
These expressions add no named clause or reserved identifier spelling. Current format 29 admits the
persisted grammar and changed precedence; earlier files have no upgrade reader.

## Conflict clause shape

[SqlInsertExpression.OnConflict](sql-insert-expression.md) retains an optional
[SqlOnConflictClause](sql-on-conflict-clause.md) with target kind, inference columns or constraint
name, action, assignments, WHERE and the parser-supplied EXCLUDED relation. The
[target](sql-on-conflict-target-kind.md) and [action](sql-on-conflict-action.md) enums keep that
shape explicit. SqlClauses.OnConflict enables the named clause. With quantified comparisons,
UPDATE FROM, DELETE USING and CREATE TABLE AS, the profile measures 74 of 76 declared clauses,
including executable MERGE, binding-only RECURSIVE and nonexecuting plain EXPLAIN. Existing
keywords/identifier quoting and format 29 are unchanged.

Malformed forms retain syntax diagnostics; unsupported inference/action subqueries and MySQL/SQLite
alternatives receive CK00001. Binding owns unique-index selection (CS03035, 42P10) and repeated
effects use CS05036 (21000). See [ON CONFLICT](../../../../database/sql/language/clauses/on-conflict.md)
for EXCLUDED scope, constraints, isolation and retries.

Server-side statement and individual lock-wait deadlines use CS05043 / SQLSTATE 57014
and CS04044 / SQLSTATE 55P03. ExecutionFailure preserves the connection and explicit
transaction after statement rollback; earlier grants and upgrades remain, and new resources are
released after undo. CS04006 has no assigned SQLSTATE and means the bound target changed during
lock preparation: retry in a fresh transaction. SQLSTATE remains a reference mapping on the wire.
See [timeouts and diagnostics](../../../../database/sql/language/diagnostics.md#statement-and-lock-timeouts).


## Quantified comparisons and grouping

Scalar [ANY/SOME/ALL comparisons](sql-quantified-comparison-expression.md) retain the comparison,
written quantifier and complete query body in the AST. SqlClauses.QuantifiedComparison advertises
the form. Rendering and depth accounting include the body; arrays, row values and excluded
subquery positions retain their diagnostics. The engine matches repeated complete subquery
grouping keys structurally, including correlations, across SELECT, HAVING and ORDER BY. IN keys
also compare query bodies. See [GROUP BY](../../../../database/sql/language/clauses/group-by.md#repeated-subquery-keys).

## Joined mutations

SqlUpdateExpression.From and SqlDeleteExpression.Using retain ordered source trees. Both forms
execute; SqlClauses.UpdateFrom (`UPDATE FROM`) and SqlClauses.DeleteUsing (`DELETE USING`)
advertise them. UPDATE raises CS05068 (21000) for multiple source associations before SET or
writes. DELETE deduplicates physical targets. Bare RETURNING * follows target-then-visible-source
order with JOIN USING merges. See [UPDATE](../../../../database/sql/language/statements/update.md#from-sources)
and [DELETE](../../../../database/sql/language/statements/delete.md#using-sources).

## WITH RECURSIVE binding

[WITH RECURSIVE](../../../../database/sql/language/clauses/with.md) binds all list names in dependency order. The non-recursive
term fixes output width, types, modifiers, domains and collations; use the cast or COLLATE hint
to preserve that contract. Recursive definitions and the consuming statement bind, then planning,
EXPLAIN and execution refuse with CK00001, even for unused recursion. Acyclic lists execute.
Mutual recursion, recursive UNION ORDER BY/LIMIT/FETCH/OFFSET, SELECT locking tails and WITH
RECURSIVE ... MERGE remain CK00001. Plain WITH ... MERGE executes. No format changes apply.

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS01054 | Recursive definition lacks top-level anchor UNION [ALL] recursive term | 42P19 |
| CS01055 | Self-reference in the non-recursive term | 42P19 |
| CS01056 | Self-reference inside a scalar, IN, quantified or EXISTS subquery | 42P19 |
| CS01057 | Self-reference on the nullable side of an outer join | 42P19 |
| CS01058 | Self-reference under INTERSECT ALL | 42P19 |
| CS01059 | Self-reference on either side of EXCEPT ALL or the right of EXCEPT DISTINCT | 42P19 |
| CS01060 | Recursive term does not have exactly one self-reference | 42P19 |
| CS01061 | Aggregate at the query level containing the self-reference | 42P19 |
| CS02065 | Anchor, recursive output or declared column-list width mismatch | 42601 |
| CS02066 | UNION changes the anchor type, domain or modifiers; cast hint | 42804 |
| CS02067 | UNION changes or conflicts with anchor collation; COLLATE hint | 42P21 |

## Sequence language surface

The parser retains written options through [SqlSequenceOption](sql-sequence-option.md), with
dedicated [CREATE](sql-create-sequence-expression.md), [ALTER](sql-alter-sequence-expression.md)
and [DROP](sql-drop-sequence-expression.md) nodes. [SqlNextValueExpression](sql-next-value-expression.md)
is distinct from ordinary function calls. DISCARD ALL uses SqlTransactionExpression with
SqlQueryCommandType.Discard. The profile enables CreateSequence, AlterSequence, DropSequence,
NextValue and DiscardAll; GROUPING SETS, ROLLUP and CUBE also execute. The measured total is 74 of 76.

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS03073 | Named sequence or object identity is absent | 42P01 |
| CS03074 | Unsupported persisted sequence expression (constant column allocation defaults execute) or sequence DDL in provisioning scripts | 0A000 |
| CS03075 | setval value outside bounds | 22003 |
| CS03076 | Restricted sequence context, including NEXT VALUE FOR in RETURNING, or detached evaluation | 0A000 |
| CS03077 | Malformed sequence reference or invalid runtime call argument | 22023 |
| CS04068 | currval undefined for the sequence in this session | 55000 |
| CS04069 | lastval undefined or its selected sequence dropped | 55000 |
| CS04070 | DISCARD ALL inside an explicit transaction | 25001 |

## Grouping sets

Grouping elements use [SqlGroupingSetExpression](sql-grouping-set-expression.md) and
[SqlGroupingSetKind](sql-grouping-set-kind.md). SqlSelectExpression.IsGroupByDistinct records
duplicate-set removal after expansion. See [GROUP BY](../../../../database/sql/language/clauses/group-by.md)
for NULLs, independent aggregate state, expansion limits and diagnostic codes.

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS00010 | GROUPING SETS inside ROLLUP/CUBE | 42601 |
| CS01062 | GROUPING argument does not match its associated level's grouping keys | 42803 |
| CS01063 | GROUPING requires 1–31 non-star arguments | 54023 |
| CS05074 | GROUPING outside grouped output or in grouping/aggregate inputs | 42803 |
| CS05075 | More than 4096 sets before DISTINCT | 54001 |
| CS05067 | Retained per-set state or grouping-mask plan exhausts statement memory | 53200 |

## Owned sequence diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS03078 | ADD GENERATED requires NOT NULL | 55000 |
| CS03079 | Identity required by ALTER is absent | 55000 |
| CS03080 | Sequence required by identity or column default | 2BP01 |
| CS03081 | Invalid OWNED BY target or identity ownership change | 55000 |

Allocator exhaustion uses CS02002 (22003); disappearing identity or unplanned live referential
default sequence grants use CS04067 (40001). Retry replans before defaults or row mutation.
[SqlAlterIdentityAction](sql-alter-identity-action.md) carries mode/options/restart operations;
[SqlIdentityOverride](sql-identity-override.md) is retained by INSERT and MERGE INSERT nodes.

## SQL window functions

See [OVER and WINDOW](../../../../database/sql/language/clauses/window.md) for placement, inheritance and memory rules.

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS00011 | Window argument count or call modifier | 42601 |
| CS01064 | Duplicate WINDOW name | 42P20 |
| CS01065 | Unknown/self/forward window reference | 42704 |
| CS01066 | Copied PARTITION BY or overridden ORDER BY | 42P20 |
| CS02068 | Non-integer offset or bucket count | 42804 |
| CS02069 | Incompatible navigation value/default types | 42804 |
| CS05076 | Window placement or missing OVER | 42P20 |
| CS05077 | Nested window | 42P20 |
| CS05078 | Nonpositive NTILE count | 22014 |
| CS05079 | Window state exceeds statement memory grant | 54000 |


## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/Assimalign.Cohesion.Database.Sql.Language.csproj`.
