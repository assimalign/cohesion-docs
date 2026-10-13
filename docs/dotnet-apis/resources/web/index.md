# Web

Web provides application contracts, HTTP pipeline features, hosting, and integration testing.

> **Status:** Partial. Package-level pages distinguish implemented behavior from deferred surfaces.

The Web resource (`resources/Web/*`) is Cohesion's L3 web application platform: the composition
abstractions, the request-pipeline feature libraries, and the hosting runtime that together form
what the `Assimalign.Cohesion.Sdk.Web` SDK delivers through the `Assimalign.Cohesion.App.Web` shared
framework.

## Packages

| Package | Role | Shared-framework delivery |
|---|---|---|
| [`Assimalign.Cohesion.Web`](assimalign-cohesion-web/index.md) | `Root` | Public reference and runtime |
| [`Assimalign.Cohesion.Web.Antiforgery`](assimalign-cohesion-web-antiforgery/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.Api`](assimalign-cohesion-web-api/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.ApplicationModel`](assimalign-cohesion-web-applicationmodel/index.md) | `Application` model | Separate package; not in this framework |
| [`Assimalign.Cohesion.Web.Authentication`](assimalign-cohesion-web-authentication/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.Authentication.Bearer`](assimalign-cohesion-web-authentication-bearer/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.Authentication.Certificate`](assimalign-cohesion-web-authentication-certificate/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.Authentication.Cookie`](assimalign-cohesion-web-authentication-cookie/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.Authorization`](assimalign-cohesion-web-authorization/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.Caching`](assimalign-cohesion-web-caching/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.Compression`](assimalign-cohesion-web-compression/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.CookiePolicy`](assimalign-cohesion-web-cookiepolicy/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.Cors`](assimalign-cohesion-web-cors/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.Diagnostics`](assimalign-cohesion-web-diagnostics/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.ErrorHandling`](assimalign-cohesion-web-errorhandling/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.Forms`](assimalign-cohesion-web-forms/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.ForwardedHeaders`](assimalign-cohesion-web-forwardedheaders/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.Health`](assimalign-cohesion-web-health/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.HostFiltering`](assimalign-cohesion-web-hostfiltering/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.Hosting`](assimalign-cohesion-web-hosting/index.md) | Hosting | Public reference and runtime |
| [`Assimalign.Cohesion.Web.HttpsPolicy`](assimalign-cohesion-web-httpspolicy/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.OpenApi`](assimalign-cohesion-web-openapi/index.md) | Feature library | Separate package; not in this framework |
| [`Assimalign.Cohesion.Web.ProblemDetails`](assimalign-cohesion-web-problemdetails/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.Query`](assimalign-cohesion-web-query/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.RateLimiting`](assimalign-cohesion-web-ratelimiting/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.RequestTimeouts`](assimalign-cohesion-web-requesttimeouts/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.Rewrite`](assimalign-cohesion-web-rewrite/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.Routing`](assimalign-cohesion-web-routing/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.SecurityHeaders`](assimalign-cohesion-web-securityheaders/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.Serialization`](assimalign-cohesion-web-serialization/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.Server`](assimalign-cohesion-web-server/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.Sessions`](assimalign-cohesion-web-sessions/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.StaticFiles`](assimalign-cohesion-web-staticfiles/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.Testing`](assimalign-cohesion-web-testing/index.md) | Testing | Not listed in this framework |
| [`Assimalign.Cohesion.Web.Validation`](assimalign-cohesion-web-validation/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.WebSockets`](assimalign-cohesion-web-websockets/index.md) | Feature library | Public reference and runtime |

## Architecture

`Web.Hosting` includes reusable [resource-management middleware](assimalign-cohesion-web-hosting/design.md#resource-control-plane),
including authentication and ahead-of-time (AOT) compilation-compatible JSON handling. Area hosts
own their listeners, readiness gates, and lifecycle. Its [health integration](assimalign-cohesion-web-hosting/design.md#health-integration)
adapts hosting health contributors into the independent `Web.Health` model. The public types keep
the `Assimalign.Cohesion.Web.Hosting.Resources` and `Assimalign.Cohesion.Web.Hosting.Health`
namespaces under `src/Resources/` and `src/Health/`, but their former separate packages are removed
(owner decision 57, 2026-10-13).

The integrations are public through `Web.Hosting` in `App.Web` and private in each other area
framework. `Web.Health` joins the 16 area framework closures that lacked it; Database already
carried it. The hosting context owns retained control-plane resources internally.

## Dependency boundary

The single runtime module is `Assimalign.Cohesion.Web.Hosting`. Roots and feature libraries do not
depend on the hosting family or on `Assimalign.Cohesion.Hosting` and its child libraries. Feature
registration verbs are component integrations on `builder.Services` (owner decision 34, #1380), and
pipeline verbs extend the root's `IWebApplicationPipelineBuilder`, so hosting a feature needs no
reference to it. The exact runtime may reference any Web library except `Web.Testing`,
`Web.ApplicationModel`, `Web.ApplicationModel.Orchestration`, the `App.Web` producers, and test,
example, sample, and fixture projects (COHRES002, relaxed by owner decision 2026-10-09).
It references only what it needs: `Web.Routing` for the pipeline terminal, `Web.Server` for the
public engine and its per-exchange features, and `Web.Health` for the integrated health adapter,
because every reference it takes ships in each framework that carries it: `App.Web` and, privately,
all 17 other area frameworks. The declarative application model remains separate from the runtime.
See the [resource dependency rules](../index.md#dependency-rules) .

`Web.Hosting` integrates `Hosting.Health` and `Hosting.Resources` itself. No separate Web
hosting-family package exists. A future integration package is only for an integration that must
not land in every framework that carries the module. `Web.Testing` exempts only `Web.Hosting` to
drive the runtime and its control plane.

`Web.Server` references `Web.Routing` for the `http.route` template; `Web.Routing` must never
reference `Web.Server`, which would form a cycle. The engine has no Hosting or connection-driver
dependency and can be constructed directly.

## Server engine and contracts

[`Assimalign.Cohesion.Web.Server`](assimalign-cohesion-web-server/index.md) owns the public
`WebApplicationServer` engine and `WebApplicationServerOptions`, so an application can run the
server without the hosting module. The server builds its listener from the options and installs
its own interceptors. `Web.Hosting` keeps HTTP default interceptors, configuration binding, and
TCP and QUIC listener helpers, and wraps every registered `IWebApplicationServer` in the public,
non-owning `WebApplicationServerHostService`.

The engine publishes the per-exchange features `IWebRequestIdFeature` (the request's W3C trace id),
`IWebRequestActivityFeature` (the server span), `IWebResponseStartingFeature` (callbacks before
final-head commit), `IWebResponseCompletionFeature` (callbacks after the response reaches the
transport), `IWebServerDrainFeature` (the lame-duck drain signal) and `IWebClientFaultFeature` (the
`4xx` the transport answers a request body that broke its framing or a limit with, which
`Web.Diagnostics`, `Web.ErrorHandling` and `Web.Compression` read so a client fault is not handled
as an application defect, #1340), plus `IWebResponseClientFaultFeature` (the response was abandoned
by the peer or failed its minimum data rate, #1432). `Web.Server` installs all seven; namespace
`Assimalign.Cohesion.Web`, moved from the root (#1379).

Connection loss mid-upload or mid-response cancels the exchange on every protocol. An incomplete
HTTP/1.1 body ended by EOF no longer reports `400`; malformed framing and configured limits keep
their client-fault statuses. The response report also lets error handling recognize a transport
write failure wrapped in another exception.

The listener seam is `IHttpConnectionListener : IConnectionListener<IHttpConnection>`:
`Protocols` identifies the configured protocols, `EndPoints` exposes the actual bound endpoints
after `BindAsync`, and `AcceptAsync` replaces `AcceptOrListenAsync`. Its live `Connections`
collection tracks accepted connections until `ConnectionClosed` fires. `IHttpConnection` inherits
the pipe-less `IConnection` root and adds `Protocol`; `IDuplexConnection` supplies byte-pipe access
for stream transports. The server reads bound endpoints from the listener instead of Hosting's
former endpoint capture (decision 58, #1459).

## Diagnostics

The [observability guide](../../../web/observability.md#diagnostic-signal-ownership) maps request
spans, metrics, operator events and access logs to their Web or transport owner, and lists all
22 Web event sources. `Web.Server` reports server diagnostics through `WebServerEventSource`,
with its EventSource, Meter and ActivitySource named `Assimalign.Cohesion.Web.Server`.
`Web.Hosting` reports control-plane events through `WebHostingEventSource`, named
`Assimalign.Cohesion.Web.Hosting`, including the exact `rejected-credentials` counter.
`Http.Connections` reports transport rejections and protocol errors, including requests that
never reach Web dispatch.

Entry-point `WebApplication.CreateBuilder(args)` applications forward the `Assimalign.Cohesion.Web.`
prefix and `Assimalign.Cohesion.Http.Connections` into configured logging sinks by default.
Nested/options builders do not. Application-selected forwarding or
`EnableEventSourceForwarding = false` takes precedence; no provider or filter rule is added.
`Sdk.Web` defaults `EventSourceSupport` to `true` when unset, retaining event delivery under NativeAOT.

## Feature registration

Owner decisions 34 and 35 (2026-10-09, #1380), and 39 (2026-10-10, #1395), set how a feature reaches
an application:

- **Registration verbs are component integrations on `builder.Services`.** Each feature package
  declares `[assembly: ComponentIntegration(...)]` targeting
  `Assimalign.Cohesion.DependencyInjection.IServiceProviderBuilder.AddSingleton` with
  `Contract = typeof(IHttpFeature)`, and the generator projects the verb into the application, so
  neither the package nor `Web.Hosting` takes a reference for it. `Sdk.Web` applications get the
  generator from the App framework; in-repo projects add a `CohesionAnalyzerReference` to
  `Assimalign.Cohesion.SourceGeneration.ComponentModel`.
- **Pipeline verbs stay `extension(...)` members** of the feature package: `Use<Feature>` on
  `IWebApplicationPipelineBuilder`, `Map*` on the pipeline and router surfaces.
- **Every `IHttpFeature` registration is a singleton.** `Web.Hosting` rejects a scoped or transient
  one, one registered under a narrower contract, and a disposable instance or implementation type at
  `Build`, and a disposable feature a factory registration produces at the pipeline build, where the
  product first exists; each error names the registration.
- **`IWebApplicationBuilder.AddFeature` stays the raw path**, for features no package ships a verb
  for.

| Package | Verb | Shape |
|---|---|---|
| `Web.Routing` | `AddRouting()` | builder template (`RoutingBuilder`) |
| `Web.Authentication` | `AddAuthentication(auth => auth.AddCookie().AddJwtBearer(...))` | builder template (`AuthenticationBuilder`) |
| `Web.Authorization` | `AddAuthorization()`, `AddAuthorization(authorization => authorization.Options.AddPolicy(...))` | builder template (`AuthorizationBuilder`) |
| `Web.Antiforgery` | `AddAntiforgery()`, `AddAntiforgery(antiforgery => antiforgery.UseDataProtection(provider))` | builder template (`AntiforgeryBuilder`) |
| `Web.ErrorHandling` | `AddErrorHandling(errors => errors.OnError(...))` | builder template (`ErrorHandlingBuilder`) |
| `Web.Serialization` | `AddContentSerialization(serialization => ...)`, `AddJsonSerialization(resolver, ...)` | builder template (`ContentSerializationBuilder`); static factory (`SerializationComponents`) |
| `Web.Validation` | `AddValidation(validation => validation.Options.AddProfile(...))` | builder template (`ValidationBuilder`) |
| `Web.OpenApi` | `AddOpenApi()`, `AddOpenApi(openApi => openApi.Options.Title = ...)` | builder template (`OpenApiBuilder`) |

Every registration verb is a builder template unless it has a required value argument. The
template's configure callback is optional (owner decision 39, 2026-10-10, #1395), so a verb with
nothing to configure is called bare: `builder.Services.AddRouting()`. A builder owns its feature's
options and exposes them as `Options`, as `AuthenticationBuilder` does, and a value the verb used to
take as an optional argument is a builder method: `antiforgery.UseDataProtection(provider)`. The one
static factory left is `AddJsonSerialization(resolver)`, because a required resolver as a builder
method would fail at registration instead of at compile time;
`AddContentSerialization(serialization => serialization.AddJson(resolver))` is the builder-template
spelling of the same registration.

```csharp
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddRouting()
    .AddJsonSerialization(AppJsonContext.Default)
    .AddAuthentication(auth => auth.AddCookie())
    .AddAuthorization();

await using WebApplication app = builder.Build();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
await app.RunAsync();
```

## Framework and SDK

`Assimalign.Cohesion.Sdk.Web` delivers the `Assimalign.Cohesion.App.Web` family. Its producers,
`Assimalign.Cohesion.Web.Refs` and `Assimalign.Cohesion.Web.Runtime`, declare
`CohesionFrameworkName` and import `libraries/App/Assimalign.Cohesion.App.props`. The public and
private assembly inventory is hand-curated in the Runtime producer's `Directory.Build.props`, which
the Refs producer imports. `CohesionFrameworkAssembly` entries appear in the reference and runtime
packs; `CohesionFrameworkPrivateAssembly` entries appear only at runtime.

| Public reference-pack assembly |
|---|
| `Assimalign.Cohesion.App.Web` |
| `Assimalign.Cohesion.Web` |
| `Assimalign.Cohesion.Web.Antiforgery` |
| `Assimalign.Cohesion.Web.Api` |
| `Assimalign.Cohesion.Web.Authentication` |
| `Assimalign.Cohesion.Web.Authentication.Bearer` |
| `Assimalign.Cohesion.Web.Authentication.Certificate` |
| `Assimalign.Cohesion.Web.Authentication.Cookie` |
| `Assimalign.Cohesion.Web.Authorization` |
| `Assimalign.Cohesion.Web.Caching` |
| `Assimalign.Cohesion.Web.Compression` |
| `Assimalign.Cohesion.Web.CookiePolicy` |
| `Assimalign.Cohesion.Web.Cors` |
| `Assimalign.Cohesion.Web.Diagnostics` |
| `Assimalign.Cohesion.Web.ErrorHandling` |
| `Assimalign.Cohesion.Web.Forms` |
| `Assimalign.Cohesion.Web.ForwardedHeaders` |
| `Assimalign.Cohesion.Web.Health` |
| `Assimalign.Cohesion.Web.HostFiltering` |
| `Assimalign.Cohesion.Web.Hosting` |
| `Assimalign.Cohesion.Web.HttpsPolicy` |
| `Assimalign.Cohesion.Web.ProblemDetails` |
| `Assimalign.Cohesion.Web.Query` |
| `Assimalign.Cohesion.Web.RateLimiting` |
| `Assimalign.Cohesion.Web.RequestTimeouts` |
| `Assimalign.Cohesion.Web.Rewrite` |
| `Assimalign.Cohesion.Web.Routing` |
| `Assimalign.Cohesion.Web.SecurityHeaders` |
| `Assimalign.Cohesion.Web.Serialization` |
| `Assimalign.Cohesion.Web.Server` |
| `Assimalign.Cohesion.Web.Sessions` |
| `Assimalign.Cohesion.Web.StaticFiles` |
| `Assimalign.Cohesion.Web.Validation` |
| `Assimalign.Cohesion.Web.WebSockets` |
| `Assimalign.Cohesion.Caching` |
| `Assimalign.Cohesion.Caching.InMemory` |
| `Assimalign.Cohesion.Http` |
| `Assimalign.Cohesion.Http.Antiforgery` |
| `Assimalign.Cohesion.Http.Connections` |
| `Assimalign.Cohesion.Http.Cookies` |
| `Assimalign.Cohesion.Http.ExtendedConnect` |
| `Assimalign.Cohesion.Http.Forms` |
| `Assimalign.Cohesion.Http.Forwarded` |
| `Assimalign.Cohesion.Http.ProtocolUpgrade` |
| `Assimalign.Cohesion.Http.RequestLimits` |
| `Assimalign.Cohesion.Http.Sessions` |
| `Assimalign.Cohesion.Http.WebSockets` |
| `Assimalign.Cohesion.ObjectValidation` |
| `Assimalign.Cohesion.Http.Tls` |
| `Assimalign.Cohesion.Http.Streaming` |
| `Assimalign.Cohesion.Http.ServerSentEvents` |
| `Assimalign.Cohesion.Http.InterimResponses` |
| `Assimalign.Cohesion.Http.DigestFields` |
| `Assimalign.Cohesion.Connections.Quic` |
| `Assimalign.Cohesion.Connections.Security` |
| `Assimalign.Cohesion.Connections.Tcp` |
| `Assimalign.Cohesion.IdentityModel` |
| `Assimalign.Cohesion.IdentityModel.Token` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` |
| `Assimalign.Cohesion.Security.DataProtection` |

`Application`-model and client packages, and `Assimalign.Cohesion.Web.OpenApi`, are NuGet-only and
are excluded from the area shared framework. The package table distinguishes assemblies present in
the source tree from those included by the current framework inventory.

The shared `App` kernel supplies `Assimalign.Cohesion.Logging.EventSource` and the connection
feature packages `Assimalign.Cohesion.Connections.Tls`, `.StreamAbort` and `.StreamLimits`.
They are shared kernel members rather than additions to the App.Web-specific inventory above.

## Related documentation

- **Product** — [Web](../../../web/index.md).
- **Middleware order** — [the registration order for every Web middleware](../../../web/middleware-order.md).
- **OpenAPI** — [serving and describing an API with Web.OpenApi](../../../web/openapi.md).
- **Routing** — [route-table registration, endpoint selection and pipeline ordering](assimalign-cohesion-web-routing/index.md).
- **WebSockets** — [socket endpoints on every protocol, the origin policy, and the drain close](../../../web/websockets.md).
- **URL rewriting** — [rewrite and redirect rules ahead of routing](../../../web/rewrite.md).
- **Observability** — [request spans, HTTP metrics, event sources, default forwarding and request ids](../../../web/observability.md).
- **SDK** — [`Assimalign.Cohesion.Sdk.Web`](../../sdks/sdk-web/index.md).
- **Parent** — [Resources](../index.md).

## Sources

- **Primary source** — `cohesion/resources/Web/README.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/libraries/App/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Refs/src/Assimalign.Cohesion.Web.Refs.csproj` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Runtime/Directory.Build.props`.
- **Architecture source** — `cohesion/docs/resources/Web/DESIGN.md`.
- **Diagnostic catalog** — `cohesion/docs/EVENT_SOURCES.md`.
- **Architecture overview** — `cohesion/docs/resources/Web/OVERVIEW.md`.
