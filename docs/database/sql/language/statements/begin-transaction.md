# BEGIN TRANSACTION (Cohesion SQL)

Starts the session's explicit transaction.

> **Status:** Implemented.

## Syntax

```syntaxsql
{ BEGIN [ WORK | TRANSACTION ] | START TRANSACTION }
    [ ISOLATION LEVEL level [ [ , ] ISOLATION LEVEL level ...n ] ] [ ; ]

<level> ::= READ COMMITTED | READ UNCOMMITTED | REPEATABLE READ | SERIALIZABLE
```

## Arguments

`WORK` and `TRANSACTION` are optional equivalent suffixes on BEGIN.
`START TRANSACTION` requires `TRANSACTION`. `START` and `WORK` remain contextual:
columns named `start` or `work` still bind.
Each repeated mode includes ISOLATION LEVEL; commas between complete modes are optional.
The last supported level wins. SERIALIZABLE anywhere in a state-valid mode list is refused.
Isolation words remain contextual identifiers. Transaction names, savepoints and `AND [NO] CHAIN`
are outside this grammar. READ ONLY, READ WRITE, DEFERRABLE and NOT DEFERRABLE use CK00001.
Malformed isolation modes use CS00012; other malformed suffixes use CS00003.

## Remarks

Optionless BEGIN and automatic transactions use Snapshot. The level mappings are:

| SQL level | Effective level | Visibility |
| --- | --- | --- |
| READ COMMITTED | ReadCommitted | Fresh committed snapshot for each statement |
| READ UNCOMMITTED | ReadCommitted | Same as READ COMMITTED; no dirty reads |
| REPEATABLE READ | Snapshot | Fixed data and catalog snapshot at BEGIN |
| SERIALIZABLE | Refused | CS04073 (0A000); no transaction starts |

Unlike PostgreSQL, which acquires its transaction snapshot at the first query, Cohesion fixes
Snapshot visibility at BEGIN. An early [SET TRANSACTION](set-transaction.md) from READ COMMITTED
to REPEATABLE READ restores that original BEGIN snapshot, even after a concurrent commit.
Neither SET nor an explicit level changes the default for future blocks or pooled reuse.

Later statements on the same session join
that transaction; its own writes are visible to it and remain hidden from other sessions until commit.
A second `BEGIN` reports `CS04001` and preserves the original active scope.

BEGIN reserves the transaction's sequence and snapshot without appending a journal record.
Its first admitted write apply enrolls it with a logical BEGIN before any physical write bracket.
A transaction that only reads appends no lifecycle records and waits for no flush on completion.
See [TransactionCoordinator](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-transactions/transaction-coordinator.md).

Data definition language (DDL) is rejected while the explicit transaction is active with
`CS04003`. Statements outside an explicit transaction auto-commit. Session disposal or wire
disconnect rolls back an unfinished transaction. Returning a client lease resets its negotiated server transaction state: an open or failed
transaction is rolled back before reuse, and an uncertified or failed reset discards the connector. `TRANSACTION` alone is not
a statement.

[TRUNCATE](truncate.md) has its own explicit-transaction refusal, CS04066 (SQLSTATE 25001).
Earlier work and the transaction's active state are kept. A snapshot predating another session's
TRUNCATE keeps its old rows, including joins, views and indexed reads, until it ends; reclamation
waits for all older snapshot pins.

An open wire transaction retains its acquired table-intent, row and unique-key locks even while
idle, until COMMIT, ROLLBACK, disconnect, pool return or server `IdleTimeout` (30 minutes by
default). Keep transactions short because an idle writer blocks conflicting writers. Graph uses
a database-wide writer lock; SQL retains locks on the affected resources. New clients do not pool
sessions against pre-#1540 servers that cannot negotiate transaction-status.

Storage, I/O and index failures abort an explicit transaction. Later statements or COMMIT
report CS04005 with the original coded cause; roll back before continuing. Ordinary constraint,
evaluation and Boolean-condition failures leave it usable. Connection usability does not mean
transaction usability; see [diagnostics](../diagnostics.md#transactions).

CS04005 is an Error in DatabaseException.Diagnostics; the in-process Message has no leading
code, and InnerException retains the cause. The wire still prefixes its refusal with CS04005.

The coordinator also has an API for selecting an isolation level. Nested transactions and
savepoints remain outside this language subset.

[ON CONFLICT](../clauses/on-conflict.md#isolation-locking-and-retries) uses latest-state unique-key
arbitration. Snapshot aborts retryably on a conflict committed after its snapshot, including
autocommit DO NOTHING; ReadCommitted skips or updates the newest committed row. Deadlock victims
must roll back and retry the complete transaction, including earlier statements.

The transaction captures one UTC start instant. CURRENT_TIMESTAMP/NOW(), CURRENT_DATE,
CURRENT_TIME, LOCALTIME and LOCALTIMESTAMP use it for all rows and later statements until COMMIT
or ROLLBACK, including SQL function bodies and child queries. Automatic statement transactions
capture their own start instant. See [clock examples](../functions/builtin-functions.md#clock-temporal-and-uuid-functions).

## Examples

Starting from the [conformance fixture](select.md#a-create-the-conformance-fixture), execute each
statement as a separate request on the same session or client connection:

```sql
START TRANSACTION;
INSERT INTO t VALUES (4, 'new', 1);
COMMIT TRANSACTION;
```

The inserted row becomes visible to an observing session after commit.

For explicit isolation, starting from that same fixture:

```sql
BEGIN WORK ISOLATION LEVEL READ UNCOMMITTED;
SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;
SELECT COUNT(*) FROM t;
ROLLBACK WORK;
```

The count is `4` when run after the committed example, with no other writer. The block begins as
ReadCommitted and switches to its BEGIN snapshot before the SELECT.

## Diagnostics

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| CS00012 | Malformed/incomplete isolation mode, including ISOLATION LEVEL SNAPSHOT | 42601 |
| CS04001 | BEGIN while a usable transaction is already open; checked before SERIALIZABLE | — |
| CS04005 | Existing transaction is aborted; roll it back before continuing | — |
| CS04073 | SERIALIZABLE appears anywhere in a state-valid BEGIN mode list | 0A000 |
| CK00001 | READ ONLY, READ WRITE, DEFERRABLE or NOT DEFERRABLE | — |

## See also

[SET TRANSACTION](set-transaction.md) · [COMMIT](commit-transaction.md) · [ROLLBACK](rollback-transaction.md)

[Statements](index.md) · [Diagnostics](../diagnostics.md)

## Execution deadlines

SQL transaction-control requests arm neither StatementTimeout nor LockTimeout. Timed DML rolls back
only its own effects: CS05043 (57014) or CS04044 (55P03) leaves earlier statements and the
explicit transaction usable. Earlier lock resources and their upgrades remain held. A started
commit completes with its real outcome. A target invalidated during lock preparation instead uses
CS04006 (no SQLSTATE); roll back and retry in a fresh transaction. See
[timeouts](../diagnostics.md#statement-and-lock-timeouts).


## Sources

- **Dialect contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Execution cases** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlLanguageConformanceTests.cs`
- **Transaction behavior** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTransactionControlTests.cs`
- **Client lifetime** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/docs/DESIGN.md`
- **Coordinator** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/docs/OVERVIEW.md`
- **Isolation and visibility** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlIsolationSyntaxTests.cs`, `SqlIsolationReviewTests.cs` and `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlIsolationWireTests.cs`.
