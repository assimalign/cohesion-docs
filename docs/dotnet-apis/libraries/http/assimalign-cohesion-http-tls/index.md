# Assimalign.Cohesion.Http.Tls

Surfaces the TLS session of the connection an exchange arrived on as a feature on `IHttpContext`.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

Gives handlers the TLS session of the connection an exchange arrived on, as a feature on
`IHttpContext`: the client certificate, the TLS protocol version, the cipher suite, and the
application protocol ALPN selected.

- **The contract**: `IHttpTlsConnectionFeature`, the session.
- **The accessor**: `context.TlsConnection`, which returns the session or `null` for a cleartext
  exchange. It works the same on HTTP/1.1, HTTP/2, and HTTP/3.
- **An override point**: a feature a middleware installs in `context.Features` before the first read
  of `context.TlsConnection` is returned instead of the connection's own session. After that read,
  the override must replace the cached feature. See the
  [design](design.md#how-the-session-reaches-a-handler).

There is no TLS configuration, no certificate validation, and no authentication from the
certificate here. See the [design](design.md#what-it-does-not-do).

## Usage

```csharp
using System.Security.Authentication;

using Assimalign.Cohesion.Http;

// context is the exchange's IHttpContext.
if (context.TlsConnection is { ClientCertificate: { } certificate } tls)
{
    // The handshake asked for a certificate and the client sent one. The connection owns it,
    // so copy what you keep beyond the exchange.
    string subject = certificate.Subject;
    SslProtocols protocol = tls.Protocol;
}
```

Whether a client certificate is requested, required, and how it is validated is configured on the
endpoint's TLS options (`TlsServerOptions.RequireClientCertificate` or `AllowClientCertificate` in
[`Assimalign.Cohesion.Connections.Security`](../../connections/assimalign-cohesion-connections-security/index.md)).

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections`](../../connections/assimalign-cohesion-connections/index.md) | `CohesionProjectReference` |

The core supplies `IHttpContext` and the feature collection; the Connections contracts library
supplies `ITlsConnectionInfo`, the handshake facet.

The server transport
([`Assimalign.Cohesion.Http.Connections`](../assimalign-cohesion-http-connections/index.md)) does not
reference this package. It publishes the handshake as the `ITlsConnectionInfo` facet of each
exchange's `ConnectionInfo`, and the accessor builds the feature from that facet on first read. A
request-parse interceptor, which runs before the exchange exists, reads the facet directly from its
context's `ConnectionInfo`. The `App.Web` shared framework delivers this package to web
applications.

## Principal public types

| Type | Source file |
|---|---|
| `HttpTlsConnectionExtensions` | `src/Extensions/HttpTlsConnectionExtensions.cs` |
| `IHttpTlsConnectionFeature` | `src/Abstractions/IHttpTlsConnectionFeature.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Tls/src/Assimalign.Cohesion.Http.Tls.csproj`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Tls/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Tls/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Tls/src/Abstractions/IHttpTlsConnectionFeature.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Tls/src/Extensions/HttpTlsConnectionExtensions.cs`.

- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Runtime/Directory.Build.props`.
