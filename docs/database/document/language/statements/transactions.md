# OQL transactions

Begins, commits or rolls back an explicit transaction on one Documents session.

> **Status:** Implemented in process and over InMemory and TCP through the Documents client.

## Syntax

```syntaxsql
{ BEGIN [ WORK | TRAN | TRANSACTION ] | START { WORK | TRAN | TRANSACTION } }
    [ ISOLATION LEVEL { SNAPSHOT | READ COMMITTED } ] [ ; ]
COMMIT [ WORK | TRAN | TRANSACTION ] [ ; ]
ROLLBACK [ WORK | TRAN | TRANSACTION ] [ ; ]
SAVEPOINT name [ ; ]
RELEASE [ SAVEPOINT ] name [ ; ]
ROLLBACK [ WORK | TRAN | TRANSACTION ] TO [ SAVEPOINT ] name [ ; ]
```

## Remarks

Execute each command separately on the same session. Snapshot is the default: the transaction
retains one snapshot. READ COMMITTED takes a fresh committed snapshot for each statement.
Bare BEGIN and explicit SNAPSHOT are Cohesion extensions to SQL++; Cohesion's default differs
from Couchbase SQL++'s READ COMMITTED default. All spellings above return Success and affected
count zero. Controls also operate on a transaction begun through the session's C# API.

A statement failure faults the explicit transaction. Subsequent work, including malformed text,
reports CD04101 with the original failure retained. ROLLBACK recovers the session and discards
pending writes. COMMIT of a faulted transaction reports CD04101 and ends it without committing.
Nested BEGIN reports CK04004 and faults the existing transaction.

On a [DocumentConnection](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents-client/document-connection.md),
send controls through ExecuteAsync on the same rental. OQL and typed CRUD commands share its
transaction. A current client negotiates `transaction-status`; CD07002 refuses controls only
when an older or third-party peer has not negotiated that capability. Ordinary autocommit work
remains available to that peer.

Returning a rental rolls back active or failed work before reuse and requires server-reported
Idle status. A failed reset, missing or invalid status, or a legacy peer that cannot certify
reuse causes discard. Disconnect also cleans up pending work.

Once COMMIT's Complete acknowledgement arrives, status cleanup uses a bounded internal token:
later caller cancellation does not turn that acknowledged commit into cancellation. A lost
completion or status can still make the call fail and leaves the caller unable to establish the
outcome; the client discards the connection and never retries COMMIT. Disconnect cannot undo
committed work. See the [client contract](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-documents-client/document-connection.md#text-transactions).

OQL CREATE/DROP COLLECTION and CREATE/DROP INDEX report CD04105 inside an explicit transaction,
including conditional no-ops, before changing the catalog. They fault the transaction. Run OQL
DDL in its automatic statement transaction. Typed C# collection operations retain their own
transaction contract.

SET TRANSACTION and EXPLAIN of transaction control remain
unsupported (CK00001). START requires WORK, TRAN or TRANSACTION. Other isolation spellings and
malformed controls report CD00002. Transaction words remain contextual identifiers.

## Named savepoints

SAVEPOINT and ROLLBACK TO SAVEPOINT follow Couchbase SQL++; RELEASE and omitting SAVEPOINT
after TO are Cohesion extensions. Names preserve decoded identifier spelling and compare
exactly, including unquoted names; double-quoted identifiers decode doubled quotes.

Duplicate names stack, unlike Couchbase's replacement behavior. The newest matching name wins.
RELEASE removes its target and every newer savepoint while retaining their effects. ROLLBACK
TO undoes changes and scoped locks after its target, removes newer savepoints and keeps the
target for repeated rollback. The transaction remains open for work and COMMIT. Controls affect
zero rows. Returning a pooled connection discards every savepoint.

An ordinary statement failure still aborts the transaction: ROLLBACK TO cannot recover it and
returns CD04101; whole ROLLBACK recovers the session. Incomplete undo instead reports CK04009
and admits only rollback to the same or an outer retained savepoint, or whole ROLLBACK. Reads,
writes, COMMIT and other named controls are fenced until recovery. The wire session stays open
and Ready reports a failed transaction (`E`). Pre-canceled controls change nothing; admitted undo
finishes despite later cancellation.

Database disposal takes precedence over CK04009. Direct transaction controls, refusals and
commit throw ObjectDisposedException while a live undo fence remains; in-process session
execution also observes disposal before that fence.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CK04004 | BEGIN/START while a transaction is open; faults the current transaction | — |
| CD04104 | COMMIT or ROLLBACK without an explicit transaction | — |
| CD04101 | Faulted explicit transaction refuses work or COMMIT | — |
| CD04105 | OQL collection/index DDL inside an explicit transaction | — |
| CD07002 | Text transaction control without the negotiated transaction-status capability | — |
| CK04012 | Named control requires an explicit transaction with an owned kernel binding | — |
| CK04013 | Savepoint name is unknown in the current transaction | — |
| CK04009 | Incomplete undo or statement/control/end activity prevents control admission | — |
| CK00001 | Unsupported SET TRANSACTION or EXPLAIN control | — |
| CD00002 | Malformed control or unsupported isolation spelling | — |

## Example

On one in-process session or one Documents client connection, execute separately:

```sql
CREATE COLLECTION items;
BEGIN;
INSERT INTO items (KEY, VALUE) VALUES ('pending', {'n': 1});
SELECT COUNT(*) FROM items;
COMMIT WORK;
BEGIN TRAN;
DELETE FROM items;
ROLLBACK TRANSACTION;
SELECT COUNT(*) FROM items;
```

Both counts return 1. Another session cannot see the pending insert before COMMIT. ROLLBACK
discards the deletion, leaving the committed document.

### Savepoint rollback

In a fresh database, execute separately on one session:

```sql
CREATE COLLECTION saved_items;
BEGIN;
INSERT INTO saved_items (KEY, VALUE) VALUES ('k1', {'n': 1});
SAVEPOINT Keep;
INSERT INTO saved_items (KEY, VALUE) VALUES ('k2', {'n': 2});
ROLLBACK TRAN TO SAVEPOINT Keep;
ROLLBACK TO Keep;
RELEASE SAVEPOINT Keep;
COMMIT;
SELECT COUNT(*) FROM saved_items;
```

The count is `1`; both rollbacks retain `Keep`. A reference to `keep` would be a different name.

## See also

[Statements](index.md) · [Diagnostics](../diagnostics.md) · [Document](../../index.md)

## Sources

- **Direct disposal precedence** — `cohesion/resources/Database/Assimalign.Cohesion.Database/tests/SavepointSeamTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/NamedSavepointTests.cs` and `SavepointFixTests.cs` in the same directory.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/NamedSavepointWireTests.cs` and `SavepointFailureWireTests.cs` in the same directory.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlTransactionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentTextTransactionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentTextTransactionWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentTransactionTextWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentTransactionCancellationTests.cs`.
