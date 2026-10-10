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
at all, as HTTP/1.1, and closes a connection that negotiated anything else. Each exchange that
arrived over TLS carries the handshake (client certificate, protocol, cipher suite, negotiated ALPN
protocol) on its `ConnectionInfo` as the `ITlsConnectionInfo` facet, which `context.TlsConnection`
in `Http.Tls` reads; the transport installs no TLS feature and references no feature package (see
the [design](design.md#the-tls-session-on-every-exchange)). A host stops a connection in two steps:
`IHttpConnectionContext.BeginGracefulClose` takes no new exchange and announces the close
(`Connection: close`, or a `GOAWAY`) while the exchanges in flight finish, and cancelling the token
`ReceiveAsync` is enumerated with cancels what is left. The package raises no events and has no
event source. See the [design](design.md#graceful-close-the-host-contract).

Trailers travel on every version that can carry them: HTTP/1.1 (for a chunked request), HTTP/2 and
HTTP/3 surface a request's trailer section on `Request.Trailers`, and HTTP/2 and HTTP/3 send
response trailers; HTTP/1.1 sends none. HTTP/2 now decodes every field block, a trailer section
included, so HPACK stays in step. A valid extended CONNECT reaches the request-parse hooks with its
`:protocol` (`HttpExchangeInterceptorRequestContext.Protocol`), and its exchange control's
`AcceptTunnelAsync` turns the stream into a duplex tunnel, which is what WebSockets on HTTP/2 and
HTTP/3 run over; `Http.ExtendedConnect`'s interceptor, which `Web.Hosting` registers by default,
wraps it as `context.ExtendedConnect` (#1368). Stage 10 also hardened the multiplexed transports:
HTTP/2 writes every frame in one piece, a request body cut off by a reset faults instead of ending
cleanly, HTTP/2 and HTTP/3 drop connection-specific fields from response heads, and an HTTP/3
client's reset fires `RequestCancelled`. See the [design](design.md#trailers-on-http2-and-http3).

The transports also follow RFC 9113 and RFC 9112 more strictly. HTTP/2 decodes a refused stream's
header block before refusing it, ignores frames on a stream it reset while crediting their flow
control, strips HEADERS padding before decoding, resets a request that lacks `:method`, `:scheme` or
`:path` instead of serving it as `GET /`, ends the connection with `COMPRESSION_ERROR` for a block
it cannot decompress, and resets only the stream of a malformed request head, an extended CONNECT
whose `:protocol` is empty or misplaced included (#1369; HTTP/3 resets it with `H3_MESSAGE_ERROR`).
HTTP/1.1 holds chunked trailers to the shared trailer rule set, answers `400` for whitespace before a
field name's colon, an empty name, obsolete line folding, or a line without a colon, and answers a
malformed chunked body with `400` and `Connection: close` without ever draining it. Every version
skips a query parameter with an empty name. See the [design](design.md#http2-request-heads-rfc-9113-83).

HTTP/2 and HTTP/3 now enforce `KeepAliveTimeout`, `RequestHeadersTimeout` and
`MinRequestBodyDataRate`, which only HTTP/1.1 read before (#1085). An HTTP/2 stream that is reset
keeps its `SETTINGS_MAX_CONCURRENT_STREAMS` slot until its exchange ends, the reset budget also
counts the resets the server sends for the peer's stream errors (MadeYouReset, CVE-2025-8671;
#1072), and a cancelled HTTP/2 send resets its stream instead of leaving it open (#1075). HTTP/3 puts
its RFC 9114 error codes on the wire through the connection contracts' code-carrying aborts (#1080),
so it refuses an unread request body with `STOP_SENDING(H3_NO_ERROR)` instead of draining it, and it
bounds the decoded field section with `Http3QPackOptions.MaxFieldSectionSize`, answered `431`
(#1082). See the [design](design.md#http2-and-http3-connection-timeouts-and-data-rates-1085).

HTTP/1.1 rejects request-line and field-value octets RFC 9112 and RFC 9110 do not allow (#1341),
caps each chunk framing line with `MaxChunkFramingLineSize` and the body's whole chunk framing with a
budget derived from it (#1375), and answers a body over a limit itself with `413`, `408` or `431`
(#1339). `IHttpExchangeControl.ClientFaultStatusCode` reports that status, so a host can tell the
client's fault from the application's (#1340). See the
[design](design.md#http11-reporting-the-client-fault-1340).

Every version refuses a response field whose name is not a token or whose value holds a control
character other than HTAB before it writes a byte, with an `HttpException` whose code is
`HttpErrorCode.InvalidResponseField` (#1183), and HTTP/2 and HTTP/3 refuse such a field when they
receive it (#1376). The method is kept as sent, so `get` is not `GET` (#1301), and
`HttpConnectionListenerOptions.ExchangeFeatureCapacity` sizes each exchange's feature collection for
the features a host installs (#1381). See the
[design](design.md#response-field-syntax-refused-before-a-byte-is-written-1183).

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
