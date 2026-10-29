# Data types

Maps SQL declarations and CAST targets to shared Database identities.

> **Status:** Implemented. Supports the names below.

## Names and identities

| SQL names | Shared type identity |
|---|---|
| `BOOLEAN`, `BOOL` | `Boolean` |
| `TINYINT` | `Int8` |
| `SMALLINT`, `INT2` | `Int16` |
| `INT`, `INTEGER`, `INT4` | `Int32` |
| `BIGINT`, `INT8` | `Int64` |
| `REAL`, `FLOAT4` | `Float32` |
| `FLOAT[(p)]` | `Float32` for p = 1..24; `Float64` for p = 25..53 or no argument |
| `FLOAT8`, `DOUBLE`, `DOUBLE PRECISION` | `Float64` |
| `DECIMAL[(p[,s])]`, `NUMERIC[(p[,s])]` | `Decimal` (single argument = precision) |
| `CHAR[(n)]`, `CHARACTER[(n)]`, `NCHAR[(n)]`, `NATIONAL CHAR[(n)]`, `NATIONAL CHARACTER[(n)]` | `Character` (bare forms have length one) |
| `VARCHAR[(n)]`, `TEXT[(n)]`, `NVARCHAR[(n)]`, `CHARACTER VARYING[(n)]`, `CHAR VARYING[(n)]`, `NCHAR VARYING[(n)]`, `NATIONAL CHAR VARYING[(n)]`, `NATIONAL CHARACTER VARYING[(n)]` | `String` |
| `BINARY[(n)]`, `VARBINARY[(n)]`, `BLOB[(n)]`, `BYTEA[(n)]`, `BINARY VARYING[(n)]` | `Binary` |
| `DATE` | `Date` |
| `TIME`, `TIME WITHOUT TIME ZONE` | `Time` |
| `TIMESTAMP`, `TIMESTAMP WITHOUT TIME ZONE`, `DATETIME`, `DATETIME2` | `DateTime` |
| `TIMESTAMPTZ`, `TIMESTAMP WITH TIME ZONE`, `DATETIMEOFFSET` | `DateTimeOffset` |
| `INTERVAL` | `TimeSpan` |
| `UUID`, `GUID`, `UNIQUEIDENTIFIER` | `Guid` |
| `JSON` / `JSONB` | `Json` / `JsonBinary` |

DDL and CAST share one contextual type-name grammar, including case, whitespace and comments
between words (`double /*x*/ precision`). No additional words are reserved. CREATE TABLE,
both column ALTER forms, function signatures and domain bases resolve built-ins at parse time;
columns expose `SqlColumnDefinition.ResolvedType`. Unknown names remain engine-resolved domains.
All 19 current INFORMATION_SCHEMA DATA_TYPE spellings parse back to the same shared type,
including CHARACTER added by #1437. The catalog stores type identity and modifiers, not aliases.

`VARCHAR(MAX)`, `NVARCHAR(MAX)` and `VARBINARY(MAX)` mean unbounded String or Binary;
MAX is accepted only on these three spellings. NCHAR, NATIONAL CHAR and NATIONAL CHARACTER
are padded Character exactly like CHAR, including DATA_TYPE CHARACTER. Adding VARYING to any
of those three names selects unpadded String exactly like VARCHAR; bare varying forms are
unbounded. NATIONAL and VARYING remain usable as unquoted table and column names.
UNIQUEIDENTIFIER is UUID/Guid and takes no arguments. DATETIME and DATETIME2 are lexical TIMESTAMP aliases: SQL Server's
DATETIME 1753 lower bound and 1/300-second rounding are not emulated. DATETIMEOFFSET is
TIMESTAMPTZ. These aliases add no Unicode storage distinction: all character spellings store Unicode.

| Rejected spelling | Diagnostic and reason; replacement |
| --- | --- |
| TIME WITH TIME ZONE | CS00005: no shared storage type; use TIME WITHOUT TIME ZONE or TIMESTAMP WITH TIME ZONE |
| BIT, BIT(n) | CS00005: PostgreSQL bit-string meaning wins under D1; use BOOLEAN for truth values |
| SMALLDATETIME | CS00005: range and minute precision cannot be honored; use TIMESTAMP |
| INT[], INTEGER ARRAY (and other scalar array forms) | CK00001: arrays are unsupported; use a scalar type |
| INTERVAL field qualifiers | CK00001: field restrictions are not enforced; use bare INTERVAL |
## Bounds and persistence

Lengths count Unicode scalars for character types and bytes for binary types. Bare fixed
character forms default to length one; bare varying forms are unbounded. NUMERIC precision is
1..28 and scale is 0..precision. A precision without scale means scale zero. Temporal precision
and INTERVAL declaration field restrictions report `CK00001`.

Unknown built-in names are resolved as engine domains; an absent type is `CS02010` (42704).
SERIAL/SERIAL4, BIGSERIAL/SERIAL8 and SMALLSERIAL/SERIAL2 are column aliases for
[BY DEFAULT identity](../clauses/identity.md) of INTEGER, BIGINT and SMALLINT. This deliberately
differs from PostgreSQL. Serial domain names remain refused, including quoted names (CK00001);
native SqlType.CreateDomain rejects them with ArgumentException. Identity requires a
built-in signed integer rather than a domain.
Domain-check failures on writes, CAST and function domain arguments/results use `CS03038`
(SQLSTATE 23514), with one Error diagnostic naming the domain and, for a write, its table.
Declaring a non-Boolean domain CHECK uses CS02003 (42804), including non-Boolean logical
operands and mixed CASE/COALESCE alternatives in Boolean positions. Untyped NULL is UNKNOWN
and passes a CHECK; use NOT NULL separately when required.
See [Constraints](../constraints.md) and [CAST](../expressions/cast.md).
See [CAST](../expressions/cast.md) for conversions and [DEFAULT](../clauses/default.md) for typed
literal defaults. Day-time INTERVAL supports [temporal arithmetic](../expressions/operators.md#temporal-arithmetic)
and [EXTRACT/DATE_TRUNC](../functions/builtin-functions.md#clock-temporal-and-uuid-functions).
SQL row-storage is format 30; compiled schema artifacts are version 9. Format 17 introduced the
insert-default/frozen-missing-value split (table extension 6); format 18 separately changes canonical
quoting of newly executable function names. These
unreleased formats have no compatibility reader for older formats.
See the [format-30/schema-v9 export and rebuild procedure](../../index.md#sql-format-30-and-schema-v9).

## Temporal text input

TIME stores a TimeOnly clock, and INTERVAL an exact day-time TimeSpan with 100 ns resolution.
INSERT, UPDATE, INSERT SELECT, DEFAULT and [CAST](../expressions/cast.md) share TIME/INTERVAL
text readers, including domain columns. Surrounding ASCII whitespace is trimmed; non-ASCII
whitespace is rejected. NULL stays NULL subject to destination constraints; SQL has no MISSING scalar.

TIME accepts one- or two-digit clock fields, optional seconds with up to seven fractional digits,
AM/PM (`1 PM`, `12:02 am`) and a trailing fractional dot (`13:45:00.`). A written zone is validated
and discarded: `10:00+05:30` stays `10:00:00`. Numeric zones accept a sign and one or two hour
digits, four/six compact digits, or colon-separated hour/minute/second fields, up to 15:59:59.
The case-insensitive abbreviations are Z, UT, UTC, GMT, EST/EDT, CST/CDT, MST/MDT, PST/PDT,
CET/CEST, EET/EEST and JST. Regional zone names are unavailable. TIME rejects 24:00 and more than
seven fractional digits; it does not round to PostgreSQL's microsecond precision. TIMESTAMP's
strict literal/CAST clock remains zone-free; plain timestamp assignment retains its existing grammar.

An unqualified INTERVAL number means seconds: `'12'` stores twelve seconds and `'-12.5'` stores
minus 12.5 seconds. Unit/clock strings and ISO day-time durations remain supported. `'1 2 hours'`
means one day plus two hours; optional `@` and final `ago` are accepted in unit/clock strings,
with `ago` negating the whole duration. CLR day-dot-clock text such as `'1.02:03:04'` is refused.
Year/month intervals remain unsupported.

Assignment, DEFAULT and CAST use CS02047 / 22007 for malformed temporal text,
CS02048 / 22008 for calendar/clock/offset fields outside range, and CS02062 / 22015 for
interval overflow. Invalid typed literal contents instead fail parsing with CS00008; typed
year/month intervals use CK00001. See [literals](../expressions/literals.md).

```sql
CREATE TABLE durations (at_time TIME, elapsed INTERVAL);
INSERT INTO durations VALUES ('1 PM', '1 2 hours');
SELECT at_time, elapsed = INTERVAL 'PT26H' AS same_duration FROM durations;
```

Execute separately; SELECT returns `(13:00:00, TRUE)`.

## Floating text input

REAL and DOUBLE PRECISION text assignment and DEFAULT use CAST's invariant floating parser.
Exponent notation is accepted. Assignment also accepts NaN and Infinity; immutable and frozen
Stable defaults still require finite values. Grouping (`'1,000'`) fails CS02022 / 22P02;
finite overflow (`'1e400'`) and nonzero underflow to zero fail CS02002 / 22003.
SQL numeric literals still evaluate as exact Decimal; cast text for approximate input.

```sql
CREATE TABLE readings (value REAL DEFAULT '1e3');
INSERT INTO readings DEFAULT VALUES;
SELECT value FROM readings;
```

Execute separately; SELECT returns `1000`. Explicit NULL stays NULL when the column permits it.

## SQL domains

[CREATE DOMAIN](../statements/create-domain.md) persists a base type, literal default, NOT NULL
and immutable CHECK; [DROP DOMAIN](../statements/drop-domain.md) refuses dependencies. After either
DDL commits, mandatory refresh and success delivery ignore late cancellation and timeout expiration.

## Examples

```sql
CREATE TABLE aliases (v DOUBLE PRECISION, n NATIONAL CHARACTER VARYING(20), stamp DATETIME2);
INSERT INTO aliases VALUES (CAST('1.25' AS FLOAT(24)), N'héllo', TIMESTAMP '2024-01-01');
SELECT v, n, stamp FROM aliases;
```

The row is `(1.25, 'héllo', 2024-01-01 00:00:00)`; canonical catalog names are
DOUBLE PRECISION, CHARACTER VARYING and TIMESTAMP.

Format 19 quotes PERCENT and TIES in persisted SQL. Format 20 adds persisted SET NULL,
SET DEFAULT and NO ACTION delete actions. Format 21 introduced namespace records of kind 15. Format 22 quotes STRING_AGG and LISTAGG
identifiers in persisted SQL. Format 23 introduced catalog views and format 24 explicit NULL
placement. Format 25 added distinct predicates, Boolean tests and LIKE ESCAPE.
Format 26 fenced temporal DEFAULT and frozen missing-value text.
Format 29 added owned-sequence metadata. Current format 30 adds ON UPDATE action metadata
in table extension 8 and retains TYPE default assignment coercion.
Every earlier format is refused at open; there is no
upgrade path. Compiled schema format remains version 9.

## See also

[Language (SQL)](../index.md) · [CAST](../expressions/cast.md) · [Literals](../expressions/literals.md) · [INFORMATION_SCHEMA](../system-views/information-schema.md)

## Sources

- **Text inputs on every write path** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTemporalAssignmentConformanceTests.cs`.
- **Name resolution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlTypeNames.cs`.
- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`.
- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCastMatrixTests.cs`.
