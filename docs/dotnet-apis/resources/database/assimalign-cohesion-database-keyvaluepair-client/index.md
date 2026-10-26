# Assimalign.Cohesion.Database.KeyValuePair.Client

The typed key-value client: `KeyValueClient`/`KeyValueConnection`, a point/range surface (get/put/delete/exists/scan with etag-conditional writes) over the shared `Database.Client` pooling core.

> **Status:** Partial.

## Contract dependencies and client errors

This client references Database.Client, KeyValuePair.Protocol, shared Protocol and Types, and Connections. Wire contracts and transferred values use the
`Assimalign.Cohesion.Database.KeyValuePair.Protocol` namespace. The client references no engine.
COHDBCLIENT001 rejects engine project references, resolved engine assemblies and engine source imports.

`KeyValueClientException` derives directly from `System.Exception` and declares its own
non-null, read-only Diagnostics. Catch this client exception for remote failures; catching
`DatabaseException` no longer handles them. See the [migration guide](../../../../database/model-project-migration.md#remote-failures).

## Reference pages

- **[KeyValueConnection](key-value-connection.md)** — Rental lifetime, operations and exceptions.
- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

The typed key-value client: `KeyValueClient` /`KeyValueConnection`, a point/range surface
(get/put/delete/exists/scan with etag-conditional writes) over the shared `Database.Client` pooling
core.

## Purpose

Gives key-value consumers a typed wire client using the model-owned codecs: the connection builds
the model's command grammar (`GET @k`, `PUT @k @v [IF …]`, `DELETE @k [IF @etag]`, `EXISTS @k`,
`SCAN [FROM/TO/PREFIX/LIMIT]` — the contract in the engine package's `docs/COMMANDS.md`) with byte
parameters, sends it over the shared core, and decodes the model's result shapes back into typed
entries and outcomes. Conditional misses (compare-and-swap) are first-class outcomes
(`KeyValueWriteResult`, `bool` returns) — never exceptions; failures map onto the stable
`KeyValueClientErrorKind` taxonomy with the wire code preserved.

## Scope

- **`KeyValueClient.Create(options)`** — → pooling `KeyValueClient`; rented
  `KeyValueConnection`s return to the pool on dispose.
- **`KeyValueClientEntry`** — (key/value/etag), `KeyValueWriteResult` (applied/etag),
  `KeyValueWriteCondition` (IfAbsent / `IfETagMatches`), `KeyValueScanRange`.
- **`KeyValueClientObserver`** — the per-command telemetry hook (grammar text and
  counts only; key/value bytes never reach the observer).
- **`KeyValueClientException` + `KeyValueClientErrorKind`** — the error surface,
  with `ConnectionUsable` distinguishing command-level failures from broken
  connections.

## Dependencies

- **`Assimalign.Cohesion.Database.Protocol`** — shared framing and wire contracts.
- **`Assimalign.Cohesion.Database.Client`** — the shared pooling/protocol core.
- **`Assimalign.Cohesion.Database.KeyValuePair.Protocol`** — family identifiers and payload codecs.
- **`Assimalign.Cohesion.Database.Types`** — the shared value codec (transitive wire
  encoding of parameters and rows).
- **`Assimalign.Cohesion.Connections`** — the transport factory that dials the server.

The contract package supplies the wire vocabulary; the client's dependency closure contains no
engine, catalog, storage or hosting implementation.

## Usage

See the [source-backed usage examples](examples/index.md).

Supply an `IDuplexConnectionFactory` through `KeyValueClientOptions.ConnectionFactory`; the shared pool owns the duplex connections it establishes.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Protocol` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Client` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.KeyValuePair.Protocol` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Additional reference pages

- **[KeyValueClientException](key-value-client-exception.md)** — Reference and usage.

## Registered scalar calls

[KeyValueConnection.InvokeFunctionAsync](key-value-connection.md#registered-call-functions)
invokes server-registered functions through CALL and returns a primitive or NULL. Function
identifiers ship in KeyValuePair.Protocol; clients retain no engine reference.

## Sources

- **Current project boundaries** — `cohesion/resources/Database/README.md`.
- **Build guard** — `cohesion/resources/Database/Directory.Build.targets`.
- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Client/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Client/src/Assimalign.Cohesion.Database.KeyValuePair.Client.csproj`.
