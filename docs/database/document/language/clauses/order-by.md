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
| [LIMIT/OFFSET](limit-offset.md) | Supported | Final output slice after ordering; either clause order |

## Arguments

- **`expression`** — a source expression or a standalone alias introduced with projection `AS`.
- **`ASC` / `DESC`** — ascending or descending direction for the preceding expression.

## Remarks

A standalone explicit alias takes precedence over a source field with the same name. Grouped
ordering follows the same group-key restrictions as grouped projections. The executor preserves
the established document or group order for equal ordering values, yielding deterministic results
across scans and index seeks. Strings use case-sensitive ordinal order.

[MISSING](../expressions/missing.md) sorts before NULL ascending; descending reverses this order.
With SELECT DISTINCT, every key must match a projected expression after explicit-alias rewriting;
an outside key fails with `CD01014` at that key. Top-level VALUE/RAW normalizes a MISSING projected
value to NULL before ordering by the matching expression.

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
