# SET TRANSACTION (Cohesion SQL)

Changes the current block's isolation before its first admitted data statement.

> **Status:** Implemented.

## Syntax

```syntaxsql
SET TRANSACTION ISOLATION LEVEL level
    [ [ , ] ISOLATION LEVEL level ...n ] [ ; ]

<level> ::= READ COMMITTED | READ UNCOMMITTED | REPEATABLE READ | SERIALIZABLE
```

## Remarks

READ COMMITTED and READ UNCOMMITTED select ReadCommitted; REPEATABLE READ selects Snapshot.
Repeated supported modes use the last level. Every mode repeats ISOLATION LEVEL; separating
commas are optional. Within a usable block, any SERIALIZABLE mode reports CS04073. READ ONLY,
READ WRITE, DEFERRABLE and NOT DEFERRABLE remain CK00001 parser refusals.

Repeated early SET is allowed. The first admitted data statement closes configuration, including
SELECT, plain EXPLAIN, failing binding/evaluation, and unsuccessful DDL attempts. Admission closes
it before metadata interlocks or deferred execution. A supported SET during an operation or after data uses
CS04072, even if it requests the current level. Parse failures, refused transaction controls and
requests cancelled before admission leave configuration open. Failed SET leaves the block usable.

Snapshot visibility and catalog state stay fixed at the original BEGIN, including an early switch
from READ COMMITTED after another session commits. PostgreSQL instead acquires its transaction
snapshot at the first query and permits an idempotent late SET; Cohesion refuses late SET.
ReadCommitted refreshes visibility per statement. SET takes no data locks and preserves transaction
identity, sequence and start time.

Outside a block, SET succeeds with zero affected rows and Warning CS04071, including when the
requested level is SERIALIZABLE. It changes no default. An aborted block retains its ordinary
transaction refusal before isolation validation. BEGIN and automatic transactions continue to
default to Snapshot; DISCARD ALL and pooled reuse do not retain the configured level.
Within a usable block, an active [savepoint](savepoint.md) refuses SET with CS04074 before
SERIALIZABLE or late-SET validation. RELEASE of all savepoints permits otherwise-early SET;
ROLLBACK TO keeps its target and the refusal. Without savepoints, SERIALIZABLE validation
precedes the late-SET check.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS00012 | Missing, malformed or incomplete isolation mode | 42601 |
| CS04071 | Outside a transaction block: warning and successful no-op | 25000 |
| CS04072 | Configuration after the first admitted data statement or during an operation | 25001 |
| CS04073 | SERIALIZABLE in an active usable block | 0A000 |
| CS04074 | SET while any named savepoint is active | 25001 |
| CS04005 | Transaction is aborted; roll back before continuing | — |
| CK00001 | Unsupported READ ONLY/WRITE or [NOT] DEFERRABLE mode | — |

## Examples

Execute each statement separately on one session:

```sql
BEGIN ISOLATION LEVEL READ COMMITTED;
SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;
SELECT 1;
SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;
ROLLBACK;
SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
```

SELECT returns `1`. The second SET reports CS04072 because SELECT closed configuration; ROLLBACK
still succeeds. The final SET succeeds with Warning CS04071 and zero affected rows.

## See also

[BEGIN TRANSACTION](begin-transaction.md) · [Statements](index.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Grammar** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlIsolationParserTests.cs`.
- **Execution and admission** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlIsolationSyntaxTests.cs` and `SqlIsolationReviewTests.cs` in the same directory.
- **Wire behavior** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlIsolationWireTests.cs`.
