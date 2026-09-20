# ORDER BY

ORDER BY orders query results by one or more expressions with an optional direction for each.

## Syntax

```syntaxsql
ORDER BY expression [ ASC | DESC ] [ , ...n ]
```

## Supported clauses

| Construct | Support | Meaning |
|---|---|---|
| Source expression | Supported | Evaluates an ordering value |
| Standalone explicit projection alias | Supported | Resolves the projected expression |
| `ASC` or `DESC` | Supported | Ascending by default, or descending |
| `LIMIT`, `OFFSET` | Recognized, not supported (`COHDBL001`) | No pagination clause |

## Arguments

- **`expression`** — a source expression or a standalone alias introduced with projection `AS`.
- **`ASC` / `DESC`** — ascending or descending direction for the preceding expression.

## Remarks

A standalone explicit alias takes precedence over a source field with the same name. Grouped
ordering follows the same group-key restrictions as grouped projections. The executor preserves
the established document or group order for equal ordering values, yielding deterministic results
across scans and index seeks. Strings use case-sensitive ordinal order.

## Examples

The parser conformance corpus covers independent directions:

```sql
SELECT region, age FROM people ORDER BY region ASC, age DESC
```

## See also

[Clauses](index.md), [SELECT](../statements/select.md), and [GROUP BY](group-by.md).

## Sources

- **Grammar and default direction** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Select.cs`.
- **Ordering semantics** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Example** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlQueryParserTests.cs`.
