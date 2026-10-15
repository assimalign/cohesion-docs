# Blob limitations

Blob supports streamed object operations with explicit limits on language, provisioning, and transfer behavior.

> **Status:** Partial. Core object storage and streaming are implemented; dedicated security and replication projects contain no implementation source.

## Supported surface

| Capability | Support | Scope |
|---|---|---|
| Named databases and containers | Supported | Engine APIs provide creation and deletion |
| Stream upload and download | Supported | Sequential content with bounded chunk transfer |
| Delete, properties, prefix listing | Supported | Engine and typed wire client |
| Atomic upload publication | Supported | Successful completion and transaction commit publish the object |
| Snapshot and read-committed transactions | Supported | Engine sessions; unsupported isolation is rejected |
| Large object client round trip | Supported (measured) | Client fixture transfers 256 MiB plus 123 bytes under a 64 MiB managed heap limit |

The measured round trip uses mebibytes (MiB) and is test evidence, not a general maximum size or a
throughput guarantee. Content exceeding available memory requires file-backed storage.

## Features outside the implemented surface

| Feature | Current behavior |
|---|---|
| Statement or query language | No Blob language; `ExecuteAsync` rejects commands |
| Database switching and server administration commands | Rejected by Blob command execution; startup fixes wire database scope |
| Database or container provisioning over the Blob wire family | No provisioning messages; use host-side engine APIs |
| Compiled-schema provisioning | Not implemented; catalog ownership markers and engine ownership enforcement exist |
| Configurable blob-name collation | Deferred; names and prefix matching are ordinal and case-sensitive |
| Built-in authorization policy | Not supplied; the server has an authenticator hook and defaults to trusting principals |
| Replication | No Blob replication implementation is present |
| Transfer resumption or exactly-once retry | Not supplied; loss of acknowledgement after commit leaves an uncertain client outcome |
| Connection multiplexing | Not supplied; one exchange per connection |
| Concurrent operations on one engine session | Not supported while an operation or stream is active |
| Random access through content streams | Streams are sequential; the client download stream is nonseekable |

`Assimalign.Cohesion.Database.Blob.Security` and
`Assimalign.Cohesion.Database.Blob.Replication` contain empty .NET project files in `src`.
Those package directories do not establish working security or replication APIs. Authentication
configuration belongs to the implemented `BlobDatabaseServer` surface.

## Lifetime limits

Applications must dispose streams; no finalizer commits a forgotten upload. Names and metadata
must fit a kernel record. Wire text fields are limited to 65,535 UTF-8 bytes. A content chunk
contains at most 65,536 bytes, and the shared frame payload ceiling is 16,777,216 bytes.

Every failed client exchange closes its connection. Early download or listing disposal aborts
unfinished work. Any downloaded bytes preceding a later failure are unverified partial content.

## See also

- **Blob** — [Engine overview](index.md).
- **Operations** — [Commit and failure semantics](operations.md).
- **Streaming client** — [Resource lifetime](streaming-client.md).
- **Database** — [Database documentation](../index.md).

## Sources

- **Engine scope and limits** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/docs/DESIGN.md`.
- **Client behavior** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Client/docs/OVERVIEW.md`.
- **Measured client fixture** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Client/README.md`.
- **Security project** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Security/src/Assimalign.Cohesion.Database.Blob.Security.csproj`.
- **Replication project** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Replication/src/Assimalign.Cohesion.Database.Blob.Replication.csproj`.
