# COMMIT TRANSACTION (Cohesion SQL)

Commits the session's active explicit transaction.

> **Status:** Implemented.

## Syntax

```syntaxsql
{ COMMIT [ WORK | TRANSACTION ] | END } [ ; ]
```

## Arguments

`WORK` and `TRANSACTION` are optional equivalent suffixes on COMMIT.
`END` maps to COMMIT and takes no suffix. Without a transaction its `COHSQLT002` message names COMMIT.
Transaction names, modes, savepoints and `AND [NO] CHAIN` are outside this grammar.
Malformed suffixes report `SQL0003` before transaction state changes.

## Remarks

A successful commit publishes the transaction's writes and clears the session's active scope.
Calling `COMMIT` without an active transaction reports `COHSQLT002`. The transaction coordinator
binds writer commit to the write-ahead log (WAL) durability path. Only an enrolled writer appends
a COMMIT record and waits for durability. A read-only commit simply ends the transaction and
releases its locks, with no lifecycle append or flush wait.

Keep `BEGIN`, all enclosed statements, and `COMMIT` on the same rented client connection.
A connection lost after server-side publication but before the response leaves an unknown outcome.
The wire protocol has no durable transaction token, status lookup, or replay deduplication.
`SqlConnection.AbortAsync` discards an unsafe rental; it cannot undo a published commit.
DDL statements committed outside this transaction are not part of its scope.

An already-aborted transaction refuses COMMIT with COHSQLT005 and commits nothing. Storage,
I/O and index failures abort explicit transactions; roll back before further work. The refusal
carries the Error code in Diagnostics and the original cause in InnerException, with ordinary
in-process Message text; the wire adds `COHSQLT005: ` once. See [transaction diagnostics](../diagnostics.md#transactions).

End the explicit transaction before disposing its pooled connection. Pool return preserves an
unfinished transaction for the next renter (known gap #1540); it does not roll back automatically.

## Examples

Starting from the [conformance fixture](select.md#a-create-the-conformance-fixture), execute each
statement as a separate request on the same session or client connection:

```sql
BEGIN;
INSERT INTO t VALUES (4, 'new', 1);
END;
```

An observing session can now select row `4`.

## See also

[BEGIN](begin-transaction.md) · [COMMIT](commit-transaction.md) · [ROLLBACK](rollback-transaction.md)

[Statements](index.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Transaction behavior** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTransactionControlTests.cs`
- **Client lifetime** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/docs/DESIGN.md`
- **Coordinator** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/docs/OVERVIEW.md`
