# Assimalign.Cohesion.Database.Documents.Protocol

Shared Documents wire messages, codecs and transferred values for engines and remote clients.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.Documents.Protocol`. Assembly: `Assimalign.Cohesion.Database.Documents.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Documents.Protocol`.

This package owns 14 public types formerly shipped in `Assimalign.Cohesion.Database.Documents`.
Both engine and client consume these same contracts and the single model family instance. Update
imports and package references and recompile. Namespaces and assembly identities change without
forwarders; wire bytes, disk formats, limits and validation behavior stay the same.

## Reference pages

- **[Design](design.md)** — Contract ownership and dependencies.
- **[Examples](examples/index.md)** — Codec use without an engine reference.
- **[DocumentProtocol](document-protocol.md)** — Public wire contract.
- **[DocumentProtocolCollectionMessage](document-protocol-collection-message.md)** — Public wire contract.
- **[DocumentProtocolCommandMessage](document-protocol-command-message.md)** — Public wire contract.
- **[DocumentProtocolCompleteMessage](document-protocol-complete-message.md)** — Public wire contract.
- **[DocumentProtocolExecuteMessage](document-protocol-execute-message.md)** — Public wire contract.
- **[DocumentProtocolFailureMessage](document-protocol-failure-message.md)** — Public wire contract.
- **[DocumentProtocolMessageType](document-protocol-message-type.md)** — Public wire contract.
- **[DocumentProtocolOperation](document-protocol-operation.md)** — Public wire contract.
- **[DocumentProtocolResultHeaderMessage](document-protocol-result-header-message.md)** — Public wire contract.
- **[DocumentProtocolResultMessage](document-protocol-result-message.md)** — Public wire contract.
- **[DocumentProtocolStoredDocumentMessage](document-protocol-stored-document-message.md)** — Public wire contract.
- **[Document](document.md)** — Public wire contract.
- **[DocumentId](document-id.md)** — Public wire contract.
- **[DocumentVersion](document-version.md)** — Public wire contract.

## Project references

| Reference | Kind |
| --- | --- |
| `Assimalign.Cohesion.Database.Protocol` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |

[Engine](../assimalign-cohesion-database-documents/index.md) · [Client](../assimalign-cohesion-database-documents-client/index.md) · [Database](../index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Protocol/docs/OVERVIEW.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Protocol/src/Assimalign.Cohesion.Database.Documents.Protocol.csproj`.
