# Assimalign.Cohesion.Database.Client

The shared client core of the Data Platform: the protocol client every per-model client (`Sql.Client`, `KeyValuePair.Client`, `Blob.Client`, …) builds on.

> **Status:** Implemented.

## Engine-free client core

Database.Client references shared Database.Protocol, Database.Types and Connections. Typed model
clients consume their model Protocol packages. COHDBCLIENT001 checks evaluated project references,
actual resolved assembly identities and source imports; engine dependencies and build-order-only
engine references fail the build. Repository-local test/example/sample/fixture harnesses are exempt.
The client-only sample checks its pre-trim reference closure and runs in NativeAOT CI.

DatabaseClientException derives directly from System.Exception and owns its Diagnostics snapshot.
Catch it when using the shared client core; catch the model client exception for typed APIs.
See the [migration guide](../../../../database/model-project-migration.md).

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[DatabaseClientException](database-client-exception.md)** — Public signature and diagnostics.
- **[DatabaseConnection](database-connection.md)** — Public signature and diagnostics.

The shared client core of the Data Platform: the protocol client every per-model client
(`Sql.Client`, `KeyValuePair.Client`, `Blob.Client`, …) builds on. It dials a
`libraries/Connections` transport, runs the startup/authenticate/ready handshake, runs model-owned
framed exchanges, and pools authenticated connections.

## Scope

- **`DatabaseClient`** — the sealed pooling entry point (`DatabaseClient.Create`):
  `RentAsync` returns a fresh lease over an authenticated session; disposing it permanently ends
  that lease and returns a healthy session to the pool.
- **`DatabaseConnection`** — one rental lease, already open:
  `ExecuteAsync<TResult>(DatabaseProtocolExchange<TResult>)` returns the model's
  result. The exchange supplies codecs and materialization policy.
  Streaming Blob downloads keep this exchange active while the caller reads;
  content stream disposal cancels unfinished work before releasing the rental.
- **`DatabaseClientOptions.Family`** — the model family fixed for every connection
  in the pool. The exchange must use the exact same family instance.
- **`DatabaseConnectionSettings`** — typed settings with a minimal `key=value;`
  connection-string parser (`Database`, `Principal`, `Endpoint=host[:port]`,
  `MaxPoolSize`), plus `For(Uri)` for generated or ambient resource
  endpoints.
- **`DatabaseClientException`** — the client error root, carrying the wire's
  stable `ProtocolErrorCode`.

## Dependencies

`Database.Protocol` (framing, shared messages and family binding), `Database.Types` (diagnostic
values), and `Connections` (transport factories). No engine or area-root reference is required.

## Usage

See the [source-backed usage examples](examples/index.md).

Generated and ambient endpoints stay typed:

See the [source-backed usage examples](examples/index.md).

`For(Uri)` requires a Cohesion endpoint URI—an absolute, host-bearing value with a valid port and no
user information, query, or fragment—and maps its `IdnHost` and `Port` directly to the socket
endpoint without formatting and reparsing a string.

See [DESIGN.md](design.md) for the pooling and settings decisions.

Disposed leases cannot reach later rentals. Metadata and execution throw ObjectDisposedException;
IsOpen returns false. Repeated disposal or abort joins the original return. Shutdown closes returned
connectors and fails waiting renters with ObjectDisposedException.

SQL, Graph and Documents negotiate server-reported transaction status. Pool return rolls back open or failed transactions before reuse; only an Idle session is pooled. Missing/invalid status, failed reset or a legacy transaction-capable peer causes connector discard.

## Declarative commands

`DatabaseCommandClient.Create(Uri controlPlaneAddress, string bearerToken, HttpMessageInvoker transport)`
accepts a caller-owned transport, including its TLS trust policy. Disposing the returned client does
not dispose that transport; the caller retains it until requests finish and disposes it afterward.
The two-argument factory still creates a transport owned and disposed by the client. Both overloads
perform the same endpoint and credential validation; a null supplied transport throws
`ArgumentNullException`. The client does not discover application trust.

`IDatabaseCommandClient` is the separate HTTP admin command contract. `DatabaseCommandClient.Create`
accepts the full manifest control-plane URI (including its path) and an opaque bootstrap bearer.
`SendCommandAsync` posts the camel-case id/kind/owner/key/payload envelope to commands; payload is
base64. `DeleteCommandAsync` sends DELETE to the same route and envelope. Both return package-local
`ResourceCommandObservation` with Status and Detail, retaining actionable provider refusal text.
Transport failures propagate; HTTP refusals become Rejected observations. Serialization uses
explicit Utf8JsonWriter/JsonDocument access. The caller disposes the client; redirect following and
cookies are disabled. No runtime Hosting or ApplicationModel dependency was added.

Supply an `IDuplexConnectionFactory` through `DatabaseClientOptions.ConnectionFactory`; the shared pool owns the duplex connections it establishes.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Protocol` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Exchange postamble contract

[DatabaseProtocolExchange](database-protocol-exchange.md) automatically drains negotiated Ready
transaction status after Execute/ExecutePaths and Documents Command, including application exchanges using its
one-argument constructor. The explicit transaction-status constructor remains available.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/src/Assimalign.Cohesion.Database.Client.csproj`.
