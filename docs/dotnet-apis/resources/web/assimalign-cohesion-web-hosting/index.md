# Assimalign.Cohesion.Web.Hosting

The Web runtime composes a concrete `WebApplication : Host<WebApplicationContext>` through `WebApplication.CreateBuilder(args)`.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

The Web runtime composes a concrete `WebApplication : Host<WebApplicationContext>` through
`WebApplication.CreateBuilder(args)`. It implements the root application and builder contracts and
integrates DI, configuration, logging, HTTP transports, and the enabled resource's control plane at
builder time.

## Composition and lifecycle

Feature verbs extend the root `IWebApplicationBuilder`, whose members this module implements as
explicit shims over `WebApplicationBuilder.Services` registrations (`IHttpFeature`,
`IWebApplicationServer`). Background work is registered through the concrete
`WebApplicationBuilder.AddService` instance or context-factory overload, which registers an
`IHostService`. `Build()` closes registration and runs each service factory once; services start in
registration order before servers and stop in reverse order after every server drains. Disposing the
application disposes the service provider and every factory-created service.

## Dependencies and hosting family

The module references the Web root and its own hosting family within its area (COHRES002), together
with Cohesion's hosting, configuration, DI, logging, and transport infrastructure. Its
`Hosting.Resources` and `Hosting.Health` integrations are runtime concerns; the Web root references
no hosting library. The reusable `Web.Hosting.Resources` and `Web.Hosting.Health` packages do not
reference this module; it consumes `Web.Hosting.Resources` for the enabled resource's control-plane
terminal.

All public composition is explicit and AOT-compatible. See [Design](design.md) for listener
ownership, cancellation, failure isolation, and control-plane behavior.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Hosting.Resources` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Health` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Telemetry` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Logging` | `CohesionProjectReference` |
| `Assimalign.Cohesion.DependencyInjection` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Configuration` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Configuration.CommandLine` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Configuration.EnvironmentVariables` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Configuration.Json` | `CohesionProjectReference` |
| `Assimalign.Cohesion.FileSystem` | `CohesionProjectReference` |
| `Assimalign.Cohesion.FileSystem.Physical` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.Connections` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.RequestLimits` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections.Tcp` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections.Quic` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections.Security` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Hosting/src/Assimalign.Cohesion.Web.Hosting.csproj`.
