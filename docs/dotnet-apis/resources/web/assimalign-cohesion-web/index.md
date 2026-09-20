# Assimalign.Cohesion.Web

The Web area root: the pipeline and composition abstractions every Web library builds against.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

The Web area root: the pipeline and composition abstractions every Web library builds against. It is
contracts-first and feature-free by design.

## Scope

- **`Application`/builder contracts** — `IWebApplication`, `IWebApplicationBuilder`,
  `IWebApplicationContext`, the `IHostService` application-lifecycle seam, and the
  server seam `IWebApplicationServer`.
- **Response completion** — `IWebResponseCompletionFeature` registers callbacks that run in
  order after the response is written to the transport. The default server installs it per
  exchange; custom servers may omit it. Registration after completion throws.
- **The middleware-first pipeline** — `IWebApplicationPipeline`,
  `IWebApplicationPipelineBuilder`, `IWebApplicationMiddleware`, the
  `WebApplicationMiddleware` delegate, and the inline `Use(...)` adapter sugar in
  `WebApplicationExtensions`.

Feature libraries (`Assimalign.Cohesion.Web.<Feature>`) reference this root and ship their own
`Add<Feature>` /`Use<Feature>` verbs against these seams; the runtime module
(`Assimalign.Cohesion.Web.Hosting`) implements the contracts. The build-enforced hosting-isolation
rule that keeps those two directions apart is documented in `resources/Web/README.md`.

## Dependencies

`Assimalign.Cohesion.Http`. The root references no `Assimalign.Cohesion.Hosting*` library;
`AddService` belongs to the concrete `Web.Hosting` builder. The root has no DI, configuration, or
logging reference, and it absorbs no feature models, so referencing it never drags a feature surface
along.

## Usage

Applications rarely reference this package directly: `Sdk.Web` delivers the whole family through the
`App.Web` shared framework, and feature verbs (for example `UseRouting` from `Web.Routing` or
`UseForwardedHeaders` from `Web.ForwardedHeaders`) compose against the `IWebApplicationBuilder`
/`IWebApplicationPipelineBuilder` seams defined here.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Http` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web/src/Assimalign.Cohesion.Web.csproj`.
