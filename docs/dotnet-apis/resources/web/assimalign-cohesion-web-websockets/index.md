# Assimalign.Cohesion.Web.WebSockets

WebSocket endpoints and the WebSocket policy for the Cohesion Web pipeline.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

WebSockets for the Web pipeline. `MapWebSocket` maps a socket endpoint that answers every handshake
shape, and `UseWebSockets` guards and shapes every WebSocket an endpoint accepts through
`context.WebSockets`. The handshake, its negotiation and the BCL framing belong to
[`Assimalign.Cohesion.Http.WebSockets`](../../../libraries/http/assimalign-cohesion-http-websockets/index.md);
this package is the policy a browser-facing server needs around them.

## Scope

- **One endpoint for every protocol.** `MapWebSocket` maps a route for the HTTP/1.1 upgrade (`GET`)
  and the HTTP/2 and HTTP/3 extended CONNECT, on the application or a route group, and runs its
  handler over the accepted socket. A request that is not a handshake gets `400` and never reaches
  the handler.
- **Cross-site WebSocket hijacking defense.** A handshake whose `Origin` is present and is neither
  the request's own origin (from the effective, proxy-resolved scheme and host) nor one of
  `AllowedOrigins` is refused with `403`. A handshake without `Origin` passes.
- **RFC 6455 refusals.** A malformed handshake gets `400`; one for another version gets `426` with
  `Sec-WebSocket-Version: 13`.
- **Every protocol.** The same policy applies to the HTTP/1.1 upgrade and to the HTTP/2 and HTTP/3
  extended CONNECT.
- **Accept defaults.** Every accept downstream takes the keep-alive interval and timeout, and the
  compression switch, unless it sets its own. Compression stays off unless enabled.
- **Drain close.** When the default server begins its drain, every open socket is closed with
  `1001 Going Away`, inside the stop's budget.

## Dependencies

- **`Assimalign.Cohesion.Web`** — the pipeline seams and the server drain feature
  (`IWebServerDrainFeature`).
- **`Assimalign.Cohesion.Web.Routing`** — the router builder and the route groups `MapWebSocket`
  maps into.
- **`Assimalign.Cohesion.Http.WebSockets`** — the handshake, the negotiation and the accept.
- **`Assimalign.Cohesion.Http.Forwarded`** — `EffectiveScheme` and `EffectiveHost` for the
  same-origin check.
- **`Assimalign.Cohesion.Http`** — the protocol core.

It references nothing in the hosting family (`COHRES001`, `COHRES004`): the drain signal reaches it
through the Web root's feature contract. The package, `Http.WebSockets`, `Http.ProtocolUpgrade` and
`Http.ExtendedConnect` are members of the `App.Web` shared framework.

## Usage

See the [source-backed usage examples](examples/index.md) and the
[WebSockets guide](../../../../web/websockets.md).

```csharp
using System;
using System.Net.WebSockets;
using System.Threading;

using Assimalign.Cohesion.Http;
using Assimalign.Cohesion.Web.Hosting;
using Assimalign.Cohesion.Web.RequestTimeouts;
using Assimalign.Cohesion.Web.Routing;
using Assimalign.Cohesion.Web.WebSockets;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddRouting();

await using WebApplication app = builder.Build();
app.UseRouting();

// Behind a proxy, register UseForwardedHeaders first: the origin check reads the effective
// scheme and host it resolves.
app.UseWebSockets(options =>
{
    options.AllowedOrigins.Add("https://app.example");   // besides the request's own origin
    options.KeepAliveInterval = TimeSpan.FromSeconds(20);
});

app.MapWebSocket("/chat/{room}", async (IHttpContext context, WebSocket socket) =>
{
    byte[] buffer = new byte[4096];
    WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), context.RequestCancelled);
    while (result.MessageType != WebSocketMessageType.Close)
    {
        await socket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, context.RequestCancelled);
        result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), context.RequestCancelled);
    }

    await socket.CloseAsync(result.CloseStatus ?? WebSocketCloseStatus.NormalClosure, result.CloseStatusDescription, CancellationToken.None);
}).DisableRequestTimeout();

await app.RunAsync();
```

Map socket endpoints with `MapWebSocket`, not `MapGet`. Over HTTP/1.1 a handshake is a `GET`, but
over HTTP/2 and HTTP/3 it is an extended CONNECT (RFC 8441, RFC 9220). A `MapGet` socket works in a
local test over `http://localhost`, which is HTTP/1.1, and fails every browser behind `UseHttps`,
which negotiates HTTP/2 through ALPN. `MapWebSocket` maps both methods on one route:

- a valid handshake is accepted and the handler gets the open socket, which is disposed when the
  handler returns;
- a request that is not a handshake (a plain `GET` or `HEAD`) gets `400`, and any other method the
  router's `405`;
- the route builder takes policies as any endpoint's does (`RequireAuthorization`, `RequireCors`,
  `RequireRateLimiting`), on the endpoint or on its route group;
- an overload chooses the accept options per handshake, for example a subprotocol the client
  offered: `app.MapWebSocket("/chat", handler, context => new HttpWebSocketAcceptOptions { SubProtocol = ... })`.

Without `UseWebSockets` in the pipeline, a `MapWebSocket` endpoint applies the default policy
itself, so a socket endpoint is never left without the cross-site defense. The endpoint's own
`CloseAsync` or `CloseOutputAsync` keeps working after a drain close: the policy coordinates the
two, so a receive loop written the usual way ends cleanly.

## Options

| Option | Default | Meaning |
| --- | --- | --- |
| `AllowedOrigins` | empty | Serialized origins, besides the request's own, whose pages may open a socket; validated when `UseWebSockets` runs |
| `AllowAnyOrigin` | `false` | Turns the hijacking defense off, for sockets that carry no ambient credentials |
| `KeepAliveInterval` | 30 seconds | How often an idle socket sends a keep-alive frame |
| `KeepAliveTimeout` | infinite | How long to wait for a pong before aborting; infinite sends unsolicited pongs |
| `DangerousEnableCompression` | `false` | Whether accepts negotiate permessage-deflate when the client offers it |

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Routing` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.Forwarded` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.WebSockets` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.WebSockets/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.WebSockets/src/Assimalign.Cohesion.Web.WebSockets.csproj`.
- **Verbs and options** — `cohesion/resources/Web/Assimalign.Cohesion.Web.WebSockets/src/Extensions/WebSocketExtensions.cs`, `cohesion/resources/Web/Assimalign.Cohesion.Web.WebSockets/src/Extensions/WebSocketEndpointExtensions.cs` and `cohesion/resources/Web/Assimalign.Cohesion.Web.WebSockets/src/WebSocketOptions.cs`.
- **Framework membership** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Runtime/Directory.Build.props`.
