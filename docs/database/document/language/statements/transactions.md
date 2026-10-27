# OQL transactions

Begins, commits or rolls back an explicit transaction on one Documents session.

> **Status:** Implemented in process and over InMemory and TCP through the Documents client.

## Syntax

```syntaxsql
{ BEGIN [ WORK | TRAN | TRANSACTION ] | START { WORK | TRAN | TRANSACTION } }
    [ ISOLATION LEVEL { SNAPSHOT | READ COMMITTED } ] [ ; ]
COMMIT [ WORK | TRAN | TRANSACTION ] [ ; ]
ROLLBACK [ WORK | TRAN | TRANSACTION ] [ ; ]
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

SAVEPOINT, ROLLBACK TO SAVEPOINT, SET TRANSACTION and EXPLAIN of transaction control remain
unsupported (CK00001). START requires WORK, TRAN or TRANSACTION. Other isolation spellings and
malformed controls report CD00002. Transaction words remain contextual identifiers.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CK04004 | BEGIN/START while a transaction is open; faults the current transaction | — |
| CD04104 | COMMIT or ROLLBACK without an explicit transaction | — |
| CD04101 | Faulted explicit transaction refuses work or COMMIT | — |
| CD04105 | OQL collection/index DDL inside an explicit transaction | — |
| CD07002 | Text transaction control without the negotiated transaction-status capability | — |
| CK00001 | Unsupported savepoint, SET TRANSACTION or EXPLAIN control | — |
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

## See also

[Statements](index.md) · [Diagnostics](../diagnostics.md) · [Document](../../index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlTransactionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentTextTransactionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentTextTransactionWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentTransactionTextWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentTransactionCancellationTests.cs`.
