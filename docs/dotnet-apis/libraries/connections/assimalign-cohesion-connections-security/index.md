# Assimalign.Cohesion.Connections.Security

TLS for Cohesion connections. Wraps an established `IDuplexConnection` (itself an `IDuplexPipe`) in a TLS
session and returns a new, secured `IDuplexConnection` whose `Input` / `Output` pipes are encrypted. Lives
in the Connections area because it composes directly over the connection contracts.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Connections](../index.md)

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections`](../../connections/assimalign-cohesion-connections/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections.Tls`](../assimalign-cohesion-connections-tls/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections.StreamAbort`](../assimalign-cohesion-connections-streamabort/index.md) | `CohesionProjectReference` |

## Key types

- `TlsConnectionLayer` — an `IConnectionLayer` that secures connections with TLS (server or client),
  composed onto a listener or factory.
- `TlsConnectionExtensions` — `IDuplexConnectionListener.UseTls(TlsServerOptions)` /
  `IDuplexConnectionFactory.UseTls(TlsClientOptions)` to compose the layer, and
  `IDuplexConnection.UpgradeToTlsAsync(…)` to upgrade a single connection.
- `TlsServerOptions` / `TlsClientOptions` — wrap the BCL `Ssl*AuthenticationOptions` (as
  `AuthenticationOptions`) plus a `HandshakeTimeout`. `TlsServerOptions.RequireClientCertificate` /
  `AllowClientCertificate` set the mutual-TLS policy, with an optional validation callback, and
  `TlsServerOptions.MaxConcurrentHandshakes` (default 512) bounds the handshakes a TLS-layered
  listener runs at once. Each handshake runs on its own task, and one that fails or times out closes
  only its connection.
- The secured connection registers `ITlsConnectionInfo` from `Connections.Tls` under its pinned
  `Tls` name: ALPN protocol, TLS version, cipher suite, and the peer's certificate. Read it with
  `connection.Tls`. Features registered below the TLS layer pass through automatically except
  `StreamAbort`: its inner implementation cannot invalidate plaintext buffered by TLS's outer pipes,
  so `connection.Features.Get<IMultiplexedStreamAbort>()` returns `null`. The TLS layer returns an
  `IDuplexConnection`; the `DirectionalAbort` accessor is available only on `IMultiplexedConnectionStream`.

## Source layout

- `src/` — `TlsConnectionLayer` and the options types.
- `src/Extensions` — TLS composition/upgrade extension members on `IDuplexConnection`,
  `IDuplexConnectionListener`, and `IDuplexConnectionFactory`.
- `src/Internal` — the `TlsConnection` decorator backed by `SslStream` over a `DuplexPipeStream`.

The driver participates in the common `IConnection` identity, feature, and lifecycle hierarchy.
Listeners expose a stable `Id` and live `Connections`. Entries disappear on closure; disposing
the listener leaves accepted connections open for their owners to dispose. `ConnectionClosed`
fires on abort, observed peer closure, and no later than local disposal.

## Principal public types

| Type | Source file |
|---|---|
| `TlsClientOptions` | `src/TlsClientOptions.cs` |
| `TlsConnectionExtensions` | `src/Extensions/TlsConnectionExtensions.cs` |
| `TlsConnectionLayer` | `src/TlsConnectionLayer.cs` |
| `TlsServerOptions` | `src/TlsServerOptions.cs` |

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/src/Assimalign.Cohesion.Connections.Security.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/src`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/src/TlsClientOptions.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/src/Extensions/TlsConnectionExtensions.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/src/TlsConnectionLayer.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/src/TlsServerOptions.cs`.
