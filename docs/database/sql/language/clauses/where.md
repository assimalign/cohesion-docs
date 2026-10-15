# WHERE (Cohesion SQL)

Filters input rows using a Boolean search condition.

> **Status:** Partial. The documented subset executes; restrictions are listed below.

## Syntax

```syntaxsql
WHERE search_condition
```

## Arguments

- **`search_condition`** — A supported predicate over source columns, parameters, literals, or
  scalar expressions.

## Remarks

Only `TRUE` retains a row. `FALSE` and `UNKNOWN` are discarded; comparison with `NULL`
does not substitute for `IS NULL`. Filtering occurs before grouping and aggregation.
Aggregates cannot occur in `WHERE`.

`SELECT`, `UPDATE`, and `DELETE` accept `WHERE`. Supported uncorrelated subqueries can
appear in a `SELECT` predicate, but not in `UPDATE` or `DELETE`. String comparison and
`LIKE` use the effective [collation](collate.md).

## Examples

These examples use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
SELECT id FROM t WHERE age > 40 AND name LIKE 'G%' ORDER BY id;
```

The result is the single identifier `2`.

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`

