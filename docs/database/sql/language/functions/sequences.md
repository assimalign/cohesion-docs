# Sequence functions

Allocates and inspects standalone sequence values with session-local observations.

## Syntax

```syntaxsql
nextval(sequence_reference)
currval(sequence_reference)
setval(sequence_reference, integer_value [ , is_called ])
lastval()
```

## Arguments and results

Every function returns BIGINT, regardless of the sequence's AS type. References are schema-qualified
text names (dbo by default), or numeric object identities. Names follow case-insensitive identifier
rules; quoted components in text use doubled double quotes. `CAST('s' AS REGCLASS)` and
`'s'::regclass` resolve a sequence to its BIGINT object identity. REGCLASS is sequence-only and
cannot be a stored column type. Names resolve per statement; they do not pin an old incarnation
across DROP/recreate. NULL arguments propagate NULL. SQL has no MISSING value.

| Call | Effect |
| --- | --- |
| nextval | Allocates the next value, sets this sequence's currval and selects it for lastval |
| currval | Returns this session's last observed value for this sequence; does not allocate |
| setval(name,n) | Equivalent to setval(name,n,TRUE) |
| setval(name,n,TRUE) | Sets the allocator and this session's currval to n; next allocation is n + increment |
| setval(name,n,FALSE) | Makes the next allocation return n itself; leaves session currval and lastval unchanged |
| lastval | Returns the current session value for the sequence most recently selected by nextval |

setval never selects a new lastval sequence. A TRUE setval on the already selected sequence changes
the value lastval reads. Its value must fit the sequence bounds; the runtime value is a signed
integer and is_called is Boolean. New sessions have no currval or lastval. Allocation and setval
are nontransactional; rollback leaves gaps and session observations survive statement failures
and rollback. Another session's allocations do not change these observations. DROP/recreate
does not inherit them. [DISCARD ALL](../statements/discard-all.md) clears them.

## Evaluation order

Calls are volatile and evaluate left to right once per reached expression. Repeated nextval calls
allocate separately; [NEXT VALUE FOR](../expressions/next-value-for.md) shares per row instead.
Short-circuited expressions do not allocate. Multi-row VALUES follows written tuple/expression
order; INSERT SELECT follows source evaluation order. Source order without ORDER BY is unspecified,
and allocation order is independent of transaction commit order.

Independent sequence outputs evaluate after ORDER BY and before LIMIT/OFFSET, even when ordering
uses another output's alias or ordinal. Each qualifying OFFSET row is projected and consumes its
values before being discarded; independent outputs beyond OFFSET + LIMIT are not projected.
If a sequence output itself supplies an ordering key (directly, by alias or ordinal), it evaluates
before sorting, once per candidate. WHERE/HAVING-rejected rows consume no projection values.
Grouping sets apply these rules to every completed group, including empty groups and subtotals.
Unordered grouped/correlated inputs project qualifying rows immediately and stop at LIMIT; joins
filter all candidate pairs before limited projection, so sequence calls in their predicates can
run for more rows than the final page. LIMIT/OFFSET sequence calls evaluate once at execution
before input rows. Plain EXPLAIN has no effects; EXPLAIN ANALYZE SELECT executes them.

Function calls execute in RETURNING for INSERT, UPDATE, DELETE, joined writes, ON CONFLICT and
MERGE. A RETURNING list containing sequence calls evaluates before the global apply gate, keeping
durable reservation/setval commits outside that gate. Row writes still roll back on statement
failure; sequence effects remain. NEXT VALUE FOR in RETURNING is refused during planning.

Calls take transaction-owned sequence locks; [ALTER/RESTART/DROP wait for users](../statements/alter-sequence.md#locking).
Dynamic references lock the captured sequence directory. Persisted DEFAULT/CHECK/domain/view/SQL
function expressions, including REGCLASS casts and typed defaults, refuse with CS03074 (0A000).
Identity integration, SERIAL and OWNED BY remain run-3 work.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS01006 | Invalid sequence-call arity, star or aggregate modifier | 42883 |
| CS02002 | Noncycling sequence range exhausted | 22003 |
| CS03073 | Named sequence or object identity is absent | 42P01 |
| CS03074 | Sequence operation/REGCLASS in a persisted expression | 0A000 |
| CS03075 | setval value is outside the sequence bounds | 22003 |
| CS03076 | Unsupported evaluation context or detached evaluation without a statement scope | 0A000 |
| CS03077 | Malformed reference or invalid runtime sequence-call argument | 22023 |
| CS04068 | currval is undefined for this sequence in this session | 55000 |
| CS04069 | lastval is undefined, or its selected sequence was dropped | 55000 |

## Examples

Execute separately in a fresh database, using one session:

```sql
CREATE SEQUENCE s START 10 INCREMENT 2;
SELECT nextval('s'),nextval('s'),currval('s'),lastval();
SELECT setval('s',30,FALSE),currval('s'),lastval();
SELECT nextval('s');
```

Returns `(10,12,12,12)`, `(30,12,12)` and `30`.
With a separate fresh sequence and table:

```sql
CREATE SEQUENCE page_s;
CREATE TABLE t(k INT);
INSERT INTO t VALUES (3),(1),(2);
SELECT nextval('page_s'),k FROM t ORDER BY k LIMIT 1 OFFSET 1;
SELECT nextval('page_s');
```

Returns `(2,2)`, then `3`: the skipped first row consumed value 1, while the third row was not projected.

## See also

[Functions](index.md) · [CREATE SEQUENCE](../statements/create-sequence.md) · [NEXT VALUE FOR](../expressions/next-value-for.md) · [RETURNING](../clauses/returning.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSequenceSqlTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSequenceSecondPassTests.Projection.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSequenceFixTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSequenceFifthPassTests.Documentation.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Execution/SqlSequenceExecution.cs`.
