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
| Integer | `42` | `Integer` | `Int32` when it fits, otherwise `Int64` |
| Fractional/exponent | `3.14`, `.5`, `1e10` | `Float` | Exact `Decimal` |
| Boolean | `TRUE`, `FALSE` | `Boolean` | Boolean |
| Null | `NULL` | `Null` | Null |

The parser's `Float` label does not promise floating-point runtime values. Fractional literals
must fit `System.Decimal` exactly; underflow or rounding is rejected. Insignificant zeros may
normalize. NaN and infinity have no literal forms; bind them as parameters or use CAST from text.

Double quotes delimit identifiers, including reserved words, spaces, and literal dots. The parser
removes delimiters for catalog binding while retaining identifier token classification.
`"dbo"."order details"` qualifies a table; `"order.details"` names one table containing a dot.
Doubled double quotes represent one embedded quote: `"a""b"` names `a"b`.
The same rule applies to function names and canonical rendering. A final escaped quote without
a closing delimiter, as in `"a""`, reports `SQL0003`. Whitespace separates delimited tokens:
`"a""x"` is one name, while `"a" "x"` is a column followed by its alias.
See the [stored-view compatibility note](../statements/create-view.md#delimited-identifiers).

Delimited names apply to targets, columns, aliases, indexes, and constraints. Single quotes always
denote values. Constant projections execute over one implicit row when FROM is absent.
In Boolean positions, valid Boolean input string literals receive the
[search-condition coercion](../clauses/where.md#boolean-search-conditions); other text retains its type.

## Typed literals

| Form | Examples | AST literal type |
|---|---|---|
| National string | `N'héllo'` (adjacent prefix; canonical output `'héllo'`) | `String` |
| String | `'it''s'` (doubled-quote escape) | `String` |
| Integer | `42` | `Integer` |
| Float | `3.14`, `.5`, `1e10` | `Float` |
| Date | `DATE '2024-01-01'`; exactly yyyy-MM-dd | `Date` (DateOnly) |
| Time | `TIME [WITHOUT TIME ZONE] '10:30[:ss[.fffffff]]'`; canonical `TIME '10:30:00'` | `Time` (TimeOnly) |
| Timestamp | `TIMESTAMP [WITHOUT TIME ZONE] '2024-01-01[ T10:30[:ss[.fffffff]]]'`; one space or T separates date and time; canonical `TIMESTAMP '2024-01-01 10:30:00'` | `Timestamp` (DateTime) |
| Timestamp with zone | `TIMESTAMPTZ '2024-01-01 10:30:00[Z or +/-HH:mm]'`, or `TIMESTAMP WITH TIME ZONE`; canonical seven fractional digits and explicit offset | `TimestampWithTimeZone` (DateTimeOffset) |
| Day-time interval | `INTERVAL '3' DAY`, `INTERVAL '1:30' HOUR TO MINUTE`, `INTERVAL '2 days 3 hours'`, `INTERVAL 'P1DT2H'`; canonical `INTERVAL 'PT93600S'` for the last example | `Interval` (TimeSpan) |
| Binary string | `X''`, `X'00ff'`, `x'ABCD'`; adjacent prefix, even-count hexadecimal digits; canonical `X'00FF'` | `Binary` (byte[]) |
| Boolean | `TRUE`, `FALSE` | `Boolean` |
| Null | `NULL` | `Null` |


Typed literals work in scalar positions and literal DEFAULTs. X and N must touch the quote.
Malformed contents report `SQL0008` over the complete literal. Years have four digits;
TIME accepts single-digit clock fields, AM/PM, a trailing fractional dot and a validated written
zone that is discarded. TIMESTAMP rejects zones; both types allow at most seven fractional digits.
TIMESTAMPTZ accepts Z or +/-HH:mm within +/-14:00; no offset means UTC. Regional named zones and
special dates are excluded. See [TIME input and the bounded zone vocabulary](../data-types/index.md#temporal-text-input).

INTERVAL stores an exact day-time TimeSpan at 100 ns resolution. DAY/HOUR/MINUTE/SECOND and
descending TO ranges, PostgreSQL unit/clock strings and ISO day-time durations execute.
An unqualified bare signed number means seconds: `INTERVAL '12'` is twelve seconds.
`INTERVAL '1 2 hours'` is 26 hours; `INTERVAL '@ 1 day 2 hours ago'` is minus 26 hours.
Surrounding ASCII whitespace is trimmed; TIME/INTERVAL reject non-ASCII whitespace.
Year-month fields report `COHDBL001`; invalid or overflowing durations report `SQL0008`.
TIME WITH TIME ZONE literals and temporal precision remain `COHDBL001`.

Binary row values are independent snapshots. Mutating a returned byte array or GetBytes backing
array cannot change another cell, later row, stored value, parameter or subsequent default insert.
Shared binary values are copied when supplied to a row; fresh stored payloads transfer directly.

```sql
SELECT DATE '2024-01-01', TIME '10:30', INTERVAL '1:30' HOUR TO MINUTE,
       X'00ff', N'héllo' FROM t WHERE id = 1;
```

On the [fixture](../statements/select.md#a-create-the-conformance-fixture), the single row contains
DateOnly 2024-01-01, TimeOnly 10:30:00, a 90-minute TimeSpan, bytes 00 FF and text héllo.

## Canonical identifier text

The renderer quotes identifiers spelled like recognized vocabulary, including SEARCH/MATERIALIZED,
newly executable function names, BOTH/LEADING/TRAILING and PERCENT/TIES. A column named sign renders as
`"sign" >= 0` in a CHECK. Defaults and SQL function bodies use the same renderer. Format 18
records this canonical quoting change, separately from format 17 (the insert-default/frozen-missing
split with table extension 6). Format 19 added PERCENT/TIES quoting. Format 20 added
persisted NO ACTION, SET NULL and SET DEFAULT actions. Format 21 introduced namespace records of kind 15. Format 22 quotes STRING_AGG and LISTAGG
identifiers in persisted SQL. Format 23 introduced persisted views and format 24 explicit NULL
placement. Format 25 added distinct predicates, Boolean tests and LIKE ESCAPE.
Current format 26 fences temporal DEFAULT and frozen missing-value text. Every earlier format is refused at open
with no upgrade path; canonical CHECK text is compared ordinally during schema verification.
Compiled schemas stay version 8.

## Examples

Use the [conformance fixture](../statements/select.md#a-create-the-conformance-fixture).

```sql
SELECT 'it''s', 42, .5, TRUE, NULL FROM t WHERE id = 1;
```

This adapts the dialect's literal forms to one row of the conformance fixture.

Execute separately in an empty database:

```sql
CREATE TABLE "t""x" ("a""b" INT);
INSERT INTO "t""x" ("a""b") VALUES (7);
SELECT "a""b" AS "c""d" FROM "t""x";
```

The query returns 7 in a column named `c"d`. Quotes delimit names; they do not change NULL
semantics, and SQL has no MISSING value.

## See also

[Expressions](index.md) · [SELECT](../statements/select.md) · [Data types](../data-types/index.md)

## Sources

- **Temporal forms** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlTypedLiteralTests.cs` and `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTemporalAssignmentConformanceTests.cs`.
- **Function-name quoting and format 18** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlT1StorageFormatTests.cs`.
- **Canonical names** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCteStorageFormatTests.cs`.
- **Embedded quotes** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs` and `Assimalign.Cohesion.Database.Sql.Language/tests/SqlQuotedIdentifierTests.cs`.

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlConformanceTests.cs`
