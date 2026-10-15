# Observability

The Web server traces and measures every dispatched request through the BCL's `ActivitySource` and
`Meter`, gives each request an id, and reports operator diagnostics through `EventSource`.

> **Status:** Implemented. Cohesion's own exporter sends logs only; traces and metrics reach whatever subscribes to the server's source and meter by name.

## Diagnostic signal ownership

Web and transport diagnostics answer different questions. Web.Server owns the server spans and
Meter instruments for requests that reach the Web pipeline. The HTTP transport also rejects input before
dispatch and enforces protocol rules after dispatch; it owns those outcomes even when no Web span
exists. Each owner reports its own events rather than forwarding another package's internal source.

| Question or outcome | Signal | Owner | What it reports |
| --- | --- | --- | --- |
| What happened during a dispatched HTTP request? | `ActivitySource` named `Assimalign.Cohesion.Web.Server` | Web.Server | One server span, caller trace context, method, route, final status, duration and cancellation/fault outcome. |
| How much dispatched work is active, and how long does it take? | `Meter` named `Assimalign.Cohesion.Web.Server` | Web.Server | `http.server.active_requests` and `http.server.request.duration`, including Web middleware's rejection statuses. |
| Did HTTP reject a request or fail a protocol operation? | `Meter` and `EventSource` named `Assimalign.Cohesion.Http.Connections` (#1300) | HTTP transport | Pre-dispatch rejection statuses, HTTP/2 and HTTP/3 protocol errors, request-rate enforcement and slow-reader response aborts. These are not Web application faults. |
| Did a listener bind, drain or stop, and did server machinery fail? | `EventSource` named `Assimalign.Cohesion.Web.Server` | Web.Server | Lifecycle, bind/accept/connection failures, completion-callback failures, refused response heads, capacity waits and drain timeouts, correlated by process-unique `serverId`. Peer-ended connections are Verbose and do not increment `connection-faults`. |
| Did the host disable the control plane or reject management work? | `EventSource` named `Assimalign.Cohesion.Web.Hosting` | Web.Hosting | Control-plane configuration, credential rejection, malformed commands and accepted stop; the `rejected-credentials` counter remains exact. |
| Which application fault reached the exception boundary? | `EventSource` named `Assimalign.Cohesion.Web.ErrorHandling` | Web exception boundary | Pipeline and handler failures, expected faults and abandoned responses; request-cancellation exceptions and latched client faults do not become application-fault counts. An original application fault remains reported if the boundary later aborts its response. |
| Which middleware rejected or degraded an exchange, and why? | The owning Web assembly's `EventSource` | Web feature package | Bounded rejection reasons and exact attribution counters for authentication, authorization, antiforgery, edge policies, timeouts, rate limits, compression, forms and content handling. There are no duplicate request start/stop events or general request counters. |
| Did health or control-plane work fail? | `Web.Health`, `Web.Hosting` and `Hosting.Resources` event sources | Web health/HTTP control surface, or shared control-plane producer | Health transitions and failed checks; HTTP authentication/command-input failures; producer-owned contributor failures and command outcomes. |
| Did the underlying connection or TLS handshake fail? | Connections driver event sources; runtime `System.Net.Security` | Connection transport and TLS runtime | Listener, connection and stream lifecycle, driver counters and TLS handshake diagnostics. These sources are outside default Web forwarding. |
| What should the application's HTTP access log contain? | `ILogger` entries, default category `Assimalign.Cohesion.Web.Diagnostics.HttpLogging` | Web.Diagnostics middleware | Explicitly composed access logging at Information with its own redaction options. The `Web.Diagnostics` EventSource reports failures of logging itself, rather than duplicating access entries. |

## What the server emits

The public engine in `Assimalign.Cohesion.Web.Server` emits one span per request and the
OpenTelemetry HTTP server metrics through BCL types only. Both are named for the emitting assembly,
the rule that names Cohesion's event sources, so one name enables them:

| Signal | Name | Emits |
|---|---|---|
| Traces | `ActivitySource` `Assimalign.Cohesion.Web.Server` | one `Server` span per request |
| Metrics | `Meter` `Assimalign.Cohesion.Web.Server` | `http.server.request.duration` and `http.server.active_requests` |

With no listener on the source and neither instrument enabled, the server creates no activity,
reads no request state, and records nothing; the request id is the one per-request cost left. A
listener callback runs inline, and one that throws costs that request its telemetry, never its
response. The server's own lifecycle and failures are reported through its same-named EventSource (see
[Server and TLS](server.md#diagnostics)). The design behind every choice on this page is in the
[Web.Server design](../dotnet-apis/resources/web/assimalign-cohesion-web-server/design.md#server-telemetry-1064).

## Subscribe

Any `ActivityListener` or `MeterListener` subscribes by name: an exporter, a test, or the
application itself. An OpenTelemetry SDK subscribes the same way when it is given the two names.
Cohesion's own OTLP export (`Hosting.Telemetry` over `Assimalign.Cohesion.OpenTelemetry`) sends
logs only and subscribes to neither yet, so exporting traces or metrics takes a listener of the
application's own for now.

This complete program prints each request's span and every measurement the server records:

```csharp
using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;

using Assimalign.Cohesion.Web;
using Assimalign.Cohesion.Web.Hosting;
using Assimalign.Cohesion.Web.Routing;

const string ServerTelemetry = "Assimalign.Cohesion.Web.Server";

using ActivityListener activities = new()
{
    ShouldListenTo = source => source.Name == ServerTelemetry,
    Sample = static (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
    ActivityStopped = activity => Console.WriteLine(
        $"{activity.DisplayName} trace {activity.TraceId}: {activity.GetTagItem("http.response.status_code")}"),
};
ActivitySource.AddActivityListener(activities);

using MeterListener meters = new()
{
    InstrumentPublished = static (instrument, listener) =>
    {
        if (instrument.Meter.Name == ServerTelemetry)
        {
            listener.EnableMeasurementEvents(instrument);
        }
    },
};
meters.SetMeasurementEventCallback<double>((instrument, seconds, tags, state) =>
    Console.WriteLine($"{instrument.Name}: {seconds} s"));
meters.SetMeasurementEventCallback<long>((instrument, delta, tags, state) =>
    Console.WriteLine($"{instrument.Name}: {delta:+0;-0}"));
meters.Start();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddRouting();

await using WebApplication app = builder.Build();
app.UseRouting();
app.MapGet("/orders/{id:int}", (int id) => $"order {id}");

await app.RunAsync();
```

A request to `/orders/7` prints a span named `GET /orders/{id:int}`, a `+1` and a `-1` from
`http.server.active_requests`, and one `http.server.request.duration` measurement.

Out of process, `dotnet-counters` reads the meter by name. A NativeAOT application needs
`EventSourceSupport` for that, as it does for event sources; in-process activity and meter listeners
do not. `Sdk.Web` now enables this support when unset. An explicit `false` disables EventSource
delivery, including in-process `EventListener` callbacks and forwarding, in the native application.

## The request span

The span is a `Server` span for the whole exchange: it starts before the pipeline runs and stops
after the response, a replacement `500` or a reset was sent and the response-completion callbacks
ran. Its end time is set from the duration measurement, so the span and
`http.server.request.duration` report the same value.

- **Its parent is the caller.** The server reads the request's W3C `traceparent` and `tracestate`.
  A missing, repeated or malformed `traceparent`, or an all-zero id, starts a new trace. A header of
  a later version continues the caller's trace too (#1299): the server reads its version-00 fields,
  as W3C Trace Context §3.2.4 has a parser do, and keeps only its sampled flag; version `ff`, and a
  version-00 value with extra fields, start a new trace. An activity that was current when the
  server started never becomes a request's parent.
- **It is current while the pipeline runs.** `Activity.Current` in a handler is the server span, so
  the handler's own activities and its outgoing `HttpClient` calls become its children.
- **Its name** is the method, `GET`, and then the method and the route template,
  `GET /orders/{id:int}`, once routing selected an endpoint. A method outside the known list is
  named `HTTP`.
- **Its status** is `Error` when the exchange failed (see the outcomes below). A `4xx` response
  leaves it unset.

| Attribute | Span | Duration | Active requests | Value |
|---|---|---|---|---|
| `http.request.method` | yes | yes | yes | the method when known, else `_OTHER` |
| `http.request.method_original` | when `_OTHER` | no | no | the method token |
| `url.scheme` | yes | yes | yes | `http` or `https`, as the transport saw the request |
| `url.path` | yes | no | no | the request path |
| `url.query` | when the request has a query | no | no | the query's parameters, each value replaced by `REDACTED` unless the application declared the parameter safe (#1298) |
| `server.address`, `server.port` | yes | no | no | the `Host` or `:authority` host, and its port when the value carries one |
| `network.protocol.version` | yes | yes | no | `1.1`, `2` or `3` |
| `http.route` | when routed | when routed | no | the matched route's template |
| `http.response.status_code` | when sent | when sent | no | the status sent |
| `error.type` | on failure | on failure | no | see the outcomes below |

| How the exchange ended | `http.response.status_code` | `error.type` |
|---|---|---|
| A response with a status below 500 | the status | none |
| The request body broke its framing or a limit while it was read, and the HTTP/1.1 transport answered in place of the staged response | the transport's `400`, `413`, `408` or `431` | none |
| The request body fell below the minimum data rate, and the HTTP/2 or HTTP/3 transport answered its stream with `408` | `408` | none |
| The request body contradicted its content-length or carried a malformed trailer section, and the HTTP/2 or HTTP/3 transport reset its stream before a response started (#1378) | `400`, the client fault the transport reports; nothing reached the wire | none |
| A `5xx` response, including the server's replacement `500` after a fault | the status | the status, for example `500` |
| Cancelled — a peer reset or closed connection, the server stopping, `IHttpContext.Cancel` — and reset | only when a streamed response had started | `request_canceled` |
| The peer abandoned the response or failed its minimum data rate, reported by `IWebResponseClientFaultFeature`, including a wrapped write failure | only when a streamed response had started | `request_canceled` |
| The exception boundary aborted a started response and returned normally | the started response's status | `request_canceled` |
| Cancelled by the HTTP/2 or HTTP/3 transport's answer to, or reset for, a client fault it reported, before the application's response started (#1378) | the transport's `400`, `408` or `413` | none |
| Cancelled by that answer or reset after a streamed response had started (#1378) | the started response's status | `client_fault` |
| The pipeline threw after its response started, or its response could not be replaced, after the transport reported a client fault (`IWebClientFaultFeature`), and the exchange was reset | only when the response had started | `client_fault` |
| The pipeline threw after its response started, or its response could not be replaced, and the exchange was reset | only when the response had started | `unhandled_exception` |
| The response could not be put on the wire | none | `response_send_failed` |

The exception itself is not recorded on the span; reporting it belongs to the application's error
handling. A body the client got wrong is not the application's fault: when the transport's status
replaced the response, the span and the duration carry that status, no `error.type`, and an unset
span status. When the response had already started, the exchange is reset and reported as
`client_fault` with the span status `Error`, so `unhandled_exception` means the application's own
defect, and `request_canceled` a cancellation that was not the transport's answer to a client fault
(see [Client faults](server.md#client-faults-and-refused-responses)). On HTTP/2 and HTTP/3 that
answer can cancel the exchange rather than fail its read, and an exchange cancelled that way before
its response started reports the transport's status with no error, as if the read had met the
fault. A client that resets its own request mid-body is reported as `request_canceled`.
Connection loss after dispatch is cancellation on every protocol, including an incomplete HTTP/1.1
body ended by EOF (#1439): the body read throws `OperationCanceledException` with the already-fired
exchange `RequestCancelled` token, even when the read supplied no token. There is no client-fault
status, and an unstarted response has no status on the span. Malformed framing and configured
limits retain their client-fault statuses.

The transport's response-side latch, `IHttpExchangeControl.ResponseAbortedByClient`, is published
as `IWebResponseClientFaultFeature` on every exchange, including a bodyless GET (#1432). It stays
true after the peer abandons the response or fails its minimum response data rate. The exception
boundary and host read it before classifying the exception type, so even a wrapped write failure
cancels with the exchange token, without `OnException`, `OnError`, a replacement response or an
application-fault report. Local application cancellation does not set the latch. Completion
callbacks do not run for a cancelled exchange, including a boundary abort followed by a normal
pipeline return.

Exchange completion and application-fault attribution are separate. The error boundary reports
an original application fault before invoking diagnostic callbacks or error handlers, including
after headers commit. A later boundary abort or peer loss while writing the error response cannot
erase that report: `total-faults` still counts it, and `aborted-responses` independently counts a
boundary-initiated abort. A cancelled token alone does not make an ordinary application exception
a client fault. A response abandoned by its peer without an earlier application fault increments
neither counter.

**Known methods** are the OpenTelemetry convention's: RFC 9110's eight methods, `PATCH`, and
`QUERY`. `OTEL_INSTRUMENTATION_HTTP_KNOWN_METHODS` replaces the list with a comma-separated,
case-sensitive one, read once per process. Methods are case-sensitive (RFC 9110 §9.1, #1301): the
HTTP stack keeps the token as the client sent it, so a lower-case `get` is an unknown method,
reported as `_OTHER` with `http.request.method_original` set to `get`. A method is known only when
it matches a list entry exactly.

### The query

`url.query` shows a query's parameter names and hides its values (#1298). A query string carries
tokens, signatures, one-time codes and personal data the server cannot recognize generically, so
every value is replaced with `REDACTED`, the convention's marker:
`?access_token=s3cr3t&page=2&debug` is reported as `access_token=REDACTED&page=REDACTED&debug`.
Declare the parameters whose values are safe to show:

```csharp
using Assimalign.Cohesion.Web.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Server.AllowQueryValuesInTelemetry("page", "sort");
```

- **Names compare case-insensitively**, as `HttpQueryKey` compares them. Calls add to one another,
  and a missing or empty name is rejected before anything is declared.
- **A parameter without a value** is written as its name alone.
- **Names are treated as the application's schema**: a secret put in a parameter's name, such as a
  bare `?<token>`, is reported.
- **Rebuilt from the parsed query.** Names and shown values are percent-encoded again, a parameter
  with an empty name is left out, and a repeated name appears once, with its first value even
  though the collection retains every occurrence. The server reads the query before the pipeline
  runs, so it reports what the client sent, not a `Web.Rewrite` rewrite of it.
- **Cost.** The value is built once per request while an `ActivityListener` subscribes, before the
  sampler runs, so a sampler can read it. Without a span listener the server never reads the query,
  and the metrics never carry `url.query`.

## The metrics

| Instrument | Kind and unit | Records |
|---|---|---|
| `http.server.request.duration` | histogram, `s` | the duration of each exchange, the value its span reports, with the convention's bucket boundaries as advice: 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, and 10 seconds |
| `http.server.active_requests` | up-down counter, `{request}` | `+1` when an exchange's telemetry starts and `-1` when it stops |

Each instrument carries the attributes the table under "The request span" marks for it: the
duration adds the route, the status, the protocol version, and `error.type` to the method and the
scheme, and the active-request count carries only the method and the scheme. Either instrument is
recorded whenever it is enabled, with or without a span.

### Transport metrics

The `Assimalign.Cohesion.Http.Connections` Meter reports transport decisions, including requests
rejected before Web dispatch. It emits no spans. Its four custom `Counter<long>` instruments
complement the Web server's request duration and active-request instruments:

| Instrument | Unit | Tags |
|---|---|---|
| `cohesion.http.server.rejected_requests` | `{request}` | `network.protocol.version`, `cohesion.http.rejection.status_code` |
| `cohesion.http.server.protocol_errors` | `{error}` | `network.protocol.version`, `error.type`, `cohesion.http.error.scope` |
| `cohesion.http.server.response_rate_aborts` | `{abort}` | `network.protocol.version` |
| `cohesion.http.server.response_aborts` | `{response}` | `network.protocol.version` |

The rejection status records the client-fault decision, even when a started response cannot be
replaced or an interceptor refusal resets a stream instead of sending the status. `error.type`
is the numeric protocol code in invariant decimal notation; interpret it with the protocol version.
The error scope is `connection` or `stream`. No connection or request identifier is a metric tag.
Measurements are exact while subscribed and independent of EventSource throttling.

The same-named EventSource reports `RequestRejected`, `ConnectionError`, `StreamReset`,
`ResponseRateAborted` and `ResponseAbortedByClient`. Each Warning event has its own process-wide
one-second throttle with `suppressedCount`; the backing counters stay exact. A response rate
abort is a reason-specific subset of client response aborts for active responses. HTTP/2 can also
report a connection-output rate abort without an active response, identified by `streamId=-1`.
Application cancellation does not count as a client response abort. Connection lifetimes and
counts remain the Connections drivers' responsibility. See the
[transport diagnostics design](../dotnet-apis/libraries/http/assimalign-cohesion-http-connections/design.md#diagnostics).

## Route templates

`http.route` comes from the endpoint that routing selected: `UseRouting` publishes the matched
route's template through `IWebEndpointFeature.RouteTemplate`, a `Web.Routing` contract since #1379,
and the server reads it when the exchange ends. The template is the route's pattern with one leading
`/`, so `/orders/{id}`, `orders/{id}`, and `~/orders/{id}` all report `/orders/{id}`, and a route in
a group reports the composed template, `/api/orders/{id:int}`. A request that no route selected has
no `http.route`, and neither has a `405` answer, which no single route selected. A custom endpoint
selector reports a template by implementing `RouteTemplate`, which defaults to `null`.

## The request id

Every exchange the default server handles carries `IWebRequestIdFeature`, with or without a
listener. The contract ships in
[`Web.Server`](../dotnet-apis/resources/web/assimalign-cohesion-web-server/index.md) and keeps the
`Assimalign.Cohesion.Web` namespace. Its `RequestId` is an `ActivityTraceId`: the server span's
trace id when the request is traced, otherwise the trace id of a valid `traceparent`, otherwise a
random id generated on first read. It is stable for the exchange, so one value finds the request in
its span, in the logs, and in whatever the application returns to the caller. `ToHexString()` gives
the 32-character lower-case form that `traceparent` uses. A custom server may omit the feature, so
check for it:

```csharp
using Assimalign.Cohesion.Http;
using Assimalign.Cohesion.Web;

// app is the built WebApplication.
app.Use(async (context, next) =>
{
    if (context.Features.Get<IWebRequestIdFeature>() is { } requestId)
    {
        context.Response.Headers[new HttpHeaderKey("X-Request-Id")] = requestId.RequestId.ToHexString();
    }

    await next.Invoke(context);
});
```

## HTTP logging correlation and capture

With `HttpLoggingFields.TraceContext` enabled, `UseHttpLogging` reads `trace.id` from
`IWebRequestIdFeature.RequestId`, including requests with no inbound `traceparent`. Its `span.id`
comes from this exchange's server span through `IWebRequestActivityFeature`; it is absent when no
server span exists, including when nobody listens for spans. The server owns the activity, and the
feature remains available through completion callbacks. Logging never uses an unrelated or child
`Activity.Current`. Custom servers may omit these features, leaving the corresponding attributes
absent. The logged span id now identifies the server span, rather than the inbound parent.

Request-body capture and byte counting work through request views, including decompression,
rewrite and request timeouts. Logging after decompression sees decoded bytes. Capture retains only
the configured prefix of allowed content types, while byte counts include all bytes consumed
downstream. Logging leaves the original request body in place. Headers and bodies remain opt-in.
See [Web.Diagnostics](../dotnet-apis/resources/web/assimalign-cohesion-web-diagnostics/index.md).

An `OperationCanceledException` after `RequestCancelled` fires stays at the configured HTTP log
level (Information by default), with no exception or `(faulted)` marker. Cancellation alone adds
no `http.client_fault` attribute. An unrelated cancellation on a live exchange, or an ordinary
application exception even after cancellation, remains an application fault. A disconnected peer
receives no response before the head starts, even if the access entry shows a staged default `200`;
that value is not evidence of wire delivery.

`Assimalign.Cohesion.Web.Diagnostics` reports failures of logging itself through its EventSource.
`failed-access-log-writes` counts failed Write calls, not lost buffered entries, and
`dropped-http-log-entries` counts failed entry emission. Its category is a prefix of the default
access-log category `Assimalign.Cohesion.Web.Diagnostics.HttpLogging`, so a rule on the source
also affects HTTP entries. No default category rule is added. The W3C provider accepts only
`http.event = "exchange"` entries, excluding forwarded diagnostic events from its file.

## Event sources and default forwarding

Cohesion libraries report runtime lifecycle, failures and attribution counters through internal
`System.Diagnostics.Tracing.EventSource` implementations. Their public subscription names are
exactly their assembly names. Web sources report operator events, without duplicate per-request
start/stop events or general request counters. Request spans and metrics remain separate signals.

### Default forwarding

`WebApplication.CreateBuilder(args)` forwards the `Assimalign.Cohesion.Web.` prefix and the exact
`Assimalign.Cohesion.Http.Connections` source into the application's configured logging providers.
Each event becomes one `ILogger` entry whose category is its source name. No provider or filter
rule is added, so existing filters decide both which entries are accepted and how verbosely each
source is enabled. HTTP access logs remain Information-level entries unless configured otherwise.

```csharp
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.EnableEventSourceForwarding = false;
```

Register `builder.Logging.AddEventSourceForwarding(options)` before Build to choose other sources;
any application forwarder registration takes precedence. Calling `AddEventSourceForwarding()`
without options selects every `Assimalign.Cohesion.*` source, a wider selection than the Web default.
The Connections drivers, runtime `System.Net.Security`, `Http.Sessions` and `Hosting.Resources`
are outside that default selection. Explicit/options-based builders for nested hosts register no
automatic forwarder. Non-Web resource control planes need a tool or their own forwarding setup.

Listeners observe the whole process. Concurrent entry-point applications can forward the same
event; use `serverId`, `resourceName` and request identifiers for attribution and opt out in secondary hosts that
share sinks. The application owns its logger factory, providers and forwarders; forwarding ends
at final application disposal, after shutdown diagnostics. A factory resolved from its service
provider cannot outlive the application. Forwarding carries events, not EventSource counters or
Meter instruments.

The server's `WebServerEventSource`, Meter and ActivitySource are named
`Assimalign.Cohesion.Web.Server`. The former logger category was
`Assimalign.Cohesion.Web.Hosting.WebApplicationServer`. Server event attributes use camelCase payload names;
exception stacks and per-host logger attribution are no longer present. Bind failures remain
Critical, accept-loop failures are Error, and peer-ended connections are Verbose and excluded
from `connection-faults`. See the [server event schema](server.md#diagnostics).

The hosting module's `WebHostingEventSource` retains `Assimalign.Cohesion.Web.Hosting` for host
and control-plane events. Its former resource-integration provider is folded into that source;
the default Web prefix forwards both the server and hosting providers. A directly constructed
`WebApplicationServer` leaves diagnostic subscriptions and forwarding to its caller.

### Observe with tools

```bash
dotnet-trace collect --process-id <pid> --providers Assimalign.Cohesion.Web.Server::5,Assimalign.Cohesion.Web.Hosting::5,Assimalign.Cohesion.Http.Connections::5
dotnet-counters monitor --process-id <pid> --counters Assimalign.Cohesion.Web.Server,Assimalign.Cohesion.Web.Hosting,Assimalign.Cohesion.Http.Connections
```

`dotnet-monitor` and PerfView take the same source names. An in-process `EventListener` can select
them by name too. Counters stay exact with no listener and while events are suppressed; throttled
events carry `suppressedCount` as documented by their owning package. Most request-related event
correlation reads `IWebRequestIdFeature`, never ambient `Activity.Current`. Transport events can
precede an exchange and use connection/stream identifiers instead.

### NativeAOT delivery

`Sdk.Web` defaults `EventSourceSupport` to `true` only when unset. An explicit `false` before its
import remains false, a project-body value overrides the default, and a command-line global
property takes precedence. Other SDKs must opt in. To remove delivery from a native Web publish:

```xml
<PropertyGroup>
    <EventSourceSupport>false</EventSourceSupport>
</PropertyGroup>
```

This disables tools, in-process EventListeners and forwarding in that native application. Server
behavior and exact internal counter maintenance are unchanged. It does not disable in-process
ActivityListener or MeterListener subscriptions. Retaining delivery increases executable size:
the source's Release Web AOT guard measurements were +1,791,488 bytes (+11.30%) on `win-arm64`
and +1,675,056 bytes (+11.01%) on `linux-x64`, excluding debug symbols and content files. These
measure the representative application's retained runtime machinery, not just the Web source.
The enabled native guard verifies lifecycle payload names, matching server ids and no event 0.

### Web event-source catalog

The 22 instrumented Web assemblies and their related transport/control-plane sources are listed
below. Each design link documents event identifiers, levels, bounded payloads and counter semantics.
Payload policy is source-specific: CORS reports bounded origins and rejected header names, and
HostFiltering reports the effective host component. Authentication events omit tokens, claims
and certificate identifiers; application exception messages follow each package's documented policy.

| Event source (= assembly) | Events | Counters | Reference |
| --- | --- | --- | --- |
| `Assimalign.Cohesion.Hosting.Resources` | Shared control-plane contributor failures and command outcomes: applied, unknown kind (501), conflict (409), handler failure. Only registered kinds are written; refusals/failures are throttled. HTTP surface events belong to Web.Hosting | `contributor-failures`, `commands-applied`, `unknown-command-kinds`, `command-conflicts`, `command-handler-failures` | [Hosting.Resources DESIGN.md](../dotnet-apis/libraries/hosting/assimalign-cohesion-hosting-resources/design.md#diagnostics) |
| `Assimalign.Cohesion.Http.Connections` | Transport rejections, HTTP/2 and HTTP/3 connection errors and stream resets, slow-reader aborts and client-caused response aborts (Warning, throttled per event with `suppressedCount`) | `rejected-requests`, `connection-errors`, `stream-resets`, `response-rate-aborts`, `response-aborts` | [Http.Connections DESIGN.md](../dotnet-apis/libraries/http/assimalign-cohesion-http-connections/design.md#diagnostics) |
| `Assimalign.Cohesion.Http.Sessions` | Store load/commit failures (type only, throttled) | `store-load-failures`, `store-commit-failures` | [Design](../dotnet-apis/libraries/http/assimalign-cohesion-http-sessions/design.md#diagnostics) |
| `Assimalign.Cohesion.Web.Antiforgery` | RequestRejected and sanitized TokenUnprotectFailed (Verbose); RejectionAborted (Warning, at most once a second with suppressedCount); FormMalformed has no exception text | `rejected-requests` | [Antiforgery DESIGN.md](../dotnet-apis/resources/web/assimalign-cohesion-web-antiforgery/design.md#diagnostics) |
| `Assimalign.Cohesion.Web.Authentication.Bearer` | TokenRejected (Verbose): bounded rejection reason, algorithm/key id and request correlation; no token or claims | `rejected-tokens` | [Bearer DESIGN.md](../dotnet-apis/resources/web/assimalign-cohesion-web-authentication-bearer/design.md#diagnostics) |
| `Assimalign.Cohesion.Web.Authentication.Certificate` | CertificateRejected (Verbose): NoCertificate on a required challenge or ValidationFailed from application mapping; no certificate identifiers. TLS trust/validity failures occur before HTTP and are unavailable to this handler | `rejected-certificates` | [Certificate DESIGN.md](../dotnet-apis/resources/web/assimalign-cohesion-web-authentication-certificate/design.md#diagnostics) |
| `Assimalign.Cohesion.Web.Authentication.Cookie` | TicketRejected (Verbose): bounded reason and sanitized unprotect exception metadata with request correlation | `rejected-tickets` | [Cookie DESIGN.md](../dotnet-apis/resources/web/assimalign-cohesion-web-authentication-cookie/design.md#diagnostics) |
| `Assimalign.Cohesion.Web.Authorization` | RequestDenied (Verbose): challenge/forbid outcome, route template, policy source, failed requirement type, schemes and request correlation | `challenged-requests`, `forbidden-requests` | [Authorization DESIGN.md](../dotnet-apis/resources/web/assimalign-cohesion-web-authorization/design.md#diagnostics) |
| `Assimalign.Cohesion.Web.Caching` | ResponseNotStored (Verbose); EntryDeclined (Warning, throttled); no cache keys or tags | `declined-entries`; hit/miss counters omitted after benchmarking | [Caching DESIGN.md](../dotnet-apis/resources/web/assimalign-cohesion-web-caching/design.md#diagnostics) |
| `Assimalign.Cohesion.Web.Compression` | Request decompression refused (Verbose); decompressed size exceeded or rejection aborted after response commit (Warning, throttled) | `decompression-limit-rejections` | [Compression DESIGN.md](../dotnet-apis/resources/web/assimalign-cohesion-web-compression/design.md#diagnostics) |
| `Assimalign.Cohesion.Web.Cors` | Refused preflights and pass-through causes, with bounded origin and rejected header name (Verbose) | `rejected-preflights` | [Cors DESIGN.md](../dotnet-apis/resources/web/assimalign-cohesion-web-cors/design.md#diagnostics) |
| `Assimalign.Cohesion.Web.Diagnostics` | Access-log file opened; write/flush/close failed (Error), retention failed, HTTP log entry dropped, client-address resolver failed (Warning); failures throttled | `failed-access-log-writes`, `dropped-http-log-entries` | [Diagnostics DESIGN.md](../dotnet-apis/resources/web/assimalign-cohesion-web-diagnostics/design.md#diagnostics) |
| `Assimalign.Cohesion.Web.ErrorHandling` | Application pipeline fault (Error), expected fault (Verbose), error-handler failure (Error), diagnostic-callback failure (Warning), peer-abandoned response (Verbose); request-cancellation exceptions and latched client faults excluded; later cancellation preserves the original application-fault report | `total-faults`, `faults-per-second`, `aborted-responses` | [ErrorHandling DESIGN.md](../dotnet-apis/resources/web/assimalign-cohesion-web-errorhandling/design.md#diagnostics) |
| `Assimalign.Cohesion.Web.Forms` | Form limit or malformed input (Verbose); rejection after response commit (Warning, throttled); configured limit names and exception types only | `form-limit-rejections`, `malformed-forms` | [Forms DESIGN.md](../dotnet-apis/resources/web/assimalign-cohesion-web-forms/design.md#diagnostics) |
| `Assimalign.Cohesion.Web.ForwardedHeaders` | Forwarding stop reason, family and applied depth (Verbose); transport peer IP only for an untrusted peer at depth zero | `untrusted-forwarding`, `malformed-forwarding` | [ForwardedHeaders DESIGN.md](../dotnet-apis/resources/web/assimalign-cohesion-web-forwardedheaders/design.md#diagnostics) |
| `Assimalign.Cohesion.Web.Health` | Health worsened (Warning), improved (Informational), thrown or timed-out check (Verbose); includes adapted Hosting.Health contributors | `failed-checks` | [Health DESIGN.md](../dotnet-apis/resources/web/assimalign-cohesion-web-health/design.md#diagnostics) |
| `Assimalign.Cohesion.Web.HostFiltering` | Rejected effective host with bounded host component, reason and request id (Verbose) | `rejected-requests` | [HostFiltering DESIGN.md](../dotnet-apis/resources/web/assimalign-cohesion-web-hostfiltering/design.md#diagnostics) |
| `Assimalign.Cohesion.Web.Hosting` | Control plane disabled; shared HTTP control-plane credential/loopback refusals and malformed commands (throttled), stop accepted, identified by resourceName; non-Web processes need a tool or their own forwarder | `rejected-credentials` | [Hosting DESIGN.md](../dotnet-apis/resources/web/assimalign-cohesion-web-hosting/design.md#diagnostics) |
| `Assimalign.Cohesion.Web.Server` | Server lifecycle and failures, peer-ended connections (Verbose), refused response heads and telemetry listener failures (throttled), completion callback and isolation failures; process-unique serverId | `connection-faults`, `connection-cap-waits`, `refused-response-heads` | [Server DESIGN.md](../dotnet-apis/resources/web/assimalign-cohesion-web-server/design.md#diagnostics) |
| `Assimalign.Cohesion.Web.RateLimiting` | Rejected global or endpoint policy (Verbose); rejection after response commit (Warning, throttled); partition keys are never written | `rejected-requests` | [RateLimiting DESIGN.md](../dotnet-apis/resources/web/assimalign-cohesion-web-ratelimiting/design.md#diagnostics) |
| `Assimalign.Cohesion.Web.RequestTimeouts` | Timeout enforced (Warning, throttled) or ignored by downstream work (Verbose); armed interval and request correlation | `timed-out-requests`, `ignored-timeouts` | [RequestTimeouts DESIGN.md](../dotnet-apis/resources/web/assimalign-cohesion-web-requesttimeouts/design.md#diagnostics) |
| `Assimalign.Cohesion.Web.SecurityHeaders` | Security policy staging skipped by read-only headers (Warning, throttled); excludes disabled endpoints and protocol takeovers | `unprotected-responses` | [SecurityHeaders DESIGN.md](../dotnet-apis/resources/web/assimalign-cohesion-web-securityheaders/design.md#diagnostics) |
| `Assimalign.Cohesion.Web.Sessions` | SessionOrphaned (Verbose), SessionStateLost (modified orphans only, Warning, throttled); store failures belong to Http.Sessions | `orphaned-sessions`, `lost-sessions` | [Sessions DESIGN.md](../dotnet-apis/resources/web/assimalign-cohesion-web-sessions/design.md#diagnostics) |
| `Assimalign.Cohesion.Web.StaticFiles` | WebRootMounted; WebRootUnavailable; TraversalRejected (Verbose); MountLookupFailed (Warning, type only, throttled) | `traversal-rejections`, `mount-lookup-failures` | [StaticFiles DESIGN.md](../dotnet-apis/resources/web/assimalign-cohesion-web-staticfiles/design.md#diagnostics) |
| `Assimalign.Cohesion.Web.WebSockets` | HandshakeRejected, WebSocketOpened, WebSocketClosed, DrainCloseFailed (all Verbose) | `current-websockets`, `total-websockets`, `rejected-handshakes` | [WebSockets DESIGN.md](../dotnet-apis/resources/web/assimalign-cohesion-web-websockets/design.md#diagnostics) |

### Deliberately uninstrumented assemblies

These Web assemblies and the framework producer shells have no source. Their observable
statuses, thrown failures or contract-only role do not need an additional diagnostic owner.

| Assembly or framework project | Why it has no source |
| --- | --- |
| `Assimalign.Cohesion.Web` | Contracts and pure pipeline helpers have no runtime state, loop or swallowed failure; the root must not claim every Web package's signals. |
| `Assimalign.Cohesion.Web.Testing` | A shipped test harness, like Connections.InMemory, rather than a runtime component; it receives no access to internal sources. |
| `Assimalign.Cohesion.Web.ApplicationModel` | Invalid declarative plans throw; runtime control-plane outcomes belong to Hosting.Resources. |
| `Assimalign.Cohesion.Web.Routing` | Failures throw at startup or reach the exception boundary; 404/405 are statuses and the matched template is already the span's `http.route`. |
| `Assimalign.Cohesion.Web.Api` | Binding failures are visible client-driven 400s in access logs and request metrics; the signal does not justify a new public rejection API. |
| `Assimalign.Cohesion.Web.OpenApi` | Build failures reach the boundary and are not cached; first-route-wins is documented and the package is development-facing. |
| `Assimalign.Cohesion.Web.Validation` | A 400 is the intended client outcome, and validation errors echo user input. |
| `Assimalign.Cohesion.Web.Serialization` | A registry without lifecycle: faults throw and failed negotiation returns false for the caller's 406. |
| `Assimalign.Cohesion.Web.Query` | Outcomes are visible client-driven protocol statuses (400, 415, 406, 304, 412), with nothing swallowed. |
| `Assimalign.Cohesion.Web.ProblemDetails` | A payload library without lifecycle; deterministic degradation serves last-chance error handling. |
| `Assimalign.Cohesion.Web.Authentication` | Handlers own failure decisions and their sources; dispatcher counts would be inexact because Authorization re-enters per scheme. |
| `Assimalign.Cohesion.Web.Rewrite` | Loops and regex timeouts throw to the boundary, while redirects are configured answers. |
| `Assimalign.Cohesion.Web.HttpsPolicy` | Redirect rates are visible by route in request metrics; a source cannot reliably diagnose loops caused by absent forwarded-header middleware. |
| `Assimalign.Cohesion.Web.CookiePolicy` | Applications observe drops through `OnRejected`; consent drops and default `Upgrade` repairs are intended policy outcomes. |
| `Assimalign.Cohesion.Web.Refs` / `Assimalign.Cohesion.Web.Runtime` | Framework producer shells that compile nothing. |

### Event-source tests

Event-source tests link the canonical test-only `build/TestSupport/EventSourceRecorder.cs` with
MSBuild's built-in Compile/Link support:

```xml
<Compile Include="$(CohesionRepositoryDirectory)build\TestSupport\EventSourceRecorder.cs" Link="Shared\EventSourceRecorder.cs" />
```

The recorder's namespace is `Assimalign.Cohesion.Testing`; it needs no support assembly or
`CohesionSharedSource` item. Each test assembly owns its EventListener instances. Serial collections
use `DisableParallelization = true`, counter deltas and event identifiers because background work
can outlive a test.

`WebEventSourceConformanceTests` discovers the actual App.Web public/private membership, including
Certificate authentication and Http.Connections. It checks at most one internal sealed source per
assembly, assembly-name identity, strict manifests, internal namespace placement, package Diagnostics
sections, and exact agreement between the Web source catalog and the deliberate no-source list.
The catalog count is discovered, not fixed in the test. The shared recorder ships only in tests.

## What is deliberately not emitted

The Web server's request spans and metrics omit the following. Operator events and transport
metrics have their own payload policies, described above.

- **Query values the application has not declared safe** — see [The query](#the-query).
- **Proxy-resolved host and scheme** — `server.address` and `url.scheme` are what the transport
  saw, not `Forwarded` or `X-Forwarded-*` values, which are believable only after
  [forwarded-headers](../dotnet-apis/resources/web/assimalign-cohesion-web-forwardedheaders/index.md)
  trust evaluation.
- **`client.address`, `network.peer.address`, and `user_agent.original`** — a client address is
  personal data, and it depends on the same forwarded-headers question.
- **`server.address` and `server.port` on the metrics** — they come from request headers, which
  would make them a cardinality attack vector.
- **Requests the transport rejects before dispatch** — the 400, 408, 413, 414, 421, 431 and 501 answers that
  `Http.Connections` gives itself never reach the server, so they have no Web span or request-duration
  measurement. The transport's EventSource and Meter report them (see [Transport metrics](#transport-metrics)).
- **W3C `baggage`.**

Return to [Web](index.md).

## Sources

- **Telemetry design** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Server/docs/OVERVIEW.md` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Server/docs/DESIGN.md`.
- **Instruments and attributes** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Server/src/Internal/WebServerTelemetry.cs` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Server/src/Internal/WebExchangeTelemetry.cs`.
- **Query values declared safe** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/src/WebApplicationServerBuilder.cs`.
- **Subscription** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/samples/Assimalign.Cohesion.Web.AotGuard/GuardSmoke.cs` and `cohesion/docs/EVENT_SOURCES.md`.
- **Request id and route template** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Server/src/Abstractions/IWebRequestIdFeature.cs`, `cohesion/resources/Web/Assimalign.Cohesion.Web.Routing/src/Abstractions/IWebEndpointFeature.cs`, and `cohesion/resources/Web/Assimalign.Cohesion.Web.Routing/docs/DESIGN.md`.
- **Client faults** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Server/src/Abstractions/IWebClientFaultFeature.cs` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Server/src/Abstractions/IWebResponseClientFaultFeature.cs`.
- **HTTP logging and request views** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Diagnostics/docs/OVERVIEW.md` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Diagnostics/docs/DESIGN.md`.
- **Event-source scope and test recorder** — `cohesion/docs/EVENT_SOURCES.md`, `cohesion/.claude/rules/event-source.md`, `cohesion/.claude/rules/testing.md`, and `cohesion/build/TestSupport/EventSourceRecorder.cs`.
- **Tests** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Server/tests/WebServerTelemetryTests.cs`.
- **Signal ownership** — `cohesion/docs/resources/Web/DESIGN.md`.
- **Transport diagnostics** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Connections/docs/DESIGN.md`.
- **NativeAOT delivery and size measurements** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Tasks/docs/DESIGN.md`.
- **Conformance** — `cohesion/libraries/App/Assimalign.Cohesion.App.Runtime/tests/WebEventSourceConformanceTests.cs`.
