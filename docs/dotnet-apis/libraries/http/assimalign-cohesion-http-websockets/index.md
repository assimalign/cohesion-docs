# Assimalign.Cohesion.Http.WebSockets

Owns the server WebSocket opening handshake on HTTP/1.1, HTTP/2, and HTTP/3, and returns a BCL `WebSocket`.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

Server WebSockets for the Cohesion HTTP family: the opening handshake on HTTP/1.1 (RFC 6455),
HTTP/2 (RFC 8441) and HTTP/3 (RFC 9220), subprotocol selection and permessage-deflate negotiation
(RFC 7692), ending in a BCL `System.Net.WebSockets.WebSocket`. The framing is the BCL's; this package
owns the handshake.

- **Detect and validate** a handshake (`context.WebSockets.HandshakeStatus`, `IsWebSocketRequest`):
  an HTTP/1.1 upgrade to `websocket`, or an HTTP/2 or HTTP/3 extended CONNECT whose `:protocol` is
  `websocket`.
- **Refuse** a handshake the server cannot accept, as RFC 6455 §4.2 prescribes: `400` when it is
  malformed, `426` with `Sec-WebSocket-Version: 13` for another version (`RejectHandshake`).
- **Accept** it: select a subprotocol the client offered, negotiate permessage-deflate when enabled,
  answer (`101` with `Sec-WebSocket-Accept` on HTTP/1.1, `200` on HTTP/2 and HTTP/3), and return the
  server end of the socket (`AcceptWebSocketAsync`).

The surface is the same on every protocol; an endpoint does not branch on the version. No origin
policy lives here: the cross-site defense, keep-alive defaults and the drain close are
[`Assimalign.Cohesion.Web.WebSockets`](../../../resources/web/assimalign-cohesion-web-websockets/index.md)'s.

## Usage

On a Web application, map the endpoint with `MapWebSocket` from `Web.WebSockets`: it routes both
handshake methods, `GET` on HTTP/1.1 and `CONNECT` on HTTP/2 and HTTP/3, refuses a request that is
not a handshake, applies the origin policy, and accepts. Code that serves the exchange itself uses
this package's surface directly:

```csharp
using System.Net.WebSockets;

using Assimalign.Cohesion.Http;

// context is the exchange's IHttpContext.
IHttpWebSocketFeature webSockets = context.WebSockets;
if (!webSockets.IsWebSocketRequest)
{
    context.Response.StatusCode = HttpStatusCode.BadRequest;
    return;
}

using WebSocket socket = await webSockets.AcceptWebSocketAsync(cancellationToken: context.RequestCancelled);
```

Such code must be reached by both handshake methods, which a `MapGet` route is not: over HTTP/2 and
HTTP/3 the handshake is a `CONNECT`. The exchange keeps running for as long as the socket is open;
the server ends the connection (HTTP/1.1) or the stream (HTTP/2, HTTP/3) when the exchange
completes. To select a subprotocol, pick one of `webSockets.RequestedProtocols` (the client's offer,
in its order of preference) and pass it as `HttpWebSocketAcceptOptions.SubProtocol`.

On a bare HTTP/1.1 listener, register the protocol-upgrade interceptor yourself, and check `Origin`
before accepting a socket that serves browsers. HTTP/2 and HTTP/3 listeners need nothing registered:
the transport surfaces extended CONNECT itself.

```csharp
using Assimalign.Cohesion.Http;
using Assimalign.Cohesion.Http.Connections;

// tcpListener is the IConnectionListener the HTTP/1.1 listener serves.
HttpConnectionListener listener = HttpConnectionListener.Create(options =>
{
    options.UseHttp1(tcpListener);
    options.Interceptors.Add(HttpProtocolUpgrade.CreateInterceptor());
});
```

Compression is off unless the accept enables it
(`new HttpWebSocketAcceptOptions { DangerousEnableCompression = true }`): compressing secrets
alongside attacker-controlled data leaks them through the compressed size.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Http.ProtocolUpgrade`](../../http/assimalign-cohesion-http-protocolupgrade/index.md) | `CohesionProjectReference` |

The core supplies `IHttpContext`, headers, and `IHttpExtendedConnectFeature`, the HTTP/2 and HTTP/3
tunnel the handshake rides; `Http.ProtocolUpgrade` supplies the HTTP/1.1 upgrade and raw-stream
takeover. The framing is `System.Net.WebSockets` from the shared framework, so there is no package
dependency. The package is a member of the `App.Web` shared framework.

## Principal public types

| Type | Source file |
|---|---|
| `HttpContextWebSocketExtensions` | `src/Extensions/HttpContextWebSocketExtensions.cs` |
| `HttpWebSocketAcceptOptions` | `src/HttpWebSocketAcceptOptions.cs` |
| `HttpWebSocketHandshakeStatus` | `src/HttpWebSocketHandshakeStatus.cs` |
| `IHttpWebSocketFeature` | `src/Abstractions/IHttpWebSocketFeature.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.WebSockets/src/Assimalign.Cohesion.Http.WebSockets.csproj`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.WebSockets/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.WebSockets/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.WebSockets/src/Abstractions/IHttpWebSocketFeature.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.WebSockets/src/Extensions/HttpContextWebSocketExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.WebSockets/src/HttpWebSocketAcceptOptions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.WebSockets/src/HttpWebSocketHandshakeStatus.cs`.

- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Runtime/Directory.Build.props`.
