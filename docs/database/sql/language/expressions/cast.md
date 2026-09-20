# CAST (Cohesion SQL)

Converts an executable value to a supported exact numeric, Boolean, or string type.

> **Status:** Partial. This page describes the executable subset and its boundaries.

## Syntax

```syntaxsql
CAST ( expression AS <target_type> )

<target_type> ::= {
    BOOLEAN | BOOL
  | TINYINT | SMALLINT | INT2 | INT | INTEGER | INT4 | BIGINT | INT8
  | DECIMAL [ ( precision [ , scale ] ) ]
  | NUMERIC [ ( precision [ , scale ] ) ]
  | CHAR [ ( length ) ] | CHARACTER [ ( length ) ]
  | VARCHAR [ ( length ) ] | TEXT [ ( length ) ]
}
```

## Arguments

- **`expression`** — The evaluated source value, including a column, parameter, or literal.
- **`precision`** — An integer from 1 through 28.
- **`scale`** — An integer from zero through precision; omitted scale is zero.
- **`length`** — A positive `Int32` count of UTF-16 code units.

## Remarks

This subset performs actual conversions with matching result values and base-type metadata,
including through the SQL server/client. Target names resolve through `SqlTypeNames` and
`DatabaseTypeInfo`.

| Evaluated non-null source | Allowed target families |
| --- | --- |
| Signed `sbyte`, `short`, `int`, `long`, or `decimal` | Any signed integer width, `Decimal`, `String` |
| `String` | Any signed integer width, `Decimal`, `Boolean`, `String` |
| `Boolean` | `Boolean`, `String` |

All other pairs are rejected. Floating-point values, unsigned integers, binary values, temporal
values, globally unique identifiers, and JavaScript Object Notation values are not supported CAST
families. Numeric-to-Boolean and Boolean-to-numeric conversions are excluded.

| Conversion rule | Behavior |
| --- | --- |
| Numeric text | Trim surrounding whitespace; require `[+-]?[0-9]+(\.[0-9]+)?`; reject grouping, exponent text, empty or invalid text |
| Boolean text | Trim whitespace; accept case-insensitive `TRUE` or `FALSE` only |
| Numeric or Boolean to string | Invariant numeric formatting or uppercase `TRUE`/`FALSE` |
| String to string | Preserve original text, including whitespace |
| Null source | Return typed `NULL` for supported targets; target validation still runs |
| Signed integer target | Reject overflow or nonzero fractional parts; no truncation, rounding, wrapping, or zero fallback |
| Decimal target | Use `System.Decimal`, with 96-bit coefficient and scale 0–28 |
| String target | Reject excess length; no truncation or fixed-character padding |

Integer ranges are -128 through 127, -32768 through 32767, -2147483648 through 2147483647, and
-9223372036854775808 through 9223372036854775807 for the four increasing widths.
Identity conversions also enforce bounds.

Bare `DECIMAL`/`NUMERIC` imposes no additional bound. `DECIMAL(p)` means scale zero.
For `DECIMAL(p,s)`, require `abs(value) < 10^(p-s)` with no nonzero digits beyond scale `s`.
Trailing fractional zeros can be discarded without changing value; excess precision or scale errors.
Output need not be padded to the declared scale. Numeric text must fit `System.Decimal` exactly
before narrowing and is never silently rounded.

SQL integer literals evaluate as `Int64`; fractional/exponent literals evaluate as exact
`Decimal`. Parameters and stored columns retain their runtime types. Bare string aliases have no
CAST length limit. Integer and Boolean targets accept no parameters.

Unknown target names produce `SQL0004`; recognized unsupported targets or invalid parameters
produce `SQL0005`; malformed syntax produces `SQL0003`. Conversion failures throw
`DatabaseException` naming CAST and its target and surface as query errors over the wire.

In-process result columns expose `DatabaseType` and conservative nullability. Wire/client metadata
exposes the same base type, but no nullability, length, precision, or scale fields.
CAST in `DEFAULT` and `CHECK` is rejected. In inserts and updates, conversion precedes
destination-column storage coercion.

## Examples

Use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
SELECT CAST('42' AS INT), CAST(age AS TEXT) FROM t ORDER BY id;
```

The fixture returns `(42, "36")`, `(42, "45")`, and `(42, "41")`, with `Int32` and `String` columns.

## See also

[Expressions](index.md) · [SELECT](../statements/select.md) · [Data types](../data-types/index.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCastTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.Cast.cs`
