# WHERE

WHERE filters matched graph bindings with scalar comparisons joined by AND.

> **Status:** Implemented.

## Syntax

```syntaxsql
WHERE comparison [ AND comparison ...n ]

<comparison> ::=
    { operand comparison_operator operand | "(" predicate ")" }

<predicate> ::= comparison [ AND comparison ...n ]
<operand> ::= { variable "." property_name | scalar_literal }
<comparison_operator> ::= { = | <> | != | < | <= | > | >= }
```

## Supported clauses

| Construct | Support |
|---|---|
| Scalar comparisons and `AND` | Supported |
| Nested predicate parentheses | Supported |
| `OR`, `NOT`, `IS NULL`, function calls | Recognized, not supported (`COHDBL001`) |
| Parameter references | Recognized, not supported (`COHDBL001`) |

## Arguments

- **`variable.property_name`** — a single scalar property on a bound graph variable.
- **`scalar_literal`** — null, Boolean, signed integer, finite double, or string.
- **`comparison_operator`** — equality, inequality, or ordering comparison.

## Remarks

`WHERE` follows `MATCH`. Both property-to-literal and property-to-property comparisons are accepted.
A bare property is not a Boolean predicate; each scalar comparison requires an operator and two
operands. Predicates are limited to 128 nesting levels and 128 scalar comparisons.

String comparisons use case-sensitive ordinal comparison. The execution corpus verifies that
`a.absent = NULL` returns no rows in its fixture; do not substitute it for an unsupported
`IS NULL` predicate.

## Examples

The parser corpus includes:

```sql
MATCH (a) WHERE (a.active = TRUE AND (a.score < 10.5)) RETURN a
```

```sql
MATCH (a) WHERE a.left = a.right RETURN a
```

## See also

- **[Clauses](index.md)** — clause navigation.
- **[Operators](../expressions/operators.md)** — operator behavior.
- **[MATCH](../statements/match.md)** — binding patterns.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlProfileExecutionTests.cs`.

