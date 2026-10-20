# Assimalign.Cohesion.Database.Documents

`Assimalign.Cohesion.Database.Documents` implements named logical databases containing versioned UTF-8 JSON documents.

> **Status:** Partial.

Assembly: `Assimalign.Cohesion.Database.Documents.dll`. NuGet package: `Assimalign.Cohesion.Database.Documents`.

Catalog and Storage ship in this assembly with their existing namespaces. The former satellite
packages are retired. Wire contracts ship separately in [Documents.Protocol](../assimalign-cohesion-database-documents-protocol/index.md).

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[DocumentDatabaseServer](document-database-server.md)** and [options](document-database-server-options.md).
- **[DocumentProtocol](../assimalign-cohesion-database-documents-protocol/document-protocol.md)** — Messages 64–72 and codec references.
- **[DocumentDuplicateKeyException](document-duplicate-key-exception.md)** — Insert-only duplicate keys.
- **[DocumentProtocolCollectionMessage](../assimalign-cohesion-database-documents-protocol/document-protocol-collection-message.md)** — Wire payload codec.
- **[DocumentProtocolCommandMessage](../assimalign-cohesion-database-documents-protocol/document-protocol-command-message.md)** — Wire payload codec.
- **[DocumentProtocolCompleteMessage](../assimalign-cohesion-database-documents-protocol/document-protocol-complete-message.md)** — Wire payload codec.
- **[DocumentProtocolExecuteMessage](../assimalign-cohesion-database-documents-protocol/document-protocol-execute-message.md)** — Wire payload codec.
- **[DocumentProtocolFailureMessage](../assimalign-cohesion-database-documents-protocol/document-protocol-failure-message.md)** — Wire payload codec.
- **[DocumentProtocolResultHeaderMessage](../assimalign-cohesion-database-documents-protocol/document-protocol-result-header-message.md)** — Wire payload codec.
- **[DocumentProtocolResultMessage](../assimalign-cohesion-database-documents-protocol/document-protocol-result-message.md)** — Wire payload codec.
- **[DocumentProtocolStoredDocumentMessage](../assimalign-cohesion-database-documents-protocol/document-protocol-stored-document-message.md)** — Wire payload codec.

`Assimalign.Cohesion.Database.Documents` implements named logical databases containing versioned
UTF-8 JSON documents. Each session is bound to one database. The engine supports document CRUD, an
OQL SQL++ query, INSERT, UPSERT, UPDATE, DELETE, EXPLAIN and collection/index DDL subset, secondary B+Tree indexes, snapshot and read-committed
transactions, and durable file or in-memory storage.

The public entry point is `DocumentDatabaseEngine.Create(name, options)`. Cast a created database to
`IDocumentDatabase`, create a collection, and use a session with the existing `IDocumentCollection`
methods. `session.Database` carries collection changes into the session transaction. `Index`
definitions are changed with OQL on the session, using its active transaction or an automatic
statement transaction; `IDocumentDatabase` has no index-management members.

See the [source-backed usage examples](examples/index.md).

`AddDocuments((context, engine) => ...)` captures engine construction on the root
`IDatabaseApplicationBuilder` and returns that application builder. During `Build`, the callback
configures `IDocumentDatabaseEngineBuilder`, including an optional borrowed
`IDocumentStorageStrategy` and deferred worker/server factories. The application owns the resulting
engine and its nested components. The model has no Hosting dependency. Standalone `Create` remains
available; all four built-in workers start with engine creation and stop when the engine is
disposed.

The engine references the Documents Language, Catalog, and `Storage` packages and the shared `Database`
root. [DESIGN.md](design.md) describes transaction ownership, OQL semantics, and limits. The
[language design](../assimalign-cohesion-database-documents-language/design.md) defines the
supported grammar; the [storage design](storage-design.md)
defines the on-disk format. ApplicationModel and compiled-schema provisioning remain outside this
composition change.

`DocumentDatabaseEngine.CreateBuilder()` returns the same model builder for standalone composition
or the concrete hosting builder's build-aware engine factory. This lets the consumer pass already
resolved values and register nested components while keeping the model package dependency-free.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Documents.Protocol` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Execution` | `CohesionSharedSource` |
| `Assimalign.Cohesion.Database` | `CohesionSharedSource` |
| `Assimalign.Cohesion.Database` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Documents.Language` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Transactions` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Indexing` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Additional reference pages

- **[Documents catalog design](catalog-design.md)** — Reference and usage.
- **[Documents catalog](catalog.md)** — Reference and usage.
- **[DocumentCatalog](document-catalog.md)** — Reference and usage.
- **[DocumentConstraintViolationException](document-constraint-violation-exception.md)** — Reference and usage.
- **[DocumentIndexMetadata](document-index-metadata.md)** — Reference and usage.
- **[Documents storage](storage.md)** — Reference and usage.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Assimalign.Cohesion.Database.Documents.csproj`.
