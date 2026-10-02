# Endpoints and responses

Web endpoints bind request inputs and write responses through the middleware pipeline and serialization registry.

> **Status:** Implemented. The public programming model is middleware-based; return-value result factories were withdrawn.

## Endpoint mapping

`Assimalign.Cohesion.Web.Api` supplies `Map`, `MapGet`, and the other endpoint mappings over
[routing](routing.md), plus `MapGroup` and `MapFallback`. Typed delegate handlers use
`Assimalign.Cohesion.SourceGeneration.Web` to generate parameter binding compatible with Native
ahead-of-time compilation (NativeAOT).

Every `Map*` returns the mapped route's `IRouterRouteBuilder`, so endpoint policies attach where
the endpoint is mapped:

```csharp
app.MapGet("/orders/{id:int}", async (int id, IHttpContext context) =>
{
    context.Response.StatusCode = HttpStatusCode.Ok;
    await context.Response.WriteContentAsync(new Order(id), context.RequestCancelled);
})
    .WithName("order")
    .RequireRateLimiting("api");
```

Because a mapping returns the route builder, `Map*` calls no longer chain into one another
(`app.MapGet(...).MapGet(...)`); write one statement per endpoint. Route groups take the same
typed handlers: `app.MapGroup("/api").MapGet("orders", handler)` binds exactly like an
application endpoint, and the group's prefix, metadata and policies apply to it.

| Input | Selection | Behavior |
|---|---|---|
| Route value | Matching parameter name or `[FromRoute]` | Bind from the matched route token. |
| Route or query value | A scalar the call site cannot place: on a group endpoint, or with a non-literal pattern | Bind from the route value when the matched route captured one, else from the query string. |
| Query value | Scalar default or `[FromQuery]` | Parse the query-string value. |
| Header | `[FromHeader]` | Explicit header binding. |
| Request body | Complex default or `[FromBody]` | One body parameter, read through the serialization registry. |
| Form field | `[FromForm]` | Bind individual scalar fields; the endpoint requires antiforgery validation. |
| `IHttpContext` | Parameter type | Inject the current exchange. |
| `CancellationToken` | Parameter type | Supply `RequestCancelled`. |
| `IHttpFeature` implementation | Parameter type | Resolve the typed context feature. |

Missing required or invalid scalar inputs produce Hypertext Transfer Protocol (HTTP) 400 problem
responses with an `errors` extension naming the parameter. Unsupported body media types produce
415; malformed bodies produce 400.

An endpoint with a `[FromForm]` parameter requires antiforgery when the application references
`Assimalign.Cohesion.Web.Antiforgery`, which every `Sdk.Web` application does through the shared
framework. Register `AddAntiforgery(...)` on the builder and `UseAntiforgery()` after
`UseRouting()`, or opt the endpoint out with `.DisableAntiforgery()`; without the middleware the
endpoint fails at dispatch instead of running unprotected. The generator attaches the requirement
where the endpoint is mapped, so a group's `DisableAntiforgery()` does not reach it. See
[Web.Antiforgery](../dotnet-apis/resources/web/assimalign-cohesion-web-antiforgery/index.md).

## Generator setup

The Web SDK adds the generator automatically. For explicit project wiring, the documented build
items are:

```xml
<ItemGroup>
  <CohesionAnalyzerReference Include="Assimalign.Cohesion.SourceGeneration.Web" />
</ItemGroup>
<PropertyGroup>
  <InterceptorsNamespaces>$(InterceptorsNamespaces);Assimalign.Cohesion.Web.Api.Generated</InterceptorsNamespaces>
</PropertyGroup>
```

Body binding also needs `Web.Serialization` registration; form binding consumes `Http.Forms`.
Those libraries are supplied through the Web shared framework, but the application still composes
the features it uses.

## Writing responses and results

Handlers set response status, headers, and body directly. The template on the [Web page](index.md)
shows a complete executable writing response bytes. There is no shipped `IResult`, `Results`, or
`TypedResults` programming model: those proposed abstractions were withdrawn before merge.
Controller and function packages were also removed from that direction.

`Web.Serialization` supplies independent `IHttpContentReader` and `IHttpContentWriter` contracts
keyed by media type. `AddJsonSerialization` uses a source-generated resolver for JavaScript Object
Notation (JSON). `ReadContentAsync` and `WriteContentAsync` provide the request/response call sites.
The registry handles serialization; application layers remain responsible for selecting response
media types, validating models, and mapping outcomes to status codes.

## Faults and problem payloads

`Web.ProblemDetails` owns the `ProblemDetails` payload, its `application/problem+json` writer,
and `WriteProblemDetailsAsync`. The writer is reflection-free. This library defines the payload;
it does not decide which application outcomes are errors.

`Web.ErrorHandling` supplies `AddErrorHandling().OnError(...)` and `UseErrorHandling`.
The middleware captures faults in `IHttpExceptionFeature`, dispatches an ordered handler chain,
and provides a terminal problem response. It avoids clobbering an already committed response.
`UseStatusCodePages` can upgrade an otherwise bodyless 404.

Return to [Web](index.md).

## Sources

- **Mapping and binding** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Api/docs/OVERVIEW.md`.
- **Antiforgery on form-bound endpoints** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Api/docs/DESIGN.md` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Antiforgery/docs/DESIGN.md`.
- **Serialization and withdrawn results** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Serialization/docs/DESIGN.md`.
- **Problem payload** — `cohesion/resources/Web/Assimalign.Cohesion.Web.ProblemDetails/docs/DESIGN.md`.
- **Error-handling surface** — `cohesion/resources/Web/README.md`.
- **Programming-model direction** — `cohesion/docs/programs/HTTP_WEB_PROGRAM_PLAN.md`.
