# ON CONFLICT (Cohesion SQL)

Inserts a proposed row, skips a unique-key conflict, or updates the conflicting row.

> **Status:** Implemented. Supports the subset below.

## Syntax

```syntaxsql
insert_statement
ON CONFLICT [ ( column_name [ , ...n ] ) | ON CONSTRAINT constraint_name ]
{ DO NOTHING
| DO UPDATE SET column_name = { expression | DEFAULT } [ , ...n ]
    [ WHERE search_condition ] }
[ RETURNING { * | expression [ [ AS ] column_alias ] } [ , ...n ] ]
[ ; ]
```

The clause follows VALUES, DEFAULT VALUES or the complete INSERT query source, before RETURNING.
Leading WITH and materialized SELECT/set-operation sources use the same conflict path.

## Arguments

- **`column_name` target** — Infers every unique index with exactly that column set. Order does
  not matter; duplicate inference names collapse to the same set.
- **`constraint_name`** — Names a unique index on the target table, including a named UNIQUE
  constraint, a generated primary-key index name, or a standalone CREATE UNIQUE INDEX name.
  Generated names can be read from INFORMATION_SCHEMA.TABLE_CONSTRAINTS.
- **`DO NOTHING`** — Discards a conflicting proposal. Without a target, it arbitrates on all
  unique indexes; with no unique indexes, it inserts normally.
- **`DO UPDATE`** — Requires an explicit target and assigns the conflicting row. WHERE controls
  whether that update occurs; FALSE or UNKNOWN skips it.

WHERE requires a [Boolean search condition](where.md#boolean-search-conditions). A non-Boolean
condition fails planning with COHSQLE003 (42804), before a conflicting row is needed. NULL is
UNKNOWN and skips the update.

## Remarks

Arbiter keys use index collation and temporal normalization. NULL conflicts with NULL, including
a component of a composite unique key. This follows Cohesion's existing UNIQUE rule and differs
from PostgreSQL's default NULL-distinct behavior. SQL has no MISSING key value. A conflict outside
an explicit target still fails ordinary unique enforcement.

DO UPDATE SET/WHERE sees the original target row and `EXCLUDED`, the proposed stored row after
coercion, defaults and identity allocation. Qualify target references by its alias or, without an
alias, its table name. An alias hides the table name. Unqualified columns common to both relations
are ambiguous (`COHSQLE015`); a target named or aliased excluded also makes that qualifier ambiguous
in the conflict action. Every SET expression reads the original target and proposal, rather than
earlier assignments. SET c = DEFAULT uses ordinary UPDATE defaults. ALWAYS identity assignments
retain `COHSQLE025`.

Defaults and conflict projections run in proposal source order. Identity allocations are consumed
even for skipped proposals. For example, a BY DEFAULT identity proposal allocating 2 followed by
a conflict SET id = DEFAULT consumes 3 before the next proposal allocates 4.

NOT NULL, domains and CHECK validate every proposed row before arbitration, even one that will
be skipped. Foreign keys validate only inserted/updated rows: a discarded DO NOTHING proposal may
have a dangling foreign key, but cannot authorize another row's self-reference. Updated rows run
assigned-domain, NOT NULL, CHECK, outgoing foreign-key and incoming-reference checks. Final pending
replacements govern self-references: a key may move with its reference, while a rekeyed-away parent
cannot authorize a dangling child. See [foreign-key enforcement](../constraints.md).

DO NOTHING discards duplicate proposed arbiter keys. DO UPDATE cannot affect a row twice in one
statement (`COHSQLE036`), whether the first effect inserted or updated it. Rekeying releases an old
arbiter key in pending source-order state, allowing a later proposal to insert that released key.
A false/UNKNOWN filter locks the existing row but does not mark it affected.

All row/index writes and RETURNING evaluations share one atomic statement bracket. An error rolls
back the entire statement, preserving earlier successful statements in an explicit transaction.
RETURNING emits inserted and post-update rows in source order; skipped proposals emit no rows.
AffectedCount is inserted plus updated rows, excluding skips. EXCLUDED is outside RETURNING scope
and EXCLUDED.column there uses `COHDBL001`; a real target named or aliased excluded can still use
that qualifier for its target columns.

## Isolation, locking and retries

Latest-state arbitration uses exclusive unique-key locks. Successful writes retain their locks
until transaction completion; unwritten arbitration can release new grants as described below.
The table holds IntentExclusive (IX), allowing unrelated different-key inserts and upserts to progress.

| Isolation | Conflict committed after the statement snapshot |
| --- | --- |
| Snapshot (session and autocommit default) | Either action aborts retryably with DatabaseTransactionAbortedException, even before a false DO UPDATE filter |
| ReadCommitted | DO NOTHING skips the row; DO UPDATE uses its newest committed values |

Two autocommit Snapshot sessions racing DO NOTHING on a new key can produce one insert and one
aborted contender. Retry the aborted transaction. ReadCommitted refreshes committed visibility for
foreign-key validation after arbitration, admitting parents committed with the newest row while
excluding other transactions' in-flight parents. Own writes remain visible. A rolled-back contender
clears its index entries before releasing locks, allowing the waiter to insert.

Without requested side-effecting defaults, keys from every unique index of every proposal lock
in one sorted, deduplicated batch before arbitration. Requested identities or defaults containing
non-immutable calls, including STABLE and sequence-like application functions, instead evaluate
and lock each proposal's unique keys in source order before its lookup/conflict projection.
Constants and immutable defaults retain the complete up-front batch. Defaults evaluate as proposals
are prepared; lock preparation never evaluates them speculatively. DO UPDATE locks each live
conflicting row before evaluating its WHERE filter. A FALSE/UNKNOWN filter keeps that row locked
without changing it. Old/new replacement keys then lock in a sorted batch before reference locks.
DO NOTHING takes no conflicting-row lock; its unique-key lock protects the visibility check,
as in PostgreSQL. When every proposal conflicts and DO NOTHING writes nothing, it releases grants
acquired since its latest retry mark after those checks. With direct conflicts and no earlier
transaction locks or retained wait rows, keeping that transaction open blocks neither an UPDATE
of the parent nor a child INSERT referencing it. Earlier grants and retained wait rows remain held.

When a conflicting row is locked, the upsert releases grants acquired since its current retry
mark, waits for the row through the lock manager, keeps the granted row, and restarts arbitration
from the first proposal. Foreign-key parent-row waits use the same release/wait/retry path.
Later retries keep rows retained by earlier completed waits. A row-first UPDATE, DELETE or
referential SET NULL/SET DEFAULT action can therefore finish without a cycle involving the
upsert's new key locks. Keeping the wait grant lets the waiter make progress instead of repeatedly
handing the same row back to another contender.

ReadCommitted re-arbitrates against the newest committed state, inserting if a DELETE removed the
conflict. Snapshot aborts retryably if the holder committed a change it cannot see, including a
rekey or deletion that removes the original conflict. A holder's rollback lets either isolation
level proceed. After a successful statement, every row retained by a completed wait stays locked
until the transaction ends, even if final arbitration no longer conflicts with that row.
Pre-statement locks also survive retries, including upgrades taken during this statement.

The restart discards pending writes and re-evaluates identities and non-immutable defaults;
identity gaps and repeated default side effects are possible. No row/index write precedes completed
arbitration. RETURNING evaluates only the final effects, each once. No lock wait runs inside the
physical apply gate.

Any failure before apply begins releases grants acquired since the original statement mark,
including retained wait rows and arbiter keys. This includes cancellation, CHECK/NOT NULL failures,
COHSQLE036 and an unhandled deadlock; earlier transaction grants and their upgrades remain held.
Once apply begins, this cleanup no longer runs, even if apply fails; transaction-duration locking
applies. The [LockManager API](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-transactions/lock-manager.md)
documents MarkStatement, ReleaseSince and LockStatementMark.

Two multi-row upserts can still deadlock when they reach keys or retained rows in opposite orders,
including source-ordered proposals with side-effecting defaults. Locks from earlier transaction
work can also participate in cycles. As in PostgreSQL, detection chooses a retryable victim with
DatabaseTransactionDeadlockException. Roll back and retry the victim's entire transaction,
including earlier statements. Consistent input ordering reduces this residual exposure.

If unique-index DDL creates, drops or replaces indexes while the statement waits for its table
intent lock, it aborts retryably before default/identity allocation. Re-execution binds current
indexes; a dropped explicit arbiter may then produce COHSQLE035. A live index entry pointing at
a missing/undecodable heap row is storage corruption, not a serialization retry.

SQL BEGIN has no isolation-level clause. In-process callers select ReadCommitted through
`SqlDatabaseSession.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken)`.
See [transactions](../statements/begin-transaction.md).

## Diagnostics and limits

| Code | Meaning and trigger | SQLSTATE |
| --- | --- | --- |
| COHSQLE003 | Non-Boolean DO UPDATE WHERE condition or logical operand | 42804 |
| COHSQLE035 | No matching unique index, unknown/nonunique constraint target, legacy primary key without an index, or DO UPDATE without a target; fails planning | 42P10 |
| COHSQLE036 | A DO UPDATE proposal would affect a row already inserted or updated by this statement | 21000 |
| COHSQLE037 | Unique/primary-key or unique-index violation outside the handled conflict, including DO UPDATE violating a second unique index | 23505 |
| COHSQLE038 | A proposed or updated row fails a CHECK/domain check; proposed checks run even for DO NOTHING | 23514 |
| COHSQLE039 | An inserted/updated row lacks a parent, or a referenced parent key would be removed | 23503 |
| COHSQLE024 | A proposed or updated row violates NOT NULL | 23502 |
| COHSQLE015 | Unknown/hidden qualifier or ambiguous target/EXCLUDED column | — |
| COHSQLE019 | Duplicate whole-column SET targets, rejected before writes | — |
| COHSQLE017 | Aggregate in conflict SET/WHERE | — |
| COHDBL001 | Expression/COLLATE/order/operator-class/partial-WHERE inference, DO SELECT, subqueries in conflict SET/WHERE, or EXCLUDED pseudo-relation in RETURNING | — |
| COHDBL001 | MySQL ON DUPLICATE KEY UPDATE or SQLite INSERT OR IGNORE/REPLACE; message suggests ON CONFLICT | — |

Each constraint exception carries exactly one Error diagnostic, naming the constraint and table
without key values. Ordinary expression and identity diagnostics retain their codes. MERGE remains SQL0002.
Execution failures cross the wire as ExecutionFailure; capability refusals use ParseFailure.
Completed failures leave the connection usable. The client does not retry transactions automatically.

## Examples

Execute statements separately. This example follows SqlUpsertWireTests:

```sql
CREATE TABLE t (id INT PRIMARY KEY, age INT);
INSERT INTO t VALUES (1, 10);
INSERT INTO t VALUES (1, 20), (2, 30)
ON CONFLICT (id) DO UPDATE SET age = EXCLUDED.age RETURNING id, age;
INSERT INTO t VALUES (1, 40) ON CONFLICT DO NOTHING RETURNING *;
```

The first conflict statement returns `(1, 20)`, `(2, 30)` with count 2. DO NOTHING returns no
rows and count 0. Repeating `(1, 30), (1, 40)` with DO UPDATE instead reports COHSQLE036 and
leaves the stored rows unchanged.

```sql
CREATE TABLE people (id INT PRIMARY KEY, age INT, CONSTRAINT uq_age UNIQUE (age));
INSERT INTO people VALUES (1, 10);
INSERT INTO people VALUES (2, 10)
ON CONFLICT ON CONSTRAINT uq_age DO UPDATE SET age = EXCLUDED.age
WHERE people.age < EXCLUDED.age RETURNING id;
```

The filter is FALSE, so the result has no rows and count 0; people still contains `(1, 10)`.

```sql
CREATE TABLE default_rows (id INT PRIMARY KEY DEFAULT 1, age INT DEFAULT 5);
INSERT INTO default_rows VALUES (1, 10), (2, 20);
INSERT INTO default_rows SELECT id, age + 1 FROM default_rows
ON CONFLICT (id) DO UPDATE SET age = EXCLUDED.age;
INSERT INTO default_rows DEFAULT VALUES ON CONFLICT (id) DO UPDATE SET age = DEFAULT;
SELECT id, age FROM default_rows ORDER BY id;
```

The conflict statements affect 2 and 1 rows; SELECT returns `(1, 5)`, `(2, 21)`.

## See also

[Clauses](index.md) · [INSERT](../statements/insert.md) · [RETURNING](returning.md) · [Constraints](../constraints.md) · [Diagnostics](../diagnostics.md)

## Sources

- **Boolean conditions** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlBooleanPredicateTests.cs`.


- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlUpsertExecutionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlUpsertConcurrencyTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlUpsertArbitrationConformanceTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlUpsertDefaultConcurrencyTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlUpsertWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlConstraintDiagnosticWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`.
