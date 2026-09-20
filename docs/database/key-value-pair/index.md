# Key-Value Pair

The Key-Value Pair engine stores opaque byte keys and values in an ordered key space.

> **Status:** Implemented. Expiration, named key spaces, and several advanced features remain deferred.

`KeyValueDatabaseEngine` composes the shared Database storage, transaction, and indexing kernel.
Each database has one implicit key space. Keys compare by unsigned lexicographic byte order;
string collation settings do not change key equality or ordering.

## Reference

- **[Commands](commands/index.md)** — point reads, conditional writes, ordered scans, and discovery.
- **[Operand types](operand-types.md)** — parameter binding and accepted scalar types.
- **[Key-space discovery](key-space-discovery.md)** — the metadata returned by `KEYSPACES`.
- **[Wire protocol](wire-protocol.md)** — execution frames, result shapes, and scalar encoding.
- **[Diagnostics](diagnostics.md)** — parse errors, conditional misses, and execution failures.
- **[Unsupported features](unsupported.md)** — grammar exclusions and deferred engine capabilities.

## Execution and storage

`IKeyValueDatabase` exposes `GetAsync`, `PutAsync`, `DeleteAsync`, `ExistsAsync`, and `ScanAsync`.
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

Storage is in memory when `KeyValueDatabaseEngineOptions.RootPath` is omitted. File-backed
databases use two file sets: `<name>` for entries and index pages, and `<name>.catalog` for
index registrations and the entry-format version. `KeyValueStorage` binds this model to the
shared storage kernel; it does not implement a separate physical storage system.

## Client

`KeyValueClient.Create` creates a pooling `IKeyValueClient`; `ConnectAsync` rents an
`IKeyValueConnection`. The client turns typed operations into the command grammar and decodes
the model's results. Disposing a rented connection returns it to the pool. Conditional write
results use `KeyValueWriteResult` or Boolean outcomes rather than exceptions.

## See also

- **[Database](../index.md)** — database engines and shared concepts.
- **[Database overview](../overview.md)** — the shared kernel.
- **[Database API reference](../../dotnet-apis/resources/database/index.md)** — resource assemblies.

## Sources

- **Engine** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/OVERVIEW.md`.
- **Design** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/DESIGN.md`.
- **Catalog** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Catalog/docs/OVERVIEW.md`.
- **Storage** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Storage/docs/OVERVIEW.md`.
- **Client** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Client/docs/OVERVIEW.md`.
