# Assimalign.Cohesion.Hosting.Health

Defines transport-neutral health contributions and aggregate report values.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Hosting](../index.md)

## Scope

Health contracts depend only on Core, not on plain Hosting or Hosting.Resources. A contributor
registry and resource-process supervision are outside this package. Shared-framework delivery makes
the contracts available without activating any runtime behavior.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `HealthContribution` | `src/HealthContribution.cs` |
| `HealthStatus` | `src/HealthStatus.cs` |
| `IHealthContributor` | `src/Abstractions/IHealthContributor.cs` |
| `ResourceHealthReport` | `src/ResourceHealthReport.cs` |

## Sources

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Health/src/Assimalign.Cohesion.Hosting.Health.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Health/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Health/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Health/src`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Health/src/HealthContribution.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Health/src/HealthStatus.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Health/src/Abstractions/IHealthContributor.cs`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting.Health/src/ResourceHealthReport.cs`.
