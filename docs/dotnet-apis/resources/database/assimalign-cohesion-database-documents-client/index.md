# Assimalign.Cohesion.Database.Documents.Client

A pooled Documents client for OQL, document operations and collection management.

> **Status:** Implemented. Support includes text transactions over InMemory and TCP, with safe pooled reset.

## Contract dependencies and client errors

This client references Database.Client, Documents.Protocol, shared Protocol and Types, and Connections,
plus model-neutral Execution. Wire contracts and transferred values use the
`Assimalign.Cohesion.Database.Documents.Protocol` namespace. The client references no engine.
COHDBCLIENT001 rejects engine project references, resolved engine assemblies and engine source imports.

`DocumentClientException` derives directly from `System.Exception` and declares its own
non-null, read-only Diagnostics. Catch this client exception for remote failures; catching
`DatabaseException` no longer handles them. See the [migration guide](../../../../database/model-project-migration.md#remote-failures).

## Reference pages

- **[Design](design.md)** — Ownership, failure completion and pooling.
- **[Examples](examples/index.md)** — Source-backed end-to-end usage.
- **[DocumentClient](document-client.md)** — Create a pool and rent authenticated connections.
- **[DocumentClientOptions](document-client-options.md)** — Settings and transport factory.
- **[DocumentConnection](document-connection.md)** — OQL, CRUD and collection operations.
- **[DocumentResultSet](document-result-set.md)** — Materialized rows and whole JSON documents.
- **[DocumentRow](document-row.md)** — Typed/name access and MISSING support.
- **[DocumentColumn](document-column.md)** — Projection metadata.
- **[DocumentClientException](document-client-exception.md)** — Protocol category and coded diagnostics.

The model owns codecs; Database.Client owns connections, handshake and pooling. The caller supplies
a Connections transport factory. Results own their data after the exchange. Projected rows preserve
SQL++ MISSING versus NULL; nested objects and arrays are owned JsonElement values.

[Parent: Database](../index.md)

## Sources

- **Current project boundaries** — `cohesion/resources/Database/README.md`.
- **Build guard** — `cohesion/resources/Database/Directory.Build.targets`.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/docs/DESIGN.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentClientWireTests.cs`.
