# Routing

Web routing matches request paths and methods while exposing typed route values and endpoint metadata.

> **Status:** Implemented.

## Composition

`Assimalign.Cohesion.Web.Routing` owns patterns, constraints, groups, metadata, matching, and
outbound links. Call `AddRouting` during builder composition and `UseRouting` on the application
pipeline. Both use the same application-local `IRouterFeature`; route state is not process-wide.
`UseRouting` fails if routing was not added first. The router is built when the pipeline is built,
at startup, so an invalid route table fails the start rather than a request, and mapping a route
after the application started throws.

Endpoint mapping lives in `Assimalign.Cohesion.Web.Api`; see
[Endpoints and responses](endpoints.md). `UseRouting` selects the endpoint and calls the next
middleware; it does not run the endpoint. The end of the pipeline runs it. Middleware registered
after `UseRouting` therefore runs for matched requests, before the endpoint, and reads the selected
endpoint through `context.GetRouteMatch()` and `context.GetEndpointMetadata<T>()`. That is the
position for endpoint policies such as rate limits, timeouts and output caching.

## Matching and precedence

Matching evaluates the path and constraints before the request method. That distinction preserves
Hypertext Transfer Protocol (HTTP) method semantics:

| Match result | Pipeline behavior |
|---|---|
| `Matched` | Publish `IRouteMatchFeature` and call the next middleware; the end of the pipeline runs the endpoint. |
| `MethodNotAllowed` | Call the next middleware; the end of the pipeline answers 405 with `Allow`. |
| `NoMatch` | Call the next middleware; the end of the pipeline answers 404 if nothing else did. |

A CORS preflight (`OPTIONS` with `Origin` and `Access-Control-Request-Method`) to a path with no
`OPTIONS` route publishes the endpoint for the requested method, flagged `IsPreflight`, so CORS
middleware can read its metadata. That candidate never runs for the preflight.

Endpoint metadata can require a middleware. When an endpoint declares a policy that only a
specific middleware applies, such as a rate limit, and that middleware did not process the
request, the end of the pipeline throws `InvalidOperationException` naming the endpoint and the
middleware instead of running the endpoint without its policy. Only the declaration that applies
counts: a route that disables a policy its group requires needs no middleware.

`RoutePattern` computes inbound precedence. Candidates sort from literal segments through
constrained parameters, ordinary parameters, constrained catch-alls, and ordinary catch-alls.
For equal pattern precedence, host-constrained routes rank first, followed by registration order.
For example, `/api/status` outranks `/api/{id}`, regardless of registration order.

## Constraints and route values

Inline policies such as `{id:int}` and `{id:range(1,10)}` resolve through
`RouteParameterPolicyMap`. Unknown policies fail during route construction. A failed policy is a
path mismatch for that route, allowing a more general route to match.

Constraints apply when a value is present; they do not make an optional value mandatory.
`/api/items/{id:int?}` accepts `/api/items` and `/api/items/7`, but not `/api/items/abc`.
`RouteParameterPolicy` is the public extension point; built-in implementations are exposed by name.

## Metadata, groups, and links

`IRouterRouteMetadataCollection` carries endpoint metadata without reflection. Consumers use
`IRouteMatchFeature` to inspect the selected route and its values. `RouteHostMetadata` constrains
host matching through `RouteHostConstraint`.

`MapGroup` creates an `IRouterGroupBuilder` combining a path prefix, shared parameter policies,
and metadata with child routes. Named routes use `RouteNameMetadata`; `ILinkGenerator` produces
outbound paths or absolute URIs from route names and values. These are builder-time declarations
on the same router, rather than a separate registry.

Every `Map` returns an `IRouterRouteBuilder`, and a group is an `IRouterGroupBuilder`; both are
`IRouterConventionBuilder`s, so policies attach where the endpoint or group is declared:

```csharp
IRouterGroupBuilder api = app.MapGroup("/api").RequireRateLimiting("api");

api.MapGet("orders/{id:int}", handler)
    .WithName("order")
    .WithRequestTimeout(TimeSpan.FromSeconds(5));
api.MapGet("health", healthHandler).DisableRateLimiting();
```

Metadata is composed when the route table is built, outer group first, so the order of these
calls does not matter, and a route-level declaration overrides its group's. Routing ships
`WithName` and `RequireHost`; feature packages ship their own verbs: `RequireRateLimiting` and
`DisableRateLimiting`, `WithRequestTimeout` and `DisableRequestTimeout`, `CacheOutput` and
`DisableOutputCache`, and `WithHttpLogging`.

## Fallback routes

`MapFallback(handler)` maps a route that is tried after every other route. It answers only `GET`
and `HEAD`, never answers a path whose last segment names a file (the `nonfile` policy), and never
turns a method mismatch into a 405, so a missing asset still returns 404. `MapFallbackToFile`
in `Web.StaticFiles` serves a file from the web root that way, which is how a single-page
application answers its client-side routes:

```csharp
app.UseStaticFiles();
app.UseRouting();
app.MapGet("/api/orders", ordersHandler);
app.MapFallbackToFile("index.html");
```

An omitted catch-all segment matches: `/files/{**path}` matches `/files`, and a fallback answers
`/` as well.

Return to [Web](index.md).

## Sources

- **Routing contract** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Routing/docs/DESIGN.md`.
- **Endpoint integration** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Api/docs/OVERVIEW.md`.
- **Single-page fallback** — `cohesion/resources/Web/Assimalign.Cohesion.Web.StaticFiles/docs/OVERVIEW.md`.
