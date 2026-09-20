# Literals

GQL literal values are null, Boolean, signed 64-bit integer, finite double, or string.

> **Status:** Implemented.

## Syntax

```syntaxsql
<scalar_literal> ::=
    { NULL | TRUE | FALSE | [ + | - ] integer_literal
    | [ + | - ] floating_literal | string_literal }
```

A `string_literal` is single-quoted text with embedded quotes doubled.

## Literal support

| Literal | Support | Representation |
|---|---|---|
| `NULL` | Supported | Null scalar |
| `TRUE`, `FALSE` | Supported | Boolean |
| Signed integer | Supported | Signed 64-bit integer |
| Floating point | Supported | Finite double |
| Single-quoted string | Supported | Text with doubled-quote escaping |
| List and nested map literals | Recognized, not supported (`COHDBL001`) | No collection-valued expression |

## Arguments

- **`integer_literal`** — integer digits with an optional preceding sign.
- **`floating_literal`** — floating-point text, including forms such as `.5` and `-2.5e2`.
- **`string_literal`** — for example `'a''b'`, representing `a'b`.

## Remarks

Numeric parsing uses invariant culture. Integers outside -9223372036854775808 through
9223372036854775807 produce `GQL0004`. Invalid or nonfinite double values also produce
`GQL0004`. Unterminated strings produce `GQL0003`.

Literal properties are accepted on both node and relationship patterns. An outer pattern map
provides property assignments; it does not make a nested map an accepted scalar literal.
Duplicate keys produce `GQL0006`.

## Examples

The boundary-value parser test contains:

```sql
INSERT (:P {min: -9223372036854775808, max: 9223372036854775807, text: 'a''b', n: -2.5e2})
```

The test verifies both integer boundaries, the decoded string `a'b`, and the double value -250.

## See also

- **[Expressions](index.md)** — expression navigation.
- **[Patterns](../patterns.md)** — property maps.
- **[Diagnostics](../diagnostics.md)** — literal errors.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlQueryParser.Expressions.cs`.

