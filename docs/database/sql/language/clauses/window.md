# OVER and WINDOW

OVER evaluates a window function over partitions of the current query's rows. WINDOW names reusable specifications.

## Syntax

```syntaxsql
window_function(...) OVER window_name
window_function(...) OVER (
    [ existing_window_name ]
    [ PARTITION BY expression [ , ...n ] ]
    [ ORDER BY expression [ ASC | DESC ] [ NULLS { FIRST | LAST } ] [ , ...n ] ]
    [ { ROWS | RANGE } frame_extent ]
)

-- After HAVING, before final ORDER BY and paging:
WINDOW window_name AS ( window_specification ) [ , ...n ]

<frame_extent> ::= frame_start | BETWEEN frame_start AND frame_end
<frame_start> ::= UNBOUNDED PRECEDING | offset PRECEDING | CURRENT ROW
                | offset FOLLOWING
<frame_end> ::= offset PRECEDING | CURRENT ROW | offset FOLLOWING | UNBOUNDED FOLLOWING
```

The short form ends at CURRENT ROW and permits only UNBOUNDED PRECEDING, offset PRECEDING or
CURRENT ROW. RANGE accepts only offset-free bounds. See the boundary restrictions below.

## Remarks

Omitted PARTITION BY makes one partition. NULL partition keys group together; SQL has no MISSING value. The window's
ORDER BY determines peers and navigation; final ORDER BY determines delivery order. ASC defaults
to NULLS FIRST and DESC to NULLS LAST, following the engine's ordinary ordering rules. These
defaults differ from PostgreSQL. Explicit NULLS FIRST/LAST overrides them. Collation and CHAR
comparison rules also apply to peer equality.

Names are case-insensitive, including quoted names. `OVER w` references a named specification;
`OVER (w ...)` copies it. A copy cannot specify PARTITION BY, even if the base has none. It can
add ORDER BY only when the base has no ordering. A WINDOW definition may inherit an earlier
definition, never itself or a later name. Duplicate and unused definitions still validate.
A named window with an explicit frame supports `OVER name`, but cannot be copied with
`OVER (name)` or another WINDOW definition (CS01066).

[Ranking, navigation and frame value functions](../functions/window-functions.md), and
[aggregate windows](../functions/aggregate-functions.md#aggregate-windows), run after WHERE, grouping
and HAVING and before DISTINCT, final ORDER BY and paging. They are allowed only in SELECT
outputs and ORDER BY at each query level. Grouped expressions, aggregates and GROUPING can be
window arguments or keys. Windows cannot appear in WHERE, JOIN ON, GROUP BY, HAVING, aggregate
inputs, LIMIT/OFFSET, or DML RETURNING, and cannot nest in another window's arguments or keys.
A derived SELECT can compute windows for an outer filter or DML source.

ROWS and offset-free RANGE frames, aggregate windows with FILTER, and
FIRST_VALUE/LAST_VALUE/NTH_VALUE execute. RANGE offsets, GROUPS, EXCLUDE, aggregate-window
DISTINCT, FROM FIRST/LAST and IGNORE/RESPECT NULLS remain named CK00001 refusals.
NEXT VALUE FOR in window inputs remains CS03076 (0A000); nextval() keeps per-call evaluation.

## Frame boundaries and defaults

ROWS counts physical rows in window order, including both boundaries. Bounds clamp to the
partition without overflow, even for Int64.MaxValue. A start cannot be UNBOUNDED FOLLOWING,
an end cannot be UNBOUNDED PRECEDING, and a start category cannot follow its end category:
UNBOUNDED PRECEDING, offset PRECEDING, CURRENT ROW, offset FOLLOWING, UNBOUNDED FOLLOWING.
Invalid categories use CS05080. Two offsets in the same direction may yield an empty frame;
their numeric values are not a planning-time ordering restriction.

Offsets accept signed integer expressions and parameters through BIGINT. They evaluate once
per window execution and are shared across partitions; functions using a named specification
share initialization. Empty input does not evaluate offsets. Current-query row variables,
aggregates and windows are prohibited; correlated outer-query values are constant for each
child execution. Noninteger, NULL and negative offsets use CS02070, CS02071 and CS02072.
**Difference from PostgreSQL:** offsets require an integer type; numeric/text assignment
coercion requires an explicit CAST.

With ORDER BY and no explicit frame, the default is RANGE BETWEEN UNBOUNDED PRECEDING AND
CURRENT ROW, through the last current-row peer. Without ORDER BY, every row is a peer and
the default covers the whole partition. Offset-free RANGE supports CURRENT ROW peer-group
starts/ends and unbounded bounds. It does not count physical peer rows as ROWS does.
Explicit frames do not change ranking or LAG/LEAD navigation.

## Aggregate windows

All catalog aggregates, including Boolean/statistical, STRING_AGG/LISTAGG and application
aggregates, support OVER and FILTER. FILTER runs before arguments and input ordering keys.
Empty frames return COUNT zero and other native aggregates NULL. Results retain the ordinary
aggregate's type, collation, coercions and rounding. DISTINCT with OVER remains CK00001.

Aggregate-local ORDER BY and LISTAGG WITHIN GROUP also work with OVER, extending PostgreSQL,
which refuses aggregate-local ordering with OVER. Native SUM, COUNT, MIN, MAX, AVG, Boolean
and statistical aggregates ignore that local ordering, including its key expressions.
Application aggregates and ordered string aggregates preserve it. See
[aggregate functions](../functions/aggregate-functions.md#aggregate-windows).

## Memory

Windows retain full input partitions; LIMIT cannot shorten them. Input/output rows, sort indexes,
keys and partition scratch share the statement memory grant. An excess reports CS05079 before
results are published. There is no spill; reservations release on success, failure and cancellation.
Accounting is conservative for aliased values and intermediate outputs. EXPLAIN describes a
WindowAgg stage with keys, functions and sort_shared for compatible sort groups.
Frame boundaries appear per function; EXPLAIN ANALYZE reports frame_visits for candidate
aggregate inputs before FILTER. Different frames can share a partition/order sort.

## Recomputation limit

Native aggregates reuse state while the frame head stays fixed, including bounded PRECEDING
heads initially clamped to the partition start. Equal consecutive frames reuse their result.
Application aggregates and ordered STRING_AGG/LISTAGG recompute changed frames, preserving
once-only finalization of each application accumulator.

**Native aggregates over moving frames, including `n PRECEDING`, are recomputed when the head
moves and can reach the cap until a later release.** This includes native numeric, Boolean,
statistical and string aggregates. A fixed-head growing native frame avoids this recomputation.

Recomputed frames share a fixed **1,000,000 candidate-input-visit cap per statement**, across
functions, partitions and window stages. The next visit raises CS05084 (54000) before FILTER,
input expressions or transitions. Even a FALSE filter spends visits; equal cached frames spend
none. LIMIT applies after complete window evaluation and cannot avoid this refusal. Reservations
release on failure. Ordered inputs and accumulator scratch also share the CS05079 memory grant;
string output cost grows with total returned text. Full retained-memory refinement is deferred.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS00011 | Invalid window-only function argument count or modifiers, including star, quantifiers, call ordering or FILTER | 42601 |
| CS01064 | Duplicate WINDOW name | 42P20 |
| CS01065 | Unknown window, self-reference or forward reference | 42704 |
| CS01066 | Copied window specifies PARTITION BY, overrides ORDER BY or copies an explicit frame | 42P20 |
| CS02068 | Non-integer navigation offset or NTILE bucket argument | 42804 |
| CS02069 | Incompatible navigation value/default types | 42804 |
| CS02070 | Noninteger ROWS frame offset | 42804 |
| CS02071 | NULL frame offset | 22004 |
| CS02072 | Negative frame offset | 22013 |
| CS02073 | Noninteger NTH_VALUE index | 42804 |
| CS05076 | Invalid placement or window function without OVER | 42P20 |
| CS05077 | Nested window function | 42P20 |
| CS05078 | Nonpositive NTILE count on the first ordered partition row | 22014 |
| CS05079 | Window state exceeds the statement memory grant | 54000 |
| CS05080 | Invalid frame boundary categories | 42P20 |
| CS05081 | Current-query row variable, aggregate or window in frame offset | 42P20 |
| CS05082 | Nonpositive NTH_VALUE index, even for an empty frame | 22016 |
| CS05083 | OVER on a scalar function | 42809 |
| CS05084 | Recomputed frames exceed 1,000,000 candidate input visits per statement | 54000 |
| CK00001 | Deferred frame modes, value modifiers or aggregate-window DISTINCT | — |

## Example

Execute separately:

```sql
CREATE TABLE t(p INT,v INT);
INSERT INTO t VALUES (1,10),(1,20),(2,40),(3,1);
SELECT p, v, ROW_NUMBER() OVER b AS n
FROM t WINDOW a AS (PARTITION BY p), b AS (a ORDER BY v)
ORDER BY p, v;
```

Returns `(1,10,1)`, `(1,20,2)`, `(2,40,1)`, `(3,1,1)`. Repeating PARTITION BY in b
would report CS01066.

For a frame example, execute separately in a fresh database:

```sql
CREATE TABLE framed(id INT,v INT);
INSERT INTO framed VALUES (1,2),(2,4),(3,6);
SELECT id, SUM(v) FILTER(WHERE v>2) OVER w AS total,
       FIRST_VALUE(v) OVER w AS first, LAST_VALUE(v) OVER w AS last,
       NTH_VALUE(v,2) OVER w AS second
FROM framed WINDOW w AS(ORDER BY id ROWS 1 PRECEDING) ORDER BY id;
```

Returns `(1,NULL,2,2,NULL)`, `(2,4,2,4,4)` and `(3,10,4,6,6)`.

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Window functions](../functions/window-functions.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlWindowExecutionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlWindowParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Planning/SqlPlanner.Windows.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlWindowFrameTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlWindowAggregateWorkTests.cs`.
