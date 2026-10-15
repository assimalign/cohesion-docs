# WHERE

WHERE filters source documents using a scalar comparison or Boolean expression.

## Syntax

```syntaxsql
WHERE predicate
```

## Supported clauses

| Predicate feature | Support |
|---|---|
| Comparisons, arithmetic, Boolean operators, null tests | Supported |
| Paths, literals, parameters | Supported |
| `IN`, `EXISTS`, `LIKE`, `BETWEEN` | Recognized, not supported (`COHDBL001`) |

## Arguments

- **`predicate`** — an expression evaluated for each source document; only true retains a document.

## Remarks

Missing properties, incompatible shapes, and out-of-range array positions evaluate to null.
Ordinary comparisons involving null are unknown, so use `IS NULL` or `IS NOT NULL` for null tests.
Scalar range comparisons require matching scalar kinds; equality also supports structural object
and array comparison. String comparison is ordinal and case-sensitive.

Applicable equality or range predicates can use a path index. The executor reapplies the complete
predicate to index candidates, preserving the same mixed-shape behavior as a scan.

## Examples

The parser corpus includes Boolean precedence and null tests:

```sql
SELECT * FROM people WHERE NOT age < 18 AND active = TRUE OR name = 'Ana'
```

```sql
SELECT * FROM people WHERE age IS NULL OR address IS NOT NULL
```

## See also

[Clauses](index.md), [Operators](../expressions/operators.md), and [HAVING](having.md).

## Sources

- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DESIGN.md`.
- **Predicate execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Examples** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlQueryParserTests.cs`.
