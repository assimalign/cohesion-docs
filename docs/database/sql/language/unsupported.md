# Unsupported and reserved syntax

Lists recognized but unimplemented SQL forms and commands outside the declared dialect.

> **Status:** Partial. Recognition does not imply execution support.

The profile measures 83 of 85 named clauses against the live engine. The 2 excluded names are
`NATURAL` and `ALL`. Further subset restrictions do not increase that denominator.

The measure includes executable MERGE with ordered actions and RETURNING, and WITH RECURSIVE
fixpoints with on-demand execution and shared readers.

[EXPLAIN](statements/explain.md) describes a plan without executing the wrapped statement.
SELECT-only EXPLAIN ANALYZE executes to report candidate work. Standalone ANALYZE remains unsupported.

## Exclusion matrix

The classification **Recognized, not supported (`CK00001`)** applies to the capability-gated
forms below. **Not in the dialect (`CS00002`)** applies to unknown leading commands.
Other cells retain their actual diagnostic rather than assigning a capability code to every failure.

| Construct | Diagnostic | Boundary |
| --- | --- | --- |
| MySQL ON DUPLICATE KEY UPDATE; SQLite INSERT OR IGNORE/REPLACE | `CK00001` | Use [ON CONFLICT](clauses/on-conflict.md) DO NOTHING/targeted DO UPDATE |
| ON CONFLICT expression/COLLATE/order/operator-class/partial-WHERE inference; DO SELECT | `CK00001` | Plain column-set or named unique-index arbitration executes |
| Subqueries in ON CONFLICT DO UPDATE SET/WHERE; EXCLUDED pseudo-relation in RETURNING | `CK00001` | Target plus EXCLUDED is available in scalar conflict actions; RETURNING sees only target values |
| Missing/nonunique ON CONFLICT arbiter or untargeted DO UPDATE | `CS03035` (42P10) | Untargeted DO NOTHING uses all unique indexes |
| CORRESPONDING [BY (...)] | `CK00001` | Contextual exclusion after a set operator; [UNION/INTERSECT/EXCEPT](clauses/set-operations.md) execute |
| CREATE SCHEMA AUTHORIZATION or nested CREATE/GRANT elements | `CK00001` | [CREATE SCHEMA](statements/create-schema.md) accepts a namespace name and existence guard |
| Mutual recursion, nested WITH, modifying CTE bodies, AS [NOT] MATERIALIZED, SEARCH/CYCLE | `CK00001` | [WITH RECURSIVE](clauses/with.md#with-recursive) executes UNION ALL/DISTINCT fixpoints |
| WITH RECURSIVE ... MERGE | `CK00001` | Refused even for acyclic lists; plain WITH ... MERGE executes |
| SELECT ... FOR UPDATE / FOR NO KEY UPDATE / FOR SHARE / FOR KEY SHARE | `CK00001` | Complete locking strengths in SELECT tails, including CTEs and subqueries |
| Aggregate-window DISTINCT; FROM FIRST/LAST | `CK00001` | [Aggregate windows and frame value functions](clauses/window.md) execute; missing OVER uses CS05076 (42P20) |
| TOP/FETCH PERCENT, WITH TIES, DML TOP, explicit SELECT ALL | `CK00001` | [FETCH and lexical SELECT TOP](clauses/limit-offset.md) execute |
| `NATURAL JOIN` | `CK00001` | INNER, LEFT, RIGHT, FULL and CROSS execute |
| FULL condition without a safe mandatory hash equality or proven non-throwing ON | `CS05070` | Refused before input reads; no loop fallback |
| Direct RIGHT, FULL or unaliased grouped joined-DML source | `CS05069` | UPDATE FROM / DELETE USING refuse before reads/writes; derived SELECT sources containing RIGHT or FULL execute |
| INNER/LEFT join without ON or USING | `CK00001` | Exactly one condition required |
| CROSS JOIN with ON or USING | `CS00003` | Products accept neither condition |
| Malformed USING list, or both ON and USING | `CS00003` | One nonempty USING name list or one ON predicate |
| Missing/repeated/ambiguous USING name, or no common type | `CS01015` | Fails binding even on empty inputs |
| Qualified wildcard, such as `t.*` | `CK00001` | Use unqualified star or explicit columns |
| Derived query without an alias | `CS00003` | FROM/JOIN derived relations require an alias |
| Derived column list with wrong width or blank output name | `CS05013` | Rename every output by position |
| Quantified ANY/SOME/ALL with arrays or value lists | `CK00001` | [Single-column subquery forms](expressions/quantified-comparisons.md) execute; row-value syntax uses CS00003 |
| LATERAL syntax or a derived body referencing a real sibling relation | `CK00001` | Enclosing-query correlation executes; typos retain CS01015 |
| Windows in RETURNING or other invalid placement | `CS05076` (42P20) | [Windows](clauses/window.md) execute in SELECT output and ORDER BY only |
| OUTPUT, RETURNING INTO, OLD/NEW pseudo-relations, RETURNING WITH (...), subqueries in RETURNING | `CK00001` | [RETURNING](clauses/returning.md) supports target-row scalar projections |
| MATERIALIZED, RECURSIVE, TEMP/TEMPORARY views, CREATE VIEW IF NOT EXISTS, view options/CHECK OPTION | `CK00001` | [Persisted read-only views](statements/create-view.md) execute |
| Subquery in UPDATE SET | `CK00001` | Correlated and uncorrelated UPDATE/DELETE WHERE subqueries execute |
| Subquery inside INSERT VALUES | `CK00001` | Use INSERT ... SELECT |
| Subquery in CHECK or DEFAULT | `CK00001` | Row-local checks and column-free scalar expression defaults |
| Subquery as LIMIT/OFFSET/FETCH/TOP expression | `CK00001` | Numeric pagination can surround supported subqueries |
| Query nesting beyond 32 levels | `CK00001` | Includes expression subqueries and derived bodies beneath the top-level query |
| FILTER on a non-aggregate call | `CS05072` (42809) | [Aggregate FILTER](clauses/filter.md) executes before input expressions; supported window calls reject modifiers with CS00011 (42601) |
| DISTINCT/ALL/ORDER BY on a scalar call, or DISTINCT key absent from arguments | `CS05032` | Catalog-aware validation before input reads |
| GROUPING SETS inside ROLLUP or CUBE | `CS00010` (42601) | Reverse nesting, composites, empty sets and [grouping masks](functions/grouping.md) execute |
| Non-LISTAGG WITHIN GROUP; LISTAGG ON OVERFLOW | `CK00001` | [LISTAGG WITHIN GROUP](functions/aggregate-functions.md) executes |
| Standalone numeric GROUP BY key | `CK00001` | Output ordinals belong only to ORDER BY |
| Unknown or culture-aware collation name | `CS01052` (42704) | Identifier parses; binding resolves only the four declared names |
| `CREATE COLLATION`, session collation override | `CK00001` | Database/column/expression rules only |
| Collation-aware full-text index | `CK00001` or explicit execution error | Not implemented |
| CREATE INDEX under `invariant` | `CS05013` | Compatibility comparison is scan-only |
| Index-backed constraint under `invariant` | `CS05057` (0A000) | Compatibility comparison is scan-only |
| CAST to an unknown name | `CS02010` | Type/domain does not resolve |
| Invalid CAST target parameters | `CS00005` | See the conversion bounds |
| Unsupported CAST source/target pair | `CS02011` | Known source types reject while planning, even on empty input |
| ALTER SET SCHEMA, RENAME CONSTRAINT or comma-separated actions | `CK00001` | Table/column RENAME, default/nullability actions and [TYPE with USING](statements/alter-table.md#alter-column-type) execute |
| Volatile ADD COLUMN DEFAULT on a populated table | `CS03027` | Requires no live rows; stable additions freeze one missing value |
| CAST in CHECK | `CS03059` (42P17) | CAST executes in column DEFAULT expressions |
| Columns, parameters or aggregates in DEFAULT | `CS05013` | Defaults have no column or parameter scope |

| Parameters, aggregates or nondeterministic expressions in CHECK | `CS03059` (42P17) | Deterministic row expressions only; non-Boolean predicates use CS02003 (42804) |
| Aggregate inside aggregate, WHERE, ON, or GROUP BY at its own query level | Planning error | Child queries retain independent aggregate levels |
| Ungrouped nonaggregate column in projection, HAVING, or ORDER BY | Planning error | No arbitrary source row is chosen |
| Projection alias in GROUP BY or HAVING | Binding/planning error when not a valid source name | Output alias scope is ORDER BY |
| Invalid numeric ORDER BY ordinal | Planning error | Zero, negative, out of range, or noninteger syntax |
| Wildcard SELECT without FROM | `CS05013` | Scalar and aggregate SELECT without FROM execute over one implicit row |
| Aggregate belonging to an outer query level | `CK00001` | Aggregate relocation is excluded; mixed local/outer dependencies can remain local |
| Parentheses on bare CURRENT_*/LOCAL* clock forms | `CS00003` | Use bare names; NOW() is a call |
| Precision on CURRENT_TIME/CURRENT_TIMESTAMP/LOCALTIME/LOCALTIMESTAMP | `CK00001` | Transaction-start UTC clock at existing precision |
| UPPER/LOWER/LENGTH on numeric values, or two-argument ROUND on approximate input | `CS01006` (42883) | Signatures reject incompatible inputs during planning |
| Unsupported temporal arithmetic, such as TIME + INTERVAL or INTERVAL * number | `CS02003` (42804) | Supported day-time arithmetic pairs are listed under [operators](expressions/operators.md#temporal-arithmetic) |
| Regex operators, including prefix or infix `~`; SIMILAR TO/NOT SIMILAR TO | `CK00001` | [LIKE and ILIKE](expressions/operators.md#ilike-and-not-ilike) execute with ESCAPE |
| Postfix ISNULL/NOTNULL, IS JSON/OF/NORMALIZED | `CK00001` | Use supported [IS tests](expressions/operators.md) |
| Binary `&`, `\|`, `#`, `^`, `<<`, `>>` | `CK00001` | Bitwise operators are deferred to SQ-O2; [expr::type](expressions/cast.md#postfix-casts) executes |
| Non-associative comparison/predicate chains, such as `a < b < c` | `CS00003` | Parenthesize an intermediate result; completed IN lists/subqueries may chain |
| CREATE INDEX direction modifiers, expression keys, or INCLUDE | No dedicated diagnostic promised by the dialect | Only plain column lists are in the contract |
| DROP TABLE CASCADE | `CS00003` for trailing DDL syntax | Foreign-key delete cascade is a separate feature |
| `ROLLBACK TO [SAVEPOINT]`, malformed transaction suffixes | `CS00003` | No savepoint rollback |
| `GRANT` | `CS00002` | Unknown leading command |
| TRUNCATE inside an explicit transaction | `CS04066` (25001) | Refusal preserves active state and prior work; use an automatic statement |
| Standalone `TRANSACTION`, `SAVEPOINT`, `RELEASE` | `CS00002` | Not implemented as statements |
| READ ONLY/WRITE, [NOT] DEFERRABLE transaction modes | `CK00001` | [Isolation options](statements/begin-transaction.md) and [SET TRANSACTION](statements/set-transaction.md) execute |
| SERIALIZABLE transaction isolation | `CS04073` (0A000) | State-valid BEGIN/SET refuses it; SET outside a block is a CS04071 warning/no-op |
| DDL in an explicit transaction | `CS04003` | DDL is self-committing |
| Mutation of a built-in system relation | `CS04021` (25006) | System relations are read-only |
| Serial-family user-domain names | `CK00001` | Column SERIAL/BIGSERIAL/SMALLSERIAL and SERIAL2/4/8 are BY DEFAULT identity aliases; quoted domain names remain excluded |
| Generated stored expressions and SCOPE_IDENTITY/pg_get_serial_sequence | `CK00001` | [Identity](clauses/identity.md), OWNED BY, SERIAL, OVERRIDING and RESTART execute |
| Sequence operations/REGCLASS in persisted expressions; sequence DDL in provisioning scripts | `CS03074` (0A000) | Constant column sequence defaults execute; CHECK, domains, views, SQL functions and other persisted sequence expressions remain excluded |
| NEXT VALUE FOR in restricted contexts | `CS03076` (0A000) | See the complete [context list](expressions/next-value-for.md#remarks); nextval/currval/setval/lastval execute in RETURNING |
| Dynamic/compound column sequence defaults | `CS03074` (0A000) | [Constant nextval/NEXT VALUE FOR defaults](clauses/default.md#sequence-defaults) track dependencies |
| Full-definition identity type/nullability/default change | `CS03025` | New-column identity backfill executes; use explicit TYPE for integer-family conversion and ALTER identity for mode/options |
| Explicit NULL with NOT NULL/PRIMARY KEY/IDENTITY | `CS00003` | Refused in either order, including table primary keys and CREATE/ADD/ALTER column definitions |

The parser can recognize syntax that is later rejected by binding or execution. Conversely, a
recovered parse tree or a name in the function vocabulary is not evidence of support.
No code is invented above for restrictions whose source contract promises only an error or exclusion.

## Evaluation and delivery boundaries

TIME's accepted PostgreSQL-style text includes AM/PM, single-digit clock fields and a written
zone discarded after validation. Cohesion stores TimeOnly, so 24:00 is refused; fractions beyond
seven digits are refused instead of rounded to microseconds. Regional zone names remain excluded.
Day-time INTERVAL accepts bare signed seconds, `1 2 hours`, `@` and final `ago`, while year/month
fields and CLR day-dot-clock text remain unavailable. See [data-type input and diagnostics](data-types/index.md#temporal-text-input).
REAL/DOUBLE text assignment and DEFAULT use CAST's parser, rejecting grouping and finite overflow.

Unordered non-DISTINCT [LIMIT](clauses/limit-offset.md) stops at the qualifying output window,
skipping later projection/HAVING errors and volatile calls, as PostgreSQL does. OFFSET rows still
evaluate. Ordinary nonvolatile projections evaluate before each row's ordering keys; independent
volatile outputs instead evaluate in sorted order before pagination, preserving their full call
count. [ORDER BY](clauses/order-by.md#evaluation-order) explains alias dependencies and collation
fallbacks. Fixed-collation ORDER BY with a non-DISTINCT limit retains only OFFSET + LIMIT candidates.
DISTINCT, aggregate input, join inputs/results and set operands retain their materialization rules.
Selected rows still materialize before delivery; ordered index output, spillable sorts and streaming
result execution are unavailable.

## See also

[Language (SQL)](index.md) · [Diagnostics](diagnostics.md) · [Statements](statements/index.md) · [Builtin functions](functions/builtin-functions.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Profile** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlLanguageProfile.cs`
- **Statement dispatch and capability gates** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.cs`
- **Aggregate gates** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Aggregates.cs`
- **Subquery gates** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Subqueries.cs`
- **Parser boundaries** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlExecutionSurfaceDiagnosticTests.cs`
- **Transaction syntax** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DESIGN.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
