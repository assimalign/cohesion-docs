# Web

Web is Cohesion's HTTP application platform for hosting, routing, endpoints, and request middleware.

> **Status:** Partial. The runtime and every feature family in the project map work; the package pages name their deferred surfaces.

## What it is

Web composes the Hypertext Transfer Protocol (HTTP) stack, application pipeline, and service host.
`Assimalign.Cohesion.Web.Hosting` owns dependency injection, configuration, logging, and transport
composition at builder time. Feature packages extend the hosting-free root contracts.

An application receives the family through the Web shared framework. The runtime does not need
compile-time references to each feature: it references `Web.Routing` for the pipeline terminal,
`Web.Server` for the public engine and its exchange features, and `Web.Health` for the integrated
health adapter. See the [Web API reference](../dotnet-apis/resources/web/index.md)
and [HTTP library reference](../dotnet-apis/libraries/http/index.md).

## Guides

- **[Server and TLS](server.md)** — Listeners, HTTPS with ALPN, client certificates, configuration keys, graceful shutdown, and diagnostics.
- **[Observability](observability.md)** — Request spans, HTTP server metrics, event sources, the request id, and default diagnostic forwarding.
- **[Routing](routing.md)** — Route matching, precedence, groups, metadata, and generated links.
- **[URL rewriting](rewrite.md)** — Rewrite and redirect rules, canonical URLs, the request view, and where `UseRewrite` goes.
- **[Endpoints and responses](endpoints.md)** — Typed parameter binding, return values, validation, file uploads, and errors.
- **[OpenAPI](openapi.md)** — Serving an OpenAPI document generated from endpoint metadata, and describing endpoints for it.
- **[WebSockets](websockets.md)** — Socket endpoints on HTTP/1.1, HTTP/2 and HTTP/3, the origin policy, keep-alive, compression, and the drain close.
- **[Middleware](middleware.md)** — Feature families, composition order, and incomplete surfaces.
- **[Middleware order](middleware-order.md)** — The registration order for every Web middleware and the reason for each position.
- **[Testing](testing.md)** — In-memory protocol tests over the composed pipeline.

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.Web`](../dotnet-apis/resources/web/assimalign-cohesion-web/index.md) | Root application, context, middleware, and pipeline contracts, and the composition verbs `Use`, `UseWhen` and `Run`; no feature contracts. |
| [`Assimalign.Cohesion.Web.Server`](../dotnet-apis/resources/web/assimalign-cohesion-web-server/index.md) | Public `WebApplicationServer` engine and `WebApplicationServerOptions`; dispatch, drain, request spans, HTTP metrics, server events, and the seven per-exchange features it publishes. |
| [`Assimalign.Cohesion.Web.Hosting`](../dotnet-apis/resources/web/assimalign-cohesion-web-hosting/index.md) | Concrete host, builder, non-owning `WebApplicationServerHostService`, configuration, logging, HTTP defaults, transport composition, and default EventSource forwarding. |
| [`Assimalign.Cohesion.Web.Hosting.ApplicationModel`](../dotnet-apis/resources/web/assimalign-cohesion-web-hosting-applicationmodel/index.md) | `builder.Services.AddResource(name, ...)`: the application as a managed resource instance whose registered settings, such as `cors`, become actions. |
| [`Assimalign.Cohesion.Web.Routing`](../dotnet-apis/resources/web/assimalign-cohesion-web-routing/index.md) | Route patterns, constraints, groups, metadata, endpoint selection, fallback routes, and link generation; the endpoint contract, the shared pipeline terminal, and the `Map` and `MapWhen` branches that end in it. |
| [`Assimalign.Cohesion.Web.Rewrite`](../dotnet-apis/resources/web/assimalign-cohesion-web-rewrite/index.md) | Ordered rewrite and redirect rules ahead of routing, the request view and `IWebRewriteFeature`, and canonicalization redirects (HTTPS, `www`, trailing slash, lowercase). |
| [`Assimalign.Cohesion.Web.Api`](../dotnet-apis/resources/web/assimalign-cohesion-web-api/index.md) | Endpoint mapping that returns route builders for endpoint policies, source-generated typed-delegate binding (uploaded files included), returned values, `COHWEB` compile errors, and endpoint-description metadata with its description verbs. |
| [`Assimalign.Cohesion.Web.WebSockets`](../dotnet-apis/resources/web/assimalign-cohesion-web-websockets/index.md) | `MapWebSocket` endpoints for every handshake shape (`GET` on HTTP/1.1, extended `CONNECT` on HTTP/2 and HTTP/3) and the `UseWebSockets` policy: the cross-site origin check, keep-alive and compression defaults, and the `1001` drain close. |
| [`Assimalign.Cohesion.Web.Serialization`](../dotnet-apis/resources/web/assimalign-cohesion-web-serialization/index.md) | Media-type-keyed request readers and response writers, and the read-only JSON contract lookup describers use. |
| [`Assimalign.Cohesion.Web.OpenApi`](../dotnet-apis/resources/web/assimalign-cohesion-web-openapi/index.md) | OpenAPI 3.0, 3.1 and 3.2 documents from endpoint metadata, served as JSON or YAML; a NuGet package outside the shared framework. |
| [`Assimalign.Cohesion.Web.Validation`](../dotnet-apis/resources/web/assimalign-cohesion-web-validation/index.md) | Request-body validation for typed endpoints over ObjectValidation, answered with 400 and an `errors` map keyed by JSON member names and collection indexes, with CLR names as fallback. |
| [`Assimalign.Cohesion.Web.ProblemDetails`](../dotnet-apis/resources/web/assimalign-cohesion-web-problemdetails/index.md) | Problem payload model and reflection-free JSON writer. |
| [`Assimalign.Cohesion.Web.ErrorHandling`](../dotnet-apis/resources/web/assimalign-cohesion-web-errorhandling/index.md) | Exception boundary, error-handler chain, and status-code pages. |
| [`Assimalign.Cohesion.Web.Query`](../dotnet-apis/resources/web/assimalign-cohesion-web-query/index.md) | QUERY method content negotiation, conditionals, and redirect helpers. |
| [`Assimalign.Cohesion.Web.HostFiltering`](../dotnet-apis/resources/web/assimalign-cohesion-web-hostfiltering/index.md) | Allowed-host enforcement against the effective host (forwarded by a trusted proxy, else transport-resolved). |
| [`Assimalign.Cohesion.Web.HttpsPolicy`](../dotnet-apis/resources/web/assimalign-cohesion-web-httpspolicy/index.md) | HTTPS redirection and secure-response transport policy headers. |
| [`Assimalign.Cohesion.Web.SecurityHeaders`](../dotnet-apis/resources/web/assimalign-cohesion-web-securityheaders/index.md) | Browser security headers with safe defaults, an opt-in Content Security Policy with per-request nonces, Permissions-Policy, cross-origin isolation fields, and endpoint overrides. |
| [`Assimalign.Cohesion.Web.RequestTimeouts`](../dotnet-apis/resources/web/assimalign-cohesion-web-requesttimeouts/index.md) | Global and endpoint timeout policies over request cancellation. |
| [`Assimalign.Cohesion.Web.RateLimiting`](../dotnet-apis/resources/web/assimalign-cohesion-web-ratelimiting/index.md) | Global and endpoint limiters, partitioning, queues, and rejection handling. |
| [`Assimalign.Cohesion.Web.Authentication`](../dotnet-apis/resources/web/assimalign-cohesion-web-authentication/index.md) | Named authentication schemes, principal feature, and handler dispatch. |
| [`Assimalign.Cohesion.Web.Authentication.Cookie`](../dotnet-apis/resources/web/assimalign-cohesion-web-authentication-cookie/index.md) | Protected cookie tickets and sign-in/out behavior. |
| [`Assimalign.Cohesion.Web.Authentication.Bearer`](../dotnet-apis/resources/web/assimalign-cohesion-web-authentication-bearer/index.md) | Bearer token validation and principal creation. |
| [`Assimalign.Cohesion.Web.Authentication.Certificate`](../dotnet-apis/resources/web/assimalign-cohesion-web-authentication-certificate/index.md) | Authentication from a TLS client certificate accepted by the connection's handshake policy, with optional account mapping. |
| [`Assimalign.Cohesion.Web.Authorization`](../dotnet-apis/resources/web/assimalign-cohesion-web-authorization/index.md) | Endpoint policies over `ClaimsPrincipal`, `RequireAuthorization` and `AllowAnonymous`, default and fallback policies, per-endpoint schemes, and each endpoint's effective policy readable for describers. |
| [`Assimalign.Cohesion.Web.ForwardedHeaders`](../dotnet-apis/resources/web/assimalign-cohesion-web-forwardedheaders/index.md) | Trusted-proxy processing and effective request identity. |
| [`Assimalign.Cohesion.Web.Sessions`](../dotnet-apis/resources/web/assimalign-cohesion-web-sessions/index.md) | Cookie-backed session identity over asynchronous session stores. |
| [`Assimalign.Cohesion.Web.CookiePolicy`](../dotnet-apis/resources/web/assimalign-cohesion-web-cookiepolicy/index.md) | Consent, attribute floors, RFC 6265bis prefix rules, and the lifetime cap for every cookie the application appends. |
| [`Assimalign.Cohesion.Web.Cors`](../dotnet-apis/resources/web/assimalign-cohesion-web-cors/index.md) | Validated cross-origin resource sharing (CORS) policies, preflight answers, and per-endpoint policy selection. |
| [`Assimalign.Cohesion.Web.Forms`](../dotnet-apis/resources/web/assimalign-cohesion-web-forms/index.md) | Pipeline integration for HTTP form parsing, answering a form over a limit `413` and a malformed one `400`. |
| [`Assimalign.Cohesion.Web.Antiforgery`](../dotnet-apis/resources/web/assimalign-cohesion-web-antiforgery/index.md) | Cross-site request forgery token validation for endpoints that require it, including every form-bound typed endpoint. |
| [`Assimalign.Cohesion.Web.Health`](../dotnet-apis/resources/web/assimalign-cohesion-web-health/index.md) | Health model, readiness/liveness selection, and HTTP endpoints. |
| [`Assimalign.Cohesion.Web.StaticFiles`](../dotnet-apis/resources/web/assimalign-cohesion-web-staticfiles/index.md) | Web-root file serving, default documents, ranges, validators, precompressed assets, the single-page-application fallback, and the `SendFileAsync`/`WriteStreamAsync` response helpers. |
| [`Assimalign.Cohesion.Web.Compression`](../dotnet-apis/resources/web/assimalign-cohesion-web-compression/index.md) | Response compression and guarded request decompression. |
| [`Assimalign.Cohesion.Web.Caching`](../dotnet-apis/resources/web/assimalign-cohesion-web-caching/index.md) | Server-owned output cache with policy metadata, tags, and variation keys. |
| [`Assimalign.Cohesion.Web.Diagnostics`](../dotnet-apis/resources/web/assimalign-cohesion-web-diagnostics/index.md) | Bounded request logging and access-log providers. |
| [`Assimalign.Cohesion.Web.Testing`](../dotnet-apis/resources/web/assimalign-cohesion-web-testing/index.md) | Manual and real-entry-point full-pipeline test factories. |

## Hosting model

`WebApplication.CreateBuilder(args)` returns `WebApplicationBuilder`; its `Build()` returns
`WebApplication : Host<WebApplicationContext>`. Explicit `AddService` registrations start
before servers and stop after servers drain. Every registered server runs through a non-owning
`WebApplicationServerHostService`, which calls its `IWebApplicationServer` lifecycle even when
the server also implements `IHostService`. Request-time features use typed context features;
the host does not resolve services per request.

Without a listener of its own, the host binds the `http` endpoint its platform names, so the same
`Program.cs` runs standalone, in a gateway's process, or in a container. See
[Server and TLS](server.md) for explicit listener configuration.

## Managed resource

A Web application becomes a managed resource instance with one registration in its `Program.cs`,
from [`Web.Hosting.ApplicationModel`](../dotnet-apis/resources/web/assimalign-cohesion-web-hosting-applicationmodel/index.md):

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.Web.Cors;
using Assimalign.Cohesion.Web.Hosting;
using Assimalign.Cohesion.Web.Hosting.ApplicationModel;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(cors => cors.Options.AddDefaultPolicy(policy => policy.WithOrigins("https://shop.example")));
builder.Services.AddResource("orders-api", resource => resource.UseInProcess());

WebApplication app = builder.Build();
app.UseCors();
await app.RunAsync();
```

When the environment names a platform the resource opted into (`COHESION_GATEWAY`), the instance
publishes its manifest and restores the state its gateway kept before any server starts, then lets
the gateway run its actions. Its actions are the registered settings the resource knows, such as
the default CORS policy as the `cors` action, followed by the ones the application adds with
`AddAction`. Without a platform, `AddResource` registers nothing and the application is a plain Web
application. A gateway declares the resource generically: `builder.AddResource("Web", "orders-api")`.
See [ApplicationModel](../dotnet-apis/libraries/application-model/index.md) and
[Platforms](../platforms/index.md).

## SDK and framework

`Assimalign.Cohesion.Sdk.Web` delivers `Assimalign.Cohesion.App.Web`, which carries
`Assimalign.Cohesion.Web.Hosting.ApplicationModel`; the application references only the platform
packages it opts into.
`Assimalign.Cohesion.Web.OpenApi` is NuGet-only: an application that serves an OpenAPI document
references it (see [OpenAPI](openapi.md)).

The SDK also supplies the Web source generator for typed endpoint binding.
It enables `EventSourceSupport` when unset so NativeAOT applications deliver framework events;
an explicit `<EventSourceSupport>false</EventSourceSupport>` disables delivery.
See the [Web SDK reference](../dotnet-apis/sdks/sdk-web/index.md).

## Getting started

This is the real `Program.cs` from the `cohesion-web` template. The terminal middleware handles
the root path and passes other requests onward.

```csharp
using System.Text;

using Assimalign.Cohesion.Http;
using Assimalign.Cohesion.Web.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
await using WebApplication application = builder.Build();

application.Use(async (context, next) =>
{
    if (context.Request.Path.Value != "/")
    {
        await next.Invoke(context).ConfigureAwait(false);
        return;
    }

    context.Response.StatusCode = HttpStatusCode.Ok;
    byte[] payload = Encoding.UTF8.GetBytes("Hello from CohesionProject");
    await context.Response.Body.WriteAsync(payload, context.RequestCancelled).ConfigureAwait(false);
});

await application.RunAsync();
```

`dotnet run` listens on `http://localhost:5000` (loopback only) until the application configures an
endpoint. Set `Http:Endpoints` in `appsettings.json`, for example
`{ "Http": { "Endpoints": { "Main": { "Host": "0.0.0.0", "Port": 8080 } } } }`, or pass
`--Http:Endpoints:Main:Port=8080` on the command line. The Web SDK copies `appsettings*.json` and
`wwwroot/**` to the build and publish output.

The `cohesion-spa` template also maps `MapFallbackToFile("index.html")`. `GET` and `HEAD` requests
for client routes such as `/settings` serve `wwwroot/index.html`; a missing file such as
`/assets/missing.js` still returns `404` (see [Fallback routes](routing.md#fallback-routes)).

Return to [Cohesion Documentation](../index.md).

## Sources

- **Project map** — `cohesion/resources/Web/README.md`.
- **Area integration** — `cohesion/docs/resources/Web/OVERVIEW.md` and `cohesion/docs/resources/Web/DESIGN.md`.
- **Hosting** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/docs/DESIGN.md`.
- **Resource integration** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting.ApplicationModel/docs/OVERVIEW.md`.
- **Middleware order** — `cohesion/docs/resources/Web/MIDDLEWARE_ORDER.md`.
- **Template** — `cohesion/tooling/templates/Assimalign.Cohesion.Templates/src/content/cohesion-web/Program.cs` and `cohesion/tooling/templates/Assimalign.Cohesion.Templates/src/content/cohesion-web/README.md`.
- **SPA template and NativeAOT events** — `cohesion/tooling/templates/Assimalign.Cohesion.Templates/src/content/cohesion-spa/README.md` and `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Tasks/docs/OVERVIEW.md`.
- **Program scope** — `cohesion/docs/programs/HTTP_WEB_PROGRAM_PLAN.md` and `cohesion/docs/programs/SERVICE_STORY_REQUIREMENTS.md`.
