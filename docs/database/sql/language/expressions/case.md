# CASE (Cohesion SQL)

Returns a value from the first matching branch of a simple or searched CASE expression.

> **Status:** Partial. This page describes the executable subset and its boundaries.

## Syntax

```syntaxsql
CASE input_expression
    WHEN comparison_expression THEN result_expression
    [ WHEN comparison_expression THEN result_expression ...n ]
    [ ELSE result_expression ]
END

CASE
    WHEN search_condition THEN result_expression
    [ WHEN search_condition THEN result_expression ...n ]
    [ ELSE result_expression ]
END
```

## Arguments

- **`input_expression`** — The value compared against each simple-form branch.
- **`search_condition`** — A searched-form predicate.
- **`result_expression`** — The value of the selected branch or fallback.

## Remarks

Simple form chooses the first equal non-null comparison. Searched form chooses the first condition
evaluating to `TRUE`. An unmatched expression returns its `ELSE` value, or `NULL` when no
`ELSE` exists. Branches remain limited to the executable scalar subset.

Searched WHEN conditions require BOOLEAN; non-Boolean conditions use CS02003 (42804),
including an unreachable WHEN. Untyped NULL is UNKNOWN and selects no arm. Valid Boolean
input string literals are accepted; text columns and parameters keep their types. Simple CASE
compares values and does not require Boolean WHEN values.

CASE and COALESCE resolve a common result type before Boolean admission. A declared Boolean
alternative lets unknown Boolean strings convert at binding; invalid input uses CS02022. An
all-unknown string/NULL result resolves to TEXT and fails a Boolean position with CS02003,
even when its strings spell Boolean values. Incompatible declared alternatives use CS02080,
including skipped branches.

### Common types

Scalar CASE considers ELSE first; COALESCE considers arguments left to right. Unknown string
and NULL literals adopt the declared alternatives' type; all-NULL/all-unknown results are TEXT.
`COALESCE('11', 1::INT)` returns INTEGER 11. Integer literal alternatives have Int32 headers.
Numeric alternatives widen through signed integer widths, NUMERIC/Decimal, REAL and DOUBLE;
REAL and DOUBLE beat NUMERIC. Calendar alternatives widen to TIMESTAMP or TIMESTAMPTZ.
Only built-in implicit conversions participate; explicit-only and registered casts do not.

Every alternative converts at its own node before an outer cast, function, comparison,
grouping key or ORDER BY key. Unknown input converts while binding, even in skipped arms:
invalid text uses CS02022 (22P02), and a value outside the chosen type uses CS02002 (22003).
Incompatible declared categories use CS02080 (42804); unavailable implicit conversions,
including JSON/JSONB, use the same code (42846).

CHAR alternatives keep padding. Mixed CHAR/TEXT or CHAR/VARCHAR resolves to TEXT in either
order, a departure from PostgreSQL's candidate selection. GROUP BY, ROLLUP, CUBE, GROUPING
SETS and aggregate inputs use the scalar rule before grouping/accumulation. Nearby NUMERIC
keys can therefore merge after conversion to REAL. A repeated key in SELECT/HAVING/ORDER BY
reads the resolved key slot. Plain window projections and ordering also use the scalar rule.

Grouped output over aggregates, grouping slots, expressions over them or constants retains
its existing normalization: Decimal above Double above Real, Int64 integer literal outputs,
CHAR/TEXT folding with padding removed, and existing all-NULL metadata. This differs from the
scalar rule and PostgreSQL; it does not retype grouping slots or aggregate inputs. A known
grouped typed-NULL polymorphic-result defect can return text under Int64 metadata; ungrouped
evaluation refuses CS02011.

### Unresolved application values

Within one expression, an unresolved polymorphic application result or parameter with an
unsupported CLR value refuses CS02080 during planning. A NULL-valued parameter remains unknown;
CAST(@p AS BIGINT) gives it a declared type. Source checking looks through COLLATE, NULLIF's
first operand, nested CASE/COALESCE, unary +/-, ABS, DATE_TRUNC's temporal argument and native
MIN/MAX arguments. A completed grouping slot or window call does not bypass the check.

Scalar subqueries, derived-table/view/CTE columns and output aliases are query boundaries.
A Null-typed value crossing one can adopt the common type during planning and fail at a row
with CS02011 when its runtime family cannot convert. Integer arithmetic, ROUND, FLOOR, CEIL,
CEILING, POWER and MOD similarly type an unresolved operand by their own rules, permitting a
row-time CS02011. PostgreSQL instead refuses the unresolved inner polymorphic call (42804).

### Persisted expressions

Creation binds column DEFAULTs, table/domain CHECKs, views, SQL function bodies and partial-index
predicates before publication. ALTER COLUMN TYPE rebinds affected CHECKs/defaults; dependencies
and view replacement signatures retain their checks. Valid mixed alternatives survive reopen.
Compiled DefaultSql and domain DEFAULTs remain literal-only. The former carries CS03105 on
apply/Build/script generation; the latter uses CS03301. No separate format bump is introduced.

Length, precision, scale and domain metadata survive only when every result alternative has
the same type, domain and modifiers. A missing ELSE or an unconstrained/different alternative
clears that metadata. [Persisted views](../statements/create-view.md) freeze this output contract
for INFORMATION_SCHEMA.COLUMNS and compatible replacement checks.

## Examples

Use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
SELECT CASE WHEN age > 40 THEN 'senior' ELSE 'junior' END,
       CASE id WHEN 1 THEN 'first' ELSE 'later' END
FROM t ORDER BY id;
```

The fixture produces `(junior, first)`, `(senior, later)`, and `(senior, later)`.

```sql
SELECT CASE WHEN NULL THEN 1 END, CASE 1 WHEN 1 THEN 2 END;
```

The single row is `(NULL, 2)`. `SELECT CASE WHEN 1 THEN 2 END` and
`SELECT 1 WHERE CASE WHEN TRUE THEN TRUE ELSE 1 END` fail CS02003 and CS02080 respectively (42804).

## See also

[Expressions](index.md) · [SELECT](../statements/select.md) · [Data types](../data-types/index.md)

## Sources

- **Common-type rules** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`, `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Evaluation/SqlExpressionEvaluator.Alternatives.cs` and `tests/SqlCommonTypeTests*.cs` at `2dd935103`.
- **Boolean conditions** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlBooleanPredicateTests.cs`.


- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Evaluation/SqlExpressionEvaluator.cs`

