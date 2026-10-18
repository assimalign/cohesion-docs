# Diagnostics (Cohesion SQL)

SQL reports coded parser, binding, planning and execution diagnostics.

> **Status:** Implemented.

## Language diagnostics

| Code | Meaning |
| --- | --- |
| `COHDBL001` | Recognized clause, function, operator or modifier outside the executable profile |
| `SQL0001` | Empty statement |
| `SQL0002` | Unknown leading command |
| `SQL0003` | Malformed syntax, invalid parameter form, leftover text, bare DEFAULT outside its DML positions, or explicit NULL conflicting with NOT NULL/PRIMARY KEY/IDENTITY |
| `SQL0004` | Retired; unknown type names now resolve in the engine and use COHSQLE010 |
| `SQL0005` | Invalid type modifiers or rejected spelling, including TIME WITH TIME ZONE, BIT and SMALLDATETIME |
| `SQL0006` | Configured expression nesting limit exceeded; default 256 |
| `SQL0007` | Parsing thread has insufficient stack; engine text execution translates to COHSQLE004 |
| `SQL0008` | Malformed or overflowing typed temporal, interval or X binary literal; span covers the literal |
| `SQL0100` | Information: optional terminating semicolon omitted |

For example, `SELECT DATE '2024-02-30' FROM t;` reports SQL0008 before execution.
Recognized unsupported functions such as SQRT report COHDBL001, including over an empty table.
Only executable functions appear in the language profile's function table.

## Planning and execution diagnostics

A dash means the source does not assign a SQLSTATE here. SQLSTATE is a reference category;
the structured diagnostic carries Code, Message, Severity and location fields.

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| `COHSQLE001` | Division or remainder by zero | 22012 |
| `COHSQLE002` | Numeric overflow or unrepresentable numeric value; non-finite float to integer | 22003 |
| `COHSQLE003` | Invalid operand type, including nonnumeric arithmetic or non-Boolean NOT | 42804 |
| `COHSQLE004` | Statement needs more stack during parsing/planning/evaluation | 54001 |
| `COHSQLE005` | Column reference where no columns are in scope: VALUES or LIMIT/OFFSET | Class 42 |
| `COHSQLE006` | No function overload accepts the arguments | 42883 |
| `COHSQLE007` | Function or registered conversion implementation failed | 38000 |
| `COHSQLE008` | Ambiguous overload; CAST an argument to choose | 42725 |
| `COHSQLE009` | Persisted call's required function implementation is unavailable or incompatible | 42883 |
| `COHSQLE010` | Unknown type/domain or unavailable stored domain | 42704 |
| `COHSQLE011` | No conversion for a CAST pair; known pairs checked during planning | 42846 |
| `COHSQLE012` | Character assignment exceeds length with non-space overflow | 22001 |
| `COHSQLE013` | Invalid statement, expression, DDL or executor query shape; derived column list width mismatch or blank output name | — |
| `COHSQLE014` | Table does not exist | — |
| `COHSQLE015` | Unknown/ambiguous column, hidden/unknown/out-of-scope qualifier, or missing/repeated/ambiguous/incompatible-type USING column | — |
| `COHSQLE016` | Unknown function absent from executable and recognized-unsupported vocabulary | — |
| `COHSQLE017` | Invalid aggregate placement/grouping, including a non-grouping outer column referenced from a completed group | — |
| `COHSQLE018` | Invalid ordering ordinal, row count or CAST declaration | — |
| `COHSQLE019` | Duplicate target column/table binding or ambiguous output alias | — |
| `COHSQLE020` | Missing parameter, checked before reading or writing rows | — |
| `COHSQLE021` | Mutation targets a read-only system relation | 25006 |
| `COHSQLE022` | Invalid CAST input text representation | 22P02 |
| `COHSQLE023` | Parameter keys collide after stripping one sigil, including unused keys | — |
| `COHSQLE024` | NOT NULL column receives NULL or DEFAULT without a usable default | 23502 |
| `COHSQLE025` | Identity definition, assignment, ALTER, backfill or allocation violates its generation contract, including range exhaustion | 428C9 (`generated_always`) for explicit ALWAYS assignments; other identity failures share the local code |
| `COHSQLE026` | Duplicate CTE name or unresolved reference to a later sibling; binding fails before reads/writes | — |

For example, `SELECT t.id FROM t AS x;` reports located COHSQLE015 because the alias hides t.
NOT NULL failures name the column and have no execution-time source span.
Correlated references resolve through enclosing query scopes. A real derived sibling reference
uses `COHDBL001` naming LATERAL; unknown, ambiguous or hidden names retain `COHSQLE015`.
An aggregate assigned to an outer level uses `COHDBL001` before grouped-column validation.
For WITH, a later sibling's name reads a stored table when one exists; only a missing future
relation receives `COHSQLE026`. No SQLSTATE is assigned to that code.

## Transactions

| Code | Meaning |
| --- | --- |
| `COHSQLT001` | BEGIN/START TRANSACTION while a transaction is active |
| `COHSQLT002` | COMMIT/END/ROLLBACK without an active transaction |
| `COHSQLT003` | DDL inside an explicit transaction |

A malformed transaction spelling reports SQL0003 before changing transaction state.

## Locations and wire behavior

`DatabaseException.Diagnostics` is never null. Supplied diagnostics are copied, ordered by
zero-based UTF-16 start offset, with unlocated entries last and stable ordering for ties.
End offsets are exclusive; line numbers are one-based. Binding rejections retain available AST
spans. Typed requests and text execution reject parser errors before planning.

Servers compose `CODE: message` from the first Error diagnostic. Negotiated protocol 2.1 with
`diagnostics` also sends Warning/Error entries in frame 14 before Error or successful completion.
Information diagnostics are omitted. At most 4,096 entries and 1 MiB are sent. Overflow retains a
leading prefix plus `COHDBP001` with exact omission counts; it preserves the original outcome and
usable session. Negotiated 2.0 retains Error text and baseline results without that frame.

ParseFailure and ExecutionFailure end the exchange while leaving the session ready. Malformed
payloads, UTF-8, message order or Diagnostics frames are protocol violations. Typed client failures
retain the wire category, structured diagnostics and connection usability.

## See also

[Language (SQL)](index.md) · [Unsupported syntax](unsupported.md) · [CAST](expressions/cast.md) · [Parameters](expressions/parameters.md)

## Sources

- **Codes** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`.
- **Spans and carriers** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDiagnosticTests.cs` and `SqlDiagnosticsFixPassTests.cs` in the same directory.
- **NOT NULL** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlDefaultAssignmentTests.cs`.
- **Negotiation and bounds** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/docs/DESIGN.md`.
