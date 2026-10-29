# Builtin functions (Cohesion SQL)

Executes 42 T1 scalar names with planning-time signature checks.

> **Status:** Implemented. Supports the signatures below.

## Syntax

```syntaxsql
function_name ( argument [ , ...n ] )
COALESCE ( expression [ , ...n ] )
NULLIF ( first_expression , second_expression )
TRIM ( [ { LEADING | TRAILING | BOTH } ] [ characters ] FROM text )
SUBSTRING ( text FROM start [ FOR length ] )
POSITION ( substring IN text )
EXTRACT ( field FROM temporal )
CURRENT_DATE | CURRENT_TIME | CURRENT_TIMESTAMP | LOCALTIME | LOCALTIMESTAMP
```

Ordinary comma-call signatures are listed below. The five bare clock forms have no parentheses;
NOW() and GEN_RANDOM_UUID() require calls. SUBSTRING and TRIM also accept their comma forms.
Directional TRIM normalizes to LTRIM or RTRIM. COALESCE, NULLIF and EXTRACT are evaluator forms,
alongside [CASE](../expressions/case.md) and [CAST](../expressions/cast.md). They do not use the
function catalog calling convention.

## Arguments and NULL behavior

- **`text`** — TEXT or CHAR; CHAR-to-TEXT removes trailing ASCII spaces. String functions do not
  implicitly stringify numbers. Use CAST, or CONCAT where conversion is part of its contract.
- **`integer`** — A signed integer width. Positions/counts are Unicode scalar counts.
- **`numeric`** — Signed integer, REAL, DOUBLE PRECISION or DECIMAL.
- **`temporal`** — DATE, TIME, TIMESTAMP, TIMESTAMPTZ or day-time INTERVAL, subject to field rules.

Unless a row below states otherwise, functions are Immutable and strict: any NULL argument returns
NULL. SQL has no separate MISSING state. Arity and incompatible argument types fail planning with
CS01006 (42883), including empty inputs. Messages list accepted signatures. No-overload and
ambiguous-overload diagnostics remain distinct (CS01006 and CS01008).

COALESCE evaluates alternatives only until the first non-NULL and returns NULL if none exists.
In a [Boolean position](../clauses/where.md#boolean-search-conditions), every alternative must
qualify, including unreachable ones; mixed Boolean/non-Boolean alternatives fail CS02003 (42804).
Compatible alternatives receive a common result type, including numeric and calendar promotion.
NULLIF evaluates its operands, returns NULL when equal, otherwise preserves the first value and
its type; a NULL first operand stays NULL, and a NULL second operand leaves the first unchanged.
It follows ordinary comparison/collation rules, including CHAR trailing-space normalization.

COALESCE retains length, precision, scale and domain metadata only when every argument agrees
in type, domain and modifiers; otherwise the common type is unconstrained. NULLIF retains its
first argument's length, precision and scale. [Persisted views](../statements/create-view.md) freeze
these facets in their column metadata.

CONCAT skips NULL arguments; all NULLs gives empty text. CONCAT_WS returns NULL for a NULL separator
and skips NULL value arguments. Both format other values using CAST-to-TEXT rules (Booleans use
TRUE/FALSE). The clock forms and GEN_RANDOM_UUID are never NULL.

## Examples and signatures

Execute these setup statements separately. Each example expression in the tables can be run as
`SELECT expression FROM scalar_values;` and produces the indicated single value. This fixture is
adapted from SqlT1ScalarTests.

```sql
CREATE TABLE scalar_values (s TEXT, i BIGINT, n DECIMAL, d DATE);
INSERT INTO scalar_values VALUES ('A😀b', -2, 2.5, DATE '2024-03-04');
```

### Conditional and string functions

| Signature | Result type and behavior | Example expression → result |
| --- | --- | --- |
| `COALESCE(value [, value ...])` | Common alternative type; first non-NULL, lazy | `COALESCE(NULL, s, 'fallback')` → `'A😀b'` |
| `NULLIF(a, b)` | First operand type; NULL when equal | `NULLIF(s, s)` → NULL |
| `UPPER(text)` | TEXT; invariant uppercase | `UPPER(s)` → `'A😀B'` |
| `LOWER(text)` | TEXT; invariant lowercase | `LOWER(s)` → `'a😀b'` |
| `LENGTH(text)` | BIGINT; Unicode scalar length | `LENGTH(s)` → 3 |
| `CHAR_LENGTH(text)` | BIGINT; Unicode scalar length | `CHAR_LENGTH(s)` → 3 |
| `CHARACTER_LENGTH(text)` | BIGINT; Unicode scalar length | `CHARACTER_LENGTH(s)` → 3 |
| `TRIM(text [, chars])`; standard form above | TEXT; removes edge characters from a set, ASCII space by default | `TRIM(BOTH 'A' FROM s)` → `'😀b'` |
| `LTRIM(text [, chars])` | TEXT; removes leading characters in the set | `LTRIM(s, 'A')` → `'😀b'` |
| `RTRIM(text [, chars])` | TEXT; removes trailing characters in the set | `RTRIM(s, 'b')` → `'A😀'` |
| `SUBSTRING(text, start [, length])`; standard form above | TEXT; one-based start, optional scalar length | `SUBSTRING(s FROM 2 FOR 1)` → `'😀'` |
| `REPLACE(text, from_text, to_text)` | TEXT; replaces occurrences; empty search preserves input | `REPLACE(s, '😀', 'x')` → `'Axb'` |
| `CONCAT(value [, value ...])` | TEXT; skips NULLs, all NULLs gives empty text | `CONCAT(s, NULL, '!')` → `'A😀b!'` |
| `CONCAT_WS(separator, value [, value ...])` | TEXT; NULL separator gives NULL; skips other NULLs | `CONCAT_WS('-', s, NULL, '!')` → `'A😀b-!'` |
| `POSITION(sub IN text)` | BIGINT; one-based scalar position, zero if absent | `POSITION('b' IN s)` → 3 |
| `STRPOS(text, sub)` | BIGINT; POSITION with reversed operands | `STRPOS(s, '😀')` → 2 |
| `LEFT(text, count)` | TEXT; negative count omits trailing scalars | `LEFT(s, 2)` → `'A😀'` |
| `RIGHT(text, count)` | TEXT; negative count omits leading scalars | `RIGHT(s, 2)` → `'😀b'` |
| `LPAD(text, length [, fill])` | TEXT; pads left or truncates; default fill is space | `LPAD(s, 5, '😀')` → `'😀😀A😀b'` |
| `RPAD(text, length [, fill])` | TEXT; pads right or truncates; default fill is space | `RPAD(s, 5, '😀')` → `'A😀b😀😀'` |
| `REVERSE(text)` | TEXT; reverses Unicode scalars | `REVERSE(s)` → `'b😀A'` |

Searches and trim character sets use ordinal Unicode scalar matching. LENGTH/CHAR_LENGTH/
CHARACTER_LENGTH, SUBSTRING, POSITION/STRPOS, LEFT/RIGHT, LPAD/RPAD and REVERSE never split a
surrogate pair. SUBSTRING uses the intersection with the string when start is before 1; a negative
length is CS02002. LPAD/RPAD with nonpositive length returns empty text; an empty fill leaves
the input unchanged when padding would otherwise be needed. UPPER/LOWER reject non-text inputs;
LENGTH also rejects numbers instead of formatting them.

### Numeric functions

Integer numeric results widen to BIGINT unless stated otherwise. DECIMAL and approximate inputs
retain their family for ABS/ROUND/FLOOR/CEILING/CEIL. Two-argument ROUND accepts exact numeric
inputs only; scale can be negative. Exact midpoint rounding is away from zero; approximate
midpoints round to even.

Negative-scale exact ROUND uses the original digits once: after creating `t(id INT)` and inserting
one row, `SELECT ROUND(4.9999999999999999999999999999,-1), ROUND(5.0,-1) FROM t;`
returns `(0, 10)`. The implementation compares the exact remainder rather than first rounding
a division result. The same rule applies to both signs and every supported negative scale (#1661).

| Signature | Result type and behavior | Example expression → result |
| --- | --- | --- |
| `ABS(numeric)` | Numeric absolute value; BIGINT minimum overflows | `ABS(i)` → 2 (BIGINT) |
| `ROUND(numeric)`; `ROUND(exact_numeric, integer_scale)` | Numeric result, with rules above | `ROUND(n)` → 3 (DECIMAL) |
| `FLOOR(numeric)` | Numeric floor | `FLOOR(n)` → 2 (DECIMAL) |
| `CEILING(numeric)` | Numeric ceiling | `CEILING(n)` → 3 (DECIMAL) |
| `CEIL(numeric)` | CEILING alias | `CEIL(n)` → 3 (DECIMAL) |
| `POWER(base, exponent)` | DECIMAL for exact base/integer exponent; approximate base type for integer exponent; otherwise DOUBLE PRECISION | `POWER(n, 2)` → 6.25 (DECIMAL) |
| `SQRT(numeric)` | DOUBLE PRECISION; nonnegative input | `SQRT(4)` → 2.0 |
| `MOD(numeric, numeric)` | BIGINT for integers, DECIMAL for exact decimal, DOUBLE PRECISION for approximate | `MOD(i, 2)` → 0 |
| `SIGN(numeric)` | BIGINT for every numeric input | `SIGN(i)` → -1 |
| `EXP(numeric)` | DOUBLE PRECISION | `EXP(0)` → 1.0 |
| `LN(numeric)` | DOUBLE PRECISION; positive input | `LN(1)` → 0.0 |
| `LOG10(numeric)` | DOUBLE PRECISION; positive input | `LOG10(100)` → 2.0 |

POWER chooses its result from operand types: a DECIMAL/REAL/DOUBLE exponent gives DOUBLE PRECISION
even if its value is integral. SQRT/EXP/LN/LOG10 always return DOUBLE PRECISION, and SIGN always
returns BIGINT. MOD by zero is CS02001 (22012). Domain/range errors, non-finite results from
finite inputs and overflow are CS02002 (22003), including SQRT(-1), LN(0), POWER(0, -1)
and EXP(1000).

### Clock, temporal and UUID functions

| Signature | Result type and behavior | Example expression → result |
| --- | --- | --- |
| `CURRENT_DATE` | DATE; Stable UTC transaction date | `CURRENT_DATE` → UTC date of transaction start |
| `CURRENT_TIME` | TIME; Stable UTC transaction clock | `CURRENT_TIME` → UTC time of transaction start |
| `CURRENT_TIMESTAMP` | TIMESTAMPTZ at +00:00; Stable | `CURRENT_TIMESTAMP` → transaction start instant |
| `LOCALTIME` | TIME; Stable UTC wall clock | `LOCALTIME` → same value as CURRENT_TIME |
| `LOCALTIMESTAMP` | TIMESTAMP; Stable UTC wall clock, Unspecified kind | `LOCALTIMESTAMP` → UTC transaction date/time without offset |
| `NOW()` | TIMESTAMPTZ at +00:00; Stable CURRENT_TIMESTAMP alias | `NOW() = CURRENT_TIMESTAMP` → TRUE |
| `EXTRACT(field FROM temporal)` | DECIMAL; Immutable, strict | `EXTRACT(YEAR FROM d)` → 2024 |
| `DATE_TRUNC(field_text, temporal)` | Input temporal type; Immutable, strict | `DATE_TRUNC('month', d)` → DATE 2024-03-01 |
| `GEN_RANDOM_UUID()` | UUID version 4; Volatile, new per call | `GEN_RANDOM_UUID()` → a new Guid |

All six clock forms use the UTC instant captured at BEGIN or BeginTransactionAsync. Without an
explicit transaction, it is captured when the automatic statement transaction begins. Seek-bound
planning, rows, subqueries, SQL function bodies and later statements in that transaction agree.
COMMIT/ROLLBACK ends that clock lifetime. There is no session time zone or public clock option.

Empty parentheses on a bare clock form give CS00003. A precision argument on CURRENT_TIME,
CURRENT_TIMESTAMP, LOCALTIME or LOCALTIMESTAMP gives CK00001. NOW is written with parentheses.

EXTRACT fields are case-insensitive: YEAR, QUARTER, MONTH, WEEK (ISO week), DAY, DOW (Sunday zero),
DOY, HOUR, MINUTE, SECOND (including fractions) and EPOCH. DATE accepts calendar fields and EPOCH;
TIME accepts HOUR/MINUTE/SECOND/EPOCH; INTERVAL also accepts DAY. TIMESTAMP and TIMESTAMPTZ accept
all these fields; TIMESTAMPTZ uses UTC. Unknown grammar fields give CS00003; incompatible constant
fields fail planning with CS01006, even on empty inputs. INTERVAL uses normalized TimeSpan
components: `EXTRACT(HOUR FROM INTERVAL '36' HOUR)` is 12 and DAY is 1. EPOCH is the full duration
in seconds. This differs from PostgreSQL interval component storage.

DATE_TRUNC accepts year/quarter/month/week/day/hour/minute/second/millisecond/microsecond;
TIME and INTERVAL accept day and finer units. Weeks start Monday. Field values are case-insensitive;
invalid/incompatible fields give CS01006. Constant and directly bound parameter fields are checked during planning;
row-dependent fields are checked during evaluation. TIMESTAMPTZ truncation uses UTC and returns +00:00. DATE stays
DATE. Negative INTERVAL truncation removes fractional units toward zero. See
[temporal arithmetic](../expressions/operators.md#temporal-arithmetic) for day-time INTERVAL operators.

For clock agreement across requests, execute each statement separately in one session:

```sql
BEGIN;
SELECT CURRENT_TIMESTAMP, NOW(), CURRENT_TIME = LOCALTIME FROM scalar_values;
SELECT CURRENT_TIMESTAMP, NOW() FROM scalar_values;
ROLLBACK;
```

All timestamp columns contain the same captured instant, and the equality is TRUE.

Application functions receive this same captured instant through
[SqlFunctionContext.TransactionTimestamp](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/sql-function-context.md).

## Limits, defaults and diagnostics

String-producing scalars cap both output and Unicode work buffers at **1,048,576 UTF-16 units**.
This allocation bound is distinct from Unicode scalar positions and SQL character-type lengths.
LPAD/RPAD conservatively budget two UTF-16 units per requested scalar. REPLACE checks expanded
length, and CONCAT/CONCAT_WS check binary-to-hex expansion and joined length before allocation.
Slicing, trimming and reversal also bound input work buffers; UPPER/LOWER use the same cap.
LENGTH/CHAR_LENGTH count without allocating a rune buffer. Oversized requests use CS02002
(22003), and long loops observe statement cancellation.

Column DEFAULT expressions admit Stable/Volatile calls and evaluate them per assigned row with
this transaction clock. Immutable defaults fold at declaration. ADD COLUMN freezes a nonvolatile
default once and refuses a volatile default on live rows (CS03027). CHECK still admits only
Immutable calls. Volatile calls never supply seek bounds. See [DEFAULT](../clauses/default.md).

| Code | Meaning and trigger | SQLSTATE where supplied |
| --- | --- | --- |
| `CS00003` | Malformed special-form grammar, unknown EXTRACT field, or parentheses on a bare clock | — |
| `CK00001` | Clock precision or recognized unsupported function | — |
| `CS03027` | Volatile ADD COLUMN default on live rows requires a rewrite | — |
| `CS02001` | MOD divisor is zero | 22012 |
| `CS02002` | Numeric domain/range/overflow, temporal range, negative substring length or string allocation bound | 22003 |
| `CS01006` | No matching arity/types or incompatible temporal field | 42883 |
| `CS01008` | Equally good overloads; CAST to disambiguate | 42725 |
| `CS05013` | DEFAULT call result cannot be assigned to the column | — |
| `CS01016` | Unknown function outside executable and recognized vocabulary | — |
| `CS05032` | DISTINCT/ALL/ORDER BY specified on a non-aggregate call | — |

These functions introduce no new code. [Window functions](window-functions.md), including frame value functions, execute with OVER.
The profile advertises only executable names; recognition alone does not imply support.

New T1 builtin names are reserved at every arity. Existing UPPER/LOWER/LENGTH/ABS application
overloads at arities unused by the standard library remain possible. Function words remain usable
as ordinary identifiers without a call, except bare niladic names require quoting to name a column.
The current SQL storage format is **30**, with table extension 8 for ON UPDATE metadata
and retained TYPE default assignment coercion. It retains distinct predicates, Boolean tests and LIKE ESCAPE
in persisted definitions. Format 24 introduced explicit NULL placement after format 23's
view records. Format 22 introduced STRING_AGG/LISTAGG identifier quoting. Format 18 introduced quoting where newly executable names and BOTH/LEADING/TRAILING render
quoted as identifiers in persisted CHECK/default/function-body SQL. Format 17 separately introduced
the default/missing-value split with table extension 6. Compiled schema version is 9; older
SQL formats are refused with no upgrade reader.

## See also

[Functions](index.md) · [Aggregates](aggregate-functions.md) · [DEFAULT](../clauses/default.md) · [Diagnostics](../diagnostics.md) · [Unsupported syntax](../unsupported.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlT1ScalarTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlT1TemporalTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlT1TemporalFieldTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlT1StringLimitTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Functions/SqlBuiltinScalarFunction.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`.
