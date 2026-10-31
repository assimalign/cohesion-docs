# Graph transactions

Starts, ends or controls a named undo scope in one explicit transaction.

> **Status:** Implemented in process and over InMemory and TCP.

## Syntax

```syntaxsql
{ START TRANSACTION | BEGIN [ TRANSACTION ] } [ ; ]
COMMIT [ ; ]
ROLLBACK [ ; ]
SAVEPOINT name [ ; ]
RELEASE [ SAVEPOINT ] name [ ; ]
ROLLBACK [ WORK | TRANSACTION ] TO [ SAVEPOINT ] name [ ; ]
```

## Remarks

Each statement is a separate Execute request on the same connection. START and BEGIN use Snapshot; typed embedded sessions also offer ReadCommitted. A statement failure aborts the explicit transaction; later work requires ROLLBACK. COMMIT of a failed transaction rolls it back and reports failure. Duplicate START uses CG04022 and aborts the current transaction; COMMIT/ROLLBACK without one uses CG04023. ExecutePaths rejects controls with CG05001 before changing transaction state.

Transaction options, EXPLAIN controls, composed controls and multiple statements in one text request remain unsupported. Transaction-start datetime functions retain one instant throughout the explicit transaction.

[Schema DDL](create-index.md#transactions-and-limits), including typed GraphSchema writes and
conditional no-ops, is refused inside an explicit transaction with CG04025 and aborts it.
ROLLBACK restores session use. Execute schema writes as separate autocommit operations.

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

## Named savepoints

SAVEPOINT, RELEASE and ROLLBACK TO use SQL spellings as Cohesion extensions to GQL. Names
preserve decoded identifier spelling and compare exactly, including unquoted names; quoted
identifiers decode doubled quotes. Duplicate names are allowed; the newest wins. RELEASE
removes its target and every newer savepoint while retaining their effects in the transaction.
ROLLBACK TO undoes changes and scoped locks after its target, removes newer savepoints and
keeps its target for repeated rollback. It leaves the transaction open for more work and COMMIT.
Successful controls affect zero rows; returning a pooled connection discards every savepoint.

A matched [MERGE](merge.md) that writes no data remains memory-only inside a savepoint. Its
lock bookkeeping opens no storage bracket and appends no journal record; if the transaction
contains no writes, COMMIT performs no durability wait, including after ROLLBACK TO.

An ordinary failed statement still aborts the transaction. ROLLBACK TO returns CG04007 and
cannot recover it; whole ROLLBACK recovers the session. Incomplete undo instead reports
CK04009 and admits only ROLLBACK TO the same or an outer retained savepoint, or whole ROLLBACK.
Reads, writes, COMMIT and other named controls are fenced until recovery. The wire session stays
open and Ready reports a failed transaction (`E`). Pre-canceled controls change nothing;
admitted undo finishes despite later caller cancellation. ExecutePaths refuses named controls
with CG05001 before changing state.

Database disposal takes precedence over CK04009. Direct transaction controls, refusals and
commit throw ObjectDisposedException while a live undo fence remains. Over the wire, Graph
keeps its existing disposal code CG09019.

## Savepoint diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CK04012 | Named control requires an explicit transaction with an owned kernel binding | — |
| CK04013 | Savepoint name is unknown in the current transaction | — |
| CK04009 | Incomplete undo or statement/control/end activity prevents control admission | — |
| CG04007 | Ordinary statement failure has aborted the transaction; whole rollback is required | — |
| CG09019 | Database disposal takes precedence over a live undo fence on the wire | — |

## Savepoint example

Execute separately in a fresh database on one connection:

```gql
BEGIN;
INSERT (:SavedItem {n: 1});
SAVEPOINT Keep;
INSERT (:SavedItem {n: 2});
ROLLBACK WORK TO SAVEPOINT Keep;
ROLLBACK TO Keep;
RELEASE Keep;
COMMIT;
MATCH (n:SavedItem) RETURN count(*);
```

The count is `1`. `Keep` remains after each rollback; `keep` would be a different name.

## Sources

- **Memory-only MERGE and direct disposal** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/SavepointMergeLockTests.cs` and `cohesion/resources/Database/Assimalign.Cohesion.Database/tests/SavepointSeamTests.cs`.
- **Disposal translation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphDatabase.Lifetime.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/NamedSavepointTests.cs` and `SavepointFixTests.cs` in the same directory.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/NamedSavepointWireTests.cs` and `SavepointFailureWireTests.cs` in the same directory.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphDatabaseSession.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphTransactionTextWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphTransactionFailureWireTests.cs`.
