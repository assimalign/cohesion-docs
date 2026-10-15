# ORDER BY (Cohesion SQL)

Orders query results using source expressions, projection aliases, or output ordinals.

> **Status:** Partial. The documented subset executes; restrictions are listed below.

## Syntax

```syntaxsql
ORDER BY <order_by_item> [ , ...n ]

<order_by_item> ::= { expression | column_alias | select_list_ordinal } [ ASC | DESC ]
```

## Arguments

- **`expression`** — A source column or executable scalar expression.
- **`column_alias`** — An explicit output alias, usable alone or inside a scalar expression.
- **`select_list_ordinal`** — A one-based output position after wildcard expansion.
- **`ASC` or `DESC`** — Ascending (the default) or descending comparison for that key.

## Remarks

Keys compare left to right. `NULL` sorts first ascending and last descending. Equal keys have
no promised relative order; add a tie-breaker for stable paging. Ordering evaluates after grouping
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

`NULLS FIRST` and `NULLS LAST` report `COHDBL001`. Derived tables are not supported.
Ordinals are exclusive to `ORDER BY`; numeric grouping keys do not identify outputs.

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

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`

