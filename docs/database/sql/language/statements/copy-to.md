# COPY TO STDOUT (Cohesion SQL)

Exports a physical table as UTF-8 text or CSV to a caller-owned stream.

> **Status:** Implemented for table exports through the COPY consumer APIs.

## Syntax

```syntaxsql
COPY table [(column, ...)] TO STDOUT [WITH] [(option, ...)]

option ::= FORMAT {text | csv}
         | DELIMITER value
         | NULL value
         | HEADER [boolean]
         | QUOTE value
         | ESCAPE value
         | FORCE_QUOTE {* | (column, ...)}
         | ENCODING value
```

The output follows the written column list, or all live columns in catalog order when omitted.
Column binding ignores case; HEADER uses catalog spelling. Row order is unspecified. Option
arguments accept strings, words and signed numbers. Unquoted option words fold to lowercase;
quoted option names and FORMAT values preserve case and require lowercase spellings.
Options require parentheses, with optional WITH. Legacy `CSV HEADER` forms use CS00003.

## Options

| Option | Default | Validation and behavior |
| --- | --- | --- |
| FORMAT | text | text or csv; binary is refused with CK00001 |
| DELIMITER | Tab in text; comma in CSV | One nonzero ASCII byte; no CR/LF. Text also forbids backslash, dot, lowercase letters and digits. CSV delimiter must differ from QUOTE |
| NULL | `\N` in text; empty unquoted field in CSV | No NUL/CR/LF, delimiter, or CSV quote |
| HEADER | false | Bare HEADER means true. Accepts true/on/1 or false/off/0, including quoted, signed and zero-padded integer forms |
| QUOTE | Double quote in CSV | CSV only; one nonzero ASCII byte |
| ESCAPE | The effective QUOTE | CSV only; one nonzero ASCII byte |
| FORCE_QUOTE | No forced fields | CSV only; `*` or an output-column list; quotes non-NULL data only |
| ENCODING | UTF-8 without BOM | UTF8, UTF-8 or UNICODE aliases only; ASCII case/punctuation cleanup, at most 63 bytes |

Unknown, repeated or incompatible options use CS01070. Validation is independent of option order.
Every record ends with LF. A duplicate output column, repeated FORCE_QUOTE column or FORCE_QUOTE
column outside the export uses CS01071; an unknown output column uses CS01015.

## Escaping and values

Text escapes backslash, delimiter, backspace, form feed, LF, CR, tab and vertical tab. Standard
control escapes are `\b`, `\f`, `\n`, `\r`, `\t` and `\v`; backslash is doubled and the delimiter
is backslash-prefixed. NULL is written verbatim.

CSV quotes fields matching the NULL marker or containing delimiter, quote, CR or LF; it also
quotes a single-column `\.`. Inside quotes, quote and escape characters are prefixed with ESCAPE.
Headers use ordinary escaping. FORCE_QUOTE never quotes SQL NULL, so the default CSV format
distinguishes NULL (empty unquoted field) from empty TEXT (`""`).

Values use the engine's actual `CAST(column AS TEXT)`, including application casts and domains.
Representation differences from PostgreSQL output functions are deliberate: BOOLEAN is
TRUE/FALSE instead of t/f; CHAR loses trailing padding through CAST; temporal fractions can
retain seven tick digits instead of PostgreSQL's six microsecond digits; TIMESTAMPTZ retains
the stored offset with `hh:mm` spelling rather than the session timezone and PostgreSQL's
abbreviated hour-only offset; INTERVAL has the engine's day/time-only, tick-precision range.
REAL/DOUBLE use invariant .NET shortest round-trip spelling, which can differ in exponent case,
notation threshold or selected shortest digits. JSONB preserves validated JSON text spelling
instead of PostgreSQL's normalized key order and whitespace. BYTEA uses PostgreSQL's default
lowercase `\x` hex spelling, without its configurable escape output. Integers, declared NUMERIC
scale, TEXT/VARCHAR, DATE and UUID have the same ordinary spelling within engine type ranges.
Custom casts own their representation; malformed UTF-16 output refuses with existing CS05045.

## Running an export

Use embedded [SqlDatabaseSession.CopyToAsync](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-sql/sql-database-session.md#copytoasync)
or remote [SqlConnection.CopyToAsync](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-sql-client/sql-connection.md#copytoasync).
Both return the data-row count, excluding the header, and leave the destination open.
The remote API requires separately negotiated `sql.outbound-stream`. Ordinary Execute/Query
refuses COPY with CS05096; a different statement passed to CopyToAsync uses CS05095.
COPY requires automatic execution outside an explicit transaction (CS04075 / 25001).

One statement snapshot covers binding, row reads and delivery; later commits are invisible.
The producer takes no writer, row or object locks. It retains one row, a nonlocking page pin,
an encoder, and an 8-byte iterator entry per table page. The 16 KiB buffer fills across rows;
the remote producer allows one outstanding data frame and waits for its empty acknowledgment.
COPY waits for migration admission before snapshot capture and binding. Overlapping statements
and transaction controls on the embedded session are refused.

Failure can leave partial destination output. A coded statement Error before CopyStart preserves
the remote connection and transaction; a missing capability refuses locally and preserves it.
Errors after CopyStart retain diagnostics but discard the connection, including a pending
acknowledgment. Cancellation during an exchange, malformed/incomplete responses, disconnect and
destination failures discard the rental. SqlClientException.ConnectionUsable is false after a
transfer has started, even for a statement error kind. Destination exceptions are rethrown unchanged
after cleanup. Destinations must honor cancellation and must not reenter the session/connection
from write callbacks. Embedded disposal cancels and waits for the export to release its snapshot.

## Example

Execute the definition and INSERT separately, then pass COPY to CopyToAsync:

```sql
CREATE TABLE export_rows (id INT, label TEXT);
INSERT INTO export_rows VALUES (1, 'a,b');
COPY export_rows TO STDOUT (FORMAT csv, HEADER);
```

The row count is 1 and the UTF-8 destination contains:

```csv
id,label
1,"a,b"
```

The final line ends with LF. This uses the escaping covered by SqlCopyToTests.

## Diagnostics and exclusions

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS00003 | Legacy unparenthesized options or malformed syntax | — |
| CS01070 | Invalid, duplicate or incompatible option | — |
| CS01071 | Duplicate output/FORCE_QUOTE column or FORCE_QUOTE column outside export | — |
| CS01015 | Missing output column | 42703 |
| CS04075 | COPY inside an explicit transaction | 25001 |
| CS05095 | CopyToAsync received a non-COPY statement | — |
| CS05096 | COPY lacks the outbound-stream capability or consumer API | — |
| CK00001 | COPY FROM, server files, PROGRAM, binary, query-form export or views | — |
| CK07005 | Malformed or out-of-sequence COPY protocol exchange | — |

## See also

[Statements](index.md) · [CAST](../expressions/cast.md) · [Diagnostics](../diagnostics.md) · [Protocol](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-sql-protocol/sql-protocol.md)

## Sources

- **Grammar and representation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`, COPY TO STDOUT (#1724, SQ-C1).
- **Execution** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlCopyToTests.cs` and `SqlCopyToFixTests.cs`.
- **Delivery** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlCopyToWireTests.cs` and `SqlCopyToFixWireTests.cs`.
