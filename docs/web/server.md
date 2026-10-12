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

Startup failures leave the host `Failed` and preserve the original exception even if rollback
service resolution also throws. Retrying starts a fresh attempt. Register middleware with `Use`
before starting the application or explicitly building its pipeline; later registration throws
`InvalidOperationException` because the pipeline is already built.

The server owns accepted connections, exchanges, and contexts, and tracks in-flight work for
shutdown, which lets the requests in flight finish before the connections close (see
[Graceful shutdown](#graceful-shutdown)).

Each exchange's feature collection is sized for the features the server knows it carries: the
application features the pipeline stamps plus the host's own five (#1381), rounded up to a size an
unsized collection grows through, so an exchange that carries more never costs more than an unsized
one. Middleware install features of their own as an exchange passes through them. A host whose
middleware put one on every exchange adds a slot for each in a `UseServer` configuration, for
example `options.ExchangeFeatureCapacity += 3` for routing, `UseResponseCompression` and
`UseSecurityHeaders`; `0` turns the sizing off.

Within a connection, HTTP/1.1 serves one exchange at a time, in order, because its transport
realigns on the next request only after the previous one finished. HTTP/2 and HTTP/3 run one task
per stream, so a slow request, a long poll, or a server-sent-events stream no longer holds up its
siblings. A fault in one stream answers that stream with a 500, or resets it once its response
started, and leaves the connection and the other streams running. The transport bounds the
concurrency: HTTP/2 admits at most `SETTINGS_MAX_CONCURRENT_STREAMS` streams and HTTP/3 at most
the QUIC stream credit, 100 each by default.

The request-body cap is enforced with 413 on HTTP/1.1, HTTP/2 and HTTP/3. HTTP/2 honors flow
control on buffered responses, HTTP/3 reads each request stream incrementally after its headers,
and both reject a malformed `:path` on its own stream without closing the connection. The three
versions accept the same request targets (#1334): a `CONNECT` names only `host:port`, the asterisk
form `*` is for `OPTIONS` alone, and a method is a token of at most 32 characters. HTTP/2 and HTTP/3
reset the stream of any other request; HTTP/1.1 answers it with `400`, or `501` for a method longer
than 32 characters. An octet above `0x7F` in a header value reaches `Request.Headers` as the same
Latin-1 character on every version (#1387).

A response that cannot carry content follows one rule on every version (#1073). The body and
trailers staged for a `HEAD` response, a `204` or a `304` are discarded, not refused; a `204` never
carries `Content-Length`, a `304` gets none synthesized from its empty body, and a `HEAD` response
states a length only when the handler staged a non-empty body or set one.

Every final response carries a cached `Date` unless the application supplied one. Interim responses
carry no automatic `Date`. To let a proxy supply it, set `options.AddDateHeader = false` in
`Server.UseServer`, or `Http:AddDateHeader` through `UseConfiguration`. Static-file date validators
require a valid response `Date` before file selection; entity tags work without it.

`KeepAliveTimeout`, `RequestHeadersTimeout` and the minimum request-body data rate apply to every
version. Before #1085 only HTTP/1.1 enforced them, so an HTTP/2 or HTTP/3 client could hold a
connection or a stream open by sending nothing. A body that falls below the rate is answered `408`.
A limit or a framing error found while a handler reads the body is answered by the transport with
`413`, `408`, `431` or `400` in place of the server's `500` (#1339); `431` is a trailer section over
its bounds (#1375). See
[Client faults and refused responses](#client-faults-and-refused-responses).

The minimum response data rate holds every response on every version too, buffered and streamed
(#1384); before it, only the HTTP/1.1 streaming sink enforced it. A client that grants no
flow-control credit or stops reading is reclaimed: the connection is closed on HTTP/1.1, the stream
reset on HTTP/2 (or the connection ended, when its whole window stalls), and the request stream
reset on HTTP/3. The exchange is recorded as cancelled, not as an application fault, because the
stalled write throws an `OperationCanceledException` carrying `RequestCancelled`.

A request that carries `Expect: 100-continue` gets its `100 Continue` at the handler's first body
read on every version (#1385), so a handler that answers `401` or `417` from the head never invites
the body. HTTP/2 and HTTP/3 used to send none, and a client that waited for it stalled until its own
timeout.

## Upgrades, WebSockets, and trailers

Every listener the default server composes gets five interceptors before any the application
registers: the request limits, the HTTP/1.1 protocol upgrade, the HTTP/2 and HTTP/3
extended CONNECT (RFC 8441, RFC 9220), the client fault, and response starting. The first surfaces
the body-size cap and the minimum request-body and response data rates per request (`context.MaxRequestBodySize`,
`context.MinRequestBodyDataRate`, `context.MinResponseDataRate`; see
[Configuration-bound endpoints](#configuration-bound-endpoints)). So `context.Upgrade`,
`context.ExtendedConnect`, and a WebSocket handshake work on every protocol without listener
configuration, a request that no handler accepts is served exactly as before, and an ordinary
request pays only a version and header check: each transition interceptor joins the response phase
only of the exchanges that ask for a transition, an HTTP/1.1 upgrade or `CONNECT`, or an extended
CONNECT whose `:protocol` the transport validated. There the extended CONNECT interceptor installs
`IHttpExtendedConnectFeature`, from `Http.ExtendedConnect`, whose `AcceptAsync` turns the stream
into a duplex tunnel. WebSockets run over both; [WebSockets](websockets.md) covers the endpoints,
the origin policy, and the drain close. The client-fault interceptor joins only a request that can
have a body, on every version (see
[Client faults and refused responses](#client-faults-and-refused-responses)).

A `UseServer` callback that clears `options.Interceptors` removes the defaults, and with them
WebSockets on every protocol: the HTTP/2 and HTTP/3 transports keep advertising extended CONNECT,
so browsers keep sending their handshakes that way, but nothing surfaces them. A host that clears
the list and still serves WebSockets adds the two transition interceptors back. Clearing it also
removes the client-fault classification: a malformed or over-limit body is then logged at `Error`
and observed by `OnException` again, while the transport still answers it with its own status.
It also removes `IWebResponseStartingFeature`, which CORS and SecurityHeaders require; both fail
explicitly when that feature is absent.

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

## Client faults and refused responses

`IWebResponseStartingFeature.OnStarting(callback, state)` stages final headers before a buffered or
streamed head is committed. Callbacks run once, in reverse registration order, and must not write
the response. A callback failure aborts before the head is written. CORS and SecurityHeaders use
this hook, so their policies survive an error response from a boundary on either side once the
middleware has processed the request.

A handler that throws before its response starts is answered with a bodyless `500`; the staged
headers, trailers and body are dropped, then starting-callback policy header changes are restored.
So is a response the transport refuses to send. Every protocol refuses a header or trailer whose
name is not a token, or whose value holds CR, LF, NUL, or
another control character but HTAB, before it writes any of the head (#1183). Such a value is
typically request text copied into a header unvalidated, which would otherwise split an HTTP/1.1
response (CWE-113). The `500` replaces the refused response, its completion callbacks do not run,
and an HTTP/1.1 connection stays usable. A refused trailer section on a response already streamed
cannot be replaced, so the transport resets that stream. HTTP/2 and HTTP/3 also send every value
without leading or trailing spaces and tabs, which those versions do not allow.

A streamed head refused before any bytes are committed stays unstarted and can be retried or
replaced with a buffered response. Successful starting callbacks do not run again; their policy
header edits survive the replacement, while its status, body and representation metadata remain
the replacement's.

Completion callbacks run in registration order only after the application's own response reaches
the transport. Every callback is attempted even if an earlier one throws; failures are collected
in an `AggregateException` and logged individually without changing the response already sent.
They do not run after a replacement `500`, a reset, or a cancelled exchange, including a boundary
that aborts a started response and returns normally.

A request body that breaks its framing or a limit while a handler reads it is a client fault.
On HTTP/1.1, this also includes a body cut short by the client cleanly closing the connection. The read still
throws, and the transport answers the exchange with its own status in place of the server's `500`:
`400`, `413`, `408` or `431`. On a request that can have a body, on every version, the
client-fault interceptor publishes that status as `IWebClientFaultFeature`, a
[`Web.Server`](../dotnet-apis/resources/web/assimalign-cohesion-web-server/index.md) contract
(#1340, #1378). The HTTP logging middleware then logs the exchange at its configured level instead
of `Error`, and the exception boundary skips `OnException` and its `OnError` chain and stages the
transport's status. Request telemetry reports the status the transport sent, with no `error.type`
(see [Observability](observability.md#the-request-span)).

On HTTP/2 and HTTP/3 a body that contradicts its content-length, or a malformed trailer section,
makes the request malformed, so the transport resets the stream instead of answering it: the client
gets no status, the feature reports `400`, and the reset also fires `RequestCancelled`, so a
pipeline that meets the cancellation rather than the read's failure ends as a cancelled request. A
client that resets its own request is not at fault on either version: the pipeline ends as
cancelled and the server resets the exchange instead of staging a `500`.

HTTP/3 connection loss after dispatch also cancels every affected exchange. A body read throws
`OperationCanceledException` with `RequestCancelled`, without a client-fault status, `OnException`,
or a staged `500`. Telemetry records `request_canceled`.

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
connection's security capability gives each request its `https` scheme. A request-target cannot
claim it on a cleartext connection by default: HTTP/1.1 answers an `https` absolute-form target on
a cleartext connection with `421 Misdirected Request` and closes the connection, and an h2c stream whose `:scheme` is
`https` is answered `421` before dispatch. A trusted gateway forwarding that scheme to an h2c
backend can opt in with `options.AllowAlternateSchemes = true` inside `Server.UseServer`, or
`Http:AllowAlternateSchemes` through `UseConfiguration`. This preserves the gateway's scheme in
`Request.Scheme`; the default is `false`.

A registration names what it serves, and the configuration's `Protocol` values are the registration
names without `Use`:

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
connection too, as does a socket that fails to be set up right after the accept. When an accept
fails for want of file descriptors or socket buffers, or on Unix for an error .NET does not name,
the TCP listener waits and retries the accept, from 5 ms doubling to at most 1 second, instead of
stopping the endpoint (#1312); it recovers once connections close.

None of these reach the server's log. Each one is reported by an event source, which a tool enables
by name:

| What happened | Event source | Event |
|---|---|---|
| A TLS handshake over TCP failed or timed out | `Assimalign.Cohesion.Connections` | `UpgradeFailed` (Warning) |
| A QUIC handshake failed | `Assimalign.Cohesion.Connections.Quic` | `HandshakeFailed` (Warning) |
| A client reset before the accept, or on Linux a network error was pending on the queued connection | `Assimalign.Cohesion.Connections.Tcp` | `AcceptSkipped` (Verbose) |
| An accepted socket could not be set up and was closed | `Assimalign.Cohesion.Connections.Tcp` | `AcceptedConnectionDropped` (Verbose) |
| An accept failed for want of descriptors or buffers, or on Unix for an error .NET does not name, and waited | `Assimalign.Cohesion.Connections.Tcp` | `AcceptBackoff` (Warning, at most once a second per listener) |

Before #1304, #1308 and #1312, any of these stopped the endpoint.

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
      "MinRequestBodyDataRate": { "BytesPerSecond": 240, "GracePeriod": "00:00:05" },
      "MinResponseDataRate": { "BytesPerSecond": 240, "GracePeriod": "00:00:05" },
      "Http2": { "MaxStreamsPerConnection": 100 }
    }
  }
}
```

| Key under `Http` | Value |
|---|---|
| `AllowAlternateSchemes` | `false` by default; `true` lets a trusted gateway forward `:scheme https` to an h2c backend and preserves it in `Request.Scheme` |
| `AddDateHeader` | `true` by default; `false` disables automatic final-response dates so a proxy can supply them |
| `Endpoints:<name>:Protocol` | `Http1` (the default), `Http2`, `Https`, `Http1s`, `Http2s`, or `Http3`; `Http/1.1`, `Http1.1`, `h1`, `Http/2`, `Http2.0`, `h2`, `Http/3`, `Http3.0` and `h3` are accepted too, and anything else fails |
| `Endpoints:<name>:Host` | a literal IP address; `localhost` or no value for loopback; `*`, `+` or `0.0.0.0` for any address; `[::]` or `::` for any IPv6 address. Host names are not resolved |
| `Endpoints:<name>:Port` | required, 0 to 65535 |
| `Endpoints:<name>:Certificate` | TLS endpoints: the name of a Secret mount (when absent, the endpoint's registered mount, or `tls`), or a section with `Path` (a PEM or PFX file), `KeyPath` (a separate PEM key file), and `Password` (an encrypted PEM key or a protected PFX) |
| `Endpoints:<name>:ClientCertificateMode` | TLS endpoints: `NoCertificate` (the default), `AllowCertificate`, or `RequireCertificate` |
| `Limits:MaxConcurrentConnections` | a positive cap on the connections served at once; a cap set in code with `LimitConcurrentConnections` wins |
| `Limits:MaxRequestLineSize`, `Limits:MaxRequestHeaderCount`, `Limits:MaxRequestHeadersTotalSize`, `Limits:MaxChunkFramingLineSize` | HTTP/1.1 request limits |
| `Limits:MaxRequestBodySize` | bytes, or `unbounded` / `none` for no cap |
| `Limits:KeepAliveTimeout`, `Limits:RequestHeadersTimeout` | a `TimeSpan` (`00:00:30`), whole seconds, or `infinite` / `-1` |
| `Limits:MinRequestBodyDataRate`, `Limits:MinResponseDataRate` | a section with `BytesPerSecond` (a positive, finite number) and `GracePeriod` (a positive `TimeSpan`), keeping the built-in value of a dimension it leaves out; `none` or `disabled` turns the rate off, and a JSON `null` keeps the default |
| `Limits:Http2:<key>` | `MaxStreamsPerConnection`, `MaxRequestHeaderListSize`, `MaxResetStreamsPerWindow`, `MaxSettingsFramesPerWindow`, `MaxPingFramesPerWindow`, and `FloodDetectionWindow` |

An HTTP/1.1 endpoint, including the HTTP/1.1 connections of an `Https` endpoint, receives every
request limit above. An HTTP/2 endpoint receives `MaxRequestBodySize`, `KeepAliveTimeout`,
`RequestHeadersTimeout`, `MinRequestBodyDataRate`, `MinResponseDataRate`, and the `Limits:Http2`
keys; an HTTP/3 endpoint receives the first five. Both timeouts take effect on every version
(#1085).
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
QPACK options, QUIC stream limits, and a validation callback.

Both data rates default to 240 octets per second after a 5-second grace period, on every version;
the request-body rate counts from the first body read. `Limits:MinRequestBodyDataRate` (#1383) and
`Limits:MinResponseDataRate` (#1384) bind the listener-wide values onto every endpoint; a value that
is neither `none`/`disabled` nor a valid section, such as a bare number, fails startup. A `null` in
JSON reads as an absent key, so it never switches the defence off by accident. An endpoint
registered in code can still set either through the HTTP options callback of `UseHttp1s`,
`UseHttp2s`, `UseHttps` or `UseHttp3`, for example
`http2 => http2.Limits.MinRequestBodyDataRate = null`.

One endpoint changes a rate for its own requests, and every other request keeps the listener's:

```csharp
using Assimalign.Cohesion.Http;

// context is the exchange's IHttpContext; the default request-limits interceptor attached both
// features. A client-streaming body idles between messages: turn the request-body rate off before
// the first read freezes it.
context.MinRequestBodyDataRate!.MinDataRate = null;

// A reader that legitimately pauses: turn the slow-read check off before the response starts.
context.MinResponseDataRate!.MinDataRate = null;
```

Without that, a request body that legitimately idles fails with `408`, or a stream reset once the
response has started, when an idle gap outlasts its allowance, about 5 seconds plus 1 second per
240 octets already received. The request-body rate freezes at the first body read on every
version, and the response rate at the response's first write.

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
  `RST_STREAM(REFUSED_STREAM)`. HTTP/3 accepts no further request stream and sends a `GOAWAY`;
  a request stream the client opens after it is reset at once with `H3_REQUEST_REJECTED`, so the
  client can retry it elsewhere (#1306).
- **The requests in flight** — nothing is cancelled while the budget lasts: a running request
  finishes, and its response is delivered.
- **QUIC delivery** — completed streams' delivery wait shares the remaining host stop budget and
  QUIC's `CloseDeliveryTimeout` (five seconds by default), whichever expires first. The host keeps
  its one-second abort-unwind allowance when its budget expires.
- **Long-lived exchanges** — every exchange carries `IWebServerDrainFeature`, a `Web.Server`
  contract, whose `Draining` token fires when the stop begins and cancels nothing, so an exchange
  that would otherwise run until the budget cuts it off can end its own work in time.
  `UseWebSockets` closes each open socket with `1001 Going Away` there (see
  [WebSockets](websockets.md#shutdown-the-drain-close)).
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
| A response-completion callback failed; one entry per failure after every callback was attempted | `Error` |
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
Without a gateway name, `/cohesion/v1` and every path beneath it accept only transport peers at
`127.0.0.1`, `::1`, or IPv4-mapped `127.0.0.1`; other or unknown peers receive `403`. Forwarded
headers cannot grant access. Bare probes remain public, and gateway-managed credential checks
are unchanged.

Graceful stop returns HTTP 202 before invoking the shutdown signal through a response-completion
hook. This lets the acknowledgement reach the caller before the server's stop begins.

Return to [Web](index.md).

## Sources

- **Runtime** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/docs/OVERVIEW.md` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/docs/DESIGN.md`.
- **TLS registrations** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/src/Extensions/WebHostingExtensions.cs`.
- **Configuration binder** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/src/Internal/HttpServerConfiguration.cs`.
- **Client certificates** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/src/TlsServerOptions.cs` and `cohesion/libraries/Http/Assimalign.Cohesion.Http.Tls/src/Abstractions/IHttpTlsConnectionFeature.cs`.
- **Drain and diagnostics** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/src/Internal/WebApplicationServer.cs` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/src/Internal/WebApplicationServerLog.cs`.
- **Default interceptors and the drain signal** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/src/WebApplicationServerBuilder.cs`, `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/src/HttpExtendedConnect.cs` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Server/src/Abstractions/IWebServerDrainFeature.cs`.
- **Client faults and refused responses** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/src/Internal/WebClientFaultInterceptor.cs`, `cohesion/resources/Web/Assimalign.Cohesion.Web.Server/docs/OVERVIEW.md` and `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/docs/DESIGN.md`.
- **Timeouts and data rates** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/docs/DESIGN.md` and `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/docs/OVERVIEW.md`.
- **Accept back-off** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/docs/DESIGN.md`.
- **Trailers** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Abstractions/IHttpResponse.cs`, `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/docs/DESIGN.md` and `cohesion/docs/libraries/Http/DECISIONS.md` (ADR 2).
- **Shutdown budget** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/Implementation/HostOptions.TContext.cs`.
- **Control-plane ownership** — `cohesion/docs/resources/Web/DESIGN.md`.
- **Certificate carrier** — `cohesion/docs/RUNTIME_CONTRACT.md`.
