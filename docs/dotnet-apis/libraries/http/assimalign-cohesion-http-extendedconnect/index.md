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

The accessors read the `IHttpExtendedConnectFeature` the HTTP/2 and HTTP/3 transports
(`Assimalign.Cohesion.Http.Connections`) install on every valid extended CONNECT, so they return the
same instance on every read, and an ordinary exchange, any HTTP/1.1 exchange included, reads `null`.
The contract and its `AcceptAsync` live in the core, `Assimalign.Cohesion.Http` (#1316); this package
keeps the accessors. Neither it nor the transport references the other.

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

The package is a member of the `App.Web` shared framework.

## Principal public types

| Type | Source file |
|---|---|
| `HttpExtendedConnectExtensions` | `src/Extensions/HttpExtendedConnectExtensions.cs` |

`IHttpExtendedConnectFeature` is declared in the core, in
`cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Abstractions/IHttpExtendedConnectFeature.cs`.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/src/Assimalign.Cohesion.Http.ExtendedConnect.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/src/Extensions/HttpExtendedConnectExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Abstractions/IHttpExtendedConnectFeature.cs`.

- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Runtime/Directory.Build.props`.
