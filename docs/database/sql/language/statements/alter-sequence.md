# ALTER SEQUENCE (Cohesion SQL)

Changes a sequence's options, ownership or allocation position.

## Syntax

```syntaxsql
ALTER SEQUENCE [ IF EXISTS ] [ schema_name . ] sequence_name
    { sequence_option | RESTART [ WITH integer ] } [ ...n ] [ ; ]
```

sequence_option is an option from [CREATE SEQUENCE](create-sequence.md#syntax), including AS and OWNED BY. Identity-sequence ownership cannot be changed (CS03081, 55000).
At least one option is required; each may appear once, in any order.

## Remarks

START changes the configured starting value without restarting allocation. Bare RESTART uses
that configured START, including a START supplied in the same statement. RESTART WITH uses its
explicit value without changing START. Restarting can deliberately repeat values. Ordinary ALTER
retains the last fetched value, discards unused reservation values and continues with the new
increment. AS changes a bound equal to the old type limit to the new type limit; other explicit
bounds remain and must fit. Value expressions continue to return BIGINT.

The statement self-commits and refuses explicit transactions with CS04003 (25001). IF EXISTS
ignores an absent sequence, including one removed during the lock wait. A replacement identity
still reports CS04067 (40001). Invalid resolved options/restart use CS03069 (22023); an absent
unguarded name uses CS03073 (42P01). Provisioning-script refusal uses CS03074 (0A000).

### Locking

Sequence calls acquire transaction-owned Shared object locks before evaluation. ALTER, RESTART,
DROP and DROP SCHEMA CASCADE acquire conflicting Exclusive locks and wait until users commit,
roll back or close. An explicit transaction retains these locks after a failed statement.
Definition DDL can also delay ALTER/DROP. Ordinary deadlock detection, cancellation and lock
timeouts apply; CS04044 (55P03) reports an expired wait. Metadata is resolved after grants, so
a concurrent ALTER does not invalidate a running allocation.

Literal and parameter references lock only the referenced objects. Row-varying names conservatively
lock every sequence in the directory captured at statement execution, including unrelated ones.
New sequences are visible to the next statement. A queued Exclusive grant stalls later dynamic-name
statements under FIFO admission. Use literal or parameter names to limit this blocking scope.

## Example

Execute separately in one session:

```sql
CREATE SEQUENCE s START 3 INCREMENT 2;
ALTER SEQUENCE s START WITH 9 RESTART;
SELECT nextval('s');
ALTER SEQUENCE s RESTART WITH 8;
SELECT nextval('s');
```

The SELECT results are `9` and `8`.

## See also

[Statements](index.md) · [CREATE SEQUENCE](create-sequence.md) · [DROP SEQUENCE](drop-sequence.md) · [Sequence functions](../functions/sequences.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSequenceSqlTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSequenceSecondPassTests.Locking.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Execution/SqlSequenceExecution.cs`.
