# Literals

OQL literal expressions represent strings, decimal numbers, Boolean values, and null.

## Syntax

```syntaxsql
<literal> ::= { string_literal | decimal_literal | TRUE | FALSE | NULL | NIL }
```

## Literal forms

| Form | Value and rules |
|---|---|
| Single-quoted text | String; doubled single quotes encode one quote |
| Decimal or scientific notation | `decimal`, parsed with invariant culture |
| `TRUE`, `FALSE` | Boolean value |
| `NULL`, `NIL` | Null value |

Keywords are case-insensitive. Double quotes delimit identifiers, not string values. Unary minus
and plus are operators applied to numeric expressions.

## Remarks

Numbers must fit the bounded `decimal` range even if JSON can represent larger numbers. Invalid or
out-of-range query numbers produce `OQL0004`; the parser does not replace them with infinity or
text. `UNDEFINED` and collection constructors are reserved but unsupported.

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
