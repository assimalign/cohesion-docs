# Literals and identifiers (Cohesion SQL)

Describes literal values and delimited names in the SQL source text.

> **Status:** Partial. This page describes the executable subset and its boundaries.

## Syntax

```syntaxsql
'string_text'
integer_digits
fractional_or_exponent_number
TRUE
FALSE
NULL
"identifier_text"
```

## Arguments

- **`string_text`** — Text in single quotes; double a single quote to include it.
- **`integer_digits`** — An integer literal.
- **`fractional_or_exponent_number`** — Decimal-point or exponent syntax within exact Decimal bounds.
- **`identifier_text`** — A delimited name, not a string value.

## Remarks

| Source form | Example | Parser literal kind | Evaluated value |
| --- | --- | --- | --- |
| String | `'it''s'` | `String` | Unescaped string |
| Integer | `42` | `Integer` | `Int64` |
| Fractional/exponent | `3.14`, `.5`, `1e10` | `Float` | Exact `Decimal` |
| Boolean | `TRUE`, `FALSE` | `Boolean` | Boolean |
| Null | `NULL` | `Null` | Null |

The parser's `Float` label does not promise floating-point runtime values. Fractional literals
must fit `System.Decimal` exactly; underflow or rounding is rejected. Insignificant zeros may
normalize. There are no binary, NaN, or infinity literal forms in this contract; use parameters
or stored columns for those values.

Double quotes delimit identifiers, including reserved words, spaces, and literal dots. The parser
removes delimiters for catalog binding while retaining identifier token classification.
`"dbo"."order details"` qualifies a table; `"order.details"` names one table containing a dot.
Embedded double quotes inside a name are outside the lexer subset.

Delimited names apply to targets, columns, aliases, indexes, and constraints. Single quotes always
denote values. Constant projections still require a `FROM` relation to execute.

## Examples

Use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
SELECT 'it''s', 42, .5, TRUE, NULL FROM t WHERE id = 1;
```

This adapts the dialect's literal forms to one row of the conformance fixture.

## See also

[Expressions](index.md) · [SELECT](../statements/select.md) · [Data types](../data-types/index.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlConformanceTests.cs`

