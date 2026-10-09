# Server and TLS

The Web host composes listeners and drives each accepted connection through the application pipeline.

> **Status:** Implemented. HTTP/3 requires a supported QUIC platform.

## Host and server ownership

`WebApplication` is a `Host<WebApplicationContext>`. The concrete builder materializes explicit
`AddService` factories once at `Build()`, preserves their order, and starts those services before
any server. Reverse shutdown drains servers before stopping application services.

`WebApplicationServer` binds its listeners before reporting successful startup. A bind failure
becomes a `HostStartupException` retaining the transport cause, and the server logs it first (see
[Diagnostics](#diagnostics)). Accepted connections have separate tracked tasks; middleware faults
are contained rather than escaping the accept loop.

The server owns accepted connections, exchanges, and contexts, and tracks in-flight work for
shutdown, which lets the requests in flight finish before the connections close (see
[Graceful shutdown](#graceful-shutdown)).

Within a connection, HTTP/1.1 serves one exchange at a time, in order, because its transport
realigns on the next request only after the previous one finished. HTTP/2 and HTTP/3 run one task
per stream, so a slow request, a long poll, or a server-sent-events stream no longer holds up its
siblings. A fault in one stream answers that stream with a 500, or resets it once its response
started, and leaves the connection and the other streams running. The transport bounds the
concurrency: HTTP/2 admits at most `SETTINGS_MAX_CONCURRENT_STREAMS` streams and HTTP/3 at most
the QUIC stream credit, 100 each by default.

The request-body cap is enforced with 413 on HTTP/1.1, HTTP/2 and HTTP/3. HTTP/2 honors flow
control on buffered responses, HTTP/3 reads each request stream incrementally after its headers,
both send no body for `HEAD`, and both reject a malformed `:path` on its own stream without
closing the connection.

## Upgrades, WebSockets, and trailers

Every listener the default server composes gets three interceptors before any the application
registers: the request-size limit, the HTTP/1.1 protocol upgrade, and the HTTP/2 and HTTP/3
extended CONNECT (RFC 8441, RFC 9220). So `context.Upgrade`, `context.ExtendedConnect`, and a
WebSocket handshake work on every protocol without listener configuration, a request that no
handler accepts is served exactly as before, and an ordinary request pays only a version and header
check: each transition interceptor joins the response phase only of the exchanges that ask for a
transition, an HTTP/1.1 upgrade or `CONNECT`, or an extended CONNECT whose `:protocol` the transport
validated. There the extended CONNECT interceptor installs `IHttpExtendedConnectFeature`, from
`Http.ExtendedConnect`, whose `AcceptAsync` turns the stream into a duplex tunnel. WebSockets run
over both; [WebSockets](websockets.md) covers the endpoints, the origin policy, and the drain close.

A `UseServer` callback that clears `options.Interceptors` removes the defaults, and with them
WebSockets on every protocol: the HTTP/2 and HTTP/3 transports keep advertising extended CONNECT,
so browsers keep sending their handshakes that way, but nothing surfaces them. A host that clears
the list and still serves WebSockets adds the two transition interceptors back.

Trailer fields travel where the protocol can carry them. `Request.Trailers` is filled once the body
has been read to its end: on HTTP/1.1 for a chunked request, and on HTTP/2 and HTTP/3 for every
request. `Response.Trailers` is supported on HTTP/2 and HTTP/3, where the fields staged before the
response completes go out after the body, buffered or streamed. On HTTP/1.1, and for a `CONNECT`
exchange, its `IsSupported` is `false` and adding to it throws, so check it before staging:

```csharp
using Assimalign.Cohesion.Http;

// context is the exchange's IHttpContext.
if (context.Response.Trailers.IsSupported)
{
    context.Response.Trailers.Add(new HttpHeaderKey("Server-Timing"), "total;dur=12");
}
```

A pseudo-header, a connection-specific field, or a field RFC 9110 §6.5.1 prohibits in trailers
(`Content-Length`, `Host`, and the rest) is refused with `ArgumentException` when it is added, and a
response to `HEAD` sends no trailers. See
[Http's trailers](../dotnet-apis/libraries/http/assimalign-cohesion-http/design.md#trailers).

## Application configuration

`WebApplication.CreateBuilder(args)` loads configuration in increasing precedence:

1. Optional `appsettings.json`.
2. Optional `appsettings.{Environment}.json`.
3. Deployment settings, including `COHESION_CONFIG__` variables or their ambient equivalents.
4. Command-line arguments.

Enabled resources resolve files from `ResourceContext.ContentRootPath`; ordinary applications
use `AppContext.BaseDirectory`. The variable prefix is removed, and double underscores become
configuration path separators. In-process settings arrive through the ambient resource context
without modifying process-wide environment variables.

The web root, which `UseStaticFiles()` serves, is `wwwroot` under the content root unless
`WebApplicationOptions.WebRootPath` names another directory. The content root itself is never
served.

`Local` selects `appsettings.Local.json`; `Development` selects `appsettings.Development.json`.
They are distinct environments. The default remains `Production`.

## Listeners and TLS

Transport Layer Security (TLS) is composed beneath Hypertext Transfer Protocol (HTTP). The TLS
registrations on `HttpConnectionListenerOptions` take a transport configuration callback and
`TlsServerOptions`, compose the secure transport, and then register the HTTP listener; the
connection's security capability gives each request its `https` scheme. A registration names what
it serves, and the configuration's `Protocol` values are the registration names without `Use`:

| Registration | `Protocol` | Serves |
|---|---|---|
| `UseHttp1` | `Http1` | HTTP/1.1, cleartext |
| `UseHttp2` | `Http2` | prior-knowledge HTTP/2, cleartext |
| `UseHttps` | `Https` | HTTP/2 and HTTP/1.1 over TLS, chosen per connection |
| `UseHttp1s` | `Http1s` | HTTP/1.1 over TLS |
| `UseHttp2s` | `Http2s` | HTTP/2 over TLS |
| `UseHttp3` | `Http3` | HTTP/3 over QUIC, whose TLS is inherent |

`UseHttps` is the registration an `https` origin normally wants. Through Application-Layer
Protocol Negotiation (ALPN, RFC 7301) the client offers the protocols it speaks in the handshake;
`UseHttps` offers `h2` then `http/1.1` and serves each connection the protocol it negotiated, so a
browser that offers both gets HTTP/2, and a client that offers only `http/1.1`, or sends no ALPN at
all, gets HTTP/1.1. `UseHttp1s` and `UseHttp2s` serve their one protocol on every connection, so
their TLS options should not offer both identifiers.

```csharp
using System.Net;

using Assimalign.Cohesion.Connections.Security;
using Assimalign.Cohesion.Web.Hosting;

// certificate is the server's X509Certificate2, with its private key.
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Server.UseServer(options => options.UseHttps(
    tcp => tcp.EndPoint = new IPEndPoint(IPAddress.Any, 443),
    new TlsServerOptions { AuthenticationOptions = { ServerCertificate = certificate } }));
```

When the caller leaves the ALPN list unset, each registration fills it in: `UseHttps` with `h2`
then `http/1.1`, `UseHttp2s` with `h2`, `UseHttp1s` with `http/1.1`, and `UseHttp3` with `h3` and
TLS 1.3. A list the caller supplies is kept. QUIC registration defers binding until the
asynchronous listener lifecycle; the platform must provide a usable QUIC stack. When the
application also serves HTTP/3, a client that reached a TCP endpoint discovers it through the
`Alt-Svc` header on HTTP/1.1 and HTTP/2 responses, both protocols of a `UseHttps` endpoint
included, once the advertisement is turned on (`options.AdvertiseAltService(...)`, or by
configuring an `Http3` endpoint).

In an enabled resource, endpoint metadata names an ordinary Secret mount, defaulting to `tls`.
The delivered PEM document contains a leaf certificate, exactly one private key, and optional
issuer chain. Empty content means absent; malformed or multi-key bundles fail. Gateway-materialized
trust anchors travel separately from this certificate mount.

### Handshakes and failing clients

Each TLS handshake runs on its own task, away from the accept loop. A client that connects and
sends nothing holds up only its own handshake. When a handshake fails, only that client's
connection is lost, and the endpoint keeps accepting. A handshake fails for any of these reasons:
- the client sends bytes that are not TLS;
- the certificate policy refuses it;
- it exceeds `TlsServerOptions.HandshakeTimeout`, 10 seconds by default.

`TlsServerOptions.MaxConcurrentHandshakes` caps the connections held while their handshakes run
(512 by default). Above the cap, new clients wait in the TCP backlog. An endpoint that expects many
slow handshakes at once raises the cap or shortens the timeout.

HTTP/3 behaves the same way for QUIC's own handshakes. It uses `System.Net.Quic`'s 10-second
timeout and the listener's backlog.

Below TLS, a client that connects and resets before the server accepts it costs only that
connection too.

These are routine events on a public endpoint, so the server's log does not report them. Each one
is reported by an event source, which a tool enables by name:

| What happened | Event source | Event |
|---|---|---|
| A TLS handshake over TCP failed or timed out | `Assimalign.Cohesion.Connections` | `UpgradeFailed` (Warning) |
| A QUIC handshake failed | `Assimalign.Cohesion.Connections.Quic` | `HandshakeFailed` (Warning) |
| A client reset before the accept | `Assimalign.Cohesion.Connections.Tcp` | `AcceptSkipped` (Verbose) |

Before #1304 and #1308, any of these stopped the endpoint.

## Client certificates

Mutual TLS is set on the `TlsServerOptions` an endpoint is registered with, so every TLS
registration honors it: `UseHttps`, `UseHttp1s`, `UseHttp2s`, and
`UseHttp3(configure, tlsOptions)`.

| Policy | A client without a certificate | A presented certificate |
|---|---|---|
| (none) | served; no certificate is requested | — |
| `AllowClientCertificate(validate)` | served | must pass validation |
| `RequireClientCertificate(validate)` | refused | must pass validation |

`validate` receives the certificate, the chain the platform built, and the platform's verdict.
Without it, a certificate passes only when it chains to a root the machine trusts, so a private
certificate authority needs a callback:

```csharp
using Assimalign.Cohesion.Connections.Security;

// certificate is the server's certificate; thumbprints holds the thumbprints
// of the client certificates the service trusts.
TlsServerOptions tlsOptions = new TlsServerOptions
{
    AuthenticationOptions = { ServerCertificate = certificate },
}.RequireClientCertificate((client, chain, errors) => thumbprints.Contains(client.Thumbprint));
```

Call the policy methods last: assigning a new `AuthenticationOptions` afterwards drops the policy,
and they refuse to replace a validation callback they did not install. The certificate is requested
during the handshake and never afterwards, because HTTP/2 forbids renegotiation and post-handshake
authentication (RFC 9113 §9.2.1, §9.2.3), so there is no deferred mode.

A handler reads the session as `context.TlsConnection`: `ClientCertificate`, `Protocol`,
`CipherSuite`, and `ApplicationProtocol`, on HTTP/1.1, HTTP/2 and HTTP/3 alike, and `null` on a
cleartext exchange. The accessor ships in
[`Http.Tls`](../dotnet-apis/libraries/http/assimalign-cohesion-http-tls/index.md), an `App.Web`
member, and builds the session on first read from the handshake facts the transport publishes on
the exchange's connection info. Every exchange on a connection shares the session, and the
connection owns the certificate, so copy it to keep it beyond the exchange. Mapping a certificate to
a principal under an authentication scheme is not provided yet.

## Configuration-bound endpoints

`builder.Server.UseConfiguration(configuration)` explicitly binds the `Http` section's endpoints,
server limits, and connection cap. The binder is explicit and reflection-free, and a value that is
present but cannot be parsed fails instead of silently selecting a default.

A plain entry-point application, `WebApplication.CreateBuilder(args)` with no generated control
plane, that configures no listener of its own gets one when it is built: the endpoints under
`Http:Endpoints`, bound as `UseConfiguration` binds them, or otherwise HTTP/1.1 on
`127.0.0.1:5000`, loopback only. An explicit `Server.UseServer`/`UseConfiguration` call or another
registered server turns that default off, and an orchestrated resource binds its ambient endpoint
instead.

```json
{
  "Http": {
    "Endpoints": {
      "Public": {
        "Protocol": "Https", "Host": "0.0.0.0", "Port": 443,
        "Certificate": { "Path": "certs/site.pem", "KeyPath": "certs/site.key" },
        "ClientCertificateMode": "AllowCertificate"
      },
      "Quic": {
        "Protocol": "Http3", "Host": "0.0.0.0", "Port": 443,
        "Certificate": { "Path": "certs/site.pfx" }
      },
      "Internal": { "Protocol": "Http1", "Host": "localhost", "Port": 8080 }
    },
    "Limits": {
      "MaxConcurrentConnections": 1000,
      "MaxRequestBodySize": 30000000,
      "KeepAliveTimeout": "00:02:10",
      "Http2": { "MaxStreamsPerConnection": 100 }
    }
  }
}
```

| Key under `Http` | Value |
|---|---|
| `Endpoints:<name>:Protocol` | `Http1` (the default), `Http2`, `Https`, `Http1s`, `Http2s`, or `Http3`; `Http/1.1`, `Http1.1`, `h1`, `Http/2`, `Http2.0`, `h2`, `Http/3`, `Http3.0` and `h3` are accepted too, and anything else fails |
| `Endpoints:<name>:Host` | a literal IP address; `localhost` or no value for loopback; `*`, `+` or `0.0.0.0` for any address; `[::]` or `::` for any IPv6 address. Host names are not resolved |
| `Endpoints:<name>:Port` | required, 0 to 65535 |
| `Endpoints:<name>:Certificate` | TLS endpoints: the name of a Secret mount (when absent, the endpoint's registered mount, or `tls`), or a section with `Path` (a PEM or PFX file), `KeyPath` (a separate PEM key file), and `Password` (an encrypted PEM key or a protected PFX) |
| `Endpoints:<name>:ClientCertificateMode` | TLS endpoints: `NoCertificate` (the default), `AllowCertificate`, or `RequireCertificate` |
| `Limits:MaxConcurrentConnections` | a positive cap on the connections served at once; a cap set in code with `LimitConcurrentConnections` wins |
| `Limits:MaxRequestLineSize`, `Limits:MaxRequestHeaderCount`, `Limits:MaxRequestHeadersTotalSize` | HTTP/1.1 request limits |
| `Limits:MaxRequestBodySize` | bytes, or `unbounded` / `none` for no cap |
| `Limits:KeepAliveTimeout`, `Limits:RequestHeadersTimeout` | a `TimeSpan` (`00:00:30`), whole seconds, or `infinite` / `-1` |
| `Limits:Http2:<key>` | `MaxStreamsPerConnection`, `MaxRequestHeaderListSize`, `MaxResetStreamsPerWindow`, `MaxSettingsFramesPerWindow`, `MaxPingFramesPerWindow`, and `FloodDetectionWindow` |

An HTTP/1.1 endpoint, including the HTTP/1.1 connections of an `Https` endpoint, receives every
request limit above. An HTTP/2 endpoint receives `MaxRequestBodySize`, `KeepAliveTimeout`,
`RequestHeadersTimeout`, and the `Limits:Http2` keys; an HTTP/3 endpoint receives the first three.
`MaxConcurrentConnections` caps the server rather than an endpoint. Configuring an `Http3` endpoint
turns on the `Alt-Svc` advertisement, which a later `UseServer` callback can turn off. On an
operating system without `System.Net.Quic` an `Http3` endpoint is refused with
`PlatformNotSupportedException`; where the platform lacks a QUIC implementation, binding fails at
start.

A certificate file's leaf is the PEM file's first certificate, or the PFX entry that carries a
private key; the rest of the file is its chain. A relative path resolves against the content root.
The leaf must carry its private key and be inside its validity window, and a file that cannot be
read or decoded fails with an error naming the endpoint and the path. A `Password` in a checked-in
`appsettings.json` is plaintext; supply it as a deployment setting, for example
`COHESION_CONFIG__Http__Endpoints__Quic__Certificate__Password`, or on the command line.

Configuration cannot carry a validation callback, so a `ClientCertificateMode` endpoint accepts a
presented certificate only when it chains to a root the machine trusts; a private certificate
authority needs the code form. A cleartext endpoint that declares a mode other than `NoCertificate`
is refused, as is an unknown mode. Not bound from configuration: the HTTP/3 header-frame limit and
QPACK options, QUIC stream limits, a validation callback, and data-rate limits.

## Graceful shutdown

When the host stops, the default server drains lame-duck style: it accepts nothing new, tells every
peer the connection is closing, lets the requests in flight finish within the host's shutdown
budget, and cancels only what outlives it.

- **The budget** — the stop's cancellation token. `Host<TContext>.StopAsync` cancels it when the
  host's `ShutdownTimeout` elapses: 30 seconds by default for an ordinary host, and for an
  orchestrated resource its stop grace less five seconds, at least five.
- **The announcement** — HTTP/1.1 answers the request in flight with `Connection: close` and ends
  the connection after it, and an idle keep-alive connection ends at once. HTTP/2 sends
  `GOAWAY(NO_ERROR)` naming the last stream processed and refuses later streams with
  `RST_STREAM(REFUSED_STREAM)`. HTTP/3 accepts no further request stream and sends a `GOAWAY`.
- **The requests in flight** — nothing is cancelled while the budget lasts: a running request
  finishes, and its response is delivered.
- **Long-lived exchanges** — every exchange carries `IWebServerDrainFeature`, whose `Draining` token
  fires when the stop begins and cancels nothing, so an exchange that would otherwise run until the
  budget cuts it off can end its own work in time. `UseWebSockets` closes each open socket with
  `1001 Going Away` there (see [WebSockets](websockets.md#shutdown-the-drain-close)).
- **When the budget runs out** — the server logs a warning with what is still in flight, every
  request still running observes `RequestCancelled`, and every connection still open is aborted.
  The stop waits up to one second for them to unwind, releases the listener, and completes
  normally; the host reports the cut-short stop itself. A handler that ignores `RequestCancelled`
  keeps running after the stop, so long-running handlers should honor it.

`WebApplicationTestFactory` disposal stops the server with a budget that has already run out, so a
test whose requests must finish calls `StopAsync` first (see [Testing](testing.md)).

## Diagnostics

The default server logs its own failures through the application's logging, `builder.Logging`,
under the category `Assimalign.Cohesion.Web.Hosting.WebApplicationServer`. With no logging provider
registered, nothing is written.

| Event | Level |
|---|---|
| A listener cannot be bound; logged before `HostStartupException` propagates | `Critical` |
| The accept loop faults, a cancellation the server did not request included; the server keeps running but accepts nothing more | `Critical` |
| A connection fault the server cannot blame on the peer, such as a response that could not be framed | `Error` |
| A connection the peer or the network ended (`IOException`, `SocketException`, `ConnectionException`), or a fault after the server aborted its drain | `Debug` |
| A stop's budget ran out with work in flight; logged before the abort | `Warning` |

Entries carry structured attributes: the listener's protocols (`http.server.listener.protocols`);
the connection's id, endpoints, and protocol version (`connection.id`, `network.local.address`,
`network.local.port`, `network.peer.address`, `network.peer.port`, `network.protocol.version`); and
the connections, exchanges, and time a cut-short drain left (`http.server.drain.connections`,
`http.server.drain.exchanges`, `http.server.drain.duration`). No request or response content is
ever logged: no header value, body, path, or query. An exception the application's pipeline throws
is not logged here; the server answers it with a 500 or a reset, and reporting it belongs to the
application's error handling and to request telemetry.

The logger factory's default minimum level is `Information`, so the `Debug` entries need a rule for
the server's category:

```csharp
using Assimalign.Cohesion.Logging;
using Assimalign.Cohesion.Web.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Logging.AddRule("Assimalign.Cohesion.Web.Hosting.WebApplicationServer", LogLevel.Debug);
```

Request spans, HTTP server metrics, and the request id are covered in
[Observability](observability.md).

## Enabled-resource management

The host discovers its entry assembly's generated `ResourceRuntime` registration. Ambient
binding tries endpoint name `http`, then `https`, and accepts the corresponding URI schemes. An
ambient `https` endpoint is registered through `UseHttps`, so it serves HTTP/2 and HTTP/1.1.
The shared `Web.Hosting.Resources` terminal authenticates namespaced management requests.

Graceful stop returns HTTP 202 before invoking the shutdown signal through a response-completion
hook. This lets the acknowledgement reach the caller before the server's stop begins.

Return to [Web](index.md).

## Sources

- **Runtime** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/docs/OVERVIEW.md` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/docs/DESIGN.md`.
- **TLS registrations** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/src/Extensions/WebHostingExtensions.cs`.
- **Configuration binder** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/src/Internal/HttpServerConfiguration.cs`.
- **Client certificates** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/src/TlsServerOptions.cs` and `cohesion/libraries/Http/Assimalign.Cohesion.Http.Tls/src/Abstractions/IHttpTlsConnectionFeature.cs`.
- **Drain and diagnostics** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/src/Internal/WebApplicationServer.cs` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/src/Internal/WebApplicationServerLog.cs`.
- **Default interceptors and the drain signal** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/src/WebApplicationServerBuilder.cs`, `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/src/HttpExtendedConnect.cs` and `cohesion/resources/Web/Assimalign.Cohesion.Web/src/Abstractions/IWebServerDrainFeature.cs`.
- **Trailers** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Abstractions/IHttpResponse.cs`, `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/docs/DESIGN.md` and `cohesion/docs/libraries/Http/DECISIONS.md` (ADR 2).
- **Shutdown budget** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/Implementation/HostOptions.TContext.cs`.
- **Control-plane ownership** — `cohesion/docs/resources/Web/DESIGN.md`.
- **Certificate carrier** — `cohesion/docs/RUNTIME_CONTRACT.md`.
