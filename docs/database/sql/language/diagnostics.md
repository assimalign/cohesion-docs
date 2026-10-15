# Diagnostics (Cohesion SQL)

Explains SQL language diagnostic codes and the distinction between parse, planning, and execution failures.

> **Status:** Implemented.

## Language diagnostics

| Code | Severity | Meaning | Example trigger |
| --- | --- | --- | --- |
| `COHDBL001` | Error | A recognized clause or form is outside the executable SQL surface | `SELECT * FROM t LEFT JOIN u ON t.id = u.id;` |
| `SQL0001` | Error | Empty query text | Empty or whitespace-only input |
| `SQL0002` | Error | Unknown leading command | `MERGE INTO t;` |
| `SQL0003` | Error | Malformed transaction-control, JOIN, grouping, HAVING, CAST, COLLATE, or constraint/DDL syntax | `SELECT CAST('42' AS );` |
| `SQL0004` | Error | Unknown CAST target type | `SELECT CAST('42' AS NOTATYPE) FROM t;` |
| `SQL0005` | Error | Recognized but unsupported CAST target or invalid target parameters | `SELECT CAST('42' AS REAL) FROM t;` or `CAST('42' AS INT(4))` |
| `SQL0100` | Information | Statement does not end with a semicolon | `SELECT id FROM t` |

The parser is error-tolerant and returns diagnostics on malformed input. An abstract syntax tree
(AST) does not establish that a recovered statement can execute. Recognized unsupported clauses use
`COHDBL001`; a genuinely unknown leading command still receives `SQL0002`, even if later
tokens are recognized unsupported syntax.

Positions are absolute character offsets in the original statement. Tools compute line and column
locations from those offsets. The statement root retains the raw text.
Omitting the terminator is informational, not a syntax rejection.

## Transaction diagnostics

| Code | Meaning | Example trigger |
| --- | --- | --- |
| `COHSQLT001` | A session transaction is already active | Send `BEGIN;` twice on the same session |
| `COHSQLT002` | No session transaction is active | Send `COMMIT;` or `ROLLBACK;` before `BEGIN;` |
| `COHSQLT003` | DDL cannot execute inside an explicit transaction | Send `DROP TABLE t;` after `BEGIN;` |

These runtime diagnostics leave the usable session scope intact. `ROLLBACK TO marker` instead
reports syntax diagnostic `SQL0003`; it does not silently roll back the active transaction.

## Planning and execution errors

Not every rejected query has an `SQL000x` code. Examples include an unknown or ambiguous column,
an invalid ordering ordinal, an ungrouped projected column, a nonliteral default, a CAST conversion
failure, and a constraint violation. The responsible planner or executor supplies the error.

For example, the measured default rejection is
`Column 'extra': only literal DEFAULT values are supported.` for
`ALTER TABLE t ADD COLUMN extra INT DEFAULT (1 + 2);`. It occurs before schema mutation.
A system relation targeted by DML or supported DDL fails as read-only.

## Wire behavior

Parse failures and execution failures use the shared wire codes `ParseFailure` and
`ExecutionFailure`. They end the current exchange without completion but leave the session ready
for another command. Protocol violations, malformed payloads, and invalid message order close the
connection. Transaction diagnostic codes are retained in the execution-error text.

The typed SQL client surfaces `SqlClientException`, retaining the wire code and a
`ConnectionUsable` flag. It does not reinterpret every diagnostic as a client-side parser result.

## See also

[Language (SQL)](index.md) · [Unsupported syntax](unsupported.md) · [CAST](expressions/cast.md) · [BEGIN](statements/begin-transaction.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Diagnostic construction** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlQueryParser.cs`
- **CAST diagnostics** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlCastParserTests.cs`
- **Transaction diagnostics** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTransactionControlTests.cs`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Wire behavior** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/WIRE-PROTOCOL.md`
- **Client errors** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/docs/DESIGN.md`

