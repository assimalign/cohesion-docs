# Blob

The Blob engine stores named containers and streams opaque objects with atomic publication.

> **Status:** Implemented. The engine, catalog, storage, wire server, and streaming client are present; replication and compiled-schema provisioning are not implemented.

`Assimalign.Cohesion.Database.Blob` implements logical databases, containers, and objects. Create
an engine with `BlobDatabaseEngine.Create`. A null `RootPath` selects memory storage; a path
selects durable files. File-backed databases use `blob.dat`, `blob.log`, and `blob.bak` beneath
the database directory. File-backed storage is required for content larger than available memory.

Blob has no statement or query language. Its `ExecuteAsync` rejects commands. Provision databases
and containers through the engine application programming interface (API); use container streams
locally or the typed client remotely.

Model Catalog and Storage components ship inside their model assembly. Wire messages, codecs and
transferred values ship in the model Protocol package, shared by engine and client. Remote clients
reference contracts without engines. See [package/import/error migration](../model-project-migration.md).

## Documentation

- **Operations** — [Requests, responses, and publication](operations.md).
- **Streaming client** — [Upload, download, metadata, and connection lifetime](streaming-client.md).
- **Wire protocol** — [Shared envelope and Blob message layouts](wire-protocol.md).
- **Limitations** — [Unsupported features and package status](unsupported.md).

## Publication and transactions

`IBlobContainer.OpenWriteAsync` returns a sequential upload stream. Successful disposal finishes
the content and publishes it in an automatic transaction. `Flush` can persist buffered chunks
but does not publish the object. Failed or canceled uploads abort. Replacements build a new
content chain; existing readers retain their selected version until their streams close.

Direct database and container operations use automatic transactions. For an explicit transaction,
create a session, begin a transaction, and obtain containers through the `IBlobDatabase` returned
by `session.Database`. Successful upload disposal then finishes the statement; committing the
transaction publishes the changes. Close every stream before another operation or commit on the
same session. Session disposal aborts pending work.

The engine supports snapshot and read-committed isolation. Writers take a database exclusive lock
for the upload lifetime, while snapshot readers can continue concurrently. Streams are sequential
and are not thread-safe.

Cancellation observed before explicit commit starts leaves the transaction usable with nothing
committed. A started commit ignores caller cancellation and returns its real outcome. Automatic
commit takes over after execution completes; earlier execution and lock waits remain cancellable.
A dead wire connection can still lose the result. See [commit and rollback](../../dotnet-apis/resources/database/assimalign-cohesion-database-transactions/transaction-coordinator.md#commit-and-rollback).

After a successful automatic commit, snapshot-cleanup failure preserves success and emits Warning
event 14, `StatementCleanupFailed`. `OutOfMemoryException` still propagates. See
[cleanup diagnostics](operations.md#operation-cleanup).

## Names, properties, and discovery

Container names, blob names, sorted listings, and prefix matching use case-sensitive ordinal
comparison. A slash in a blob name is ordinary name content. Database lookup is case-insensitive;
database names must be single file-name components. Structured Query Language (SQL) collation
settings do not affect Blob.

`IBlobDatabase.GetContainersAsync` discovers containers. `IBlobContainer.GetBlobsAsync` lists
metadata with an optional name prefix, without scanning content pages. `GetPropertiesAsync`
returns length, content type, entity tag, timestamps, and the content cyclic redundancy check
(CRC-32). Empty objects and replacements are supported.

Current catalog lookups read three metadata records after 256 overwrites in the tests, also
after reopen. Old snapshots retain needed history. The purge worker retires obsolete references
for idle names without later lookups; sustained overwrites with purge/checkpoint work reuse
pages and reach a data-file plateau. See [catalog history](../../dotnet-apis/resources/database/assimalign-cohesion-database-blob/catalog-design.md#bounded-version-history).

The `GetOwnershipAsync` extension returns a read-only property bag with `OWNER` and
`OWNING_SCHEMA`. Runtime-created containers are `DatabaseObjectOwner.Adhoc`. A container marked
`DatabaseObjectOwner.Schema` cannot be dropped through the engine, but a compiled-schema
provisioner is not supplied. This discovery extension is an engine API, not an additional wire
operation.

## Server composition

`BlobDatabaseServer` accepts an engine and `BlobDatabaseServerOptions` with a generic
`IDuplexConnectionListener`. The composition root supplies the transport and retains engine ownership.
`StartAsync` binds the listener; `StopAsync` drains work and releases it. A stopped server cannot
restart; create a new server and listener.

Startup selects one logical database for the session. The default authenticator trusts every
principal; set `Authenticator` to supply authentication. Session limits, authentication deadlines,
idle eviction, and bounded shutdown are configurable. The server exposes the five object
operations described in [Operations](operations.md).

## See also

- **Database** — [Database documentation](../index.md) and [overview](../overview.md).
- **API reference** — [Database resource APIs](../../dotnet-apis/resources/database/index.md).

## Database lifecycle

Detected storage damage uses [COHDBS003](operations.md#database-file-diagnostics) through
DatabaseCorruptionException with database and optional page context. A damaged read fails;
refused corrupt opens also retain COHDBB004. Restore from a verified backup as directed by the
error. No SQLSTATE is assigned.

Create/open require the original blob.dat, blob.log and blob.bak file set. Missing sidecars
are refused with COHDBB004 and never recreated, including for an empty closed database. Preserve
ambiguous files and restore the complete backup. A directory without data or a data file no larger
than one page is an incomplete create; its refusal directs DropDatabaseAsync then CreateDatabaseAsync.
Drop deletes the data file first. Recovery tolerates a torn uncommitted extension at the tail;
transient I/O failures ask for an open retry after resolution, without restore advice.

See [lifecycle APIs and examples](../../dotnet-apis/resources/database/assimalign-cohesion-database-blob/design.md#database-creation-open-and-drop)
and [file diagnostics](operations.md#database-file-diagnostics).

## Sources

- **Engine** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/docs/OVERVIEW.md`.
- **Design** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/docs/DESIGN.md`.
- **Catalog** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/docs/OVERVIEW.md`.
- **Storage** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/docs/OVERVIEW.md`.
