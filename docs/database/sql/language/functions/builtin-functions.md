# Builtin functions (Cohesion SQL)

Describes the five executable scalar builtin functions and the larger lexical vocabulary.

> **Status:** Partial. The declared subset and its execution limits are documented below.

## Syntax

```syntaxsql
COALESCE ( expression [ , ...n ] )
UPPER ( expression )
LOWER ( expression )
LENGTH ( expression )
ABS ( expression )
```

## Arguments

- **`expression`** — A supported scalar value; use string inputs for string operations and
  numeric inputs for `ABS`.

## Remarks

Only the calls in the diagram are advertised as executable scalar functions. The function table in
`SqlLanguageProfile` is lexical vocabulary, not an execution guarantee or part of the 49-clause
denominator.

| Function | Evaluator behavior |
| --- | --- |
| `COALESCE` | Returns the first non-null evaluated argument; null when all arguments are null |
| `UPPER` | Applies invariant uppercase to a string; preserves null |
| `LOWER` | Applies invariant lowercase to a string; preserves null |
| `LENGTH` | Returns an `Int64` length of the invariant string representation; null for null |
| `ABS` | Returns numeric absolute value; null for null; rejects unsupported numeric inputs |

String lengths use the .NET string length, measured in UTF-16 code units. The evaluator preserves
a non-string single argument passed to `UPPER`/`LOWER`; these functions do not define a general
string-cast mechanism. Use [CAST](../expressions/cast.md) for explicit supported conversions.
The supported `ABS` runtime cases are signed integer widths, `double`, and `decimal`;
integer cases return `Int64`, while `double` and `decimal` retain their family. Overflow can error.

In grouped projections, `COALESCE` numeric alternatives receive a common numeric result type;
incompatible nonnumeric alternatives fail planning.

Recognized but unimplemented scalar names include `NULLIF`, `TRIM`, `LTRIM`, `RTRIM`,
`SUBSTRING`, `REPLACE`, `CONCAT`, `CEILING`, `FLOOR`, `ROUND`, `POWER`,
`SQRT`, `MOD`, `NOW`, `CURRENT_DATE`, `CURRENT_TIME`, `CURRENT_TIMESTAMP`,
and `EXTRACT`. A parsed call can fail in planning or evaluation; it does not inherit support from
`SELECT`. Unsupported scalar calls have no promised dedicated language diagnostic.
Window-function calls and clauses are explicitly rejected with `COHDBL001`.

## Examples

This minimally adapted test expression uses the
[conformance fixture](../statements/select.md#a-create-the-conformance-fixture):

```sql
SELECT ABS(CAST('-12' AS SMALLINT)) FROM t WHERE id = 1;
```

It returns `12` as `Int64`.

## See also

[Functions](index.md) · [Aggregates](aggregate-functions.md) · [Unsupported syntax](../unsupported.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/SqlExpressionEvaluator.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCastTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlLanguageProfile.cs`

