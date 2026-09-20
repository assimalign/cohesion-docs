# HAVING (Cohesion SQL)

Filters completed groups or the implicit group of an aggregate query.

> **Status:** Partial. The documented subset executes; restrictions are listed below.

## Syntax

```syntaxsql
HAVING search_condition
```

## Arguments

- **`search_condition`** — A predicate over grouping expressions and aggregate results.

## Remarks

Only `TRUE` retains a group. `WHERE` runs on input rows; `HAVING` runs after aggregation
and before ordering, `OFFSET`, and `LIMIT`. An ungrouped aggregate can have `HAVING`.

Output aliases are not bound here: repeat the grouping expression or aggregate. A column outside
an aggregate must satisfy grouping validity. Invalid ungrouped references are planning errors even
when no input rows exist. A supported uncorrelated subquery may participate in the predicate.

## Examples

These examples use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
SELECT age > 40, COUNT(*), SUM(age)
FROM t
WHERE age > 35
GROUP BY age > 40
HAVING SUM(age) > 50;
```

The surviving row is `(TRUE, 2, 86)`.

## See also

[Clauses](index.md) · [SELECT](../statements/select.md) · [Unsupported syntax](../unsupported.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlAggregateExecutionTests.cs`

