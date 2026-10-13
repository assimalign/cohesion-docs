# Assimalign.Cohesion.Connections.Tls

Exposes a connection's TLS handshake facts through the optional `Tls` connection feature:
ALPN application protocol, TLS version, cipher suite and remote certificate. The package contains
contracts and accessors; `Connections.Security` runs TLS over byte streams and `Connections.Quic`
reports the TLS handshake QUIC carries itself.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Connections](../index.md)

## Usage

Reference `Assimalign.Cohesion.Connections.Tls` and use the family namespace:

```csharp
using Assimalign.Cohesion.Connections;

ITlsConnectionInfo? tls = connection.Tls;
if (tls is not null)
{
    SslApplicationProtocol protocol = tls.ApplicationProtocol;
}
```

`Tls` is declared once on `IConnection`, shared by duplex, multiplexed, datagram and HTTP
connections, and returns `null` when absent.
Multiplexed streams implement `IConnection` and expose only their own features, without inheriting
the parent connection's TLS facts. A driver registers its implementation in `ConfigureFeatures`; its `Name` returns
`ITlsConnectionInfo.FeatureName`, pinned to `"Tls"`. A layer exposes the wrapped collection through
`InnerFeatures`, so its consumers keep seeing TLS facts without forwarding the TLS contract.

## Dependencies and lifetime

The only production reference is `Assimalign.Cohesion.Connections`. The package's namespace is
`Assimalign.Cohesion.Connections`, matching the family pin. The connection owns the remote certificate;
consumers copy it before retaining it beyond the connection's lifetime.

See [design](design.md) for registration, layering and compatibility.

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Connections`](../assimalign-cohesion-connections/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `ITlsConnectionInfo` | `src/Abstractions/ITlsConnectionInfo.cs` |
| `TlsConnectionFeatureExtensions` | `src/Extensions/TlsConnectionFeatureExtensions.cs` |

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tls/src/Assimalign.Cohesion.Connections.Tls.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tls/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tls/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tls/src`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tls/src/Abstractions/ITlsConnectionInfo.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tls/src/Extensions/TlsConnectionFeatureExtensions.cs`.
