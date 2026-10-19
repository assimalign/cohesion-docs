# Unsupported and reserved syntax

Lists recognized but unimplemented SQL forms and commands outside the declared dialect.

> **Status:** Partial. Recognition does not imply execution support.

The profile measures 51 of 57 named clauses against the live engine. The 6 excluded names are
`RECURSIVE`, `WINDOW`, `OVER`, `PARTITION`, `NATURAL`, and `ALL`. Further subset restrictions do not increase that denominator.

[EXPLAIN](statements/explain.md) adds the plan-description clause. It executes no data statement.

## Exclusion matrix

The classification **Recognized, not supported (`COHDBL001`)** applies to the capability-gated
forms below. **Not in the dialect (`SQL0002`)** applies to unknown leading commands.
Other cells retain their actual diagnostic rather than assigning a capability code to every failure.

| Construct | Diagnostic | Boundary |
| --- | --- | --- |
| MySQL ON DUPLICATE KEY UPDATE; SQLite INSERT OR IGNORE/REPLACE | `COHDBL001` | Use [ON CONFLICT](clauses/on-conflict.md) DO NOTHING/targeted DO UPDATE |
| ON CONFLICT expression/COLLATE/order/operator-class/partial-WHERE inference; DO SELECT | `COHDBL001` | Plain column-set or named unique-index arbitration executes |
| Subqueries in ON CONFLICT DO UPDATE SET/WHERE; EXCLUDED pseudo-relation in RETURNING | `COHDBL001` | Target plus EXCLUDED is available in scalar conflict actions; RETURNING sees only target values |
| Missing/nonunique ON CONFLICT arbiter or untargeted DO UPDATE | `COHSQLE035` (42P10) | Untargeted DO NOTHING uses all unique indexes |
| CORRESPONDING [BY (...)] | `COHDBL001` | Contextual exclusion after a set operator; [UNION/INTERSECT/EXCEPT](clauses/set-operations.md) execute |
| CREATE SCHEMA AUTHORIZATION or nested CREATE/GRANT elements | `COHDBL001` | [CREATE SCHEMA](statements/create-schema.md) accepts a namespace name and existence guard |
| Recursive/nested WITH, modifying CTE bodies, AS [NOT] MATERIALIZED, SEARCH/CYCLE | `COHDBL001` | Non-recursive [WITH](clauses/with.md) before SELECT/INSERT/UPDATE/DELETE executes |
| `WINDOW`, `OVER`, `PARTITION BY` | `COHDBL001` | Window clauses are excluded |
| `ROW_NUMBER`, `RANK`, `DENSE_RANK`, `LEAD`, `LAG`, `FIRST_VALUE`, `LAST_VALUE`, `NTH_VALUE`, `NTILE` calls | `COHDBL001` | Window-function calls are rejected even without OVER |
| TOP/FETCH PERCENT, WITH TIES, DML TOP, explicit SELECT ALL | `COHDBL001` | [FETCH and lexical SELECT TOP](clauses/limit-offset.md) execute |
| `NATURAL JOIN`, RIGHT/FULL joins with optional `OUTER` | `COHDBL001` | INNER, LEFT and CROSS execute |
| INNER/LEFT join without ON or USING | `COHDBL001` | Exactly one condition required |
| CROSS JOIN with ON or USING | `SQL0003` | Products accept neither condition |
| Malformed USING list, or both ON and USING | `SQL0003` | One nonempty USING name list or one ON predicate |
| Missing/repeated/ambiguous USING name, or no common type | `COHSQLE015` | Fails binding even on empty inputs |
| Qualified wildcard, such as `t.*` | `COHDBL001` | Use unqualified star or explicit columns |
| Derived query without an alias | `SQL0003` | FROM/JOIN derived relations require an alias |
| Derived column list with wrong width or blank output name | `COHSQLE013` | Rename every output by position |
| Quantified `ANY`, `ALL`, `SOME` comparison | `COHDBL001` | Use supported scalar/membership/existence forms |
| LATERAL syntax or a derived body referencing a real sibling relation | `COHDBL001` | Enclosing-query correlation executes; typos retain COHSQLE015 |
| OUTPUT, RETURNING INTO, OLD/NEW pseudo-relations, RETURNING WITH (...), subqueries/windows in RETURNING | `COHDBL001` | [RETURNING](clauses/returning.md) supports target-row scalar projections |
| MATERIALIZED, RECURSIVE, TEMP/TEMPORARY views, CREATE VIEW IF NOT EXISTS, view options/CHECK OPTION | `COHDBL001` | [Persisted read-only views](statements/create-view.md) execute |
| Subquery in UPDATE SET | `COHDBL001` | Correlated and uncorrelated UPDATE/DELETE WHERE subqueries execute |
| Subquery inside INSERT VALUES | `COHDBL001` | Use INSERT ... SELECT |
| Subquery in CHECK or DEFAULT | `COHDBL001` | Row-local checks and column-free scalar expression defaults |
| Subquery as LIMIT/OFFSET/FETCH/TOP expression | `COHDBL001` | Numeric pagination can surround supported subqueries |
| Query nesting beyond 32 levels | `COHDBL001` | Includes expression subqueries and derived bodies beneath the top-level query |
| Aggregate FILTER | `COHDBL001` | DISTINCT/ALL and input ORDER BY execute on any aggregate |
| DISTINCT/ALL/ORDER BY on a scalar call, or DISTINCT key absent from arguments | `COHSQLE032` | Catalog-aware validation before input reads |
| `GROUP BY ()`, `GROUPING SETS`, `ROLLUP`, `CUBE` | `COHDBL001` | One or more ordinary scalar grouping expressions |
| `GROUPING`, `GROUPING_ID` | `COHDBL001` | Grouping extensions are excluded |
| Non-LISTAGG WITHIN GROUP; LISTAGG ON OVERFLOW | `COHDBL001` | [LISTAGG WITHIN GROUP](functions/aggregate-functions.md) executes |
| Standalone numeric GROUP BY key | `COHDBL001` | Output ordinals belong only to ORDER BY |
| Foreign-key `ON UPDATE` | `COHDBL001` | ON DELETE CASCADE, RESTRICT, NO ACTION, SET NULL and SET DEFAULT execute |
| Unknown or culture-aware collation name | `COHDBL001` | Four declared names only |
| `CREATE COLLATION`, session collation override | `COHDBL001` | Database/column/expression rules only |
| Collation-aware full-text index | `COHDBL001` or explicit execution error | Not implemented |
| Index or index-backed constraint under `invariant` | Explicit execution error | Compatibility comparison is scan-only |
| CAST to an unknown name | `COHSQLE010` | Type/domain does not resolve |
| Invalid CAST target parameters | `SQL0005` | See the conversion bounds |
| Unsupported CAST source/target pair | `COHSQLE011` | Known source types reject while planning, even on empty input |
| ALTER SET SCHEMA, RENAME CONSTRAINT, TYPE/SET DATA TYPE or comma-separated actions | `COHDBL001` | Table/column RENAME and column default/nullability actions execute |
| Volatile ADD COLUMN DEFAULT on a populated table | `COHSQLE027` | Requires no live rows; stable additions freeze one missing value |
| CAST in CHECK | Planning error | CAST executes in column DEFAULT expressions |
| Columns, parameters or aggregates in DEFAULT | `COHSQLE013` | Defaults have no column or parameter scope |

| Parameters, aggregates, nondeterministic or non-Boolean expressions in CHECK | Planning error | Deterministic Boolean row-local checks only |
| Aggregate inside aggregate, WHERE, ON, or GROUP BY at its own query level | Planning error | Child queries retain independent aggregate levels |
| Ungrouped nonaggregate column in projection, HAVING, or ORDER BY | Planning error | No arbitrary source row is chosen |
| Projection alias in GROUP BY or HAVING | Binding/planning error when not a valid source name | Output alias scope is ORDER BY |
| Invalid numeric ORDER BY ordinal | Planning error | Zero, negative, out of range, or noninteger syntax |
| SELECT without FROM outside a CTE definition root | `COHSQLE013` | A CTE root alone can use one implicit row |
| Aggregate belonging to an outer query level | `COHDBL001` | Aggregate relocation is excluded; mixed local/outer dependencies can remain local |
| Parentheses on bare CURRENT_*/LOCAL* clock forms | `SQL0003` | Use bare names; NOW() is a call |
| Precision on CURRENT_TIME/CURRENT_TIMESTAMP/LOCALTIME/LOCALTIMESTAMP | `COHDBL001` | Transaction-start UTC clock at existing precision |
| UPPER/LOWER/LENGTH on numeric values, or two-argument ROUND on approximate input | `COHSQLE006` (42883) | Signatures reject incompatible inputs during planning |
| Unsupported temporal arithmetic, such as TIME + INTERVAL or INTERVAL * number | `COHSQLE003` (42804) | Supported day-time arithmetic pairs are listed under [operators](expressions/operators.md#temporal-arithmetic) |
| Prefix or infix `~` | `COHDBL001` | Recognized unsupported operator |
| ILIKE/NOT ILIKE, SIMILAR TO/NOT SIMILAR TO, postfix ISNULL/NOTNULL | `COHDBL001` | Use [LIKE ESCAPE and IS tests](expressions/operators.md); ordinary identifier positions retain their meaning |
| Binary `&`, `\|`, `<<`, `>>`, `::`, IS JSON/OF/NORMALIZED | `COHDBL001` | Use CAST(x AS type) for casts; these other operators/tests are excluded |
| Non-associative comparison/predicate chains, such as `a < b < c` | `SQL0003` | Parenthesize an intermediate result; completed IN lists/subqueries may chain |
| CREATE INDEX direction modifiers, expression keys, or INCLUDE | No dedicated diagnostic promised by the dialect | Only plain column lists are in the contract |
| DROP TABLE CASCADE | `SQL0003` for trailing DDL syntax | Foreign-key delete cascade is a separate feature |
| `ROLLBACK TO [SAVEPOINT]`, malformed transaction suffixes | `SQL0003` | No savepoint rollback |
| `MERGE`, `TRUNCATE`, `GRANT` | `SQL0002` | Unknown leading commands |
| Standalone `TRANSACTION`, `SAVEPOINT`, `RELEASE`, isolation-level SET command | `SQL0002` | Not implemented as statements |
| DDL in an explicit transaction | `COHSQLT003` | DDL is self-committing |
| Mutation of a built-in system relation | `COHSQLE021` (25006) | System relations are read-only |
| SERIAL/BIGSERIAL/SMALLSERIAL type or user-domain name | `COHDBL001` | Use GENERATED BY DEFAULT AS IDENTITY; quoted domain names cannot bypass this rule |
| CREATE SEQUENCE, OVERRIDING ... VALUE | `COHDBL001` | Only column IDENTITY generation executes |
| Identity MINVALUE/MAXVALUE/CYCLE/CACHE, ALTER RESTART/SET GENERATED, generated stored expression | `COHDBL001` | Fixed directional bounds and no cycling or option mutation |
| Populated-table ADD IDENTITY, identity type/nullability/default change | `COHSQLE025` | ADD requires no live rows; same type and NOT NULL preserves an existing identity |
| Explicit NULL with NOT NULL/PRIMARY KEY/IDENTITY | `SQL0003` | Refused in either order, including table primary keys and CREATE/ADD/ALTER column definitions |

The parser can recognize syntax that is later rejected by binding or execution. Conversely, a
recovered parse tree or a name in the function vocabulary is not evidence of support.
No code is invented above for restrictions whose source contract promises only an error or exclusion.

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
