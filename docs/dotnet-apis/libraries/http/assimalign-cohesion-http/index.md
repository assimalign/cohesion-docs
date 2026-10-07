# Assimalign.Cohesion.Http

Defines Hypertext Transfer Protocol (HTTP) messages, fields, features, and exchange contracts.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

Headers and trailers are distinct ordered field sections using compatible collection primitives.
Optional concerns attach through feature and interceptor seams rather than widening the protocol
root. The core remains independent of hosting and resource platforms.

`IHttpTlsConnectionFeature` tells a handler how the connection its exchange arrived on is secured:
the client certificate (`null` when the client presented none), the TLS protocol version, the
cipher suite, and the application protocol ALPN selected. The server transport
(`Assimalign.Cohesion.Http.Connections`) attaches it to every exchange that arrived over TLS —
HTTP/1.1 and HTTP/2 over the TLS layer, HTTP/3 over QUIC — and attaches none to a cleartext
exchange. Code reads it as `context.TlsConnection` (`HttpTlsConnectionExtensions`). The values
belong to the connection, so every exchange on it carries the same instance.

`IHttpExtendedConnectFeature` is the HTTP/2 and HTTP/3 extended CONNECT capability (RFC 8441,
RFC 9220): the `Protocol` a `CONNECT` with `:protocol` asked for, and `AcceptAsync`, which answers
`200` without ending the stream and returns the stream as a duplex tunnel. The contract moved here
from `Http.ExtendedConnect` (#1316), because the transport produces it; the transport installs it on
every valid extended CONNECT, and `context.ExtendedConnect` still ships in `Http.ExtendedConnect`.

`Request.Trailers` and `Response.Trailers` report per exchange whether a trailer section is
supported (`IsSupported`). The transports fill request trailers on every version (on HTTP/1.1 for a
chunked request) once the body has been read to its end, and send response trailers on HTTP/2 and
HTTP/3 only. See the [design](design.md#trailers).

A request-parse hook can add an interceptor to its own exchange's response phase
(`HttpExchangeInterceptorRequestContext.AddResponseInterceptor`), so an interceptor that needs the
response phase for a few exchanges keeps every other exchange on the transport's fast path. See the
[design](design.md#per-exchange-response-interceptors).

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `HttpAcceptParser` | `src/HttpAcceptParser.cs` |
| `HttpAcceptQuery` | `src/HttpAcceptQuery.cs` |
| `HttpAltService` | `src/HttpAltService.cs` |
| `HttpCacheControl` | `src/HttpCacheControl.cs` |
| `HttpCacheControlExtension` | `src/HttpCacheControlExtension.cs` |
| `HttpConditionalRequest` | `src/HttpConditionalRequest.cs` |
| `HttpConditionalRequestContext` | `src/HttpConditionalRequestContext.cs` |
| `HttpConnectionInfo` | `src/HttpConnectionInfo.cs` |
| `HttpContentNegotiation` | `src/HttpContentNegotiation.cs` |
| `HttpContentRange` | `src/HttpContentRange.cs` |
| `HttpContentTypes` | `src/HttpContentTypes.cs` |
| `HttpContext` | `src/HttpContext.cs` |
| `HttpContextExtensions` | `src/Extensions/HttpContextExtensions.cs` |
| `HttpDate` | `src/HttpDate.cs` |
| `HttpEntityTag` | `src/HttpEntityTag.cs` |
| `HttpEntityTagCondition` | `src/HttpEntityTagCondition.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Assimalign.Cohesion.Http.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpAcceptParser.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpAcceptQuery.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpAltService.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpCacheControl.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpCacheControlExtension.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpConditionalRequest.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpConditionalRequestContext.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpConnectionInfo.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpContentNegotiation.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpContentRange.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpContentTypes.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpContext.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Extensions/HttpContextExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpDate.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpEntityTag.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpEntityTagCondition.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Abstractions/IHttpTlsConnectionFeature.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Extensions/HttpTlsConnectionExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Abstractions/IHttpExtendedConnectFeature.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Abstractions/IHttpResponse.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpExchangeInterceptorRequestContext.cs`.

- **Source** — `cohesion/docs/libraries/Http/DECISIONS.md`.
