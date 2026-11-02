# Assimalign.Cohesion.Database.Hosting

`DatabaseApplication.CreateBuilder(args)` composes a complete Database host through dependency-free model intent, one-shot Build, then ordinary engine access or the Hosting Run extensions.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

`DatabaseApplication.CreateBuilder(args)` composes a complete `Database` host through dependency-free
model intent, one-shot `Build`, then ordinary engine access or the Hosting `Run` extensions. Engines are
operational at `Build`; optional nested servers begin listening at Start.

See the [source-backed usage examples](examples/index.md).

Model verbs are `AddSql`, `AddDocuments`, `AddGraph`, `AddKeyValue`, and `AddBlob`; they return
the application builder and ship with their model package. Servers and workers are registered with
deferred factories on the model engine builder. There is no application-level `AddServer` or `Use`
composition method. Direct model `Engine.Create(options)` remains available for standalone use.

The concrete hosting builder supplies `Environment`, `Configuration`, `Logging`, `Services`, and
build-aware `AddEngine(name, factory)`. Each model's engine builder declares and provisions its
databases inside application Build. Lifecycle services come from `AddService` and `IHostService`
registrations on `builder.Services`. Start order is `AddService` registrations, container services,
the optional reopen service, then servers. Each group preserves registration order; stop reverses
the complete order, draining servers first. The root interfaces expose no configuration or DI types.

Instance engines and services remain caller-owned. Factory products belong to the application;
engines own their nested servers/workers. `Application` disposal stops the host, disposes its owned
services and engines, then its provider, logger factory, configuration and configuration file system.
Container factory products belong to the provider, with no second application ownership; container
instance registrations remain borrowed. Each application supports one start lifecycle.

Configuration loads optional base/environment JSON, `COHESION_CONFIG__` environment variables,
captured args and explicit providers. Services use the Cohesion interpreted resolver
(`EnableDynamicCode = false`) on both JIT and NativeAOT, with scope validation and explicit closed
factories/instances. See [DESIGN.md](design.md) for ordering, rollback, isolation and ownership
details.

The runtime references the area root and non-area infrastructure, never Database model packages.
[Database.Hosting.ApplicationModel](../assimalign-cohesion-database-hosting-applicationmodel/index.md)
makes a program managed with `builder.Services.AddResource(...)`. Its lifecycle service publishes
and restores during `StartingAsync`, after Build has opened, recovered, and migrated the engines,
before any service or server starts. It opens the platform client in `StartAsync`; servers drain
before it closes on stop. The gateway's startup window must cover Build's inline work.

When composed, the internal admin health listener accepts namespaced health probes only from transport
peers at `127.0.0.1`, `::1`, or IPv4-mapped `127.0.0.1`; other peers receive 403 regardless of
forwarded headers. This restricts remote standalone access supported by 10.0.0-preview.1 (#1431).
Bare `/healthz`, `/readyz`, and `/livez` stay open. Other `/cohesion/v1` routes return 403 even with
credentials. Managed action calls travel through the platform client. See [admin behavior](design.md#standalone-admin-control-plane-1431).

The model server options take `IDuplexConnectionListener`; client composition supplies an `IDuplexConnectionFactory` for the shared database pool.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database` | `CohesionProjectReference` |
| `Assimalign.Cohesion.DependencyInjection` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Configuration` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Configuration.Json` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Configuration.EnvironmentVariables` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Configuration.CommandLine` | `CohesionProjectReference` |
| `Assimalign.Cohesion.FileSystem.Physical` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.ApplicationModel.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Logging` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Hosting` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Web.Health` | `CohesionPrivateProjectReference` |

[Parent: Database](../index.md)

## Health builder dependency

[HealthChecksBuilder](health-checks-builder.md) documents the public constructor used by Database admin health composition.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting/src/Assimalign.Cohesion.Database.Hosting.csproj`.
