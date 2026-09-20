# Assimalign.Cohesion.Database.Blob

`Assimalign.Cohesion.Database.Blob` implements named databases, containers, and streamed objects.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`BlobChunkAcknowledgementMessage`](blob-chunk-acknowledgement-message.md)** — Documented public type.
- **[`BlobChunkMessage`](blob-chunk-message.md)** — Documented public type.
- **[`BlobProtocol`](blob-protocol.md)** — Documented public type.
- **[`BlobProtocolMessageType`](blob-protocol-message-type.md)** — Documented public type.
- **[`BlobProtocolTransfer`](blob-protocol-transfer.md)** — Documented public type.
- **[`BlobReadMessage`](blob-read-message.md)** — Documented public type.
- **[`BlobTransferCompleteMessage`](blob-transfer-complete-message.md)** — Documented public type.
- **[`BlobTransferStartMessage`](blob-transfer-start-message.md)** — Documented public type.
- **[`BlobWriteMessage`](blob-write-message.md)** — Documented public type.

`Assimalign.Cohesion.Database.Blob` implements named databases, containers, and streamed objects.
`Create` an engine with `BlobDatabaseEngine.Create`; a null `RootPath` selects memory, and a path
selects durable files. `AddBlob((context, engine) => ...)` captures construction through the root
`IDatabaseApplicationBuilder` and returns that builder. At `Build` the callback configures
`IBlobDatabaseEngineBuilder`, including an optional borrowed `IBlobStorageStrategy` and deferred
nested worker/server factories. The application owns the resulting engine and its nested components.
The feature has no Hosting reference.

See the [source-backed usage examples](examples/index.md).

Successful upload disposal publishes the completed content atomically. `Flush` writes buffered chunks
but does not publish the object. Failed or cancelled uploads abort. Readers retain the selected
version until their streams close. Listings use catalog metadata and optional ordinal name prefixes.
Empty objects, replacements, checksums, and persisted timestamps are supported.

`For` explicit transactions, create a session and use the `IBlobDatabase` returned by its existing
`Database` property. Containers obtained from that view stay bound to the session.

See the [source-backed usage examples](examples/index.md).

Direct database/container operations use automatic transactions. Session operations use the active
explicit transaction when present; otherwise they also use automatic transactions. Close each stream
before starting another operation on the same session or committing. Disposing a session aborts its
pending work. Blob has no statement or query language: `ExecuteAsync` rejects commands, including
database switching and server administration. Creating and dropping logical databases remains the
host-side engine API.

The package also supplies `BlobDatabaseServer`, created with the engine and a
`BlobDatabaseServerOptions.Listener` implementing `IConnectionListener`. Call `StartAsync` to bind
and accept, then `StopAsync` to drain and terminally release the listener. The composition root
retains engine ownership. Startup binds each authenticated session to one database; requests
identify only containers and objects in that database. The default authenticator trusts every
principal; configure `Authenticator` for authenticated access. Session limits, authentication
deadlines, idle eviction, and bounded two-phase shutdown are configurable. A non-running engine
rejects new sessions and operations.

The package owns the Blob wire message family. `Bind` a `ProtocolChannel` to `BlobProtocol.Family` at
the Blob endpoint. `BlobReadMessage` and `BlobWriteMessage` identify an object;
`BlobProtocolTransfer.SendAsync` and `ReceiveAsync` copy its content using at most one 64 KiB chunk
in flight, with a receiver acknowledgement after each destination write. The helpers accept
non-seekable streams and unknown lengths. The caller supplies the shared handshake, request
dispatch, authentication, and upload publication when using the helpers directly. The server
supplies those responsibilities. The separate
[Blob.Client](../assimalign-cohesion-database-blob-client/index.md) package provides stream
upload/download and typed delete, property, and prefix-listing operations over the shared
`Database.Client` connection. An upload is acknowledged only after complete content validation and
transaction commit. Cancellation, disconnect, malformed completion, or engine failure before commit
rolls back, preserving the previous object. A connection loss after commit but before its
acknowledgement leaves the caller uncertain whether publication succeeded. `Verify` properties before
retrying when that distinction matters.

Dependencies are Connections, the `Database` root, `Database.Protocol`, `Blob.Storage`, Blob.Catalog,
`Database.Storage`, and `Database.Transactions`. The implementation targets .NET 10, Preview C#, and
NativeAOT without reflection or Microsoft.Extensions packages. It references no concrete transport.
Transport construction belongs to the composition root; security policy beyond the supplied
authenticator, replication, hosting integration, and compiled-schema provisioning remain outside
this package.

See [DESIGN.md](design.md) , the
[storage format](../assimalign-cohesion-database-blob-storage/design.md) , and the
[catalog format](../assimalign-cohesion-database-blob-catalog/design.md) .

`BlobDatabaseEngine.CreateBuilder()` returns the same model builder for standalone composition or
the concrete hosting builder's build-aware engine factory. This lets the consumer pass already
resolved values and register nested components while keeping the model package dependency-free.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Protocol` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Transactions` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Blob.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Blob.Catalog` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/src/Assimalign.Cohesion.Database.Blob.csproj`.
