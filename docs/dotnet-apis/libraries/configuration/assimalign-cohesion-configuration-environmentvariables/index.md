# Assimalign.Cohesion.Configuration.EnvironmentVariables

Loads filtered environment variables as configuration entries.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Configuration](../index.md)

## Scope

Provider options describe filtering rather than taking over process-environment ownership. The
provider enumerates values and projects their names onto configuration keys; builder extensions use
the same registration seam as other sources.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Configuration`](../../configuration/assimalign-cohesion-configuration/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `ConfigurationBuilderExtensions` | `src/Extensions/ConfigurationBuilderExtensions.cs` |
| `ConfigurationEnvironmentVariablesOptions` | `src/ConfigurationEnvironmentVariablesOptions.cs` |
| `ConfigurationEnvironmentVariablesProvider` | `src/ConfigurationEnvironmentVariablesProvider.cs` |

## Sources

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.EnvironmentVariables/src/Assimalign.Cohesion.Configuration.EnvironmentVariables.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.EnvironmentVariables/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.EnvironmentVariables/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.EnvironmentVariables/src`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.EnvironmentVariables/src/Extensions/ConfigurationBuilderExtensions.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.EnvironmentVariables/src/ConfigurationEnvironmentVariablesOptions.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.EnvironmentVariables/src/ConfigurationEnvironmentVariablesProvider.cs`.
