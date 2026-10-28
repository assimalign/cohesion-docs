# NEXT VALUE FOR

Allocates one BIGINT sequence value per row, sharing repeated standard occurrences of that sequence.

## Syntax

```syntaxsql
NEXT VALUE FOR [ schema_name . ] sequence_name
```

## Remarks

Use in row value positions: SELECT output, INSERT VALUES/SELECT and SET values, including admitted
MERGE action values. Every occurrence of the same sequence in a projected or VALUES row shares
one value. Each new row has its own cache; INSERT SELECT shares within a row and allocates afresh
for the next row. Ordinary nextval calls remain independent. Results are BIGINT even for SMALLINT
or INTEGER sequences. The named sequence cannot be NULL or MISSING; SQL has no MISSING value.

The expression is refused with CS03076 (0A000), before allocation, in:

- Predicates, including WHERE, HAVING, JOIN/MERGE ON and action/conflict conditions.
- Aggregate arguments and FILTER/input ordering, and GROUP BY keys.
- Direct ORDER BY expressions; ordering by a projected alias or ordinal remains valid.
- LIMIT/OFFSET/FETCH/TOP row-count expressions.
- Subqueries at any depth, derived tables and CTE definitions, including unused CTEs.
- CASE, COALESCE and NULLIF arguments.
- Every RETURNING list, including joined writes, ON CONFLICT and MERGE.
- Detached evaluation without a statement execution scope.

Persisted DEFAULT, CHECK, domain, view and SQL function expressions use CS03074 (0A000).
An absent sequence uses CS03073 (42P01); range exhaustion uses CS02002 (22003).
Allocation is nontransactional and follows the
[sequence evaluation and locking rules](../functions/sequences.md#evaluation-order).

## Example

Execute separately in a fresh database:

```sql
CREATE SEQUENCE s;
CREATE TABLE t(a BIGINT,b BIGINT,c BIGINT);
INSERT INTO t VALUES
  (NEXT VALUE FOR s,NEXT VALUE FOR s,nextval('s')),
  (NEXT VALUE FOR s,NEXT VALUE FOR s,nextval('s'));
SELECT * FROM t ORDER BY a;
```

Returns `(1,1,2)`, `(3,3,4)`.

## See also

[Expressions](index.md) · [Sequence functions](../functions/sequences.md) · [CREATE SEQUENCE](../statements/create-sequence.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSequenceSqlTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSequenceSecondPassTests.Validation.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Planning/SqlPlanner.Functions.cs`.
