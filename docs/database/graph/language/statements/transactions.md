# Graph transactions

```syntaxsql
{ START TRANSACTION | BEGIN [ TRANSACTION ] } [ ; ]
COMMIT [ ; ]
ROLLBACK [ ; ]
```

Each statement is a separate Execute request on the same connection. START and BEGIN use Snapshot; typed embedded sessions also offer ReadCommitted. A statement failure aborts the explicit transaction; later work requires ROLLBACK. COMMIT of a failed transaction rolls it back and reports failure. Duplicate START uses CG04022 and aborts the current transaction; COMMIT/ROLLBACK without one uses CG04023. ExecutePaths rejects controls with CG05001 before changing transaction state.

Savepoints, transaction options, EXPLAIN controls, composed controls and multiple statements in one text request remain unsupported. Transaction-start datetime functions retain one instant throughout the explicit transaction.

```csharp
await connection.ExecuteAsync("START TRANSACTION");
await connection.ExecuteAsync("INSERT (:Person {name: 'Ada'})");
await connection.ExecuteAsync("ROLLBACK");
```

The client drains negotiated server I/T/E transaction status after Execute. Returning a SQL/Graph connection rolls back an open or failed transaction before reuse; a failed or uncertified reset discards it. Session teardown/disconnect also rolls back. A lost COMMIT response leaves an unknown outcome; pool cleanup cannot establish it. See [client transaction behavior](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph-client/design.md#explicit-transaction-text).

Once an open wire transaction acquires the database writer lock, it holds it even while idle,
until COMMIT, ROLLBACK, disconnect, pool return or server `IdleTimeout` (30 minutes by default).
Keep transactions short because an idle writer blocks other writers. New clients do not pool
sessions against pre-#1540 servers that cannot negotiate transaction-status.

[Statements](index.md) · [Diagnostics](../diagnostics.md)

## BEGIN alias

BEGIN and BEGIN TRANSACTION execute in process and over the wire with the same Snapshot
semantics as START TRANSACTION. WORK, isolation clauses and other options report CG00002.
Duplicate BEGIN uses CG04022; absent COMMIT/ROLLBACK uses CG04023. These conditions have no
assigned SQLSTATE. The same rollback, pool reset and failed-transaction rules apply.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphDatabaseSession.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphTransactionTextWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphTransactionFailureWireTests.cs`.
