# Assimalign.Cohesion.Http.ExtendedConnect

Surfaces the HTTP/2 and HTTP/3 extended CONNECT feature on `IHttpContext`.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

Gives applications the HTTP/2 and HTTP/3 *extended CONNECT* mechanism (RFC 8441, RFC 9220) as a
feature on `IHttpContext`: a `CONNECT` request that carries the `:protocol` pseudo-header asks to run
another protocol, most often WebSocket, over its one stream.

- **Detect** that the current exchange is an extended CONNECT: `context.IsExtendedConnect`, and
  `context.ExtendedConnect`, the feature or `null`.
- **Read** the requested `:protocol` value (for example `websocket`).
- **Accept** the exchange as a duplex tunnel: a `200` response head, then raw octets in both
  directions over the exchange's stream.

The package owns the `IHttpExtendedConnectFeature` contract and the exchange interceptor that
installs it (#1368). On an HTTP/2 or HTTP/3 extended CONNECT the transport validated, the
interceptor installs the feature and binds it to the transport's exchange control, whose
`AcceptTunnelAsync` commits the `200` and returns the tunnel. The accessors are plain feature reads,
so they return the same instance on every read, and an ordinary exchange, any HTTP/1.1 exchange
included, reads `null`.

## Registration

The feature is installed by an exchange interceptor, so register it on the listener:

```csharp
using Assimalign.Cohesion.Http;
using Assimalign.Cohesion.Http.Connections;

HttpConnectionListenerOptions options = new();
options.Interceptors.Add(HttpExtendedConnect.CreateInterceptor());
```

The Web host (`Assimalign.Cohesion.Web.Hosting`) registers it by default. Without it, an extended
CONNECT reaches the application as an ordinary `CONNECT` and `context.ExtendedConnect` is `null`,
although the HTTP/2 and HTTP/3 transports still advertise extended CONNECT to clients. An ordinary
exchange pays one null check and the interceptor's no-op body hooks, and the interceptor allocates
nothing for it. On a listener with no other request-scoped interceptor, registering it does make the
transport build its per-exchange request-parse context (see the [design](design.md#the-interceptor)).

## Usage

```csharp
using System.IO;
using System.Net.WebSockets;

using Assimalign.Cohesion.Http;

// context is the exchange's IHttpContext.
if (context.ExtendedConnect is { Protocol: "websocket" } extendedConnect)
{
    await using Stream tunnel = await extendedConnect.AcceptAsync(context.RequestCancelled);

    // Reads return the client's DATA; writes go out as DATA at once. Disposing ends the server's
    // side (END_STREAM / FIN). For WebSocket, run the BCL framing over the tunnel:
    using WebSocket socket = WebSocket.CreateFromStream(tunnel, new WebSocketCreationOptions { IsServer = true });
    // ...
}
```

Set any response headers (for example `sec-websocket-protocol`) before accepting; they travel on the
`200`. For a WebSocket, use `context.WebSockets`
([`Assimalign.Cohesion.Http.WebSockets`](../assimalign-cohesion-http-websockets/index.md)) instead: it
validates the RFC 8441 and RFC 9220 handshake (`sec-websocket-version: 13`), negotiates the
subprotocol and permessage-deflate, and accepts through this feature, with the same calls as on
HTTP/1.1.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |

The core supplies the interceptor seam, the validated `:protocol` on the request context, and the
exchange control's `AcceptTunnelAsync`. The HTTP/2 and HTTP/3 transports
(`Assimalign.Cohesion.Http.Connections`) validate extended CONNECT and implement the tunnel accept on
their exchange controls; this package's interceptor wraps it into the feature. Neither references the
other. The package is a member of the `App.Web` shared framework, and a private runtime member of
every area framework that carries `Web.Hosting`.

## Principal public types

| Type | Source file |
|---|---|
| `HttpExtendedConnect` | `src/HttpExtendedConnect.cs` |
| `HttpExtendedConnectExtensions` | `src/Extensions/HttpExtendedConnectExtensions.cs` |
| `IHttpExtendedConnectFeature` | `src/Abstractions/IHttpExtendedConnectFeature.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/src/Assimalign.Cohesion.Http.ExtendedConnect.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/src/HttpExtendedConnect.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/src/Extensions/HttpExtendedConnectExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/src/Abstractions/IHttpExtendedConnectFeature.cs`.

- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Runtime/Directory.Build.props`.

- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/docs/DESIGN.md`.
