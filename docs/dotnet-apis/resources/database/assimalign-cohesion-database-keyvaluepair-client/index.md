# Assimalign.Cohesion.Database.KeyValuePair.Client

The typed key-value client: `IKeyValueClient`/`IKeyValueConnection`, a point/range surface (get/put/delete/exists/scan with etag-conditional writes) over the shared `Database.Client` pooling core.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

The typed key-value client: `IKeyValueClient` /`IKeyValueConnection`, a point/range surface
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

- **`KeyValueClient.Create(options)`** — → pooling `IKeyValueClient`; rented
  `IKeyValueConnection`s return to the pool on dispose.
- **`KeyValueClientEntry`** — (key/value/etag), `KeyValueWriteResult` (applied/etag),
  `KeyValueWriteCondition` (IfAbsent / `IfETagMatches`), `KeyValueScanRange`.
- **`IKeyValueClientObserver`** — the per-command telemetry hook (grammar text and
  counts only; key/value bytes never reach the observer).
- **`KeyValueClientException` + `KeyValueClientErrorKind`** — the error surface,
  with `ConnectionUsable` distinguishing command-level failures from broken
  connections.

## Dependencies

- **`Assimalign.Cohesion.Database`** — the area root (exception ancestry).
- **`Assimalign.Cohesion.Database.Client`** — the shared pooling/protocol core.
- **`Assimalign.Cohesion.Database.KeyValuePair`** — family identifiers and payload codecs.
- **`Assimalign.Cohesion.Database.Types`** — the shared value codec (transitive wire
  encoding of parameters and rows).
- **`Assimalign.Cohesion.Connections`** — the transport factory that dials the server.

The model reference supplies the wire contract; the client constructs no engine and references no
hosting module.

## Usage

See the [source-backed usage examples](examples/index.md).

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Client` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.KeyValuePair` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Client/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Client/src/Assimalign.Cohesion.Database.KeyValuePair.Client.csproj`.
