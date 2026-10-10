# Observability

The Web server traces and measures every request through the BCL's `ActivitySource` and `Meter`, and gives each request an id.

> **Status:** Implemented. Cohesion's own exporter sends logs only; traces and metrics reach whatever subscribes to the server's source and meter by name.

## What the server emits

The default server in `Assimalign.Cohesion.Web.Hosting` emits one span per request and the
OpenTelemetry HTTP server metrics through BCL types only. Both are named for the emitting assembly,
the rule that names Cohesion's event sources, so one name enables them:

| Signal | Name | Emits |
|---|---|---|
| Traces | `ActivitySource` `Assimalign.Cohesion.Web.Hosting` | one `Server` span per request |
| Metrics | `Meter` `Assimalign.Cohesion.Web.Hosting` | `http.server.request.duration` and `http.server.active_requests` |

With no listener on the source and neither instrument enabled, the server creates no activity,
reads no request state, and records nothing; the request id is the one per-request cost left. A
listener callback runs inline, and one that throws costs that request its telemetry, never its
response. The server's own failures are logged rather than traced (see
[Server and TLS](server.md#diagnostics)). The design behind every choice on this page is in the
[Web.Hosting design](../dotnet-apis/resources/web/assimalign-cohesion-web-hosting/design.md#server-telemetry-1064).

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

const string ServerTelemetry = "Assimalign.Cohesion.Web.Hosting";

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
`EventSourceSupport` for that, as it does for event sources; in-process listeners do not.

## The request span

The span is a `Server` span for the whole exchange: it starts before the pipeline runs and stops
after the response, a replacement `500` or a reset was sent and the response-completion callbacks
ran. Its end time is set from the duration measurement, so the span and
`http.server.request.duration` report the same value.

- **Its parent is the caller.** The server reads the request's W3C `traceparent` and `tracestate`.
  A missing, repeated or malformed `traceparent`, or an all-zero id, starts a new trace. An activity
  that was current when the server started never becomes a request's parent.
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
| `server.address`, `server.port` | yes | no | no | the `Host` or `:authority` host, and its port when the value carries one |
| `network.protocol.version` | yes | yes | no | `1.1`, `2` or `3` |
| `http.route` | when routed | when routed | no | the matched route's template |
| `http.response.status_code` | when sent | when sent | no | the status sent |
| `error.type` | on failure | on failure | no | see the outcomes below |

| How the exchange ended | `http.response.status_code` | `error.type` |
|---|---|---|
| A response with a status below 500 | the status | none |
| A `5xx` response, including the server's replacement `500` after a fault | the status | the status, for example `500` |
| Cancelled — a peer reset or closed connection, the server stopping, `IHttpContext.Cancel` — and reset | only when a streamed response had started | `request_canceled` |
| The pipeline threw after its response started, or its response could not be replaced, and the exchange was reset | only when the response had started | `unhandled_exception` |
| The response could not be put on the wire | none | `response_send_failed` |

The exception itself is not recorded on the span; reporting it belongs to the application's error
handling.

**Known methods** are the OpenTelemetry convention's: RFC 9110's eight methods, `PATCH`, and
`QUERY`. `OTEL_INSTRUMENTATION_HTTP_KNOWN_METHODS` replaces the list with a comma-separated,
case-sensitive one, read once per process. The HTTP stack upper-cases method tokens when it parses
a request, so its entries should be upper case.

## The metrics

| Instrument | Kind and unit | Records |
|---|---|---|
| `http.server.request.duration` | histogram, `s` | the duration of each exchange, the value its span reports, with the convention's bucket boundaries as advice: 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, and 10 seconds |
| `http.server.active_requests` | up-down counter, `{request}` | `+1` when an exchange's telemetry starts and `-1` when it stops |

Each instrument carries the attributes the table under "The request span" marks for it: the
duration adds the route, the status, the protocol version, and `error.type` to the method and the
scheme, and the active-request count carries only the method and the scheme. Either instrument is
recorded whenever it is enabled, with or without a span.

## Route templates

`http.route` comes from the endpoint that routing selected: `UseRouting` publishes the matched
route's template through the Web root's `IWebEndpointFeature.RouteTemplate`, and the server reads it
when the exchange ends. The template is the route's pattern with one leading `/`, so
`/orders/{id}`, `orders/{id}`, and `~/orders/{id}` all report `/orders/{id}`, and a route in a
group reports the composed template, `/api/orders/{id:int}`. A request that no route selected has
no `http.route`, and neither has a `405` answer, which no single route selected. A custom endpoint
selector reports a template by implementing `RouteTemplate`, which defaults to `null`.

## The request id

Every exchange the default server handles carries `IWebRequestIdFeature`, with or without a
listener. Its `RequestId` is an `ActivityTraceId`: the server span's trace id when the request is
traced, otherwise the trace id of a valid `traceparent`, otherwise a random id generated on first
read. It is stable for the exchange, so one value finds the request in its span, in the logs, and in
whatever the application returns to the caller. `ToHexString()` gives the 32-character lower-case
form that `traceparent` uses. A custom server may omit the feature, so check for it:

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

## What is deliberately not emitted

- **`url.query`** — query strings carry tokens and signatures the server cannot recognize; an
  application that wants it tags `Activity.Current` itself.
- **Proxy-resolved host and scheme** — `server.address` and `url.scheme` are what the transport
  saw, not `Forwarded` or `X-Forwarded-*` values, which are believable only after
  [forwarded-headers](../dotnet-apis/resources/web/assimalign-cohesion-web-forwardedheaders/index.md)
  trust evaluation.
- **`client.address`, `network.peer.address`, and `user_agent.original`** — a client address is
  personal data, and it depends on the same forwarded-headers question.
- **`server.address` and `server.port` on the metrics** — they come from request headers, which
  would make them a cardinality attack vector.
- **Requests the transport rejects before dispatch** — the 400, 408, 413, 414 and 431 answers that
  `Http.Connections` gives itself never reach the server, so they have no span or measurement, and
  the transport does not report them either.
- **W3C `baggage`.**

Return to [Web](index.md).

## Sources

- **Telemetry design** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/docs/OVERVIEW.md` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/docs/DESIGN.md`.
- **Instruments and attributes** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/src/Internal/WebServerTelemetry.cs` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/src/Internal/WebExchangeTelemetry.cs`.
- **Subscription** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/samples/Assimalign.Cohesion.Web.AotGuard/GuardSmoke.cs` and `cohesion/docs/EVENT_SOURCES.md`.
- **Request id and route template** — `cohesion/resources/Web/Assimalign.Cohesion.Web/src/Abstractions/IWebRequestIdFeature.cs`, `cohesion/resources/Web/Assimalign.Cohesion.Web/src/Abstractions/IWebEndpointFeature.cs`, and `cohesion/resources/Web/Assimalign.Cohesion.Web.Routing/docs/DESIGN.md`.
- **Tests** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/tests/WebServerTelemetryTests.cs`.
