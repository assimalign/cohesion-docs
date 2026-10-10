# Assimalign.Cohesion.Web.Hosting

The Web runtime composes a concrete `WebApplication : Host<WebApplicationContext>` through `WebApplication.CreateBuilder(args)`.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

The Web runtime composes a concrete `WebApplication : Host<WebApplicationContext>` through
`WebApplication.CreateBuilder(args)`. It implements the root application and builder contracts and
integrates DI, configuration, logging, HTTP transports, and the enabled resource's control plane at
builder time.

## Composition and lifecycle

Feature packages register through `WebApplicationBuilder.Services`, with component-integration
verbs the application's compilation receives (`builder.Services.AddRouting()`); the root
`IWebApplicationBuilder` members, `AddFeature` among them, are explicit shims over the same
registrations (`IHttpFeature`, `IWebApplicationServer`). Every `IHttpFeature` registration must be a
singleton typed as `IHttpFeature`, and no feature may be disposable: `Build()` rejects the rest,
naming the registration, except a disposable feature a factory produces, which the pipeline build
rejects when the factory first runs. Background work is registered through the concrete
`WebApplicationBuilder.AddService` instance or context-factory overload, which registers an
`IHostService`. `Build()` closes registration and runs each service factory once; services start in
registration order before servers and stop in reverse order after every server drains. The default
server drains lame-duck style: it accepts nothing new, tells every peer the connection is closing
(`Connection: close` or `GOAWAY`), lets the requests in flight finish within the host's shutdown
budget, and cancels only what outlives it. Each exchange sees the drain begin through
`IWebServerDrainFeature`, so a long-lived one, such as a WebSocket, can end itself inside the budget.
It logs its own failures — a listener that cannot bind, a connection fault, a drain the budget cut
short — through `builder.Logging`, never with request content. Disposing the application disposes
the service provider and every factory-created service.

Every listener the default server composes gets three interceptors before any of the application's
own: the request-size limit, the HTTP/1.1 protocol upgrade, and the HTTP/2 and HTTP/3 extended
CONNECT, so `context.Upgrade`, `context.ExtendedConnect` and a WebSocket handshake
(`context.WebSockets`) work on every protocol without listener configuration. A request no handler
accepts is served as before. A `UseServer` callback that clears `options.Interceptors` removes them,
and with them WebSockets on every protocol: the HTTP/2 and HTTP/3 transports keep advertising
extended CONNECT, but nothing surfaces it. See [Design](design.md#default-interceptors).

## Telemetry

The default server traces and measures every request (#1064). Subscribe by name:

- **Traces** — the `ActivitySource` `Assimalign.Cohesion.Web.Hosting` emits one `Server` span per
  request, parented to the caller's W3C `traceparent`, named `GET /orders/{id}` once routing has
  selected the endpoint, and tagged per the OpenTelemetry HTTP server conventions.
- **Metrics** — the `Meter` `Assimalign.Cohesion.Web.Hosting` emits
  `http.server.request.duration` (seconds) and `http.server.active_requests`.
- **Request id** — `context.Features.Get<IWebRequestIdFeature>()?.RequestId` is the request's
  trace id, with or without a listener.

With no listener the server creates no activity and records nothing. Exporting these signals is not
this module's job; see [Design](design.md#server-telemetry-1064), "Server telemetry", for the
attributes, the outcomes and what is deliberately not emitted. The
[observability guide](../../../../web/observability.md) shows a subscription in an application, and
the [server guide](../../../../web/server.md) covers TLS endpoints, shutdown, and diagnostics.

## Dependencies and hosting family

The module references the Web root and its own hosting family within its area (COHRES002), together
with Cohesion's hosting, configuration, DI, logging, and transport infrastructure. Its
`Hosting.Resources` and `Hosting.Health` integrations are runtime concerns; the Web root references
no hosting library. The reusable `Web.Hosting.Resources` and `Web.Hosting.Health` packages do not
reference this module; it consumes `Web.Hosting.Resources` for the enabled resource's control-plane
terminal.

All public composition is explicit and AOT-compatible. See [Design](design.md) for listener
ownership, cancellation, failure isolation, and control-plane behavior.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Hosting.Resources` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Health` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Telemetry` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Logging` | `CohesionProjectReference` |
| `Assimalign.Cohesion.DependencyInjection` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Configuration` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Configuration.CommandLine` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Configuration.EnvironmentVariables` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Configuration.Json` | `CohesionProjectReference` |
| `Assimalign.Cohesion.FileSystem` | `CohesionProjectReference` |
| `Assimalign.Cohesion.FileSystem.Physical` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.Connections` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.ProtocolUpgrade` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.ExtendedConnect` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.RequestLimits` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections.Tcp` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections.Quic` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections.Security` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/src/Assimalign.Cohesion.Web.Hosting.csproj`.
