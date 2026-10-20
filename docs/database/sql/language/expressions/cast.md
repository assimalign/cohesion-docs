# CAST (Cohesion SQL)

Converts a value using the engine's registered conversion, then applies the destination bounds.

> **Status:** Implemented for the pairs below.

## Syntax

```syntaxsql
CAST ( expression AS target_type )
```

The target uses the same [type names and modifiers](../data-types/index.md) as DDL, including
multi-word names and engine-resolved domains.

## Conversion pairs

Identity conversions work for all 19 shared non-null SQL types. This table lists additional
built-in conversions. Character types mean TEXT/VARCHAR and fixed CHAR; numeric types mean
TINYINT, SMALLINT, INTEGER, BIGINT, REAL, DOUBLE PRECISION and NUMERIC.

| Source | Additional targets |
| --- | --- |
| Boolean | Character types |
| Any numeric type | Every other numeric type; character types |
| TEXT/VARCHAR or CHAR | Every other shared type, including Boolean, BYTEA, temporal, UUID, JSON and JSONB |
| BYTEA | Character types |
| DATE | TIMESTAMP, TIMESTAMPTZ, character types |
| TIME | INTERVAL, character types |
| TIMESTAMP | DATE, TIME, TIMESTAMPTZ, character types |
| TIMESTAMPTZ | DATE, TIME, TIMESTAMP, character types |
| INTERVAL | TIME, character types |
| UUID | Character types |
| JSON | JSONB, character types |
| JSONB | JSON, character types |

A pair without a conversion is `COHSQLE011` (SQLSTATE 42846), rejected while planning when the
source type is known, including over empty input or a typed NULL. Parameter types are resolved
from runtime values. Untyped NULL converts to a typed NULL; target validation still applies.
Application-registered casts and domains use the same conversion path.

## Value rules

- Integer targets require a whole value in range. They never truncate a fraction. NaN or infinity
  to an integer is `COHSQLE002` (22003).
- Exact numeric text permits surrounding whitespace and `[+-]?[0-9]+(\.[0-9]+)?`; exponent text,
  grouping and a missing digit beside the decimal point are rejected. Floating targets additionally
  accept exponent notation, NaN and Infinity. Finite overflow and nonzero underflow to zero fail.
- NUMERIC(p,s) rounds to scale with midpoint ties away from zero, then checks precision. Bare
  NUMERIC requires an exact `System.Decimal` representation. Numeric range faults are `COHSQLE002`.
- Explicit character CAST truncates to the declared number of Unicode scalars; CHAR then pads with
  ASCII spaces. CHAR to TEXT strips padding. Binary bounds reject excess bytes rather than truncate.
- Temporal text uses the strict [literal grammar](literals.md). DATE to timestamp means midnight.
  TIMESTAMP is a zone-less wall clock interpreted as UTC when converting to TIMESTAMPTZ; the reverse
  uses UTC and returns `DateTimeKind.Unspecified`. TIMESTAMPTZ to DATE/TIME also uses UTC.
- TIME to INTERVAL uses time since midnight. INTERVAL to TIME requires a nonnegative duration below
  one day. Year-month interval text has no conversion to the stored day-time type.
- BYTEA text accepts a leading backslash-x with even hexadecimal digits, or plain UTF-8 text without
  backslashes. JSON and JSONB validate JSON at depth 128 and retain its text rather than reformat it;
  JSONB uses UTF-8 bytes. SQL wire scalar codecs still exclude direct JSON/JSONB components: CAST
  those results to TEXT when returning them through that codec.

Invalid value text uses `COHSQLE022` (22P02). Unknown type names use `COHSQLE010` (42704);
invalid target modifiers use `SQL0005`. CAST remains excluded from DEFAULT declarations and CHECK.

A domain check that rejects the converted value reports `COHSQLE038` (23514) through
SqlConstraintViolationException with exactly one Error diagnostic. Its message names the domain
without key values or a table, because CAST writes no table. The session attaches the statement
span when available; the error also reaches clients as ExecutionFailure with a usable connection.

## Text output and concatenation

CAST to TEXT and `||` use the same rendering: shortest round-trip floats; `yyyy-MM-dd` dates;
`HH:mm:ss` with up to seven fractional digits; a space between timestamp date and time; and an
explicit `+/-HH:mm` offset for TIMESTAMPTZ. UUIDs use lowercase hyphenated text and BYTEA uses
lowercase backslash-x hex. Intervals render day fields and a clock, such as `1 day 02:00:00`.
Boolean text is uppercase TRUE/FALSE. NULL concatenation remains NULL.

## Examples

Use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
SELECT CAST(CAST('1.25' AS DOUBLE PRECISION) AS NUMERIC),
       CAST(DATE '2024-01-01' AS TIMESTAMP),
       CAST(X'00FF' AS TEXT)
FROM t WHERE id = 1;
```

The row contains Decimal `1.25`, timestamp `2024-01-01 00:00:00`, and text `\x00ff`.

Execute separately in a fresh database:

```sql
CREATE DOMAIN positive AS INT CHECK(VALUE > 0);
CREATE TABLE domain_values (id INT);
INSERT INTO domain_values VALUES (1);
SELECT CAST(0 AS positive) FROM domain_values;
```

The SELECT reports COHSQLE038 (23514), naming `positive`; the stored row remains unchanged.

## See also

[Expressions](index.md) · [Literals](literals.md) · [Data types](../data-types/index.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Pair matrix and examples** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCastMatrixTests.cs`.
- **Conversions** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Types/SqlStandardCasts.cs`.
- **Bounds** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Types/SqlCastConverter.cs`.
- **Text output** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Types/SqlCastText.cs`.
- **Domain diagnostics** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlConstraintDiagnosticWireTests.cs`.
