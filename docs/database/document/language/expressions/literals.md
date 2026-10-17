# Literals

OQL literal expressions represent strings, decimal numbers, Boolean values, and null.

## Syntax

```syntaxsql
<literal> ::= { string_literal | decimal_literal | TRUE | FALSE | NULL | NIL | MISSING }
```

## Literal forms

| Form | Value and rules |
|---|---|
| Single-quoted text | String; doubled single quotes encode one quote |
| Decimal or scientific notation | `decimal`, parsed with invariant culture |
| `TRUE`, `FALSE` | Boolean value |
| `NULL`, `NIL` | Null value |
| `MISSING` | Absent value, distinct from NULL |

Keywords are case-insensitive. Double quotes delimit identifiers, not string values. Unary minus
and plus are operators applied to numeric expressions.

## Remarks

Numbers retain their digits as JsonElement tokens during parsing. Malformed syntax is `OQL0004`;
valid numbers that cannot fit decimal exactly, including underflow and lost fractional digits,
report `COHDOC007` (22003) during planning/evaluation. UNDEFINED remains reserved and unsupported;
[object constructors](object-constructors.md) execute.

## Examples

This conformance statement exercises every literal category, escaped quotes, and numeric notation:

```sql
SELECT TRUE, FALSE, NULL, NIL, 'it''s', .5, 1e2 FROM valueset
```

The example demonstrates parser acceptance; the engine separately validates projected column names
and other query semantics before execution.

## See also

[Expressions](index.md), [Operators](operators.md), and [Diagnostics](../diagnostics.md).

## Sources

- **Literal contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/docs/DESIGN.md`.
- **Literal parser** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryParser.Expressions.cs`.
- **Example** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlQueryParserTests.cs`.
