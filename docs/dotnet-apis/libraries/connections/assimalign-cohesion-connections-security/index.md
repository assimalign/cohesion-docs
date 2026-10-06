# Assimalign.Cohesion.Connections.Security

Applies Transport Layer Security (TLS) to established Cohesion connections.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Connections](../index.md)

## Scope

A TLS upgrade returns a new secured connection over `SslStream`; it does not mutate the carrier's
identity into a middleware context. Listener and factory composition use `TlsConnectionLayer`.
Client and server authentication settings and handshake timeouts are explicit options.

`TlsServerOptions.RequireClientCertificate` and `AllowClientCertificate` set the server's mutual-TLS
policy, each with an optional validation callback: both request a client certificate in the
handshake, `RequireClientCertificate` refuses a client without one, and a presented certificate must
pass the callback or, without one, the platform's chain validation. The secured connection
implements the contracts' `ITlsConnectionInfo`: the ALPN protocol, the TLS version, the cipher
suite, and the peer's certificate, which the connection disposes with itself. See the
[design](design.md#client-certificates).

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections`](../../connections/assimalign-cohesion-connections/index.md) | `CohesionProjectReference` |

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
