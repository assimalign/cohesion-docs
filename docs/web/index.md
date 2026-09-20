# Web

Web is Cohesion's HTTP application platform for hosting, routing, endpoints, and request middleware.

> **Status:** Partial. The runtime and many feature families work; Authorization and Cors remain project scaffolds.

## What it is

Web composes the Hypertext Transfer Protocol (HTTP) stack, application pipeline, and service host.
`Assimalign.Cohesion.Web.Hosting` owns dependency injection, configuration, logging, and transport
composition at builder time. Feature packages extend the hosting-free root contracts.

An application receives the family through the Web shared framework. The runtime does not need
compile-time references to each feature. See the [Web API reference](../dotnet-apis/resources/web/index.md)
and [HTTP library reference](../dotnet-apis/libraries/http/index.md).

## Guides

- **[Server and TLS](server.md)** — Listener ownership, lifecycle, configuration, and certificates.
- **[Routing](routing.md)** — Route matching, precedence, groups, metadata, and generated links.
- **[Endpoints and responses](endpoints.md)** — Typed parameter binding, serialization, and errors.
- **[Middleware](middleware.md)** — Feature families, composition order, and incomplete surfaces.
- **[Testing](testing.md)** — In-memory protocol tests and real `Program.cs` execution.

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.Web`](../dotnet-apis/resources/web/assimalign-cohesion-web/index.md) | Root application, context, middleware, and pipeline contracts. |
| [`Assimalign.Cohesion.Web.Hosting`](../dotnet-apis/resources/web/assimalign-cohesion-web-hosting/index.md) | Concrete host, builder, server, configuration, logging, and transport composition. |
| [`Assimalign.Cohesion.Web.Hosting.Resources`](../dotnet-apis/resources/web/assimalign-cohesion-web-hosting-resources/index.md) | Shared resource-management terminal, bootstrap verification, and deferred stop. |
| [`Assimalign.Cohesion.Web.Hosting.Health`](../dotnet-apis/resources/web/assimalign-cohesion-web-hosting-health/index.md) | Adapter from shared hosting health contributors to Web health checks. |
| [`Assimalign.Cohesion.Web.ApplicationModel`](../dotnet-apis/resources/web/assimalign-cohesion-web-applicationmodel/index.md) | Manifest-backed Web resource, stateless planner, and default control plane. |
| [`Assimalign.Cohesion.Web.Routing`](../dotnet-apis/resources/web/assimalign-cohesion-web-routing/index.md) | Route patterns, constraints, groups, metadata, matching, and link generation. |
| [`Assimalign.Cohesion.Web.Api`](../dotnet-apis/resources/web/assimalign-cohesion-web-api/index.md) | Terminal endpoint mapping and source-generated typed-delegate binding. |
| [`Assimalign.Cohesion.Web.Serialization`](../dotnet-apis/resources/web/assimalign-cohesion-web-serialization/index.md) | Media-type-keyed request readers and response writers. |
| [`Assimalign.Cohesion.Web.ProblemDetails`](../dotnet-apis/resources/web/assimalign-cohesion-web-problemdetails/index.md) | Problem payload model and reflection-free JSON writer. |
| [`Assimalign.Cohesion.Web.ErrorHandling`](../dotnet-apis/resources/web/assimalign-cohesion-web-errorhandling/index.md) | Exception boundary, error-handler chain, and status-code pages. |
| [`Assimalign.Cohesion.Web.Query`](../dotnet-apis/resources/web/assimalign-cohesion-web-query/index.md) | QUERY method content negotiation, conditionals, and redirect helpers. |
| [`Assimalign.Cohesion.Web.HostFiltering`](../dotnet-apis/resources/web/assimalign-cohesion-web-hostfiltering/index.md) | Allowed-host enforcement against the transport-resolved host. |
| [`Assimalign.Cohesion.Web.HttpsPolicy`](../dotnet-apis/resources/web/assimalign-cohesion-web-httpspolicy/index.md) | HTTPS redirection and secure-response transport policy headers. |
| [`Assimalign.Cohesion.Web.RequestTimeouts`](../dotnet-apis/resources/web/assimalign-cohesion-web-requesttimeouts/index.md) | Global and endpoint timeout policies over request cancellation. |
| [`Assimalign.Cohesion.Web.RateLimiting`](../dotnet-apis/resources/web/assimalign-cohesion-web-ratelimiting/index.md) | Global and endpoint limiters, partitioning, queues, and rejection handling. |
| [`Assimalign.Cohesion.Web.Authentication`](../dotnet-apis/resources/web/assimalign-cohesion-web-authentication/index.md) | Named authentication schemes, principal feature, and handler dispatch. |
| [`Assimalign.Cohesion.Web.Authentication.Cookie`](../dotnet-apis/resources/web/assimalign-cohesion-web-authentication-cookie/index.md) | Protected cookie tickets and sign-in/out behavior. |
| [`Assimalign.Cohesion.Web.Authentication.Bearer`](../dotnet-apis/resources/web/assimalign-cohesion-web-authentication-bearer/index.md) | Bearer token validation and principal creation. |
| [`Assimalign.Cohesion.Web.Authorization`](../dotnet-apis/resources/web/assimalign-cohesion-web-authorization/index.md) | Project scaffold; authorization implementation is not present. |
| [`Assimalign.Cohesion.Web.ForwardedHeaders`](../dotnet-apis/resources/web/assimalign-cohesion-web-forwardedheaders/index.md) | Trusted-proxy processing and effective request identity. |
| [`Assimalign.Cohesion.Web.Sessions`](../dotnet-apis/resources/web/assimalign-cohesion-web-sessions/index.md) | Cookie-backed session identity over asynchronous session stores. |
| [`Assimalign.Cohesion.Web.CookiePolicy`](../dotnet-apis/resources/web/assimalign-cohesion-web-cookiepolicy/index.md) | Request/response cookie policy enforcement. |
| [`Assimalign.Cohesion.Web.Cors`](../dotnet-apis/resources/web/assimalign-cohesion-web-cors/index.md) | Project scaffold; cross-origin resource sharing middleware is not present. |
| [`Assimalign.Cohesion.Web.Forms`](../dotnet-apis/resources/web/assimalign-cohesion-web-forms/index.md) | Pipeline integration for HTTP form parsing. |
| [`Assimalign.Cohesion.Web.Health`](../dotnet-apis/resources/web/assimalign-cohesion-web-health/index.md) | Health model, readiness/liveness selection, and HTTP endpoints. |
| [`Assimalign.Cohesion.Web.StaticFiles`](../dotnet-apis/resources/web/assimalign-cohesion-web-staticfiles/index.md) | File serving, default documents, ranges, validators, and precompressed assets. |
| [`Assimalign.Cohesion.Web.Compression`](../dotnet-apis/resources/web/assimalign-cohesion-web-compression/index.md) | Response compression and guarded request decompression. |
| [`Assimalign.Cohesion.Web.Caching`](../dotnet-apis/resources/web/assimalign-cohesion-web-caching/index.md) | Server-owned output cache with policy metadata, tags, and variation keys. |
| [`Assimalign.Cohesion.Web.Diagnostics`](../dotnet-apis/resources/web/assimalign-cohesion-web-diagnostics/index.md) | Bounded request logging and access-log providers. |
| [`Assimalign.Cohesion.Web.Testing`](../dotnet-apis/resources/web/assimalign-cohesion-web-testing/index.md) | Manual and real-entry-point full-pipeline test factories. |

## Hosting model

`WebApplication.CreateBuilder(args)` returns `WebApplicationBuilder`; its `Build()` returns
`WebApplication : Host<WebApplicationContext>`. Explicit `AddService` registrations start
before servers and stop after servers drain. Request-time features use typed context features;
the host does not resolve services per request.

An enabled executable discovers its generated `ResourceRuntime` registration and ambient
`ResourceContext`. `Web.Hosting.Resources` supplies the management terminal for health,
readiness, liveness, endpoint discovery, commands, and graceful stop. Managed routes authenticate
bootstrap credentials. `Web.Hosting.Health` adapts shared host health contributors onto the
independent Web health model.

Configuration mounts and deployment settings arrive through the gateway's runtime contract.
An HTTPS endpoint can name a Secret mount, default `tls`, carrying a PEM leaf, private key,
and chain. See [Server and TLS](server.md) for explicit listener configuration.

## Application model

`AddWeb(manifest, options)` returns `IWebResourceDescriptor` over a `WebResource`.
`RemoteReferenceWeb` supplies the typed external-reference surface. The planner emits a stateless
`Deployment`, with no persistent Volumes, a service per endpoint, and exposure for each public
endpoint. `WebResourceOptions.Replicas` is the typed replica override.

`WebResourceControlPlane.Create()` supplies the default plane; accepted command kinds are empty.
The declarative package references the generic model and `Hosting.Resources`, never Web.Hosting
or a platform gateway. Platform-specific ingress and certificate resolution remain gateway work.

## SDK and framework

`Assimalign.Cohesion.Sdk.Web` delivers `Assimalign.Cohesion.App.Web`.
`Assimalign.Cohesion.Web.ApplicationModel` is NuGet-only and added when
`CohesionApplicationModel=enabled`. Disabled executables remain plain Web applications.

The SDK also supplies the Web source generator for typed endpoint binding.
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

Return to [Cohesion Documentation](../index.md).

## Sources

- **Project map** — `cohesion/resources/Web/README.md`.
- **Area integration** — `cohesion/docs/resources/Web/OVERVIEW.md` and `cohesion/docs/resources/Web/DESIGN.md`.
- **Hosting** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/docs/DESIGN.md`.
- **Declarative plane** — `cohesion/resources/Web/Assimalign.Cohesion.Web.ApplicationModel/docs/OVERVIEW.md`.
- **Scaffolds** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authorization/src/` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Cors/src/`.
- **Template** — `cohesion/tooling/templates/Assimalign.Cohesion.Templates/src/content/cohesion-web/Program.cs`.
- **Program scope** — `cohesion/docs/programs/HTTP_WEB_PROGRAM_PLAN.md` and `cohesion/docs/programs/SERVICE_STORY_REQUIREMENTS.md`.
