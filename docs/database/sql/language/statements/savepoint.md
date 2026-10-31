# SAVEPOINT and RELEASE SAVEPOINT (Cohesion SQL)

Creates or releases a named undo scope inside an explicit transaction.

> **Status:** Implemented in process and over InMemory and TCP.

## Syntax

```syntaxsql
SAVEPOINT name [ ; ]
RELEASE [ SAVEPOINT ] name [ ; ]
ROLLBACK [ WORK | TRANSACTION ] TO [ SAVEPOINT ] name [ ; ]
```

## Remarks

Execute each control separately on the same session or client connection. Names follow SQL
identifier rules: unquoted names fold to lowercase; quoted names preserve case and decode
doubled quotes. Canonical rendering quotes the normalized name.

Duplicate names are allowed; the newest matching savepoint wins. RELEASE removes the named
savepoint and every newer savepoint, keeping their effects in the transaction. Releasing the
newest duplicate exposes the older one again. ROLLBACK TO undoes changes and scoped locks after
its target, removes newer savepoints and keeps the target. It can be repeated, leaves the
transaction open and allows later work and COMMIT. See [ROLLBACK](rollback-transaction.md).

Ordinary SQL statement failures roll back the statement and leave earlier work usable, including
ROLLBACK TO. Storage corruption and I/O failures in named controls, including parameter reads
and result preparation, abort the explicit transaction under #1364; I/O uses CS06041. A
recoverable incomplete undo instead retains CK04009. SET TRANSACTION refuses with CS04074
while any savepoint is active; releasing all savepoints permits otherwise-early configuration
again. ROLLBACK TO retains that refusal.

An incomplete undo fences reads, writes, COMMIT, SAVEPOINT and RELEASE with CK04009. Only
ROLLBACK TO the same or an outer retained savepoint, or whole ROLLBACK, is admitted. Successful
retry restores usability. Over the wire, the session remains open and Ready reports a failed
transaction (`E`). Returning a pooled connection rolls back and discards every savepoint.
Closing the database takes precedence over the fence: SQL reports CS04005 and keeps the wire
session open with Ready `E`, including repeated error/Ready exchanges on InMemory and TCP.
Pre-canceled controls change nothing; admitted undo finishes despite later caller cancellation.
Successful controls affect zero rows.

## Diagnostics

CK04009 refusals of SAVEPOINT, RELEASE and ROLLBACK TO carry the complete statement span and
retain the translated cause.

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CK04012 | Named control requires an explicit transaction with an owned kernel binding | — |
| CK04013 | No savepoint with that name in the current transaction | — |
| CK04009 | Incomplete undo or concurrent statement/control/end prevents admission; retry the target/outer scope or roll back | — |
| CS04074 | SET TRANSACTION while a named savepoint is active | 25001 |
| CS04005 | Database closed while an undo fence remains | — |
| CS06041 | I/O failure in a named control; aborts the explicit transaction | 58030 |

## Example

Execute separately on one session in a fresh database:

```sql
CREATE TABLE saved_items (id INT PRIMARY KEY);
BEGIN;
INSERT INTO saved_items VALUES (1);
SAVEPOINT keep;
INSERT INTO saved_items VALUES (2);
SAVEPOINT keep;
INSERT INTO saved_items VALUES (3);
ROLLBACK TO keep;
INSERT INTO saved_items VALUES (4);
ROLLBACK TO SAVEPOINT keep;
RELEASE keep;
ROLLBACK WORK TO keep;
RELEASE SAVEPOINT keep;
COMMIT;
SELECT id FROM saved_items;
```

SELECT returns only `1`. Both rollbacks to the newest `keep` remove work after that scope;
RELEASE exposes the older `keep`, whose rollback also removes row 2.

## See also

[BEGIN](begin-transaction.md) · [ROLLBACK](rollback-transaction.md) · [SET TRANSACTION](set-transaction.md) · [Statements](index.md) · [Diagnostics](../diagnostics.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/NamedSavepointTests.cs` and `SavepointFixTests.cs` in the same directory.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SavepointConformanceTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/NamedSavepointWireTests.cs` and `SavepointFailureWireTests.cs` in the same directory.
