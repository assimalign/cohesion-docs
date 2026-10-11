# Assimalign.Cohesion.Hosting.Resources

Adds opt-in resource context and supervisor behavior to plain hosts.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Hosting](../index.md)

## Scope

Resources composes the Hosting run seam and Health vocabulary without reverse dependencies. Resource
SDKs opt into generated `ResourceRuntime` calls; assembly presence alone does not activate
supervision. Invocation context owns environment snapshots, mounts, control planes, and health
aggregation.

The control plane's health report never carries a contributor's exception (#1409). A
contributor that throws becomes an `Unhealthy` contribution described by the fixed text
`The contributor threw an exception.`, and an `Exception` value a contributor places in
`HealthContribution.Data` is replaced by `The contributor reported an exception.`. Area hosts
publish the report on the unauthenticated `/healthz`, `/readyz` and `/livez` probes, and an
exception message routinely names hosts, databases, users and ports. A contributor that wants its
failure diagnosed records it itself. See the [design](design.md#health-reports-withhold-exceptions).

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Hosting`](../../hosting/assimalign-cohesion-hosting/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Hosting.Health`](../../hosting/assimalign-cohesion-hosting-health/index.md) | `CohesionProjectReference` |
| `System.Security.Cryptography.ProtectedData` | `CohesionPackageReference` |

## Principal public types

| Type | Source file |
|---|---|
| `IResourceCommandHandler` | `src/Abstractions/IResourceCommandHandler.cs` |
| `IResourceControlPlane` | `src/Abstractions/IResourceControlPlane.cs` |
| `IResourceEntryInvocation` | `src/Abstractions/IResourceEntryInvocation.cs` |
| `ResourceCommand` | `src/ResourceCommand.cs` |
| `ResourceContext` | `src/ResourceContext.cs` |
| `ResourceControlPlane` | `src/ResourceControlPlane.cs` |
| `ResourceEntryExitException` | `src/Exceptions/ResourceEntryExitException.cs` |
| `ResourceMount` | `src/ResourceMount.cs` |
| `ResourceRuntime` | `src/ResourceRuntime.cs` |
| `ResourceCommandRejectedException` | `src/Exceptions/ResourceCommandRejectedException.cs` |

## Sources

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/src/Assimalign.Cohesion.Hosting.Resources.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/src`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/src/Abstractions/IResourceCommandHandler.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/src/Abstractions/IResourceControlPlane.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/src/Abstractions/IResourceEntryInvocation.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/src/ResourceCommand.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/src/ResourceContext.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/src/ResourceControlPlane.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/src/Exceptions/ResourceEntryExitException.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/src/ResourceMount.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/src/ResourceRuntime.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Resources/src/Exceptions/ResourceCommandRejectedException.cs`.
