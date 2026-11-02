# ROLLBACK TRANSACTION (Cohesion SQL)

Aborts the session's active explicit transaction, or undoes work after a named savepoint.

> **Status:** Implemented.

## Syntax

```syntaxsql
ROLLBACK [ WORK | TRANSACTION ] [ ; ]
ROLLBACK [ WORK | TRANSACTION ] TO [ SAVEPOINT ] name [ ; ]
```

## Arguments

`WORK` and `TRANSACTION` are optional equivalent suffixes on ROLLBACK.
Transaction names, modes and `AND [NO] CHAIN` are outside this grammar.
Malformed suffixes report `CS00003` before transaction state changes.

## Remarks

Rollback clears the session's active scope and undoes its row changes. Calling `ROLLBACK`
without an active transaction reports `CS04002`. Disconnect also aborts an open transaction.

Only an enrolled writer appends an abort record, after undo, without waiting for that record's
durability. Read-only rollback or disposal appends no lifecycle record and waits for no flush;
the transaction still ends and releases its locks.

Like [COMMIT](commit-transaction.md), rollback observes caller cancellation only before its end
starts. Cancellation observed then leaves the transaction active and usable. A started rollback
ignores caller cancellation and completes its end.

`ROLLBACK TO` keeps the [named savepoint](savepoint.md) and the transaction open, removes newer
savepoints, and can be repeated. Duplicate names select the newest; unknown names use CK04013
and a missing explicit transaction uses CK04012 (neither has an assigned SQLSTATE).
After incomplete undo, CK04009 admits only retry to that or an outer retained savepoint, or
whole ROLLBACK. The wire session stays open with failed Ready status until recovery.
If the database closes while the fence is live, SQL reports CS04005 ahead of CK04009 and
retains the wire session with Ready `E`. Named-control I/O failures instead abort the explicit
transaction under #1364; see [savepoint failure rules](savepoint.md#remarks).
Completed DDL is self-committing and cannot be undone by a later rollback.

Pool return rolls back a negotiated open or failed transaction and discards every savepoint before reuse. Failed cleanup or
missing transaction-status support discards the connector. Disconnect/session disposal also rolls back.

If client-side rollback cleanup fails, `SqlConnection.AbortAsync` discards the rental instead
of returning an unreset session to the pool.

## Examples

Starting from the [conformance fixture](select.md#a-create-the-conformance-fixture), execute each
statement as a separate request on the same session or client connection:

```sql
BEGIN;
INSERT INTO t VALUES (4, 'new', 1);
DELETE FROM t WHERE id = 2;
ROLLBACK WORK;
SELECT id FROM t ORDER BY id;
```

The final query returns the original identifiers `1`, `2`, and `3`.

## See also

[BEGIN](begin-transaction.md) · [COMMIT](commit-transaction.md) · [ROLLBACK](rollback-transaction.md)

[Statements](index.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Named-control failures and closure** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SavepointFixTests.cs` and `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SavepointFailureWireTests.cs`.
- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Transaction behavior** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTransactionControlTests.cs`
- **Client lifetime** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/docs/DESIGN.md`
- **Coordinator** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/docs/OVERVIEW.md`
