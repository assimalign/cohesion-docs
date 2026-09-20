# Operators and predicates (Cohesion SQL)

Combines scalar values and evaluates SQL search conditions.

> **Status:** Partial. This page describes the executable subset and its boundaries.

## Syntax

```syntaxsql
left_expression { + | - | * | / | % | || } right_expression
- expression
left_expression { = | <> | < | <= | > | >= } right_expression
expression IS [ NOT ] NULL
expression [ NOT ] BETWEEN lower_expression AND upper_expression
expression [ NOT ] IN ( expression [ , ...n ] )
expression [ NOT ] LIKE pattern_expression
NOT search_condition
left_condition { AND | OR } right_condition
```

## Arguments

- **`expression`** — An executable scalar value.
- **`search_condition`** — A Boolean predicate using SQL three-valued logic.
- **`pattern_expression`** — A `LIKE` pattern; percent matches any sequence and underscore
  matches one character.

## Remarks

Precedence from low to high is `OR`, `AND`, logical `NOT`, comparison/predicates,
additive operators (including concatenation), multiplicative operators, unary operators, then
primary expressions. Parentheses explicitly group evaluation. Unary bitwise complement `~`
is parsed but not evaluated and is intentionally absent from the executable diagram.

Comparison with a null operand yields `UNKNOWN`; `IS NULL` and `IS NOT NULL` test nullness.
`BETWEEN` includes its lower and upper bounds and returns unknown with a null operand or bound.
Only true conditions retain rows in `WHERE`, `ON`, and `HAVING`.

| Inputs | `AND` | `OR` |
| --- | --- | --- |
| `TRUE`, `UNKNOWN` | `UNKNOWN` | `TRUE` |
| `FALSE`, `UNKNOWN` | `FALSE` | `UNKNOWN` |
| `UNKNOWN`, `UNKNOWN` | `UNKNOWN` | `UNKNOWN` |

`NOT UNKNOWN` remains unknown. Membership and its empty-result rules are detailed in
[Subqueries](subqueries.md); a null-containing candidate list can also produce unknown.
`LIKE` uses effective collation and propagates null operands.

Predicates, ordering, distinct results, grouping, and extrema share one non-null comparator.
Binary values compare unsigned bytes lexicographically without padding; a proper prefix sorts first.
Signed integers and decimals compare exactly. Mixed exact/approximate numerics compare represented
values without rounding either operand; for example, a bound binary floating-point `0.1` is
greater than exact decimal `0.1`. Compatible floating types retain their represented precision.

All floating-point not-a-number (NaN) values compare equal and below other non-null numerics.
The order is NaN, negative infinity, finite values, positive infinity. Signed zeros compare equal.
NaN is not null. Bind these values as parameters; SQL has no NaN/infinity literals. These comparison
rules do not broaden CAST, arithmetic, aggregate accumulation, or physical index-key semantics.
Floating range and signed-zero equality predicates scan or remain residual filters rather than
supplying unsafe index bounds.

Strings use effective collation; Boolean false sorts before true. Incompatible type families
raise a query error.

## Examples

Use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
SELECT id FROM t WHERE age > 40 AND name LIKE 'G%' ORDER BY id;
SELECT id, age + 1 AS next_age FROM t ORDER BY id;
```

These conformance queries exercise predicates and row arithmetic.

## See also

[Expressions](index.md) · [SELECT](../statements/select.md) · [Data types](../data-types/index.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/SqlExpressionEvaluator.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Expressions.cs`
