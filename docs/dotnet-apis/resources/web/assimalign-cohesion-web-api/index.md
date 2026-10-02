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

Parameters bind from the request by convention or by explicit attribute:

| Source | Attribute | Notes |
| --- | --- | --- |
| `Route` value | `[FromRoute]` | Inferred when the name matches a `{token}` in the pattern |
| Query string | `[FromQuery]` | Default for scalar parameters |
| `Header` | `[FromHeader]` | Explicit only |
| Body | `[FromBody]` | Default for complex parameters; one per handler |
| Form field | `[FromForm]` | Per-field scalars |
| `IHttpContext` | — | Injected directly |
| `CancellationToken` | — | Bound from `RequestCancelled` |
| `IHttpFeature` types | — | Resolved from `context.Features` |

Unparseable or missing-required scalars produce a 400 problem+json (with an `errors` extension
naming the parameter); an unsupported body Content-Type produces 415; a malformed body produces 400.

## Endpoint Metadata and Groups

Every `Map*` returns the mapped route's `IRouterRouteBuilder`, so per-endpoint policies attach where
the endpoint is mapped. Groups hold typed endpoints too, and group metadata reaches every child
whenever it is attached:

See the [source-backed usage examples](examples/index.md).

A group endpoint binds a parameter its own template does not name from the route value first (the
group prefix supplies `tenant` above), then from the query string.

## Wiring

- **Reference the generator** — `<CohesionAnalyzerReference Include="Assimalign.Cohesion.SourceGeneration.Web" />`
  (automatic for Sdk.Web consumers).
- **Allow-list the generated** — namespace:
  `<InterceptorsNamespaces>$(InterceptorsNamespaces);Assimalign.Cohesion.Web.Api.Generated</InterceptorsNamespaces>`.
- **Body binding needs `Web.Serialization`** — (`AddJsonSerialization(...)`); form binding needs
  `Http.Forms`. Both are carried by the `App.Web` shared framework.
- **Form-bound endpoints require antiforgery** — when the application references
  `Assimalign.Cohesion.Web.Antiforgery` (every `Sdk.Web` application does): register
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
