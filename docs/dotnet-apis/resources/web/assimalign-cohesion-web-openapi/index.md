# Assimalign.Cohesion.Web.OpenApi

OpenAPI documents for Cohesion Web applications, generated from endpoint metadata rather than runtime reflection.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

OpenAPI documents for Cohesion Web applications, generated from the metadata endpoints already
carry rather than from runtime reflection, so the same document is produced under NativeAOT. The
package ships as a NuGet package, not with the `App.Web` shared framework: an application that
documents its API references it.

## Scope

- **The Web adapter** for `OpenApi.Integration`'s `IOpenApiEndpointSource`, over the application's
  route table: paths from route templates, parameters and responses from the source-generated
  endpoint descriptions, request and response schemas from the application's source-generated
  System.Text.Json contracts (through `JsonSchemaExporter`), tags, summaries and exclusion from the
  Web.Api description verbs, and security requirements from each endpoint's effective
  Web.Authorization policy, the one `UseAuthorization` applies (the fallback policy and named
  policies included).
- **`builder.Services.AddOpenApi(options => ...)`** registers the document: title, API version,
  description, the OpenAPI line (3.1 by default; 3.0 and 3.2 as well), declared security schemes
  and tags, extra endpoint sources, and document transformers. The options are read-only once the
  callback returns. The verb is a component integration the application's compilation receives
  (#1380); this package takes no dependency-injection reference.
- **`MapOpenApi(pattern)`** serves the document from a `GET` route as JSON, or as YAML for a
  `.yaml`/`.yml` pattern. The document is built on the first request, cached, and revalidated with a
  strong `ETag`: a matching `If-None-Match` is answered `304`.
- **`GetOpenApiDescriptionProvider()`** returns the same document as a model, for tools and tests.

## Dependencies

- **`Assimalign.Cohesion.Web`** and **`Assimalign.Cohesion.Web.Routing`** — the builder, the
  application context, and the route table the document describes.
- **`Assimalign.Cohesion.Web.Api`** — the endpoint descriptions and the description verbs
  (`WithTags`, `WithSummary`, `WithDescription`, `ExcludeFromDescription`).
- **`Assimalign.Cohesion.Web.Serialization`** — the registered readers and writers, and
  `TryGetJsonTypeInfo`, the read-only seam to the JSON writer's contracts.
- **`Assimalign.Cohesion.Web.Authorization`** and **`Assimalign.Cohesion.Web.Authentication`** — the
  registered authorization options and each route's effective policy, and the default authenticate
  scheme.
- **`Assimalign.Cohesion.Web.ProblemDetails`** — the RFC 9457 type binding failures are written as.
- **`Assimalign.Cohesion.Http.Forms`** — the uploaded-file types a file parameter's declared type is
  matched against.
- **`Assimalign.Cohesion.Http`** — the HTTP context, status codes, and the conditional-request
  primitive the document endpoint revalidates with.
- **`Assimalign.Cohesion.OpenApi.Integration`**, **`Assimalign.Cohesion.OpenApi.Attributes`** and
  **`Assimalign.Cohesion.OpenApi`** — the endpoint-source contract, the description provider and the
  JSON/YAML exporter; the intermediate metadata records; the document model. Through them come
  generation, serialization, versioning and validation.

It never references `Assimalign.Cohesion.Web.Hosting` or any `Assimalign.Cohesion.Hosting*` library
(the resource hosting-isolation rules `COHRES001` and `COHRES004`), and `Web.Hosting` does not
reference it. It is not a member of the `App.Web` shared framework: an application that does not
document its API carries none of the OpenApi family.

## Usage

See the [source-backed usage examples](examples/index.md).

Register the document on `builder.Services`, beside `AddRouting` and `AddJsonSerialization`, with
`builder.Services.AddOpenApi(options => options.Title = "Orders API")`, and map it in the pipeline:
`app.MapOpenApi()` serves `GET /openapi/v1.json` (OpenAPI 3.1, JSON),
`app.MapOpenApi("/openapi/v1.yaml")` serves the same document as YAML, and
`app.MapOpenApi(pattern, OpenApiSpecVersion.V3_0)` serves a 3.0 rendition beside the default.
`MapOpenApi` returns the route's `IRouterRouteBuilder`, so the document route can carry policies
such as `RequireAuthorization`; requests reach it through `UseRouting`, like every route.

Declare each authentication scheme an endpoint's authorization uses with
`options.AddSecurityScheme(new OpenApiSecuritySchemeMetadata { Name = "Bearer", ... })`, under the
authentication scheme's own name; every endpoint whose effective policy authenticates through it
lists it as a security requirement.

Every type a typed endpoint reads or returns must be in the `JsonSerializerContext` passed to
`AddJsonSerialization`, as it must for the endpoint to serialize it; a missing one fails the
document request with an `InvalidOperationException` naming the endpoint.

See `docs/DESIGN.md` for how each element of the document is derived, the packaging decision, the
schema pipeline, and the security-requirement rules. The [OpenAPI guide](../../../../web/openapi.md)
walks through a complete application.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Http` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.Forms` | `CohesionProjectReference` |
| `Assimalign.Cohesion.OpenApi` | `CohesionProjectReference` |
| `Assimalign.Cohesion.OpenApi.Attributes` | `CohesionProjectReference` |
| `Assimalign.Cohesion.OpenApi.Integration` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Api` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Authentication` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Authorization` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.ProblemDetails` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Routing` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Serialization` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.OpenApi/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.OpenApi/README.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.OpenApi/src/Assimalign.Cohesion.Web.OpenApi.csproj`.
- **Registration and mapping** — `cohesion/resources/Web/Assimalign.Cohesion.Web.OpenApi/src/ComponentModel/OpenApiComponents.cs`, `cohesion/resources/Web/Assimalign.Cohesion.Web.OpenApi/src/Extensions/OpenApiWebApplicationExtensions.cs`, and `cohesion/resources/Web/Assimalign.Cohesion.Web.OpenApi/src/OpenApiOptions.cs`.
