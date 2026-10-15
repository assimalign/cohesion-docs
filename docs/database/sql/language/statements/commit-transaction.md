# COMMIT TRANSACTION (Cohesion SQL)

Commits the session's active explicit transaction.

> **Status:** Implemented.

## Syntax

```syntaxsql
COMMIT [ TRANSACTION ] [ ; ]
```

## Arguments

The optional `TRANSACTION` keyword is accepted without a transaction name.

## Remarks

A successful commit publishes the transaction's writes and clears the session's active scope.
Calling `COMMIT` without an active transaction reports `COHSQLT002`. The transaction coordinator
binds commit to the write-ahead log (WAL) durability path.

Keep `BEGIN`, all enclosed statements, and `COMMIT` on the same rented client connection.
A connection lost after server-side publication but before the response leaves an unknown outcome.
The wire protocol has no durable transaction token, status lookup, or replay deduplication.
`ISqlConnection.AbortAsync` discards an unsafe rental; it cannot undo a published commit.
DDL statements committed outside this transaction are not part of its scope.

## Examples

Starting from the [conformance fixture](select.md#a-create-the-conformance-fixture), execute each
statement as a separate request on the same session or client connection:

```sql
BEGIN;
INSERT INTO t VALUES (4, 'new', 1);
COMMIT;
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

