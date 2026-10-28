# Aggregate functions (Cohesion SQL)

Summarizes rows using count, numeric, Boolean, statistical and string aggregates.

> **Status:** Partial. The declared subset and its execution limits are documented below.

## Syntax

```syntaxsql
COUNT ( * )
aggregate_name ( [ DISTINCT | ALL ] expression [ ORDER BY key [ ASC | DESC ] [ NULLS FIRST | NULLS LAST ] [ , ...n ] ] )
STRING_AGG ( [ DISTINCT | ALL ] value, delimiter [ ORDER BY key [ ASC | DESC ] [ NULLS FIRST | NULLS LAST ] [ , ...n ] ] )
LISTAGG ( [ DISTINCT | ALL ] value [ , delimiter ] ) WITHIN GROUP ( ORDER BY key [ ASC | DESC ] [ NULLS FIRST | NULLS LAST ] [ , ...n ] )
aggregate_call FILTER ( WHERE condition )
```

## Arguments

- **`*`** — Counts every row, including rows whose values are all null.
- **`expression`** — A per-row argument; another aggregate at the same query level is invalid.

## Remarks

These functions are measured in grouped and ungrouped queries, including multiple projections and
scalar expressions around aggregate results. Aggregates cannot occur in `WHERE`, `JOIN ... ON`,
or `GROUP BY` at their own query level. A child query in those expressions may aggregate at
its own level. Columns outside aggregates must satisfy grouping validity.

| Function | Null and empty-input behavior | Result type |
| --- | --- | --- |
| `COUNT(*)` | Counts every row; zero for empty input | Nonnullable `Int64` |
| `COUNT(expression)` | Counts non-null arguments; zero for all-null or empty input | Nonnullable `Int64` |
| `SUM(expression)` | Ignores nulls; null for all-null or empty input | Nullable `Decimal` |
| `AVG(expression)` | Ignores nulls in sum and divisor; null for all-null or empty input | Nullable `Decimal` |
| `MIN(expression)`, `MAX(expression)` | Ignore nulls; null for all-null or empty input | Nullable argument type |
| `BOOL_AND(expression)`, `EVERY(expression)` | Ignore nulls; TRUE if all non-NULL values are TRUE; NULL for empty/all-NULL input | Nullable Boolean |
| `BOOL_OR(expression)` | Ignores nulls; TRUE if any non-NULL value is TRUE; NULL for empty/all-NULL input | Nullable Boolean |
| `STDDEV(expression)`, `STDDEV_SAMP(expression)` | Sample standard deviation; NULL with fewer than two non-NULL values | Nullable Decimal or Float64 |
| `VARIANCE(expression)`, `VAR_SAMP(expression)` | Sample variance; NULL with fewer than two non-NULL values | Nullable Decimal or Float64 |
| `STDDEV_POP(expression)`, `VAR_POP(expression)` | Population standard deviation/variance; NULL for empty/all-NULL input, zero for one finite value | Nullable Decimal or Float64 |

An ungrouped aggregate has one implicit group, so empty input returns one row unless removed by
`HAVING` or pagination. Explicit grouping over empty input returns no rows.

`SUM` and `AVG` accept signed integers, `Decimal`, `Float32`, and `Float64`.
Arguments convert to `System.Decimal` before accumulation; approximate values follow the runtime
floating-to-decimal conversion. Nonnumeric arguments, nonfinite values, and overflow are errors.
This accumulation rule differs from exact represented-value comparison.

`AVG` divides the Decimal sum by the non-null `Int64` count using Decimal division: nearest
representable Decimal, midpoint ties to even, up to 28 fractional digits. It never truncates integer
averages and promises neither arbitrary precision nor a fixed output scale. In-process and wire
metadata agree on the base types.

Extrema use the shared value comparator and effective string collation. Grouped numeric
`CASE`/`COALESCE` alternatives use a common numeric type; incompatible nonnumeric alternatives
are planning errors. Ordering can use aggregates, output aliases, or ordinals.

## Aggregate modifiers

[FILTER](../clauses/filter.md) accepts only TRUE rows before arguments, coercion, input ordering,
DISTINCT retention or transition calls. It follows the call, or WITHIN GROUP for LISTAGG. It works
with every native/application aggregate. FALSE/NULL skips the call's input evaluation for that row.

DISTINCT consumes each coerced argument tuple once under its effective collations and exact
numeric equality. Numeric 1 and 1.0 share a class; NaNs share a class. Strict aggregates skip
NULL arguments before deduplication. ALL is the default and retains duplicates. DISTINCT state
grows with the number of distinct values in each group. Neither quantifier accepts star (CS00003).

Every aggregate, including application aggregates, accepts input ORDER BY. Order-insensitive
aggregates ignore the order. Keys bind to source rows, may contain supported subqueries, and
use stable sorting with NULL first in ASC and last in DESC by default. Each key accepts
NULLS FIRST/LAST independently of direction, including STRING_AGG input keys and LISTAGG
WITHIN GROUP keys. An integer key is a constant,
never a projection ordinal. With DISTINCT each key must structurally match an argument after
column binding (CS05032). DISTINCT, ALL or ORDER BY on a scalar function is also CS05032,
with no assigned SQLSTATE. Invalid aggregate arity uses CS01006 (42883).

## String aggregation

STRING_AGG takes two arguments; LISTAGG takes one or two and requires WITHIN GROUP. Both return
nullable String, skipping NULL values. A NULL or omitted LISTAGG delimiter is empty; STRING_AGG
requires its delimiter. No delimiter precedes the first or follows the last value. Empty/all-NULL
input returns NULL. Value and delimiter must be String or Character; CHAR trims trailing spaces
when converted. Other types require CAST(... AS TEXT), otherwise CS01006 (42883).

The delimiter must be a string/NULL literal or a parameter known during planning. A column or
expression delimiter is CK00001. Result collation comes only from value. The output cap is
1,048,576 UTF-16 units, checked while buffering ordered input too; overflow uses CS02002
(22003). Ordered DISTINCT chooses the first representative in sorted input order.

LISTAGG cannot also contain an inner ORDER BY (CS00003). The special form accepts unquoted
LISTAGG or exactly lowercase quoted `"listagg"`. Other quoted spellings are ordinary calls:
WITHIN GROUP is CK00001, and a call without that ordering is refused with CS00003.

[Empty sets, GROUPING SETS, ROLLUP and CUBE](../clauses/group-by.md#grouping-sets-rollup-and-cube)
execute with independent aggregate states. [GROUPING masks](grouping.md) distinguish subtotals
from real NULL keys. Windows, non-LISTAGG WITHIN GROUP and LISTAGG ON OVERFLOW
(ERROR or TRUNCATE) remain CK00001.

## Boolean and statistical aggregates

BOOL_AND and its alias EVERY accept Boolean input; BOOL_OR accepts Boolean input. NULLs are
ignored. Empty/all-NULL input returns NULL, including EVERY; it does not produce a vacuous TRUE.
Incompatible argument types fail binding with CS01006 (42883).

STDDEV aliases STDDEV_SAMP; VARIANCE aliases VAR_SAMP. All six statistics ignore NULLs.
Signed integer and Decimal arguments return nullable Decimal (`numeric`); REAL/DOUBLE arguments
return nullable Float64 (`double precision`). Untyped NULL selects Float64. Other argument types
fail CS01006 (42883). Samples divide by N-1 and require two non-NULL values; population variants
divide by N and require one. A singleton finite population returns zero; all empty/all-NULL
statistics return NULL.

Integer/Decimal accumulation uses exact scaled sums and sums of squares, avoiding intermediate
overflow and cancellation rounding. The final result is bounded by System.Decimal's 96-bit
coefficient and up to 28 fractional digits, with midpoint ties rounded away from zero. A final
result outside that domain uses CS02002 (22003). Standard deviation rounds the root of the exact
rational variance directly, preserving representable roots even when the variance rounds to zero.
There is no arbitrary-precision numeric result type.

Within a statistical argument, each CASE/COALESCE node converts to its common numeric type before
the enclosing expression, DISTINCT and accumulation. This includes intermediate REAL rounding
before widening to Double, through NULLIF, casts and arithmetic. NULLIF retains its normalized
first operand's type. Ordinary scalar alternative typing retains its existing behavior.
Floating transitions use Youngs-Cramer accumulation; nonfinite input yields NaN when a result is
defined, while finite-input overflow uses CS02002. A sample with one non-NULL value still returns NULL.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS05072 | FILTER on a non-aggregate call | 42809 |
| CS05073 | Same-scope aggregate inside FILTER | 42803 |
| CS02003 | Non-Boolean FILTER condition | 42804 |
| CS01006 | Incompatible aggregate arity or argument types | 42883 |
| CS02002 | Final Decimal statistical result out of range or finite floating transition overflow | 22003 |

## Correlated query levels

A subquery over completed groups can reference only outer grouping-key columns (CS05017
otherwise). Within an aggregate argument, a child instead reads the outer source row. The lowest
variable or aggregate dependency determines an aggregate's query level, including dependencies in
nested queries adjusted for nesting depth. An outer-level aggregate is refused with CK00001
before grouping checks; moving it to the enclosing query is outside this subset. Mixed local and
outer dependencies remain local when the lowest dependency is local. See
[Subqueries](../expressions/subqueries.md#correlation-and-grouping) for an executable example and
an outer-only aggregate rejection.

## Examples

With the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture):

```sql
SELECT COUNT(age) FROM t;
SELECT SUM(age) FROM t;
SELECT AVG(age) FROM t;
SELECT MIN(age) FROM t;
SELECT MAX(age) FROM t;
```

The results are `3`, `122`, Decimal `122 / 3`, `36`, and `45`.
This empty-input adaptation demonstrates the implicit group:

```sql
SELECT COUNT(*), SUM(age), AVG(age), MIN(age), MAX(age)
FROM t WHERE id < 0;
```

It returns one row with zero followed by four nulls.

### Modifiers and ordered text

Execute each statement separately:

```sql
CREATE TABLE agg (id INT, name TEXT, x INT);
INSERT INTO agg VALUES (3, 'c', 1), (1, 'a', 1), (2, 'b', 2), (4, NULL, NULL);
SELECT COUNT(DISTINCT x), SUM(ALL x), SUM(x ORDER BY id) FROM agg;
SELECT STRING_AGG(name, ',' ORDER BY id),
       LISTAGG(name, ',') WITHIN GROUP (ORDER BY id),
       STRING_AGG(name, NULL ORDER BY id) FROM agg;
SELECT STRING_AGG(name, ',' ORDER BY 1) FROM agg;
```

Results are `(2, 4, 4)`, `('a,b,c', 'a,b,c', 'abc')`, and `'c,a,b'`.
The constant ORDER BY 1 preserves input order. SUM results are Decimal. A DISTINCT call such as
`STRING_AGG(DISTINCT name, ',' ORDER BY name)` is valid; replacing its key with id uses CS05032.

### NULL placement in aggregate inputs

```sql
CREATE TABLE texts (name TEXT, x INT);
INSERT INTO texts VALUES ('z', NULL), ('a', 1);
SELECT STRING_AGG(name, ',' ORDER BY x NULLS LAST),
       LISTAGG(name, ',') WITHIN GROUP (ORDER BY x NULLS FIRST) FROM texts;
```

The result is ('a,z', 'z,a'). NULL values of name are still skipped.

### Boolean, statistical and filtered results

Execute separately in a fresh database:

```sql
CREATE TABLE stats (v INT, b BOOLEAN);
INSERT INTO stats VALUES (1,TRUE),(3,FALSE),(NULL,NULL);
SELECT BOOL_AND(b), EVERY(b), BOOL_OR(b),
       VAR_SAMP(v), VARIANCE(v), VAR_POP(v), STDDEV_POP(v) FROM stats;
SELECT STDDEV(v), STDDEV_SAMP(v) FROM stats;
SELECT BOOL_AND(b), VAR_SAMP(v), VAR_POP(v) FROM stats WHERE v=1;
SELECT EVERY(b), VAR_POP(v) FROM stats WHERE FALSE;
```

Results are `(FALSE, FALSE, TRUE, 2, 2, 1, 1)`,
`(1.4142135623730950488016887242, 1.4142135623730950488016887242)`,
`(TRUE, NULL, 0)` and `(NULL, NULL)`. All statistical results here are Decimal.
For Decimal inputs 9223372036854775806 and 9223372036854775807, VAR_SAMP, VAR_POP and STDDEV_POP
return 0.5, 0.25 and 0.5 without cancellation error. See [FILTER](../clauses/filter.md#examples)
for filtering before a division or argument subquery.

## See also

[Functions](index.md) · [GROUP BY](../clauses/group-by.md) · [HAVING](../clauses/having.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAggregateExecutionTests.cs`
- **FILTER/statistics cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAggregateExecutionTests.FilterStatistics.cs`.
- **Exact transitions and final rounding** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Functions/SqlStatisticsFunction.cs`.

