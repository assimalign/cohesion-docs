# Assimalign.Cohesion.Database.Testing

`Assimalign.Cohesion.Database.Testing` runs an SDK-enabled Database resource's real entry point in the test process.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

## Purpose

`Assimalign.Cohesion.Database.Testing` runs an SDK-enabled `Database` resource's real entry point in
the test process. It installs a test-owned `Hosting.Resources` `ResourceContext`, invokes
`Program.Main`, waits until the `Database` default control plane reports ready, and shuts the
resource down through its public graceful-stop endpoint. A test therefore exercises the same
composition in `Program.cs` —engine, schema/provisioning, servers, generated control plane, and host
lifecycle—that a gateway launches in another process or container.

## Public surface

- **`IDatabaseApplicationTestFactory`** — the interface-first lifecycle and context contract.
- **`DatabaseApplicationTestFactory`** — the sealed implementation and
  `FromProgram<TProgram>()` entry point.
- **`DatabaseApplicationTestFactoryOptions`** — custom context, entry arguments, startup and
  shutdown budgets, and probe cadence.

The package intentionally carries no xUnit, NUnit, MSTest, or assertion dependency. It hosts the
resource; callers choose their own fixture and assertion conventions.

## Default context

When no context is supplied, a factory creates:

| Input | Default test value |
| --- | --- |
| `Application` | `tests` |
| Environment | `Testing` |
| Gateway | `inprocess` |
| `db` endpoint | Unique loopback port, `cohesion-db` scheme |
| `admin` endpoint | Unique loopback port, `http` scheme |
| `data` mount | Unique temporary directory, removed on disposal |

The loopback ports are allocated independently per factory, and the context is carried by the
`Hosting.Resources` `ResourceRuntime` asynchronous invocation scope rather than process environment
variables. Tests that need deterministic ports or additional generated inputs can pass a complete
`ResourceContext` through the options object. A custom context must include the `admin` endpoint
because readiness and graceful stop are part of the factory contract.

## Usage

See the [source-backed usage examples](examples/index.md).

The context exposes endpoints and references as validated `System.Uri` values. Pass those values
directly to `DatabaseConnectionSettings.For(Uri)` for clients or
`SqlDatabaseServerOptions.Listen(Uri)` for server binding; neither path requires string
reconstruction.

The executable project must have `CohesionApplicationModel=enabled` so its SDK-generated module
initializer registers the `Database` default control plane. A top-level program adds
`public partial class Program { }` so another assembly can use it as the statically rooted generic
marker.

The real-process acceptance harness verifies discovery of the two `Database` command kinds,
`database.add-database` and `database.add-principal`, alongside the existing readiness and
graceful-stop routes. `Principal` mutation currently returns an explicit provider refusal.

## Relationships

- **`Assimalign.Cohesion.Hosting`** — owns the plain host lifecycle.
- **`Assimalign.Cohesion.Hosting.Resources`** — owns `ResourceContext`, the scoped runtime carrier,
  resource entry registration, and the host/control-plane bridge.
- **`Assimalign.Cohesion.Database.Hosting`** — owns `DatabaseApplication.CreateBuilder(args)` and
  serves the private admin endpoint the factory probes.
- **`Assimalign.Cohesion.Database.ApplicationModel`** — is injected into enabled resource
  executables by `Sdk.Database`; its generated registration supplies the `Database` default
  control plane. It is not a dependency of this testing library.

See [`DESIGN.md`](design.md) for lifecycle, entry-point, isolation, and AOT details.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Health` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Core` | `CohesionProjectReference` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Testing/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Testing/src/Assimalign.Cohesion.Database.Testing.csproj`.
