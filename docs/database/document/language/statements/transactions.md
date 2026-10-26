# OQL transactions

Begins, commits or rolls back an explicit transaction on one in-process Documents session.

> **Status:** Implemented in process. Documents servers refuse text transaction control over both InMemory and TCP with `CD07002` in this batch.

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
| CD07002 | Documents server refuses text transaction control in this batch | — |
| CK00001 | Unsupported savepoint, SET TRANSACTION or EXPLAIN control | — |
| CD00002 | Malformed control or unsupported isolation spelling | — |

## Example

On an in-process session, execute separately:

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
