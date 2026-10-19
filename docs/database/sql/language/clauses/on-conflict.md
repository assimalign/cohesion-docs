# ON CONFLICT (Cohesion SQL)

Inserts a proposed row, skips a unique-key conflict, or updates the conflicting row.

> **Status:** Implemented for the subset below.

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

Latest-state arbitration waits on exclusive unique-key locks, held until transaction completion.
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
Constants and immutable defaults retain the complete up-front batch. Defaults are never evaluated
speculatively or replayed for lock preparation. Old/new replacement keys lock in a sorted batch
before conflicting rows; row locks form a sorted batch, followed by reference locks. No lock wait
runs inside the physical apply gate.

Reversed multi-row proposal orders with side-effecting defaults can deadlock. Consistent input
ordering reduces that exposure. Key-before-row acquisition can also form cycles with ordinary
UPDATE/DELETE and referential SET NULL/SET DEFAULT. Even an UPDATE leaving unique values unchanged
replaces index entries at a new heap location and needs their key locks. Detection reports
DatabaseTransactionDeadlockException; roll back the victim and retry its entire transaction,
including earlier statements. The engine does not perform an internal row-wait/re-arbitration retry.

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
| COHSQLE035 | No matching unique index, unknown/nonunique constraint target, legacy primary key without an index, or DO UPDATE without a target; fails planning | 42P10 |
| COHSQLE036 | A DO UPDATE proposal would affect a row already inserted or updated by this statement | 21000 |
| COHSQLE015 | Unknown/hidden qualifier or ambiguous target/EXCLUDED column | — |
| COHSQLE019 | Duplicate whole-column SET targets, rejected before writes | — |
| COHSQLE017 | Aggregate in conflict SET/WHERE | — |
| COHDBL001 | Expression/COLLATE/order/operator-class/partial-WHERE inference, DO SELECT, subqueries in conflict SET/WHERE, or EXCLUDED pseudo-relation in RETURNING | — |
| COHDBL001 | MySQL ON DUPLICATE KEY UPDATE or SQLite INSERT OR IGNORE/REPLACE; message suggests ON CONFLICT | — |

Ordinary expression, identity and constraint diagnostics retain their codes. MERGE remains SQL0002.
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

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlUpsertExecutionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlUpsertConcurrencyTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlUpsertDefaultConcurrencyTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlUpsertWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`.
