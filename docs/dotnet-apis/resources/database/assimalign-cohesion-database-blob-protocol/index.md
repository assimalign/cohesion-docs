# Assimalign.Cohesion.Database.Blob.Protocol

Shared Blob wire messages, codecs and transferred values for engines and remote clients.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.Blob.Protocol`. Assembly: `Assimalign.Cohesion.Database.Blob.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Blob.Protocol`.

This package owns 15 public types formerly shipped in `Assimalign.Cohesion.Database.Blob`.
Both engine and client consume these same contracts and the single model family instance. Update
imports and package references and recompile. Namespaces and assembly identities change without
forwarders; wire bytes, disk formats, limits and validation behavior stay the same.

## Reference pages

- **[Design](design.md)** — Contract ownership and dependencies.
- **[Examples](examples/index.md)** — Codec use without an engine reference.
- **[BlobChunkAcknowledgementMessage](blob-chunk-acknowledgement-message.md)** — Public wire contract.
- **[BlobChunkMessage](blob-chunk-message.md)** — Public wire contract.
- **[BlobDeleteMessage](blob-delete-message.md)** — Public wire contract.
- **[BlobGetPropertiesMessage](blob-get-properties-message.md)** — Public wire contract.
- **[BlobListMessage](blob-list-message.md)** — Public wire contract.
- **[BlobOperationCompleteMessage](blob-operation-complete-message.md)** — Public wire contract.
- **[BlobPropertiesMessage](blob-properties-message.md)** — Public wire contract.
- **[BlobProtocol](blob-protocol.md)** — Public wire contract.
- **[BlobProtocolMessageType](blob-protocol-message-type.md)** — Public wire contract.
- **[BlobProtocolTransfer](blob-protocol-transfer.md)** — Public wire contract.
- **[BlobReadMessage](blob-read-message.md)** — Public wire contract.
- **[BlobTransferCompleteMessage](blob-transfer-complete-message.md)** — Public wire contract.
- **[BlobTransferStartMessage](blob-transfer-start-message.md)** — Public wire contract.
- **[BlobWriteMessage](blob-write-message.md)** — Public wire contract.
- **[BlobProperties](blob-properties.md)** — Public wire contract.

## Project references

| Reference | Kind |
| --- | --- |
| `Assimalign.Cohesion.Database.Protocol` | `CohesionProjectReference` |

[Engine](../assimalign-cohesion-database-blob/index.md) · [Client](../assimalign-cohesion-database-blob-client/index.md) · [Database](../index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Protocol/docs/OVERVIEW.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Protocol/src/Assimalign.Cohesion.Database.Blob.Protocol.csproj`.
