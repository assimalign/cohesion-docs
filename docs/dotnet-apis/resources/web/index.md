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
| [`Assimalign.Cohesion.Web.Hosting.Health`](assimalign-cohesion-web-hosting-health/index.md) | Hosting integration | Public reference and runtime |
| [`Assimalign.Cohesion.Web.Hosting.Resources`](assimalign-cohesion-web-hosting-resources/index.md) | Hosting integration | Public reference and runtime |
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
| [`Assimalign.Cohesion.Web.Sessions`](assimalign-cohesion-web-sessions/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.StaticFiles`](assimalign-cohesion-web-staticfiles/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.Testing`](assimalign-cohesion-web-testing/index.md) | Testing | Not listed in this framework |
| [`Assimalign.Cohesion.Web.Validation`](assimalign-cohesion-web-validation/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Web.WebSockets`](assimalign-cohesion-web-websockets/index.md) | Feature library | Public reference and runtime |

## Architecture

`Web.Hosting.Resources` owns reusable resource-management middleware, including
authentication and ahead-of-time (AOT) compilation-compatible JSON handling. Area hosts own
their listeners, readiness gates, and lifecycle. The integration is public in
`App.Web` and private in generic resource frameworks. `Web.Hosting.Health`
adapts hosting health contributors into the independent `Web.Health` model.

## Dependency boundary

The single runtime module is `Assimalign.Cohesion.Web.Hosting`. Roots and feature libraries do not
depend on the hosting family or on `Assimalign.Cohesion.Hosting` and its child libraries. Feature
registration composes against the root contracts. The exact runtime may reference its area root and
its hosting-family integrations; integrations may not reference the exact runtime. The declarative
application model remains separate from the runtime. See the
[resource dependency rules](../index.md#dependency-rules) .

The hosting family in this area contains `Assimalign.Cohesion.Web.Hosting`,
`Assimalign.Cohesion.Web.Hosting.Health`, `Assimalign.Cohesion.Web.Hosting.Resources`.

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
| `Assimalign.Cohesion.Web.Hosting.Health` |
| `Assimalign.Cohesion.Web.Hosting.Resources` |
| `Assimalign.Cohesion.Web.HttpsPolicy` |
| `Assimalign.Cohesion.Web.ProblemDetails` |
| `Assimalign.Cohesion.Web.Query` |
| `Assimalign.Cohesion.Web.RateLimiting` |
| `Assimalign.Cohesion.Web.RequestTimeouts` |
| `Assimalign.Cohesion.Web.Rewrite` |
| `Assimalign.Cohesion.Web.Routing` |
| `Assimalign.Cohesion.Web.SecurityHeaders` |
| `Assimalign.Cohesion.Web.Serialization` |
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
| `Assimalign.Cohesion.Http.Forms` |
| `Assimalign.Cohesion.Http.Forwarded` |
| `Assimalign.Cohesion.Http.ProtocolUpgrade` |
| `Assimalign.Cohesion.Http.RequestLimits` |
| `Assimalign.Cohesion.Http.Sessions` |
| `Assimalign.Cohesion.Http.WebSockets` |
| `Assimalign.Cohesion.ObjectValidation` |
| `Assimalign.Cohesion.Http.ExtendedConnect` |
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

## Related documentation

- **Product** — [Web](../../../web/index.md).
- **Middleware order** — [the registration order for every Web middleware](../../../web/middleware-order.md).
- **OpenAPI** — [serving and describing an API with Web.OpenApi](../../../web/openapi.md).
- **WebSockets** — [socket endpoints on every protocol, the origin policy, and the drain close](../../../web/websockets.md).
- **URL rewriting** — [rewrite and redirect rules ahead of routing](../../../web/rewrite.md).
- **Observability** — [the server's request spans, HTTP metrics, and request id](../../../web/observability.md).
- **SDK** — [`Assimalign.Cohesion.Sdk.Web`](../../sdks/sdk-web/index.md).
- **Parent** — [Resources](../index.md).

## Sources

- **Primary source** — `cohesion/resources/Web/README.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/libraries/App/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Refs/src/Assimalign.Cohesion.Web.Refs.csproj` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Runtime/Directory.Build.props`.
- **Architecture source** — `cohesion/docs/resources/Web/DESIGN.md`.
- **Architecture overview** — `cohesion/docs/resources/Web/OVERVIEW.md`.
