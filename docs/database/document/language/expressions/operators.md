# Operators

OQL provides scalar arithmetic, comparisons, Boolean operators, and null tests with explicit precedence.

## Syntax

```syntaxsql
<binary_expression> ::= expression binary_operator expression
<binary_operator> ::= { OR | AND | = | != | <> | < | <= | > | >= | + | - | * | / | % }
<unary_expression> ::= { NOT | + | - } expression
<null_test> ::= expression IS [ NOT ] NULL
```

## Precedence

| Order, lowest first | Operators |
|---|---|
| 1 | `OR` |
| 2 | `AND` |
| 3 | Comparisons, `IS NULL`, `IS NOT NULL` |
| 4 | Binary `+`, `-` |
| 5 | `*`, `/`, `%` |
| 6 | Unary `+`, `-` |

`NOT` wraps comparison-level expressions: `NOT age < 18` means `NOT (age < 18)`.
Parentheses override grouping. The expression tree normalizes `<>` to `!=`.

## Remarks

Arithmetic requires decimal operands. Invalid numeric input and division by zero fail explicitly.
Equality compares scalars and structurally compares arrays and objects. Range predicates compare
only matching scalar kinds. Ordinary comparisons with null are unknown; `WHERE` and `HAVING`
retain only true. Missing values participate in `IS NULL` in the same way as explicit null.

String comparisons are ordinal and case-sensitive. There is no document collation configuration
and SQL `COLLATE` does not change this model's string semantics.

## Examples

These arithmetic examples are accepted in the parser corpus:

```sql
SELECT -price + 2 * quantity AS adjusted FROM products
```

```sql
SELECT price / 2 % 3 FROM products
```

## See also

[Expressions](index.md), [Literals](literals.md), and [WHERE](../clauses/where.md).

## Sources

- **Precedence** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Expressions.cs`.
- **Evaluation semantics** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Examples** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlQueryParserTests.cs`.
