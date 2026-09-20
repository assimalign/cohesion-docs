# Aggregate functions (Cohesion SQL)

Summarizes rows using COUNT, SUM, AVG, MIN, and MAX.

> **Status:** Partial. The declared subset and its execution limits are documented below.

## Syntax

```syntaxsql
COUNT ( * )
COUNT ( expression )
SUM ( expression )
AVG ( expression )
MIN ( expression )
MAX ( expression )
```

## Arguments

- **`*`** — Counts every row, including rows whose values are all null.
- **`expression`** — A per-row argument; it cannot contain another aggregate.

## Remarks

These functions are measured in grouped and ungrouped queries, including multiple projections and
scalar expressions around aggregate results. Aggregates cannot occur in `WHERE`, `JOIN ... ON`,
or `GROUP BY`. Columns outside aggregates must satisfy grouping validity.

| Function | Null and empty-input behavior | Result type |
| --- | --- | --- |
| `COUNT(*)` | Counts every row; zero for empty input | Nonnullable `Int64` |
| `COUNT(expression)` | Counts non-null arguments; zero for all-null or empty input | Nonnullable `Int64` |
| `SUM(expression)` | Ignores nulls; null for all-null or empty input | Nullable `Decimal` |
| `AVG(expression)` | Ignores nulls in sum and divisor; null for all-null or empty input | Nullable `Decimal` |
| `MIN(expression)`, `MAX(expression)` | Ignore nulls; null for all-null or empty input | Nullable argument type |

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

Aggregate `DISTINCT`/`ALL`, `FILTER`, in-aggregate `ORDER BY`, empty grouping sets,
`GROUPING SETS`, `ROLLUP`, `CUBE`, grouping functions, windows, and ordered-set
`WITHIN GROUP` forms report `COHDBL001`.

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

## See also

[Functions](index.md) · [GROUP BY](../clauses/group-by.md) · [HAVING](../clauses/having.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAggregateExecutionTests.cs`

