# GROUP BY

GROUP BY partitions documents by one or more expression values for grouped projection and aggregation.

## Syntax

```syntaxsql
GROUP BY expression [ , ...n ]
```

## Supported clauses

| Construct | Support | Meaning |
|---|---|---|
| One or more grouping expressions | Supported | Structural grouping keys |
| Subsequent `HAVING` | Supported | Filters groups |
| Subsequent `ORDER BY` | Supported | Orders grouped results |

## Arguments

- **`expression`** — a grouping key expression; multiple expressions are comma separated.

## Remarks

Null and missing values share a group. Group values compare structurally. Group ordering uses the
total order null, Boolean, decimal, ordinal string, array, object. Arrays compare element by
element; objects compare ordinal property names and their values.

Outside aggregate calls, grouped projection, `HAVING`, and `ORDER BY` must use group keys or
expressions built from group keys and constants. Binding compares expression structure, preserves
literal scalar types, and treats qualified and unqualified paths for the same iteration variable
as equivalent. Aggregate queries without explicit grouping form one group even for an empty source.

## Examples

The parser corpus uses a country key and document count:

```sql
SELECT country, COUNT(*) AS total FROM people GROUP BY country HAVING COUNT(*) > 1 ORDER BY total DESC
```

## See also

[Clauses](index.md), [HAVING](having.md), and [Aggregate functions](../functions/aggregate-functions.md).

## Sources

- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Select.cs`.
- **Grouping semantics** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Example** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlQueryParserTests.cs`.
