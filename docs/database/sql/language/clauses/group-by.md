# GROUP BY (Cohesion SQL)

Forms groups from one or more scalar expressions over the filtered input.

> **Status:** Partial. The documented subset executes; restrictions are listed below.

## Syntax

```syntaxsql
GROUP BY [ ALL | DISTINCT ] grouping_element [ , ...n ]

grouping_element := expression | () | (grouping_element [ , ...n ])
                  | GROUPING SETS (grouping_element [ , ...n ])
                  | ROLLUP (grouping_element [ , ...n ])
                  | CUBE (grouping_element [ , ...n ])
```

## Arguments

- **`expression`** — A scalar grouping key bound to the input source.

## Remarks

Rows surviving `WHERE` form groups by the tuple of grouping values. `NULL` keys group
together. Strings use matching equality and hashing under the effective collation; a folded group
preserves a member's original spelling.

A projected, ordered, or `HAVING` column outside an aggregate must be a grouping expression or
derived from grouped columns. Invalid ungrouped columns fail planning even on empty input.
Aggregate arguments cannot contain aggregates, and aggregates cannot be grouping keys.

Over empty input, each empty set produces one group and nonempty sets produce none. An ungrouped
aggregate has one implicit group. Grouping composes with supported joins and system relations.
`HAVING` filters groups before ordering and pagination.

[FILTER](filter.md) selects input separately for each aggregate before argument evaluation.
Rejecting every row of an aggregate does not remove its group; its empty-input rule applies.

Projection aliases are outside grouping's alias-binding scope. Standalone numeric keys
(`GROUP BY 1`) report `CK00001`; `GROUP BY 1 + 1` is an ordinary constant expression.

### Grouping sets, ROLLUP and CUBE

GROUPING SETS concatenates its children's sets. ROLLUP includes every prefix and the empty
prefix: `ROLLUP(a,b)` expands to `(a,b)`, `(a)`, `()`. CUBE includes every subset:
`CUBE(a,b)` expands to `()`, `(a)`, `(b)`, `(a,b)`. A composite item is one unit:
`ROLLUP((a,b),c)` yields `(a,b,c)`, `(a,b)`, `()`. Nested composites and nested GROUPING SETS
flatten. ROLLUP/CUBE inside GROUPING SETS execute; GROUPING SETS inside ROLLUP/CUBE fails
planning with CS00010 (42601), including through composite wrappers.

Comma-separated elements combine by Cartesian product. For example, `a, ROLLUP(b,c)` yields
`(a,b,c)`, `(a,b)`, `(a)`. Repeated columns within a set collapse. Duplicate sets retain independent
results by default and with GROUP BY ALL; GROUP BY DISTINCT removes duplicate sets after expansion.
The expansion limit is 4096 sets before DISTINCT (CS05075, 54001).

Absent grouping expressions become NULL in SELECT, HAVING, ORDER BY and correlated reads of
completed groups. Real NULL keys remain separate from subtotals; use
[GROUPING/GROUPING_ID](../functions/grouping.md) to distinguish them. Each set owns aggregate
state, DISTINCT tuples and ordered-input representatives. An aggregate's FILTER evaluates once
per input before arguments/order keys; admitted values feed each set. Separate volatile logical
calls remain separate. Retained grouping-set/mask state charges StatementMemoryBudget;
CS05067 (53200) refuses exhaustion and there is no spill.

EXPLAIN's Group detail lists `grouping_sets=N; set_keys=[(...),...]`, using zero-based key ordinals
and retaining duplicates. For `CUBE(a,b)`, the listing is `[(),(0),(1),(0,1)]`.

[Aggregate calls](../functions/aggregate-functions.md) accept DISTINCT/ALL and input ORDER BY.
STRING_AGG and LISTAGG concatenate ordered text within each group.

## Repeated subquery keys

A scalar subquery, EXISTS, quantified comparison or IN expression repeated in the select list,
HAVING or ORDER BY reuses its GROUP BY value when the complete expression matches structurally
in the same scope, including identical correlations. This follows PostgreSQL's whole-expression
grouping rule. Operators, quantifiers (including ANY versus SOME), negation, bound scalar operands
and query bodies remain significant. Different or unproven expressions do not share a group slot;
an ungrouped reference still reports CS05017 (no assigned SQLSTATE).

For `candidates(id)` containing 1, 2, 3 and NULL:

```sql
SELECT c.id = ANY (SELECT 2), COUNT(*)
FROM candidates c
GROUP BY c.id = ANY (SELECT 2)
HAVING c.id = ANY (SELECT 2)
ORDER BY c.id = ANY (SELECT 2);
```

The result is `(TRUE, 1)`. See [quantified comparisons](../expressions/quantified-comparisons.md).

## Examples

These examples use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
SELECT age > 40, COUNT(*), SUM(age)
FROM t
GROUP BY age > 40
ORDER BY age > 40;
```

The rows are `(FALSE, 1, 36)` and `(TRUE, 2, 86)`; sums have shared type `Decimal`.

In a fresh database, execute separately:

```sql
CREATE TABLE sales(region TEXT, amount INT);
INSERT INTO sales VALUES ('east',10),('east',20),('west',5);
SELECT region, SUM(amount), GROUPING(region) AS subtotal
FROM sales GROUP BY ROLLUP(region) ORDER BY subtotal, region;
```

Returns `('east',30,0)`, `('west',5,0)`, `(NULL,35,1)`. An empty sales table would return
only `(NULL,NULL,1)`; COUNT(*) in that row would be zero.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS00010 | GROUPING SETS appears inside ROLLUP or CUBE | 42601 |
| CS05075 | Expansion exceeds 4096 sets before DISTINCT | 54001 |
| CS05067 | Retained grouping-set/mask state exhausts the statement budget | 53200 |
| CS01062 | GROUPING argument does not match a grouping expression at its associated query level | 42803 |
| CS01063 | GROUPING has fewer than 1 or more than 31 arguments, or uses star | 54023 |
| CS05074 | GROUPING outside grouped output or in grouping/aggregate inputs | 42803 |

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAggregateExecutionTests.GroupingSets.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAggregateExecutionTests.GroupingSetsFixes.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlSubqueryExecutionTests.Grouping.cs`.

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAggregateExecutionTests.cs`

