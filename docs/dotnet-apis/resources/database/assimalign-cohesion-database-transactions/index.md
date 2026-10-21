# Assimalign.Cohesion.Database.Transactions

The shared ACID substrate for every Cohesion database engine: MVCC visibility snapshots, isolation levels, hierarchical locking for write-write conflict control, and the transaction-log seam that binds commits to the write-ahead log.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[TransactionCoordinator](transaction-coordinator.md)** — Lazy writer enrollment, snapshot pins and begin/commit/rollback lifecycle.
- **[LockManager](lock-manager.md)** — Writer locks, MarkStatement and ReleaseSince for unwritten arbitration.
- **[LockStatementMark](lock-statement-mark.md)** — Opaque grant boundary with value equality.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

The shared ACID substrate for every Cohesion database engine: MVCC visibility snapshots, isolation
levels, hierarchical locking for write-write conflict control, and the transaction-log seam that
binds commits to the write-ahead log.

## Scope

- **`TransactionManager`** — begin/commit/rollback lifecycle, sequence assignment, snapshot capture
- **`TransactionContext`** — the engine-internal state of one in-flight transaction
- **`TransactionId` / `TransactionState`** — the transaction vocabulary the area root's `IDatabaseTransaction` contract consumes
- **`TransactionSnapshot` / `TransactionSequence`** — MVCC visibility (implemented, tested)
- **`LockManager` / `LockMode` / `LockResource`** — hierarchical write locking with deadlock resolution;
  statement marks preserve earlier grants while an unwritten statement releases new resources
- **The internal transaction log** — the WAL binding (storage owns the physical journal)
- **`VersionStore`** — the version-chain base used by record-space and in-memory stores
- **`TransactionAbortedException`** — an independent exception root (inherits `Exception`, not `DatabaseException`)

The coordinator's record ledger supports streamed model records: logical rollback, pruning, and
recovery scrub use bounded physical mutation batches, while lifecycle analysis streams the shared
journal instead of retaining page-image payloads.

Read-only transactions reserve normal sequences and snapshots but append no lifecycle records and
wait for no flush. The first admitted write apply enrolls a writer with one BEGIN before its physical
bracket. Only writers append COMMIT/abort records, and only writer commits wait for durability.
See [coordinator lifecycle](transaction-coordinator.md) for completion, cancellation and recovery rules.

## Dependencies

- **`Assimalign.Cohesion.Database.Storage` (child-to-child** — the journal/page substrate the implementations bind to)

This package is a **child root** of the `Database` area: the area root
(`Assimalign.Cohesion.Database`) references it — never the reverse — so the ACID substrate stays
independently consumable.

## Consumers

Every model engine (`Sql`, `Documents`, `Graph`, `Blob`, `KeyValuePair`) composes a transaction
manager; execution operators carry `TransactionContext` into storage, index, and catalog
operations.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Storage` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/src/Assimalign.Cohesion.Database.Transactions.csproj`.
