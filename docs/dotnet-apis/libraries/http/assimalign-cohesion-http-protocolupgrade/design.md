# Assimalign.Cohesion.Http.ProtocolUpgrade design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.ProtocolUpgrade`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The package models the HTTP/1.1 *connection transitions* — an RFC 9110 §7.8 protocol **upgrade**
(`Connection: upgrade` and `Upgrade`, answered with `101 Switching Protocols`) and an RFC 9110
§9.3.6 **CONNECT** tunnel (answered with `200 OK`) — as an explicit capability on `IHttpContext`. An
application detects the transition through `context.Upgrade`, accepts it, and receives the raw
duplex transport stream to drive the negotiated protocol or the tunnel. The WebSocket handshake of
`Assimalign.Cohesion.Http.WebSockets` is the main consumer: it accepts the upgrade and hands the
stream to the BCL's RFC 6455 framing.

It is the HTTP/1.1 counterpart of `Assimalign.Cohesion.Http.ExtendedConnect`. HTTP/2 and HTTP/3
removed the `Upgrade` mechanism (RFC 9113 §8.6, RFC 9114 §4.2), so this package is HTTP/1.1 only.

## Two interceptor hooks, no transport dependency (#751)

The transport (`Assimalign.Cohesion.Http.Connections`) must not reference this package, so the whole
capability lives here, wired through the transport's two generic interceptor seams:

1. **Detection.** `AfterRequestHead` sees the parsed head before dispatch and records a matched
   transition as an internal candidate feature. It checks the version itself, because the
   request-parse seam runs on every version. A CONNECT is `Method == CONNECT`; an upgrade requires
   both a `Connection: upgrade` token and a non-empty `Upgrade` header, and CONNECT takes precedence
   (§7.8 requires ignoring `Upgrade` on CONNECT). A matched transition also adds the interceptor to
   that exchange's response phase (`HttpExchangeInterceptorRequestContext.AddResponseInterceptor`).
2. **Materialization.** `BeforeResponse` runs at the setup of each exchange step 1 joined, after the
   head is parsed and before the handler. When the transport's exchange control can surrender the
   connection (`CanTakeOver`), it installs `IHttpProtocolUpgradeFeature`; otherwise `context.Upgrade`
   reads `null` rather than surfacing an upgrade whose accept could never work.

The transport installs nothing: a host registers the interceptor on each listener it wants upgrades
on. The Web host (`Web.Hosting`) registers it on every listener by default, after the request-size
interceptor, so a WebSocket handshake works with no listener configuration (decision 16, the Http
area's ADR 1 in `cohesion/docs/libraries/Http/DECISIONS.md`); a request that no application accepts
is served exactly as before. Neither the core nor the transport carries an upgrade-specific type: the
core owns the generic `IHttpExchangeControl`, the transport implements it per version and owns the
raw-stream handover, and this package owns every upgrade semantic.

## The cost of a default-on interceptor

Materialization needs the exchange control, and the transport builds an exchange's response sink and
exchange control only when some interceptor takes part in its response phase. Declaring
`HttpInterceptorScopes.All` would make that every exchange, on every protocol version, because the
Web host registers this interceptor on every listener. So the interceptor declares
`HttpInterceptorScopes.Request` and joins the response phase only of the exchange whose head asked
for a transition. An ordinary HTTP/1.1 request, and every HTTP/2 and HTTP/3 request (including an
extended CONNECT WebSocket, which `Http.ExtendedConnect`'s interceptor carries the same way, over the
control's `AcceptTunnelAsync`), costs one version check and, on HTTP/1.1, a method check and a
`Connection` header lookup. The transport's
`HttpExchangeResponseInterceptorTests` pin the fast path on all three versions and the takeover on an
upgrade; `Web.Hosting`'s design records the measured allocations.

## The accept path

`AcceptAsync` is single-shot: an `Interlocked` guard throws `InvalidOperationException` on a second
call before any byte is written, so a second response can never reach the wire. It:

1. resolves the status line (`101` for an upgrade, `200` for a CONNECT) before side effects;
2. claims the connection first (`IHttpExchangeControl.TakeOver()`), so even a cancelled or failed
   head write cannot be followed by a second HTTP response on a desynchronized stream;
3. scrubs `Content-Length` and `Transfer-Encoding` (RFC 9112 §9.9, RFC 9110 §9.3.6);
4. writes the head to the surrendered stream — `Connection: Upgrade` and `Upgrade: <protocol>` for an
   upgrade, no `Connection` header for a CONNECT — with the response headers and cookies the
   application set before accepting (`Sec-WebSocket-Accept`, for example);
5. returns the raw stream. The caller owns I/O on it; the transport still owns the connection's
   disposal.

The HTTP/1.1 parser reads no body for a bodyless upgrade `GET` and skips body framing for CONNECT,
so no post-transition octet is buffered: octets the client pipelined behind the handshake are
readable from the returned stream.

## Non-goals

- **No protocol beyond the transition.** The package writes the `101` and surrenders the stream;
  what runs over it is the consumer's. WebSockets are `Http.WebSockets`, which validates the RFC 6455
  handshake, stages `Sec-WebSocket-Accept` and its negotiated fields before accepting through
  `context.Upgrade`, and hands the stream to the BCL's `WebSocket.CreateFromStream`.
- **No client-side initiation.** Server-side accept surface only.
- **No HTTP/2 or HTTP/3 upgrade.** Those versions removed `Upgrade`; their bootstrap is extended
  CONNECT ([`Assimalign.Cohesion.Http.ExtendedConnect`](../assimalign-cohesion-http-extendedconnect/index.md)).
- **No installation by the transport.** A host registers the interceptor per listener; the Web host
  does so by default.

## AOT posture

Pure managed code with no reflection, dynamic code generation, or runtime type inspection. Detection
is token scanning over parsed headers, and the accept path is string and buffer work over the
surrendered stream.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Http`, `Assimalign.Cohesion.Http.Cookies`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/src/Assimalign.Cohesion.Http.ProtocolUpgrade.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ProtocolUpgrade/src`.

- **Source** — `cohesion/docs/libraries/Http/DECISIONS.md`.
