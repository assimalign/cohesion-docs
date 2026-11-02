# Assimalign.Cohesion.IdentityHub.Hosting

The IdentityHub runtime module: `IdentityHubApplication.CreateBuilder(args)`, the concrete host, and its services.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`IdentityHubApplication`](identity-hub-application.md)** — Documented public type.

## Summary

Provides `IdentityHubApplication.CreateBuilder(args)` and the public concrete application, builder,
and context for the AOT-safe OpenID Connect issuer runtime. Issuer services remain internal.

The host serves discovery, persisted ES256 JWKS, client-credentials tokens, and public health
probes. Configuration is code-first through `AddAudience` and `AddClient`. Only `openid` and an
empty scope are accepted.

The area's resource integration is `Assimalign.Cohesion.IdentityHub.Hosting.ApplicationModel`, which
references this module; this module references no application-model library.

Device authorization and its fixed-subject browser approval page are exposed only by a loopback
Local endpoint. Applications needing account login, subject selection, consent, recovery, or
federation compose an authenticated user-facing flow separately.

Self-signed TLS is limited to loopback Local; non-Local HTTPS fails closed rather than generating
production TLS material.

## Concrete composition (T10 / O34)

`IdentityHubApplication.CreateBuilder(args)` returns the public concrete
`IdentityHubApplicationBuilder`; its `Build()` returns the public `IdentityHubApplication :
Host<IdentityHubApplicationContext>`. The public `IdentityHubApplicationContext` implements
`IIdentityHubApplicationContext`, reading `ContentRootPath` from the host environment. The
application explicitly forwards the root lifecycle contract to `IHost`, and consumers use the
concrete application for `RunAsync` and `await using`. Runtime options and supporting services
remain internal.

Background-work registration belongs to the concrete `IdentityHubApplicationBuilder`:
`AddService(IHostService)` and `AddService(Func<IdentityHubApplicationContext, IHostService>)`. The
factory receives the same concrete context as Web's and Database's AddService, so hosting consumers
can use environment, state, and hosted-service members beyond the small root contract. Database's
root-level AddServer keeps the interface context. Factories run once per build against the same
context retained by the application; the hosted-service snapshot is installed after factory
evaluation. Services start in registration order and stop in reverse. No area-owned service
abstraction is introduced.

The base host owns the already-cancelled run semantic: one complete start and graceful stop with
fresh lifecycle tokens, normal run-observer notifications, and a final Stopped state. The concrete
application and IHost route share it. Startup failures still roll back and propagate.

## Dependency injection composition

`IdentityHubApplicationBuilder.Services` holds every registration the application runs with.
`AddService` registers an `IHostService`, and a direct `IHostService` registration joins the
lifecycle in the same order. `Build` closes registration and resolves the services once. Disposing
the application disposes every factory-created service. See
[DESIGN.md](design.md#dependency-injection-composition).

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.IdentityHub` | `CohesionProjectReference` |
| `Assimalign.Cohesion.DependencyInjection` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Connections.Security` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Connections.Tcp` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Http` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Http.Connections` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Http.Forms` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.IdentityModel` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.IdentityModel.Token` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Web` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Web.Hosting` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Web.SecurityHeaders` | `CohesionPrivateProjectReference` |

The area's resource integration, [`Assimalign.Cohesion.IdentityHub.Hosting.ApplicationModel`](../assimalign-cohesion-identityhub-hosting-applicationmodel/index.md), references this module and registers the resource service in its `Services`.

[Parent: IdentityHub](../index.md)

## Sources

- **Primary source** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Hosting/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Hosting/src/Assimalign.Cohesion.IdentityHub.Hosting.csproj`.
