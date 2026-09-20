# HAVING

HAVING filters groups with a predicate that may contain aggregate calls.

## Syntax

```syntaxsql
HAVING predicate
```

## Supported clauses

| Construct | Support | Meaning |
|---|---|---|
| Aggregate calls in predicates | Supported | `COUNT`, `SUM`, `AVG`, `MIN`, `MAX` |
| Group-key expressions | Supported | Subject to grouped-expression validation |
| Subsequent `ORDER BY` | Supported | Orders retained results |

## Arguments

- **`predicate`** — a group predicate; only true retains the group.

## Remarks

The Documents planner validates aggregate placement and grouping consistency. Outside aggregate
calls, expressions must use group keys or expressions built from group keys and constants.
Null comparisons are unknown and do not retain a group. `WHERE` filters source documents;
`HAVING` supplies the group predicate.

## Examples

This parser structural test filters on a count greater than two:

```sql
SELECT country, COUNT(*) AS n FROM people GROUP BY country HAVING COUNT(*) > 2 ORDER BY n DESC
```

## See also

[Clauses](index.md), [GROUP BY](group-by.md), [WHERE](where.md),
and [Aggregate functions](../functions/aggregate-functions.md).

## Sources

- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Select.cs`.
- **Group filtering** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Example** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlQueryParserTests.cs`.
