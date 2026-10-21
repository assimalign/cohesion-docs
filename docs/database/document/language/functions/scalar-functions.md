# Scalar functions

OQL executes 34 SQL++ scalar functions. Names are case-insensitive and arguments are ordinary
expressions in projections, filters, grouping, ordering, aggregate arguments and UPDATE values.

## NULL, MISSING and argument types

Except for TYPE, the conditional functions and the zero-argument clocks, calls propagate
[MISSING](../expressions/missing.md) before NULL. A wrong-kind stored value returns NULL;
an incompatible literal or constructor fails planning with COHDOC001, even on an empty collection.
For example, LOWER(amount) returns NULL for a numeric field, while LOWER(1) fails planning.
IS_* returns false for another known kind, but propagates NULL and MISSING.

The four conditional functions evaluate only through the selected argument. Planning still
validates every branch. If no argument qualifies, they return NULL.

## Function reference

In these examples, `items` contains one document
`{"name":"Alpha","amount":2,"tags":["blue","green"]}`. Run each expression as
`SELECT expression AS value FROM items;`. Strings in the result column below are JSON strings.

| Syntax | Behavior | Example expression → result |
| --- | --- | --- |
| LOWER(text) | Invariant Unicode lowercase | `LOWER(name)` → `"alpha"` |
| UPPER(text) | Invariant Unicode uppercase | `UPPER(name)` → `"ALPHA"` |
| LENGTH(text) | Unicode code-point count, Int64 | `LENGTH('😀a')` → `2` |
| SUBSTR(text, start [, length]) | Zero-based code-point slice; negative start counts from the end and clamps at zero; omitted length means the remainder | `SUBSTR('😀ab',1,1)` → `"a"` |
| POSITION(text, search) | Zero-based code-point position; -1 if absent, 0 for an empty search | `POSITION('😀ab','a')` → `1` |
| CONTAINS(text, search) | Ordinal, case-sensitive containment | `CONTAINS('Alpha','a')` → `true` |
| STARTS_WITH(text, prefix) | Ordinal prefix test | `STARTS_WITH('Alpha','a')` → `false` |
| ENDS_WITH(text, suffix) | Ordinal suffix test | `ENDS_WITH('Alpha','a')` → `true` |
| REPLACE(text, search, replacement [, count]) | Replaces nonoverlapping matches left to right; omitted/negative count means all, zero means none; empty search leaves text unchanged | `REPLACE('abcabc','b','X',1)` → `"aXcabc"` |
| TRIM(text [, characters]) | Removes any supplied code points from both ends; default is space, tab, LF, form feed and CR | `TRIM('😀abc😀','😀')` → `"abc"` |
| CONCAT(text, text [, ...]) | Concatenates two or more strings | `CONCAT(name,' ',name)` → `"Alpha Alpha"` |
| ABS(number) | Absolute Decimal value | `ABS(-2)` → `2` |
| ROUND(number [, scale]) | Exact Decimal rounding, ties away from zero; default scale 0; negative scales round left of the decimal point | `ROUND(125,-1)` → `130` |
| TRUNC(number [, scale]) | Decimal truncation toward zero; default scale 0; negative scales allowed | `TRUNC(-2.567,2)` → `-2.56` |
| FLOOR(number) | Greatest integral Decimal no greater than the value | `FLOOR(-2.5)` → `-3` |
| CEIL(number) | Least integral Decimal no less than the value | `CEIL(-2.5)` → `-2` |
| POWER(base, exponent) | Decimal arithmetic for integral exponents; double calculation for fractional exponents | `POWER(2,-2)` → `0.25` |
| SQRT(number) | Double calculation converted through its shortest round-trip representation to Decimal | `SQRT(4)` → `2` |
| NOW_STR() | Transaction-start UTC instant as `yyyy-MM-ddTHH:mm:ss.fffZ` | `NOW_STR() = NOW_STR()` → `true` |
| NOW_MILLIS() | Same transaction-start instant in Int64 Unix milliseconds | `NOW_MILLIS() = NOW_MILLIS()` → `true` |
| STR_TO_MILLIS(text) | Parses an ISO date/time to Int64 Unix milliseconds; invalid input gives NULL | `STR_TO_MILLIS('1970-01-01T01:00:00+01:00')` → `0` |
| MILLIS_TO_STR(number) | UTC string in the NOW_STR format; truncates fractional milliseconds; out-of-range dates give NULL | `MILLIS_TO_STR(0)` → `"1970-01-01T00:00:00.000Z"` |
| TYPE(value) | Returns `missing`, `null`, `boolean`, `number`, `string`, `array` or `object` | `TYPE(absent)` → `"missing"` |
| IS_ARRAY(value) | Tests a known value's kind | `IS_ARRAY(tags)` → `true` |
| IS_OBJECT(value) | Tests a known value's kind | `IS_OBJECT({x:1})` → `true` |
| IS_STRING(value) | Tests a known value's kind | `IS_STRING(name)` → `true` |
| IS_NUMBER(value) | Tests a known value's kind | `IS_NUMBER(amount)` → `true` |
| IS_BOOLEAN(value) | Tests a known value's kind | `IS_BOOLEAN(TRUE)` → `true` |
| ARRAY_LENGTH(array) | Int64 element count; wrong-kind stored input gives NULL | `ARRAY_LENGTH(tags)` → `2` |
| ARRAY_CONTAINS(array, value) | Structural membership; wrong-kind stored array input gives NULL | `ARRAY_CONTAINS(tags,'blue')` → `true` |
| COALESCE(value, value [, ...]) | First value neither NULL nor MISSING | `COALESCE(absent,NULL,1)` → `1` |
| IFMISSING(value, value [, ...]) | First non-MISSING value, including NULL | `IFMISSING(NULL,'x')` → `null` |
| IFNULL(value, value [, ...]) | First non-NULL value, including MISSING | `IFNULL(absent,0) IS MISSING` → `true` |
| IFMISSINGORNULL(value, value [, ...]) | Same selection rule as COALESCE | `IFMISSINGORNULL(absent,NULL,1)` → `1` |

SUBSTR returns NULL for fractional starts/lengths or negative lengths. TRIM's default set excludes
vertical tab and nonbreaking space. Constructed strings and work buffers are limited to 1,048,576
UTF-16 units; inputs are not capped. Indexing and LENGTH count code points rather than UTF-16 units.

ROUND uses the original exact digits once, including at negative scales:
`ROUND(4.9999999999999999999999999999,-1)` returns `0`. Integral scales of 28 or greater leave
the value unchanged. Very negative scales return zero unless rounding would overflow Decimal.

STR_TO_MILLIS accepts `yyyy-MM-dd`, or that date followed by `T` or space and `HH:mm:ss`, with
optional 1–7 fractional digits and optional `Z` or numeric offset. A missing zone means UTC.
NOW_STR and NOW_MILLIS share one instant across all rows and statements of an explicit transaction;
autocommit captures one instant per statement. Output dates use UTC `Z`.

COUNT, LENGTH, POSITION, ARRAY_LENGTH and the epoch-millisecond functions expose Int64 metadata
and CLR long values, including over the wire and for empty grouped results. Other numeric scalars
return Decimal. Mixed Int64/Decimal columns promote to Decimal; numeric cells in heterogeneous
columns also normalize to Decimal so embedded and client values agree.

Calls are not planner constants or index seek bounds. INSERT/UPSERT VALUES does not accept them;
use a SELECT source when computing replacement values.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| OQL0008 | Unknown function name, such as LOWER2; located at the name after arguments are parsed | — |
| OQL0006 | Invalid argument count or star operand; only COUNT accepts star | — |
| COHDBL001 | Deferred calls such as ARRAY_AGG and REGEXP_LIKE/REGEXP_REPLACE/REGEXP_POSITION | — |
| COHDOC001 | Incompatible literal kind, invalid function context or grouping | — |
| COHDOC007 | Decimal overflow or constructed string/work-buffer limit | 22003 |
| COHDOC008 | Fractional REPLACE count | — |
| COHDOC019 | Negative SQRT, invalid POWER domain (zero to negative power or negative base to fractional power), or fractional ROUND/TRUNC scale | — |

For example, `SELECT SQRT(-1) FROM items;` fails with COHDOC019. Function evaluation failures
abort an explicit transaction; roll it back before further work. Ordinary NULL/MISSING results
are values rather than errors. See [Diagnostics](../diagnostics.md).

## See also

[Functions](index.md) · [Aggregates](aggregate-functions.md) · [MISSING](../expressions/missing.md) · [OqlFunctions .NET API](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents-language/oql-functions.md)

## Sources

- **Function table** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlFunctions.cs`.
- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/Evaluation/DocumentExpressionEvaluator.Functions.cs`.
- **Examples** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentQueryTests.Functions.cs`.
- **Wire types** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentClientWireTests.cs`.
