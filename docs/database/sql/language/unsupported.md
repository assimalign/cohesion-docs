# Unsupported and reserved syntax

Lists recognized but unimplemented SQL forms and commands outside the declared dialect.

> **Status:** Partial. Recognition does not imply execution support.

Phase 22 measures 33 of 49 named clauses against the live engine. The 16 excluded names are
`UNION`, `INTERSECT`, `EXCEPT`, `WITH`, `RECURSIVE`, `WINDOW`, `OVER`,
`PARTITION`, `CREATE VIEW`, `DROP VIEW`, `NATURAL`, `USING`, `TOP`, `ALL`,
`FETCH`, and `RETURNING`. Further subset restrictions do not increase that denominator.

## Exclusion matrix

The classification **Recognized, not supported (`COHDBL001`)** applies to the capability-gated
forms below. **Not in the dialect (`SQL0002`)** applies to unknown leading commands.
Other cells retain their actual diagnostic rather than assigning a capability code to every failure.

| Construct | Diagnostic | Boundary |
| --- | --- | --- |
| `UNION`, `INTERSECT`, `EXCEPT` | `COHDBL001` | Set operations are excluded |
| `WITH`, `WITH RECURSIVE` | `COHDBL001` | Common table expressions are excluded |
| `WINDOW`, `OVER`, `PARTITION BY` | `COHDBL001` | Window clauses are excluded |
| `ROW_NUMBER`, `RANK`, `DENSE_RANK`, `LEAD`, `LAG`, `FIRST_VALUE`, `LAST_VALUE`, `NTH_VALUE`, `NTILE` calls | `COHDBL001` | Window-function calls are rejected even without OVER |
| `CREATE VIEW`, `DROP VIEW` | `COHDBL001` | Built-in system relations do not imply user-defined views |
| `TOP`, explicit `SELECT ALL`, `FETCH` | `COHDBL001` | Use ordinary SELECT and LIMIT/OFFSET |
| DML `RETURNING` | `COHDBL001` | Writes return affected counts |
| `NATURAL JOIN`, `JOIN ... USING` | `COHDBL001` | Only inner joins with ON execute |
| `LEFT`, `RIGHT`, `FULL` joins, optional `OUTER` | `COHDBL001` | No outer joins |
| `CROSS JOIN`, comma joins, join without ON | `COHDBL001` | No additional product/join syntax |
| More than two joined tables | `COHDBL001` | At most one join per SELECT |
| Join involving `INFORMATION_SCHEMA` or `COHESION_SCHEMA` | `COHDBL001` | Join inputs must be stored tables |
| Qualified wildcard, such as `t.*` | `COHDBL001` | Use unqualified star or explicit columns |
| Derived table in FROM or JOIN | `COHDBL001` | No relational subquery sources |
| Correlated subquery | `COHDBL001` | Inner names must resolve locally |
| Quantified `ANY`, `ALL`, `SOME` comparison | `COHDBL001` | Use supported scalar/membership/existence forms |
| Lateral join | `COHDBL001` | Outside the subquery surface |
| Subquery in UPDATE or DELETE | `COHDBL001` | SELECT contexts only |
| Subquery inside INSERT VALUES | `COHDBL001` | Use INSERT ... SELECT |
| Subquery in CHECK or DEFAULT | `COHDBL001` | Row-local checks and literal defaults only |
| Subquery as LIMIT/OFFSET expression | `COHDBL001` | Numeric pagination can surround supported subqueries |
| Expression-subquery nesting beyond 32 levels | `COHDBL001` | Depth limit beneath the top-level query |
| Aggregate `DISTINCT` or explicit `ALL` | `COHDBL001` | Top-level SELECT DISTINCT still executes |
| Aggregate `FILTER`, in-aggregate ORDER BY | `COHDBL001` | Basic aggregate calls only |
| `GROUP BY ()`, `GROUPING SETS`, `ROLLUP`, `CUBE` | `COHDBL001` | One or more ordinary scalar grouping expressions |
| `GROUPING`, `GROUPING_ID` | `COHDBL001` | Grouping extensions are excluded |
| Ordered-set `WITHIN GROUP` | `COHDBL001` | No ordered-set aggregates |
| Standalone numeric GROUP BY key | `COHDBL001` | Output ordinals belong only to ORDER BY |
| `NULLS FIRST`, `NULLS LAST` | `COHDBL001` | Null first ascending, last descending |
| Foreign-key `ON UPDATE` | `COHDBL001` | Only ON DELETE CASCADE/RESTRICT |
| Unknown or culture-aware collation name | `COHDBL001` | Four declared names only |
| `CREATE COLLATION`, session collation override | `COHDBL001` | Database/column/expression rules only |
| Collation-aware full-text index | `COHDBL001` or explicit execution error | Not implemented |
| Index or index-backed constraint under `invariant` | Explicit execution error | Compatibility comparison is scan-only |
| CAST to approximate numeric, binary, temporal, GUID, or JSON family | `SQL0005` | These target families are recognized but excluded |
| CAST to an unknown name | `SQL0004` | Target name does not resolve |
| Invalid CAST target parameters | `SQL0005` | See the conversion bounds |
| Unsupported CAST source/target pair or lossy conversion | Execution error | Exact pair and range checks apply |
| CAST in DEFAULT or CHECK | Planning error | Conversion support does not broaden these contexts |
| Nonliteral DEFAULT | Planning error | No functions, parameters, or computed defaults |
| Parameters, aggregates, nondeterministic or non-Boolean expressions in CHECK | Planning error | Deterministic Boolean row-local checks only |
| Aggregate inside aggregate, WHERE, ON, or GROUP BY | Planning error | Aggregate stage restrictions |
| Ungrouped nonaggregate column in projection, HAVING, or ORDER BY | Planning error | No arbitrary source row is chosen |
| Projection alias in GROUP BY or HAVING | Binding/planning error when not a valid source name | Output alias scope is ORDER BY |
| Invalid numeric ORDER BY ordinal | Planning error | Zero, negative, out of range, or noninteger syntax |
| SELECT without FROM | Planning error | Every executable query needs a relation |
| `NULLIF`, `TRIM`, `LTRIM`, `RTRIM`, `SUBSTRING`, `REPLACE`, `CONCAT` | Planning/evaluation error; no dedicated language code promised | Recognized scalar names without executor support |
| `CEILING`, `FLOOR`, `ROUND`, `POWER`, `SQRT`, `MOD` | Planning/evaluation error; no dedicated language code promised | Recognized numeric names without executor support |
| `NOW`, `CURRENT_DATE`, `CURRENT_TIME`, `CURRENT_TIMESTAMP`, `EXTRACT` | Planning/evaluation error; no dedicated language code promised | Recognized temporal names without executor support |
| Unary bitwise complement `~` | Evaluation error | Parsed but not evaluated |
| CREATE INDEX direction modifiers, expression keys, or INCLUDE | No dedicated diagnostic promised by the dialect | Only plain column lists are in the contract |
| DROP TABLE CASCADE | `SQL0003` for trailing DDL syntax | Foreign-key delete cascade is a separate feature |
| `ROLLBACK TO [SAVEPOINT]`, malformed transaction suffixes | `SQL0003` | No savepoint rollback |
| `MERGE`, `TRUNCATE`, `GRANT` | `SQL0002` | Unknown leading commands |
| Standalone `TRANSACTION`, `SAVEPOINT`, `RELEASE`, isolation-level SET command | `SQL0002` | Not implemented as statements |
| DDL in an explicit transaction | `COHSQLT003` | DDL is self-committing |
| Mutation of a built-in system relation | `ExecutionFailure` on the wire | System views are read-only |

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

