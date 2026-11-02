# OVER and WINDOW

OVER evaluates a window function over partitions of the current query's rows. WINDOW names reusable specifications.

## Syntax

```syntaxsql
window_function(...) OVER window_name
window_function(...) OVER (
    [ existing_window_name ]
    [ PARTITION BY expression [ , ...n ] ]
    [ ORDER BY expression [ ASC | DESC ] [ NULLS { FIRST | LAST } ] [ , ...n ] ]
)

-- After HAVING, before final ORDER BY and paging:
WINDOW window_name AS ( window_specification ) [ , ...n ]
```

## Remarks

Omitted PARTITION BY makes one partition. NULL partition keys group together. The window's
ORDER BY determines peers and navigation; final ORDER BY determines delivery order. ASC defaults
to NULLS FIRST and DESC to NULLS LAST, following the engine's ordinary ordering rules. These
defaults differ from PostgreSQL. Explicit NULLS FIRST/LAST overrides them. Collation and CHAR
comparison rules also apply to peer equality.

Names are case-insensitive, including quoted names. `OVER w` references a named specification;
`OVER (w ...)` copies it. A copy cannot specify PARTITION BY, even if the base has none. It can
add ORDER BY only when the base has no ordering. A WINDOW definition may inherit an earlier
definition, never itself or a later name. Duplicate and unused definitions still validate.

[Ranking and navigation functions](../functions/window-functions.md) run after WHERE, grouping
and HAVING and before DISTINCT, final ORDER BY and paging. They are allowed only in SELECT
outputs and ORDER BY at each query level. Grouped expressions, aggregates and GROUPING can be
window arguments or keys. Windows cannot appear in WHERE, JOIN ON, GROUP BY, HAVING, aggregate
inputs, LIMIT/OFFSET, or DML RETURNING, and cannot nest in another window's arguments or keys.
A derived SELECT can compute windows for an outer filter or DML source.

Explicit ROWS, RANGE, GROUPS and EXCLUDE, aggregate windows such as SUM(v) OVER (), and
FIRST_VALUE/LAST_VALUE/NTH_VALUE remain CK00001 for batch 12. No frame is silently approximated.
NEXT VALUE FOR in window inputs remains CS03076 (0A000); nextval() keeps per-call evaluation.

## Memory

Windows retain full input partitions; LIMIT cannot shorten them. Input/output rows, sort indexes,
keys and partition scratch share the statement memory grant. An excess reports CS05079 before
results are published. There is no spill; reservations release on success, failure and cancellation.
Accounting is conservative for aliased values and intermediate outputs. EXPLAIN describes a
WindowAgg stage with keys, functions and sort_shared for compatible sort groups.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS00011 | Invalid window argument count or modifiers, including star, quantifiers, call ordering or FILTER | 42601 |
| CS01064 | Duplicate WINDOW name | 42P20 |
| CS01065 | Unknown window, self-reference or forward reference | 42704 |
| CS01066 | Copied window specifies PARTITION BY or overrides an existing ORDER BY | 42P20 |
| CS02068 | Non-integer navigation offset or NTILE bucket argument | 42804 |
| CS02069 | Incompatible navigation value/default types | 42804 |
| CS05076 | Invalid placement or window function without OVER | 42P20 |
| CS05077 | Nested window function | 42P20 |
| CS05078 | Nonpositive NTILE count on the first ordered partition row | 22014 |
| CS05079 | Window state exceeds the statement memory grant | 54000 |
| CK00001 | Deferred frames, aggregate windows or value functions | — |

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

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Window functions](../functions/window-functions.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlWindowExecutionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlWindowParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Planning/SqlPlanner.Windows.cs`.
