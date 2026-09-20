# Assimalign.Cohesion.Http.Connections

Carries HTTP exchanges over Cohesion stream and multiplexed connections.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

The package consumes `IConnection` and `IMultiplexedConnection` rather than owning a second
transport stack. Protocol handling uses the core feature and interceptor seams, allowing optional
concerns to attach without reverse references from the transport.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Connections`](../../connections/assimalign-cohesion-connections/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `Http1ConnectionListenerOptions` | `src/Options/Http1ConnectionListenerOptions.cs` |
| `Http2ConnectionListenerOptions` | `src/Options/Http2ConnectionListenerOptions.cs` |
| `Http3ConnectionListenerOptions` | `src/Options/Http3ConnectionListenerOptions.cs` |
| `Http3QPackOptions` | `src/Options/Http3QPackOptions.cs` |
| `HttpAltServiceAdvertisementOptions` | `src/HttpAltServiceAdvertisementOptions.cs` |
| `HttpConnection` | `src/HttpConnection.cs` |
| `HttpConnectionContext` | `src/HttpConnectionContext.cs` |
| `HttpConnectionListener` | `src/HttpConnectionListener.cs` |
| `HttpConnectionListenerLimits` | `src/HttpConnectionListenerLimits.cs` |
| `HttpConnectionListenerOptions` | `src/HttpConnectionListenerOptions.cs` |
| `HttpMinDataRate` | `src/HttpMinDataRate.cs` |
| `HttpProtocol` | `src/HttpProtocol.cs` |
| `IHttpConnection` | `src/Abstractions/IHttpConnection.cs` |
| `IHttpConnectionContext` | `src/Abstractions/IHttpConnectionContext.cs` |
| `IHttpConnectionListener` | `src/Abstractions/IHttpConnectionListener.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/Assimalign.Cohesion.Http.Connections.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/Options/Http1ConnectionListenerOptions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/Options/Http2ConnectionListenerOptions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/Options/Http3ConnectionListenerOptions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/Options/Http3QPackOptions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/HttpAltServiceAdvertisementOptions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/HttpConnection.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/HttpConnectionContext.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/HttpConnectionListener.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/HttpConnectionListenerLimits.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/HttpConnectionListenerOptions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/HttpMinDataRate.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/HttpProtocol.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/Abstractions/IHttpConnection.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/Abstractions/IHttpConnectionContext.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/Abstractions/IHttpConnectionListener.cs`.
