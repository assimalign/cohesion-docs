# ORDER BY (Cohesion SQL)

Orders query results using source expressions, projection aliases, or output ordinals.

> **Status:** Partial. The documented subset executes; restrictions are listed below.

## Syntax

```syntaxsql
ORDER BY <order_by_item> [ , ...n ]

<order_by_item> ::= { expression | column_alias | select_list_ordinal } [ ASC | DESC ] [ NULLS FIRST | NULLS LAST ]
```

## Arguments

- **`expression`** — A source column or executable scalar expression.
- **`column_alias`** — An explicit output alias, usable alone or inside a scalar expression.
- **`select_list_ordinal`** — A one-based output position after wildcard expansion.
- **`ASC` or `DESC`** — Ascending (the default) or descending comparison for that key.

## Remarks

After [UNION/INTERSECT/EXCEPT](set-operations.md), ordering accepts first-operand output names or
positive one-based ordinals only. Expressions and qualified names use CS02018; ambiguous
output names use CS01019. Parenthesized operands can retain their own ordinary SELECT ordering.

Keys compare left to right. `NULL` sorts first ascending and last descending by default. Per-key NULLS FIRST/LAST overrides
that placement independently of direction. Equal keys retain input order; add a tie-breaker for
deterministic paging across access paths. Ordering evaluates after grouping
and `HAVING`, before pagination, and composes with `DISTINCT` and supported system relations,
joins, aggregates, and subqueries.

An unqualified name resolves to an explicit projection alias first, case-insensitively, then a
source column. This also applies inside scalar expressions. A qualified name selects the source.
Duplicate aliases are ambiguous; an ordinal can disambiguate. Aliases use their selected expression's
source scope and never recursively bind to themselves or other output aliases. Aggregate arguments
remain in input scope; inner queries resolve their own names. Collation follows the referenced value.

| Ordering item | Meaning |
| --- | --- |
| `years` or `years + 1` after `age AS years` | Uses projected `age`, even if a source `years` exists |
| `t.years` | Uses the source column |
| `1`, `+1`, or `(1)` | First output column |
| `0`, negative or out-of-range integer | Planning error, even on empty input |
| `1.0`, `.5`, or `1e0` | Invalid numeric ordinal syntax |
| `1 + 1` | Constant scalar expression, not ordinal two |
| Parameter or numeric literal inside a larger expression | Value, not an output position |

Placement works in SELECT tails, aggregate inputs, set results, derived tables, CTEs and views,
including aliases, ordinals and merged USING values. Malformed `ORDER BY a NULLS` or
`ORDER BY a NULLS MIDDLE` reports CS00003. NULLS/FIRST/LAST remain contextual identifiers.
EXPLAIN records the effective placement per key. Every ORDER BY sorts after its access path;
indexes do not supply result order.
Ordinals are exclusive to `ORDER BY`; numeric grouping keys do not identify outputs.

With [USING joins](join.md#using-columns), aliases after `*` and numeric ordinals count the
visible merged output: each USING pair contributes one column. Repeated wildcards each expand
their visible columns. Out-of-range positions report `CS02018`, even on empty inputs.

[Aggregate input ORDER BY](../functions/aggregate-functions.md#aggregate-modifiers) binds to source
rows. Unlike this outer clause, its integer keys are constants rather than output ordinals.

## Evaluation order

For an ordinary SELECT whose ordering does not reference outputs, nonvolatile projections and
ordering keys evaluate together in input order, projection first. A projection error can therefore
precede a key error. When keys reference an output alias or ordinal, that output is evaluated once
per input and reused by the keys.

Independent volatile outputs keep full sorting: keys evaluate in input order, then projections
evaluate in sorted output order before pagination, even with LIMIT 0 or without a limit. A limit
does not reduce their call count. This postponement follows PostgreSQL's treatment of independent
volatile outputs. Grouped and correlated projections retain evaluation before sorting.

Outside that volatile-output fallback, a non-DISTINCT stored-table or system-view SELECT with
LIMIT 0 skips its scan and output/key evaluation even when ORDER BY is present. Planning still
validates the query.

For non-DISTINCT ORDER BY with a positive limit and fixed ordering collations, execution keeps
only the best OFFSET + LIMIT candidates while examining all qualifying inputs. Ties preserve the
same input order as a full sort, including collation-equivalent keys and NULLs. Deferred ordering
collations use full sorting with the last qualifying row's resolved collations; a later row rejected
by WHERE or HAVING does not replace them. DISTINCT completes before pagination. SQL has no
MISSING value. Results remain materialized before delivery.

## Examples

```sql
CREATE TABLE ordering_rows (id INT PRIMARY KEY, age INT);
INSERT INTO ordering_rows VALUES
    (40, 20), (10, 10), (50, 20), (20, 30), (30, 10);
SELECT id, age AS years
FROM ordering_rows
ORDER BY years + 1 ASC, 1 DESC;
```

The identifiers return in order `30`, `10`, `50`, `40`, `20`, unlike insertion order.

Adding `LIMIT 2 OFFSET 1` returns `10`, `50` with the same ordering as the full result.

```sql
CREATE TABLE a (id INT NOT NULL);
CREATE TABLE b (id INT NOT NULL);
INSERT INTO a VALUES (2), (1);
INSERT INTO b VALUES (1), (2);
SELECT *, a.id AS sort, -a.id AS tail
FROM a JOIN b USING (id) ORDER BY sort;
SELECT *, -a.id AS sort FROM a LEFT JOIN b USING (id) ORDER BY 2;
```

The first query returns `(1, 1, -1)`, `(2, 2, -2)` with three columns. The second returns
`(2, -2)`, `(1, -1)` with two columns; `ORDER BY 3` there would report `CS02018`.

### Explicit placement

```sql
CREATE TABLE nullable_order (id INT, x INT);
INSERT INTO nullable_order VALUES (1, NULL), (2, 2), (3, 1);
SELECT d.id FROM (SELECT id, x FROM nullable_order) d ORDER BY d.x NULLS LAST, d.id;
SELECT id FROM nullable_order ORDER BY x DESC NULLS FIRST, id;
```

The results are 3, 2, 1 and 1, 2, 3. NaN remains a non-NULL numeric value; placement does not
change its order relative to other non-NULL numbers.

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- **Zero-limit execution paths** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Execution/SqlPlanExecutor.cs` and `SqlRowCollector.cs` in the same directory.
- **Stable bounded ordering and volatile outputs** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlBoundedExecutionTests.cs` and `SqlBoundedSelectionRegressionTests.cs` in the same directory.
- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Wildcard positions** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlNWayJoinTests.cs`

