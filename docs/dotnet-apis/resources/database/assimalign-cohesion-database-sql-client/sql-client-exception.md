# SqlClientException

A SQL client failure with a model error kind, wire category and structured diagnostics.

## Signature

```csharp
public sealed class SqlClientException : Exception
{
    public SqlClientException(SqlClientErrorKind kind, ProtocolErrorCode code, string message);
    public SqlClientException(SqlClientErrorKind kind, ProtocolErrorCode code, string message, Exception? innerException);
    public IReadOnlyList<Diagnostic> Diagnostics { get; }
    public SqlClientErrorKind Kind { get; }
    public ProtocolErrorCode Code { get; }
    public bool ConnectionUsable { get; }
}
```

## Behavior and exceptions

SqlClientException derives directly from System.Exception. Catch SqlClientException for remote SQL
failures; catching DatabaseException no longer handles them. Diagnostics is declared locally.

The inner-exception constructor copies diagnostics when its inner cause is a DatabaseClientException
or a nested SqlClientException. The
three-argument constructor has an empty list. Each copied snapshot is read-only and stably sorted
by source offset, with unlocated entries last. The carrier preserves code, severity and span while
Kind/Code retain client and wire categories. ConnectionUsable is true for ParseFailure,
ExecutionFailure, TransactionAborted, NotAuthorized and InvalidCast; protocol/connection failures
are unusable. Constructors add no new validation exception.

SQL constraint failures use Kind SqlClientErrorKind.ExecutionFailure and Code
ProtocolErrorCode.ExecutionFailure. With protocol 2.1 and `diagnostics` negotiated, Diagnostics
contains exactly one Error entry with the available enclosing statement span:

| Diagnostic code | Meaning and trigger | SQLSTATE mapping |
| --- | --- | --- |
| `COHSQLE037` | Duplicate PRIMARY KEY, UNIQUE constraint or unique-index key, including a second unique index in DO UPDATE | 23505 |
| `COHSQLE038` | CHECK/domain-check failure on a write, domain CAST, or function domain argument/result | 23514 |
| `COHSQLE039` | Missing foreign-key parent or referenced key protected on DELETE/UPDATE | 23503 |
| `COHSQLE024` | NOT NULL column receives NULL | 23502 |

Message starts with `CODE: ` and names the constraint and table without key values. Domain CAST
and function checks name the domain without a table. Protocol 2.0 still receives the code-prefixed
Error message without structured diagnostics. SQLSTATE is a reference mapping, with no separate
carrier member. A constraint failure rolls back the statement; the connection and explicit
transaction remain usable. Handle the diagnostic code separately from the wire-category Code.

Invariant LIKE/NOT LIKE work exhaustion uses ExecutionFailure with located `COHSQLE040`
in negotiated Diagnostics and `ConnectionUsable = true`; no SQLSTATE is assigned. See the
[LIKE budget and example](../../../../database/sql/language/expressions/operators.md#like-and-escape).

Detected storage damage uses ExecutionFailure with `COHDBS003` and database/page context,
with a usable connection (SQLSTATE XX001). Attached-index damage also retains COHDBI002 and
COHSQLE041. Storage/index I/O failures use COHSQLE041 (58030 for I/O), including wrapped causes.
Both failures abort an explicit transaction, which must be rolled back before continuing.
Autocommit may run the next statement after a recoverable failure. Structured diagnostics are
available when negotiated. See [storage integrity diagnostics](../../../../database/sql/language/diagnostics.md#database-creation-and-open).

Non-Boolean search conditions use COHSQLE003 (42804) as ExecutionFailure with a usable
connection and explicit transaction. Only valid Boolean input string literals coerce; supplied
text parameters retain their types. See [search conditions](../../../../database/sql/language/clauses/where.md#boolean-search-conditions).

## Example

Oversized encoded rows and write/build index keys report `COHSQLE042` / SQLSTATE 54000 through
ExecutionFailure, with `ConnectionUsable = true`. The message names the table, byte size and
limit. No statement rows change; an explicit transaction remains active with earlier work intact.
See [size limits](../../../../database/sql/language/diagnostics.md#row-and-index-key-limits).

```csharp
try { await connection.QueryAsync("SELECT absent FROM t;"); }
catch (SqlClientException failure) {
    Console.WriteLine(failure.Diagnostics[0].Code); // COHSQLE015 with diagnostics negotiated
    Console.WriteLine(failure.ConnectionUsable); // True
}
```

With `t` containing `(id, u)` rows `(1, 10)` and `(2, 20)`, and a unique index `uq_t` on `u`:

```csharp
await connection.ExecuteAsync("BEGIN");
try { await connection.ExecuteAsync("UPDATE t SET u = 10 WHERE id = 2"); }
catch (SqlClientException failure) when (failure.Code == ProtocolErrorCode.ExecutionFailure) {
    Console.WriteLine(failure.Diagnostics[0].Code); // COHSQLE037 with diagnostics negotiated
    Console.WriteLine(failure.ConnectionUsable);   // True
}
await connection.ExecuteAsync("COMMIT");
```

The UPDATE changes no rows; the transaction can still commit. Snapshot aborts and deadlock victims
instead require [whole-transaction rollback and retry](../../../../database/sql/language/clauses/on-conflict.md#isolation-locking-and-retries).

## Related reference

[Assembly](index.md) · [DatabaseException](../assimalign-cohesion-database/database-exception.md) · [SQL diagnostics](../../../../database/sql/language/diagnostics.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/src/Exceptions/SqlClientException.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlWireDiagnosticTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlConstraintDiagnosticWireTests.cs`.
