# WebSockets

Web.WebSockets maps WebSocket endpoints that answer on HTTP/1.1, HTTP/2, and HTTP/3, and guards every handshake with an origin policy.

> **Status:** Implemented. No message-size limits, hub layer, or per-endpoint origin policy.

A Web application serves WebSockets with two packages, both in the `App.Web` shared framework:
[`Http.WebSockets`](../dotnet-apis/libraries/http/assimalign-cohesion-http-websockets/index.md)
owns the opening handshake and hands the socket to the .NET runtime's RFC 6455 implementation, so an
endpoint works with an ordinary `System.Net.WebSockets.WebSocket`;
[`Web.WebSockets`](../dotnet-apis/resources/web/assimalign-cohesion-web-websockets/index.md) maps the
endpoints and applies the policy a browser-facing server needs. Nothing has to be registered on the
listeners: the default server installs the HTTP/1.1 upgrade interceptor, and HTTP/2 and HTTP/3
surface extended CONNECT on their own.

## Map a socket endpoint

`MapWebSocket` maps one route for every handshake shape and runs its handler over the accepted
socket. This echo endpoint is a complete `Program.cs`:

```csharp
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

using Assimalign.Cohesion.Http;
using Assimalign.Cohesion.Web.Hosting;
using Assimalign.Cohesion.Web.RequestTimeouts;
using Assimalign.Cohesion.Web.Routing;
using Assimalign.Cohesion.Web.WebSockets;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddRouting();

await using WebApplication app = builder.Build();
app.UseRouting();
app.UseWebSockets();

app.MapWebSocket("/ws/echo", EchoAsync).DisableRequestTimeout();

await app.RunAsync();

static async Task EchoAsync(IHttpContext context, WebSocket socket)
{
    byte[] buffer = new byte[4096];
    WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), context.RequestCancelled);
    while (result.MessageType != WebSocketMessageType.Close)
    {
        await socket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, context.RequestCancelled);
        result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), context.RequestCancelled);
    }

    await socket.CloseAsync(result.CloseStatus ?? WebSocketCloseStatus.NormalClosure, result.CloseStatusDescription, CancellationToken.None);
}
```

- **The handler receives the accepted socket.** The context is still there for route values, the
  user, and `RequestCancelled`. The socket is disposed when the handler returns, so the handler
  completes the close handshake; one that returns early aborts the socket.
- **The exchange lives as long as the socket.** When the handler returns, the server ends the
  HTTP/1.1 connection, or the HTTP/2 or HTTP/3 stream, that carried it. An open socket therefore
  counts against `MaxConcurrentConnections` on HTTP/1.1 and against the stream limits on HTTP/2 and
  HTTP/3.
- **A request that is not a handshake** — a plain `GET` or `HEAD` — is answered `400` and never
  reaches the handler. Any other method gets the router's `405` with `Allow: GET, CONNECT, HEAD`, so
  a page and a socket cannot share a URL through this verb: serve the page at another path, or write
  one handler that branches on `context.WebSockets.IsWebSocketRequest`.
- **Routing is required.** `MapWebSocket` maps into the router `AddRouting` registers, and throws
  `InvalidOperationException` without it. It also maps on a route group, relative to the group's
  prefix: `app.MapGroup("/rooms").MapWebSocket("{room}", handler)`.

## Why not `MapGet`

Over HTTP/1.1 a WebSocket handshake is a `GET` that asks to upgrade (RFC 6455). Over HTTP/2 and
HTTP/3 it is an extended `CONNECT` (RFC 8441, RFC 9220). A socket endpoint mapped with `MapGet`
works in a local test over `http://localhost`, which is HTTP/1.1, and fails every browser behind
`UseHttps`: the browser negotiates HTTP/2 through ALPN, opens the socket with RFC 8441, and gets the
router's `405`. `MapWebSocket` maps both methods on one route, so the same endpoint serves every
protocol (#1336).

## Choose the accept options per handshake

An overload takes a callback that chooses the options of each accept after the handshake was
validated. Its typical job is the subprotocol: `context.WebSockets.RequestedProtocols` is the
client's offer, in its order of preference, and the endpoint picks one it speaks.

```csharp
using System.Linq;

using Assimalign.Cohesion.Http;
using Assimalign.Cohesion.Web.WebSockets;

// app is the built WebApplication; ChatAsync is the endpoint's handler.
app.MapWebSocket(
    "/chat/{room}",
    ChatAsync,
    context => new HttpWebSocketAcceptOptions
    {
        SubProtocol = context.WebSockets.RequestedProtocols.Contains("chat.v1") ? "chat.v1" : null,
    });
```

A subprotocol the client did not offer is an `ArgumentException`, because RFC 6455 has the client
fail the connection otherwise. The options also carry `KeepAliveInterval`, `KeepAliveTimeout`, and
`DangerousEnableCompression`; a value the callback leaves `null` takes the policy's default.

## The origin policy

A browser sends cookies with a WebSocket handshake to any site, and neither the same-origin policy
nor CORS applies to it. Without a check, any page a user visits could open a socket authenticated as
that user (cross-site WebSocket hijacking). `UseWebSockets` refuses a handshake whose `Origin` is
present and is neither the request's own origin nor one of `AllowedOrigins`:

| Handshake `Origin` | Result |
| --- | --- |
| absent (a client that is not a page) | allowed |
| the request's own origin | allowed |
| one of `AllowedOrigins` | allowed |
| anything else, including the same host on another port | `403` |
| `null` (a sandboxed page, a local file, a cross-origin redirect) | `403` |
| malformed, or several `Origin` fields | `403` |
| any, with `AllowAnyOrigin` set | allowed |

```csharp
using System;

using Assimalign.Cohesion.Web.WebSockets;

// app is the built WebApplication.
app.UseWebSockets(options =>
{
    options.AllowedOrigins.Add("https://app.example");   // besides the request's own origin
    options.KeepAliveInterval = TimeSpan.FromSeconds(20);
});
```

- **Behind a proxy**, register `UseForwardedHeaders` first: the request's own origin is built from
  the effective scheme and host, so a page at `https://app.example` matches a request the server sees
  as `http://10.0.0.5:8080` once the forwarded headers are resolved. Without that middleware, list
  the public origin in `AllowedOrigins`.
- **Origins are compared in serialized form**, `scheme://host[:port]`: lower case, the default port
  dropped. Each configured origin is validated when `UseWebSockets` runs; a path, a trailing `/`, a
  wildcard, or `null` fails there with a message that names the fix.
- **`AllowAnyOrigin`** turns the defense off. Use it only for sockets that carry no ambient
  credentials, such as a token sent in a message.
- **Without `UseWebSockets`**, a `MapWebSocket` endpoint applies the default policy itself, so a
  socket endpoint is never left without the cross-site defense. Configure origins, keep-alive or
  compression with the middleware.

The policy also answers the refusals RFC 6455 prescribes: a malformed handshake gets `400`, and one
for a version other than 13 gets `426` with `Sec-WebSocket-Version: 13`.

## Keep-alive and compression

Every accept downstream of the policy takes its defaults unless it sets its own:

| Option | Default | Meaning |
| --- | --- | --- |
| `KeepAliveInterval` | 30 seconds | How often an idle socket sends a keep-alive frame, under the 60-second idle timeout common to proxies and load balancers |
| `KeepAliveTimeout` | infinite | How long to wait for the pong to a keep-alive ping before aborting; infinite sends unsolicited pongs instead |
| `DangerousEnableCompression` | `false` | Whether an accept negotiates permessage-deflate (RFC 7692) when the client offers it |

Compression stays off unless enabled, at the policy or per accept, because compressing data an
attacker controls together with a secret leaks the secret through the compressed size (the CRIME
and BREACH attacks). Enable it only for sockets whose messages never mix the two.

## Shutdown: the drain close

When the host stops, the default server begins its lame-duck drain, and every exchange sees it
through `IWebServerDrainFeature`. The policy closes each open socket with `1001 Going Away` at that
moment, so a socket ends cleanly within the stop's budget instead of being cut off without a close
frame when the budget runs out. The handler's receive returns the close, and its own `CloseAsync`
or `CloseOutputAsync` completes without throwing, so a receive loop written as above ends cleanly.
A socket accepted after the drain began is closed at once, and a peer that never answers the close
is cut off when the budget runs out, as any exchange is. A custom server that omits the drain feature
gets no drain close. See [Graceful shutdown](server.md#graceful-shutdown).

## HTTP/2 and HTTP/3

The same endpoint and the same policy serve every protocol. Over HTTP/2 and HTTP/3 the handshake is
an extended CONNECT whose `:protocol` is `websocket`: the transports advertise
`SETTINGS_ENABLE_CONNECT_PROTOCOL`, so a browser on an HTTP/2 connection opens its socket there
rather than on a separate HTTP/1.1 connection. The success response is `200` with no
`Sec-WebSocket-Accept`, a `426` carries no `Upgrade` or `Connection` field (both protocols prohibit
them), and the socket's frames travel in the stream's `DATA` frames. During a drain, the server also
sends `GOAWAY`, which stops new streams and leaves an open socket's stream to finish its close
handshake. The .NET `ClientWebSocket` has no HTTP/3 WebSockets, so the HTTP/3 suites use a minimal
client of their own.

On real QUIC, ending the server's side of a stream also stops its read side, so a server that closes
first cannot read what the client still sends. A WebSocket closes after its close handshake, when
nothing more is expected, so it is unaffected.

## Endpoint policies and request timeouts

`MapWebSocket` returns the route's builder, which takes policies as any endpoint's does, on the
endpoint or on its route group. `RequireAuthorization` refuses an anonymous handshake with the
scheme's `401` before the handler runs, on every protocol; `RequireCors` and `RequireRateLimiting`
apply as well. The [middleware order](middleware-order.md) places `UseWebSockets` last, after
`UseForwardedHeaders` and closest to the endpoints, so host filtering, authorization and rate
limiting apply to a handshake first.

A socket outlives any request timeout: when `UseRequestTimeouts` applies a timeout, it cancels the
socket's endpoint as it would any request. Disable the timeout on socket endpoints, or on their
group, with `DisableRequestTimeout()`, as the echo endpoint above does.

## Without `MapWebSocket`

Code that serves the exchange itself reads `context.WebSockets`: `IsWebSocketRequest`,
`HandshakeStatus`, `RequestedProtocols`, `AcceptWebSocketAsync`, and `RejectHandshake`, which stages
the `400` or `426` a refused handshake gets. Such code has to be reached by both handshake methods.
Under `UseWebSockets` it is guarded too: the middleware refuses a cross-site or malformed handshake
before the code runs, and decorates the feature, so every accept takes the policy's defaults and the
drain close. On a bare HTTP/1.1 listener outside the Web host, register
`HttpProtocolUpgrade.CreateInterceptor()` on the listener and check `Origin` before accepting a
socket that serves browsers.

For the server's listeners, TLS and shutdown, see [Server and TLS](server.md). Return to
[Web](index.md).

## Sources

- **Packages** — `cohesion/resources/Web/Assimalign.Cohesion.Web.WebSockets/docs/OVERVIEW.md`, `cohesion/resources/Web/Assimalign.Cohesion.Web.WebSockets/docs/DESIGN.md`, `cohesion/libraries/Http/Assimalign.Cohesion.Http.WebSockets/docs/OVERVIEW.md` and `cohesion/libraries/Http/Assimalign.Cohesion.Http.WebSockets/docs/DESIGN.md`.
- **Verbs and options** — `cohesion/resources/Web/Assimalign.Cohesion.Web.WebSockets/src/Extensions/WebSocketEndpointExtensions.cs`, `cohesion/resources/Web/Assimalign.Cohesion.Web.WebSockets/src/Extensions/WebSocketExtensions.cs`, `cohesion/resources/Web/Assimalign.Cohesion.Web.WebSockets/src/WebSocketOptions.cs` and `cohesion/libraries/Http/Assimalign.Cohesion.Http.WebSockets/src/HttpWebSocketAcceptOptions.cs`.
- **Decision record** — `cohesion/docs/libraries/Http/DECISIONS.md` (ADR 1).
- **Tests** — `cohesion/resources/Web/Assimalign.Cohesion.Web.WebSockets/tests/WebSocketEndpointTests.cs` and `cohesion/resources/Web/Assimalign.Cohesion.Web.WebSockets/tests/WebSocketEndToEndTests.cs`.
- **NativeAOT guard** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/samples/Assimalign.Cohesion.Web.AotGuard/Program.cs`.
- **Middleware order** — `cohesion/docs/resources/Web/MIDDLEWARE_ORDER.md`.
