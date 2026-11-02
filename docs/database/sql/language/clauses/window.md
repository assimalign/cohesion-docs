# OVER and WINDOW

OVER evaluates a window function over partitions of the current query's rows. WINDOW names reusable specifications.

## Syntax

```syntaxsql
window_function(...) [ { IGNORE | RESPECT } NULLS ] OVER window_name
window_function(...) [ { IGNORE | RESPECT } NULLS ] OVER (
    [ existing_window_name ]
    [ PARTITION BY expression [ , ...n ] ]
    [ ORDER BY expression [ ASC | DESC ] [ NULLS { FIRST | LAST } ] [ , ...n ] ]
    [ { ROWS | RANGE | GROUPS } frame_extent
      [ EXCLUDE { CURRENT ROW | GROUP | TIES | NO OTHERS } ] ]
)

-- After HAVING, before final ORDER BY and paging:
WINDOW window_name AS ( window_specification ) [ , ...n ]

<frame_extent> ::= frame_start | BETWEEN frame_start AND frame_end
<frame_start> ::= UNBOUNDED PRECEDING | offset PRECEDING | CURRENT ROW
                | offset FOLLOWING
<frame_end> ::= offset PRECEDING | CURRENT ROW | offset FOLLOWING | UNBOUNDED FOLLOWING
```

The short form ends at CURRENT ROW and permits only UNBOUNDED PRECEDING, offset PRECEDING or
CURRENT ROW. NULL treatment applies only to LAG, LEAD, FIRST_VALUE, LAST_VALUE and NTH_VALUE.
See the boundary restrictions below.

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

ROWS, RANGE with offsets, GROUPS, all four exclusions and IGNORE/RESPECT NULLS execute.
Aggregate-window DISTINCT and FROM FIRST/LAST remain named CK00001 refusals.
NEXT VALUE FOR in window inputs remains CS03076 (0A000); nextval() keeps per-call evaluation.

## Frame boundaries and defaults

ROWS counts physical rows in window order, including both boundaries. Bounds clamp to the
partition without overflow, even for Int64.MaxValue. A start cannot be UNBOUNDED FOLLOWING,
an end cannot be UNBOUNDED PRECEDING, and a start category cannot follow its end category:
UNBOUNDED PRECEDING, offset PRECEDING, CURRENT ROW, offset FOLLOWING, UNBOUNDED FOLLOWING.
Invalid categories use CS05080. Two offsets in the same direction may yield an empty frame;
their numeric values are not a planning-time ordering restriction.

ROWS and GROUPS offsets accept signed integer expressions and parameters through BIGINT. They evaluate once
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

## RANGE offsets and GROUPS

Offset RANGE requires exactly one ORDER BY key (CS05086). Bounds use distances in that key's
domain; PRECEDING/FOLLOWING follows ASC or DESC, with peer-inclusive zero offsets.

| Ordering key | Accepted offset domain |
| --- | --- |
| TINYINT, SMALLINT, INTEGER, BIGINT | Signed integer |
| DECIMAL | Integer or DECIMAL |
| REAL, DOUBLE PRECISION | Numeric |
| DATE, TIME, TIMESTAMP, TIMESTAMPTZ, INTERVAL | INTERVAL |

Other key types use CS02074; incompatible offset types use CS02075. NULL offsets use CS02071;
negative or NaN RANGE offsets use CS02076. Floating positive infinity is accepted. Offsets initialize
once per execution across partitions; empty input skips initialization. Exact numeric boundaries
do not overflow the key's storage type. Unrepresentable date/timestamp boundaries retain CS02048
(22008); interval boundaries retain CS02062 (22015).

A NULL current key uses its NULL peer boundary for an offset bound. A non-NULL current key compares
NULL candidates by the selected NULL sort placement. TIME distances never wrap at midnight.
INTERVAL is the engine's TimeSpan duration; calendar months and infinite temporal values are absent.
TIMESTAMP uses wall-clock distances and TIMESTAMPTZ uses UTC distances, a departure from PostgreSQL's
zone-aware temporal arithmetic. NaN sorts low, unlike PostgreSQL's NaN-high ordering.

GROUPS counts peer groups rather than physical rows. It requires ORDER BY (CS05087), uses the
ROWS signed-integer offset domain and clamps out-of-partition bounds. CURRENT ROW covers the
current peer group.

## Exclusion

Exclusion applies after determining the frame, to aggregate and frame-value functions.
Ranking and LAG/LEAD remain independent of the frame.

| Clause | Members removed |
| --- | --- |
| EXCLUDE NO OTHERS (default) | None |
| EXCLUDE CURRENT ROW | Only the current row |
| EXCLUDE GROUP | Current row and all its ORDER BY peers |
| EXCLUDE TIES | Other peers; keeps the current row only if it belongs to the frame |

The same rules apply to ROWS. Without ORDER BY the entire partition is one peer group.
Canonical SQL omits explicit NO OTHERS.

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

Windows retain complete input partitions; LIMIT cannot shorten them. StatementMemoryBudget
(default 64 MiB) charges rows, output cells, keys, sort indexes, peer indexes, cached arguments,
transition state, string buffers and the ownership ledger itself. Reference-identical payloads
are charged once, including aliased text, CHAR and binary values. Augmented rows replace consumed
input rows; incompatible sorts release old keys. Partition scratch releases at its boundary;
output cells remain charged until the stage ends. Buffer capacity and growth overlap are charged.
Application accumulator internals remain application-owned; their handles and engine-owned inputs
and sort buffers are charged. This is a conservative retained-memory estimate, not a GC heap limit.

CS05079 refuses an excess before publication. Pre-existing subquery vectors and CTE/relation rows
are admitted to the same owner; failed admission can use CS05067 (53200). There is no spill;
success, failure and cancellation release reservations.

The source's full-output fixture admits 138,961 rows for ROW_NUMBER plus RANK over the three-INT
id/p/k source, and 162,448 rows for moving AVG(k) with ROWS 99 PRECEDING, at the default grant.
The next row is refused. These are fixture limits, not universal row caps; widths, functions,
partitions and other retained statement state change them.

EXPLAIN describes WindowAgg keys, functions, frames and sort_shared. Different frames can share
a partition/order sort. ANALYZE reports frame_visits, transitions, inverse_transitions and
boundary_visits.

## Recomputation limit

Native SUM, COUNT, AVG, Boolean and statistical aggregates use inverse transitions for safe
arguments and FILTER; MIN/MAX use monotonic queues. Work is O(n) per function and partition,
including moving frames and exclusion. FILTER evaluates once per visited input and arguments
only for admitted inputs. Cached inputs and state release at the partition boundary. Exact
numeric state preserves small terms through cancellation and rounds only final results.

Application aggregates and order-sensitive STRING_AGG/LISTAGG recompute changed frames, with
once-only application finalization. They share a **1,000,000 candidate-input-visit cap per
statement** across functions, partitions and stages (CS05084 / 54000), counted before FILTER or
arguments. Equal cached frames spend no visits; LIMIT cannot avoid this work.

Volatile functions or subqueries in arguments or FILTER disable inverse transitions and input
caching. When the frame head moves, restart evaluation re-evaluates the new frame, following
PostgreSQL. This includes nextval, setval and application scalar functions unless Stable or
Immutable. **This restart path can also reach CS05084, even for a native aggregate.** For example,
the branch's volatile-input regression refuses a 2,000-row shrinking SUM(observe(id)) frame.
Safe native moving aggregates avoid that cap. Value-function NULL skipping has no O(n) promise.
String output cost also grows with total returned text.

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
| CS02074 | Unsupported offset RANGE ordering-key domain | 0A000 |
| CS02075 | Incompatible RANGE offset domain | 0A000 |
| CS02076 | Negative or NaN RANGE offset | 22013 |
| CS05076 | Invalid placement or window function without OVER | 42P20 |
| CS05077 | Nested window function | 42P20 |
| CS05078 | Nonpositive NTILE count on the first ordered partition row | 22014 |
| CS05079 | Window state exceeds the statement memory grant | 54000 |
| CS05080 | Invalid frame boundary categories | 42P20 |
| CS05081 | Current-query row variable, aggregate or window in frame offset | 42P20 |
| CS05082 | Nonpositive NTH_VALUE index, even for an empty frame | 22016 |
| CS05083 | OVER on a scalar function | 42809 |
| CS05084 | Application/order-sensitive or restart-path frame recomputation exceeds 1,000,000 candidate visits per statement | 54000 |
| CS05086 | Offset RANGE has other than one ORDER BY key | 42P20 |
| CS05087 | GROUPS without ORDER BY | 42P20 |
| CS05088 | NULL treatment on a function other than LAG/LEAD/FIRST_VALUE/LAST_VALUE/NTH_VALUE | 42601 |
| CK00001 | FROM FIRST/LAST or aggregate-window DISTINCT | — |

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

On a fresh table, these examples compare distance, peer groups and exclusion:

```sql
CREATE TABLE peers(id INT,k INT,v INT);
INSERT INTO peers VALUES (1,1,10),(2,1,20),(3,3,30),(4,4,40);
SELECT id, SUM(v) OVER(ORDER BY k RANGE 1 PRECEDING) AS distance,
       SUM(v) OVER(ORDER BY k GROUPS 1 PRECEDING EXCLUDE TIES) AS grouped_total
FROM peers ORDER BY id;
```

Returns (1,30,10), (2,30,20), (3,30,60), (4,70,70).

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Window functions](../functions/window-functions.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlWindowRangeGroupsTests.cs`, `SqlWindowMovingTests.cs`, `SqlWindowRetainedMemoryTests.cs` and `SqlWindowReviewRegressionTests.cs`.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlWindowExecutionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlWindowParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Planning/SqlPlanner.Windows.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlWindowFrameTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlWindowAggregateWorkTests.cs`.
