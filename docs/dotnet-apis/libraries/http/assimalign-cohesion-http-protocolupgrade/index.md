# Assimalign.Cohesion.Http.ProtocolUpgrade

Models HTTP/1.1 upgrades and CONNECT tunnels.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

Models the HTTP/1.1 connection transitions — an RFC 9110 §7.8 protocol **upgrade**
(`101 Switching Protocols`) and an RFC 9110 §9.3.6 **CONNECT** tunnel (`200 OK`) — as an opt-in
capability on `IHttpContext`, wired entirely through the server transport's interceptor seams.

- **Detect** that an HTTP/1.1 exchange is an upgrade or a CONNECT, surfaced as `context.Upgrade`.
- **Accept** the transition: write the `101` or `200` response and surrender the raw duplex
  transport stream for the negotiated protocol or tunnel.

WebSockets are the main consumer: `context.WebSockets`
([`Assimalign.Cohesion.Http.WebSockets`](../assimalign-cohesion-http-websockets/index.md)) validates
the RFC 6455 handshake, answers with `Sec-WebSocket-Accept`, and accepts through this upgrade. HTTP/2
and HTTP/3 removed `Upgrade`; their bootstrap is extended CONNECT, which
[`Assimalign.Cohesion.Http.ExtendedConnect`](../assimalign-cohesion-http-extendedconnect/index.md)'s
interceptor surfaces.

The Web host (`Web.Hosting`) registers the interceptor on every listener by default, after the
request-size interceptor, so a WebSocket handshake works with no listener configuration. The
interceptor declares the request scope and joins the response phase only of the exchanges whose head
asks for a transition, an HTTP/1.1 upgrade or `CONNECT`, so every other request, and every HTTP/2 and
HTTP/3 request, stays on the transport's fast path. A request that no application accepts is served
exactly as before. See the [design](design.md#the-cost-of-a-default-on-interceptor).

## Usage

On a host other than `Web.Hosting`, register the single interceptor on the listener options:

```csharp
using Assimalign.Cohesion.Http;
using Assimalign.Cohesion.Http.Connections;

HttpConnectionListenerOptions options = new();
options.Interceptors.Add(HttpProtocolUpgrade.CreateInterceptor());
```

Then, in a handler:

```csharp
using System.IO;

using Assimalign.Cohesion.Http;

// context is the exchange's IHttpContext.
if (context.Upgrade is { Kind: HttpProtocolUpgradeKind.Upgrade, Protocol: "example/1" } upgrade)
{
    Stream tunnel = await upgrade.AcceptAsync(context.RequestCancelled);
    // ... drive the negotiated protocol over tunnel.
}
// context.Upgrade is null for ordinary exchanges, on HTTP/2 and HTTP/3, and when the
// interceptor is not registered.
```

`AcceptAsync` checks every response field before it claims the connection: a name that is not a
token, or a value holding CR, LF, NUL, or another control character but HTAB, throws an
`HttpException` with `HttpErrorCode.InvalidResponseField`. Nothing has been written then, so the
exchange can still be answered with an ordinary response; validate any request text a handler
copies into a response header. See the [design](design.md#the-accept-path).

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Http.Cookies`](../../http/assimalign-cohesion-http-cookies/index.md) | `CohesionProjectReference` |

The server transport (`Assimalign.Cohesion.Http.Connections`) exposes its exchange control on the
response-interceptor seam, and this package consumes its takeover capability, which only the
HTTP/1.1 exchange control offers. Neither references the other. The package is a member of the
`App.Web` shared framework, and a private runtime member of every area framework that carries
`Web.Hosting`.

## Principal public types

| Type | Source file |
|---|---|
| `HttpProtocolUpgrade` | `src/HttpProtocolUpgrade.cs` |
| `HttpProtocolUpgradeKind` | `src/HttpProtocolUpgradeKind.cs` |
| `HttpContextProtocolUpgradeExtensions` | `src/Extensions/HttpContextProtocolUpgradeExtensions.cs` |
| `IHttpProtocolUpgrade` | `src/Abstractions/IHttpProtocolUpgrade.cs` |
| `IHttpProtocolUpgradeFeature` | `src/Abstractions/IHttpProtocolUpgradeFeature.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/src/Assimalign.Cohesion.Http.ProtocolUpgrade.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/src/HttpProtocolUpgrade.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/src/HttpProtocolUpgradeKind.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/src/Extensions/HttpContextProtocolUpgradeExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/src/Abstractions/IHttpProtocolUpgrade.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/src/Abstractions/IHttpProtocolUpgradeFeature.cs`.

- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/docs/DESIGN.md`.
