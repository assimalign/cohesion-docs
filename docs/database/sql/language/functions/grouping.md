# GROUPING and GROUPING_ID

Returns a non-NULL INTEGER mask identifying omitted grouping expressions in a completed group.
GROUPING_ID is an alias with the same behavior.

## Syntax

```syntaxsql
{ GROUPING | GROUPING_ID } (expression [ , ...n ])
```

## Remarks

Supply 1–31 non-star arguments matching grouping expressions at the associated query level.
The call does not evaluate its arguments. The rightmost argument is the least significant bit;
one means that expression is absent from the current set, zero means it participates. Repeated
arguments repeat the bit. Plain GROUP BY produces zero masks. A real NULL grouping key has bit
zero; a subtotal's omitted key has bit one. SQL has no MISSING value.

Allowed positions are grouped SELECT, HAVING and ORDER BY, outside aggregate arguments, aggregate
FILTER/order inputs and grouping keys. WHERE and ungrouped output are refused. FILTER and
aggregate DISTINCT/ALL/input ORDER BY modifiers are not accepted on these special forms.
COHESION_SCHEMA.FUNCTIONS lists both names as SPECIAL FORM, without catalog overloads or a
separately invocable scalar callback.

Arguments determine the owning query level. A call inside a subquery referring only to an outer
level's keys reads the outer mask, whether or not the inner query has GROUP BY. Mixing local and
outer arguments cannot match one associated grouping level and reports CS01062.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS01062 | Argument is not a grouping expression of the associated query level | 42803 |
| CS01063 | Argument count outside 1–31, or star argument | 54023 |
| CS05074 | Invalid placement, including grouping keys, WHERE or aggregate inputs | 42803 |
| CS05072 | FILTER on this non-aggregate form | 42809 |
| CS05032 | Aggregate quantifier or input ordering modifier | — |

## Examples

Execute separately in a fresh database:

```sql
CREATE TABLE t(a INT,b INT);
INSERT INTO t VALUES (1,2);
SELECT a,b,GROUPING(a,b) AS mask, GROUPING_ID(b,a) AS reversed
FROM t GROUP BY CUBE(a,b) ORDER BY mask;
```

Returns `(1,2,0,0)`, `(1,NULL,1,2)`, `(NULL,2,2,1)`, `(NULL,NULL,3,3)`.
With the same table:

```sql
SELECT a,(SELECT GROUPING(t.a)) AS mask
FROM t GROUP BY ROLLUP(a) ORDER BY GROUPING(a),a;
```

Returns `(1,0)`, `(NULL,1)`; the subquery reads the outer grouping mask.

## See also

[Functions](index.md) · [GROUP BY](../clauses/group-by.md) · [HAVING](../clauses/having.md) · [EXPLAIN](../statements/explain.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAggregateExecutionTests.GroupingSets.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAggregateExecutionTests.GroupingSetsFixes.cs`.
