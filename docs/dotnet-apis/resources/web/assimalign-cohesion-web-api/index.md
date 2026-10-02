# Assimalign.Cohesion.Web.Api

`Web.Api` is the endpoint-mapping surface over `Web.Routing`.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

`Web.Api` is the endpoint-mapping surface over `Web.Routing`. It offers plain terminal-middleware
mapping and, through the `Assimalign.Cohesion.SourceGeneration.Web` source generator, AOT-safe
typed-delegate parameter binding.

## Typed Endpoints

See the [source-backed usage examples](examples/index.md).

A handler can be a lambda or a method group (`app.MapGet("/users/{id}", GetUser)`). Parameters bind
from the request by convention or by explicit attribute:

| Source | Attribute | Notes |
| --- | --- | --- |
| Route value | `[FromRoute]` | Inferred when the name matches a `{token}` in the pattern |
| Query string | `[FromQuery]` | Default for scalar parameters |
| Header | `[FromHeader]` | Explicit only |
| Body | `[FromBody]` | Default for complex parameters; one per handler |
| Form field | `[FromForm]` | Per-field scalars |
| Uploaded file | `[FromForm]` (optional) | `IHttpFormFile` (by field name), `IHttpFormFile[]` / `IReadOnlyList<IHttpFormFile>` / `IReadOnlyCollection<IHttpFormFile>` / `IEnumerable<IHttpFormFile>` (every file under the name), `IHttpFormFileCollection` (every file) |
| `IHttpContext` | — | Injected directly |
| `IHttpRequest` / `IHttpResponse` | — | Injected as `context.Request` / `context.Response` |
| `CancellationToken` | — | Bound from `RequestCancelled` |
| `IHttpFeature` types | — | Resolved from `context.Features` |

Unparseable or missing-required scalars, and a missing required file, produce a 400 problem+json
(with an `errors` extension naming the parameter); a form over an Http.Forms size limit produces 413
and a malformed form 400; an unsupported body Content-Type produces 415; a malformed body produces
400. A body type the registered resolver has no contract for, or an application with no
serialization registry, is the server's fault, not the client's: `HttpContentSerializationException`
reaches the exception boundary (a 500), as it does for a returned value.

## Validation

When the application references `Assimalign.Cohesion.Web.Validation` (every `Sdk.Web` application
does, through `App.Web`) and registers validators with `AddValidation`, a typed endpoint validates
the request-body model it binds before the handler runs, and answers an invalid one with 400
problem+json and an `errors` map keyed by member path. `DisableValidation()` opts an endpoint or a
group out. `Web.Api` itself takes no validation dependency; see
[Web.Validation](../assimalign-cohesion-web-validation/index.md).

## Return Values

A handler that returns a value — directly, or through `Task<T>` or `ValueTask<T>` — has it written
as the response. There are no result types: a handler that needs control of the response sets it on
`IHttpContext`.

| The handler returns | The response |
| --- | --- |
| Nothing (`void`, `Task`, `ValueTask`) | Whatever the handler wrote |
| A `string` | The text as UTF-8, `text/plain; charset=utf-8` unless the handler set a `Content-Type` |
| `null` | No body; `204 No Content` unless the handler set another status |
| Any other value | Serialized through the content-serialization registry for the request's `Accept`, with `Vary: Accept`; `406` when nothing registered is acceptable |

The status is 200 unless the handler set one: a handler that sets
`context.Response.StatusCode = HttpStatusCode.Created` and returns the order answers 201 with the
order as its body. A serialized type
needs a contract in the registered resolver (`[JsonSerializable(typeof(Order))]` on the
application's `JsonSerializerContext`); a missing contract or registry throws
`HttpContentSerializationException` to the exception boundary (a 500), never a reflection fallback.

## Compile-Time Diagnostics

A handler the source generator cannot bind fails the build with a `COHWEB` error that says what to
write instead, rather than throwing when the endpoint is mapped: a delegate instance in place of a
lambda (COHWEB0001), a return type an endpoint cannot write such as a `Stream` or `async void`
(COHWEB0002), a parameter that cannot be bound (COHWEB0003), two request bodies (COHWEB0004), a body
with form fields or files (COHWEB0005), a delegate type generated code cannot name (COHWEB0006), and
a body or serialized return without `Web.Serialization` referenced (COHWEB0007). The table is in the
[design](design.md#compile-time-diagnostics-1059).

## Endpoint Metadata and Groups

Every `Map*` returns the mapped route's `IRouterRouteBuilder`, so per-endpoint policies attach where
the endpoint is mapped (`.WithName("user")` for link generation, `.WithMetadata(...)` for any
metadata item). Groups hold typed endpoints too, and group metadata reaches every child whenever it
is attached.

A group endpoint binds a parameter its own template does not name from the route value first (a
group prefix such as `api/{tenant}` supplies `tenant`), then from the query string.

## Endpoint Descriptions

Every typed endpoint also describes itself, for documentation adapters such as the OpenAPI adapter:
an `EndpointParameterMetadata` per request-bound parameter (name, source, CLR type, required) and
`EndpointResponseMetadata` for its responses (status, the written CLR type, `text/plain` for a
string, and a `204` when the result may be `null`). Read them from a built route with
`route.Metadata.GetOrderedMetadata<EndpointParameterMetadata>()` and
`GetOrderedMetadata<EndpointResponseMetadata>()`, or from the matched endpoint during a request.
Another response the handler can answer is described with
`.WithMetadata(new EndpointResponseMetadata(HttpStatusCode.NotFound))`; it composes with the
generated ones.

The types are `typeof(...)` values written by the source generator, so a schema comes from the
application's source-generated `JsonTypeInfo`, never from reflection.

Four convention verbs curate the description on a route or a whole group, without depending on any
documentation format: `WithTags` (tags compose, group first), `WithSummary` and `WithDescription`
(the most specific wins), and `ExcludeFromDescription`. The OpenAPI adapter,
[`Assimalign.Cohesion.Web.OpenApi`](../assimalign-cohesion-web-openapi/index.md) (a NuGet package),
maps them onto operation tags, summaries and descriptions.

## Wiring

- **Reference the generator** — `<CohesionAnalyzerReference Include="Assimalign.Cohesion.SourceGeneration.Web" />`
  (automatic for Sdk.Web consumers).
- **Allow-list the generated namespace** —
  `<InterceptorsNamespaces>$(InterceptorsNamespaces);Assimalign.Cohesion.Web.Api.Generated</InterceptorsNamespaces>`.
- **Body binding and serialized return values need `Web.Serialization`** —
  `AddJsonSerialization(...)` with the application's source-generated `JsonSerializerContext`; form
  binding needs `Http.Forms`. Both are carried by the `App.Web` shared framework.
- **File uploads honor the Http.Forms limits** of the exchange's form feature (`HttpFormOptions`) —
  install `new HttpFormFeature(context.Request, options)` in a middleware ahead of the endpoint to
  change them.
- **Form-bound endpoints (form fields or files) require antiforgery** — when the application
  references `Assimalign.Cohesion.Web.Antiforgery` (every `Sdk.Web` application does): register
  `AddAntiforgery(...)` and `UseAntiforgery()` after `UseRouting()`, or opt an endpoint out with
  `.DisableAntiforgery()`. Without the middleware those endpoints fail at dispatch.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Routing` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.ProblemDetails` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Api/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Api/src/Assimalign.Cohesion.Web.Api.csproj`.
- **Diagnostics** — `cohesion/analyzers/Assimalign.Cohesion.SourceGeneration.Web/src/AnalyzerReleases.Unshipped.md`.
