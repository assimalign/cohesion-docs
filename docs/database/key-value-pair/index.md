# Key-Value Pair

The Key-Value Pair engine stores opaque byte keys and values in an ordered key space.

> **Status:** Implemented. Expiration, named key spaces, and several advanced features remain deferred.

`KeyValueDatabaseEngine` composes the shared Database storage, transaction, and indexing kernel.
Each database has one implicit key space. Keys compare by unsigned lexicographic byte order;
string collation settings do not change key equality or ordering.

Model Catalog and Storage components ship inside their model assembly. Wire messages, codecs and
transferred values ship in the model Protocol package, shared by engine and client. Remote clients
reference contracts without engines. See [package/import/error migration](../model-project-migration.md).

## Reference

- **[Commands](commands/index.md)** — point reads, conditional writes, ordered scans, discovery and registered CALL functions.
- **[Operand types](operand-types.md)** — parameter binding and accepted scalar types.
- **[Key-space discovery](key-space-discovery.md)** — the metadata returned by `KEYSPACES`.
- **[Wire protocol](wire-protocol.md)** — execution frames, result shapes, and scalar encoding.
- **[Diagnostics](diagnostics.md)** — parse errors, conditional misses, and execution failures.
- **[Unsupported features](unsupported.md)** — grammar exclusions and deferred engine capabilities.

## Execution and storage

`KeyValueDatabase` exposes `GetAsync`, `PutAsync`, `DeleteAsync`, `ExistsAsync`, and `ScanAsync`.
The session text surface parses commands into the same typed requests as the typed execution seam.
A session addresses exactly one database; typed operations reject a session from another database.
Database creation, opening, and deletion belong to the host-owned engine.

Reads use the unique primary B+Tree index and the command's visibility snapshot. An entry's
`ETag` is the transaction sequence that wrote its visible version. Conditional writes report
whether they applied; a condition miss does not throw an exception. A concurrent committed write
can instead produce a retryable transaction conflict.

Commands use the session's explicit transaction when present and auto-commit otherwise. The
default isolation is `Snapshot`; `ReadCommitted` refreshes visibility per command. `Serializable`
is rejected. The text grammar and wire protocol do not expose explicit transaction control.

Caller cancellation before explicit commit starts leaves the transaction usable and commits
nothing. A started commit ignores caller cancellation and returns its real outcome. Automatic
commit takes over without the command token after execution completes; `PutAsync` also delivers
its materialized outcome without that token. Execution and lock waits remain cancellable. A dead
wire connection can still lose the result frame. See [commit and rollback](../../dotnet-apis/resources/database/assimalign-cohesion-database-transactions/transaction-coordinator.md#commit-and-rollback).

Storage/index corruption and I/O failures abort an explicit transaction; CV04101 refuses
further commands until rollback. Ordinary command errors retain statement atomicity and keep
the transaction usable. Storage/I/O uses CV06105 (58030 for I/O); corruption uses CK06003
(XX001), with CK06102 and CV06105 additionally retained for attached-index damage.
See [diagnostics](diagnostics.md#storage-failures-and-transaction-recovery).

Aborted commands and COMMIT refusals carry CV04101 as a structured Error with the original
cause. The in-process Message is unprefixed; the wire message still starts with CV04101.

Storage is in memory when `KeyValueDatabaseEngineOptions.RootPath` is omitted. File-backed
databases use two file sets: `<name>` for entries and index pages, and `<name>.catalog` for
index registrations and the entry-format version. `KeyValueStorage` binds this model to the
shared storage kernel; it does not implement a separate physical storage system.

## Missing file sets and interrupted drop

File-backed open requires the original data and catalog file sets, including each journal and
backup. With the primary data file present, missing sidecars in an existing file set raise
DatabaseException with CV06104 in Diagnostics (Error, no
assigned SQLSTATE), before replacement files can be created. Preserve all files and restore the
complete backup, since the journal may contain acknowledged commits. Drop deletes keyvalue.dat
first, then other data files, then the remaining directory. It cannot resurrect a partially
dropped database by recreating a sidecar.

`OpenDatabaseAsync` refuses an entirely missing catalog with `CV06106` (Error, no assigned
SQLSTATE) before either storage opens. For file-backed data of at most one page, creation
stopped before both initial pages were flushed: follow the refusal's `DropDatabaseAsync`, then
`CreateDatabaseAsync` remedy. Longer data without a catalog requires preserving all storage
and restoring the complete database from backup. In-memory missing catalogs are also refused.
These refused opens change no files and create no replacement catalog. Existing catalogs are
decoded and format-validated before data opens or recovers; create still creates both sets.

## Client

`KeyValueClient.Create` creates a pooling `KeyValueClient`; `ConnectAsync` rents an
`KeyValueConnection`. The client turns typed operations into the command grammar and decodes
the model's results. Disposing a rented connection returns it to the pool. Conditional write
results use `KeyValueWriteResult` or Boolean outcomes rather than exceptions.

## See also

- **[Database](../index.md)** — database engines and shared concepts.
- **[Database overview](../overview.md)** — the shared kernel.
- **[Database API reference](../../dotnet-apis/resources/database/index.md)** — resource assemblies.

## Engine health
Background write-back, group-commit flush, version purge and checkpoint work run per database
on capped lanes: 64 non-stuck operations and at most 128 lane threads per engine/worker kind.
Admission is round-robin; stuck I/O releases its admission slot after four worker intervals,
clamped to 1–4 seconds, while keeping storage ownership. Idle checks dispatch no lane; parked
runners retire after 30 seconds. Saturation/stuck observations leave databases serving and emit
root warning 35; disposal joins in-flight I/O and emits warning 36 periodically. See
[background worker limits and operations](../../dotnet-apis/resources/database/assimalign-cohesion-database/design.md#maintenance-lane-isolation).
SQL/Key-Value index-maintenance bodies remain empty; they dispatch no production lane work.


Persistent version-purge refusals are reported once after three actual attempts spanning at least
one second on the engine clock. They set engine Faulted while the database stays in service and
HasFailingWorker remains false for the observation. ForeignStorage refusals report immediately as
operational failures. See [purge behavior](../../dotnet-apis/resources/database/assimalign-cohesion-database-keyvaluepair/design.md#version-purge-refusals)
and [shared engine health](../overview.md#engine-health-and-version-purge).

## Sources

- **Engine** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/OVERVIEW.md`.
- **Design** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/DESIGN.md`.
- **Catalog** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/OVERVIEW.md`.
- **Storage** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/OVERVIEW.md`.
- **Client** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Client/docs/OVERVIEW.md`.
