# Operators

GQL supports scalar comparison operators and AND conjunction in predicates.

> **Status:** Implemented.

## Syntax

```syntaxsql
<comparison> ::= operand { = | <> | != | < | <= | > | >= } operand
<predicate> ::= comparison [ AND comparison ...n ]
```

Parenthesized predicates are also permitted as described in [WHERE](../clauses/where.md).

## Operator support

| Operator | Support | Meaning |
|---|---|---|
| `=` | Supported | Equality |
| `<>`, `!=` | Supported | Inequality; `!=` is an accepted convenience |
| `<`, `<=`, `>`, `>=` | Supported | Scalar ordering comparisons |
| `AND` | Supported | Conjunction |
| `OR`, `NOT`, `XOR`, `IN`, `IS` | Recognized, not supported (`COHDBL001`) | Outside executable predicates |

## Arguments

- **`operand`** — a scalar literal or a bound variable's single property.
- **`comparison`** — two operands and a scalar comparison operator.

## Remarks

The parser normalizes `<>` to `!=` in `GqlBinaryExpression.Operator`. Use `<>` when choosing
the portable spelling. `AND` builds a left-associated conjunction tree; both comparisons and
parentheses are subject to the predicate limits.

Strings compare using ordinal, case-sensitive semantics. Integral and decimal comparisons retain
exactness; comparisons involving a floating-point operand use double precision. The query literal
grammar supplies signed integers and finite doubles, while typed graph properties can contain
additional numeric runtime types.

## Examples

The parser corpus covers inequality and conjunction:

```sql
MATCH (a) WHERE a.age >= 18 AND a.name <> 'Bob' RETURN a
```

## See also

- **[Expressions](index.md)** — expression navigation.
- **[WHERE](../clauses/where.md)** — complete predicate grammar.
- **[Literals](literals.md)** — operand types.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlQueryParser.Expressions.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.

