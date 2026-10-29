# Window functions

Ranking, distribution and navigation functions compute a value for every row without collapsing its partition.

## Signatures and results

Every call requires [OVER](../clauses/window.md).

| Call | Result type | Behavior |
| --- | --- | --- |
| row_number() | BIGINT (Int64) | Sequential position starting at 1 |
| rank() | BIGINT (Int64) | First position of the peer group; leaves gaps after ties |
| dense_rank() | BIGINT (Int64) | Peer-group number starting at 1, without gaps |
| percent_rank() | DOUBLE PRECISION (Double) | `(rank - 1) / (partition size - 1)`; 0 for a singleton |
| cume_dist() | DOUBLE PRECISION (Double) | Number of rows through the last peer divided by partition size |
| ntile(buckets) | INTEGER (Int32) | Bucket number starting at 1; larger buckets precede smaller ones |
| lag(value [, offset [, default]]) | Common type of value/default | Value at offset rows before the current row |
| lead(value [, offset [, default]]) | Common type of value/default | Value at offset rows after the current row |

Peers have equal window ORDER BY keys, including equal NULL keys. Without ORDER BY all rows are
peers: rank and dense_rank are 1, percent_rank is 0 and cume_dist is 1. row_number, navigation
and ntile then use an unspecified order. An empty input produces no rows. A singleton returns
row_number/rank/dense_rank 1, percent_rank 0, cume_dist 1 and ntile(9) 1.

## NTILE

The signed integer bucket expression evaluates once, on the first row of each ordered partition.
If it is NULL, every result in that partition is NULL, even if later rows would supply another
count. This is deliberate: PostgreSQL can retry initialization after NULL. Zero or negative counts
report CS05078 (22014). Non-integer types report CS02068 (42804). Counts greater than the number
of rows leave trailing buckets empty; the function still returns consecutive bucket numbers.

## LAG and LEAD

The omitted offset is 1 and the omitted default is NULL. Offsets evaluate per current row and
accept signed integers through the full Int64 range. Zero selects the current row; a negative
offset reverses direction. NULL offset returns NULL. A target outside the partition uses the
current row's default expression. NULL target values remain NULL: navigation respects NULLs.

Value/default use common-type implicit casts, including registered casts. Unknown string
constants coerce at binding even when the default will not be used; for example,
`LAG(1,1,'8') OVER ()` returns integer 8, but a bad integer string can fail with CS02022.
Incompatible value/default types use CS02069 (42804). Non-integer offsets use CS02068 (42804).
See [OVER diagnostics](../clauses/window.md#diagnostics) for modifiers and placement refusals.

## Example

Execute separately in a fresh database:

```sql
CREATE TABLE t(id INT,k INT);
INSERT INTO t VALUES (1,NULL),(2,10),(3,10),(4,20);
SELECT id, ROW_NUMBER() OVER (ORDER BY id) AS rn,
       RANK() OVER w AS r, DENSE_RANK() OVER w AS d,
       PERCENT_RANK() OVER w AS pr, CUME_DIST() OVER w AS cd,
       NTILE(2) OVER (ORDER BY id) AS tile
FROM t WINDOW w AS (ORDER BY k) ORDER BY id;
```

| id | rn | r | d | pr | cd | tile |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | 1 | 1 | 1 | 0 | 0.25 | 1 |
| 2 | 2 | 2 | 2 | 1/3 (Double) | 0.75 | 1 |
| 3 | 3 | 2 | 2 | 1/3 (Double) | 0.75 | 2 |
| 4 | 4 | 4 | 3 | 1 | 1 | 2 |

NULL sorts first in the window's ascending order. On the same table:

```sql
SELECT id, LAG(k) OVER (ORDER BY id), LEAD(k) OVER (ORDER BY id)
FROM t ORDER BY id;
```

Returns `(1,NULL,10)`, `(2,NULL,10)`, `(3,10,20)`, `(4,10,NULL)`.

## See also

[Functions](index.md) · [OVER and WINDOW](../clauses/window.md) · [Aggregate functions](aggregate-functions.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlWindowExecutionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Execution/SqlPlanExecutor.WindowAggregate.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Evaluation/SqlExpressionEvaluator.Windows.cs`.
