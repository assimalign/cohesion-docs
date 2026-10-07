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

One TLS listener can serve HTTP/2 and HTTP/1.1, chosen per connection through ALPN (RFC 7301):
`HttpConnectionListenerOptions.UseHttp1AndHttp2` serves `h2` as HTTP/2 and `http/1.1`, or no ALPN
at all, as HTTP/1.1, and closes a connection that negotiated anything else. Every exchange that
arrived over TLS carries the core's `IHttpTlsConnectionFeature`, copied from the connection's
`ITlsConnectionInfo`. A host stops a connection in two steps:
`IHttpConnectionContext.BeginGracefulClose` takes no new exchange and announces the close
(`Connection: close`, or a `GOAWAY`) while the exchanges in flight finish, and cancelling the token
`ReceiveAsync` is enumerated with cancels what is left. The package raises no events and has no
event source. See the [design](design.md#graceful-close-the-host-contract).

Trailers travel on every version that can carry them: HTTP/1.1 (for a chunked request), HTTP/2 and
HTTP/3 surface a request's trailer section on `Request.Trailers`, and HTTP/2 and HTTP/3 send
response trailers; HTTP/1.1 sends none. HTTP/2 now decodes every field block, a trailer section
included, so HPACK stays in step. A valid extended CONNECT carries the core's
`IHttpExtendedConnectFeature`, whose `AcceptAsync` turns the stream into a duplex tunnel, which is
what WebSockets on HTTP/2 and HTTP/3 run over. Stage 10 also hardened the multiplexed transports:
HTTP/2 writes every frame in one piece, a request body cut off by a reset faults instead of ending
cleanly, HTTP/2 and HTTP/3 drop connection-specific fields from response heads, and an HTTP/3
client's reset fires `RequestCancelled`. See the [design](design.md#trailers-on-http2-and-http3).

The transports also follow RFC 9113 and RFC 9112 more strictly. HTTP/2 decodes a refused stream's
header block before refusing it, ignores frames on a stream it reset while crediting their flow
control, strips HEADERS padding before decoding, resets a request that lacks `:method`, `:scheme` or
`:path` instead of serving it as `GET /`, ends the connection with `COMPRESSION_ERROR` for a block
it cannot decompress, and resets only the stream of a malformed request head. HTTP/1.1 holds chunked
trailers to the shared trailer rule set, answers `400` for whitespace before a field name's colon,
an empty name, obsolete line folding, or a line without a colon, and answers a malformed chunked body
with `400` and `Connection: close` without ever draining it. Every version skips a query parameter
with an empty name. See the [design](design.md#http2-request-heads-rfc-9113-83).

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
| `HttpContextTransportExtensions` | `src/Extensions/HttpContextTransportExtensions.cs` |
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

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/Extensions/HttpContextTransportExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/HttpMinDataRate.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/HttpProtocol.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/Abstractions/IHttpConnection.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/Abstractions/IHttpConnectionContext.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/src/Abstractions/IHttpConnectionListener.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/README.md`.
