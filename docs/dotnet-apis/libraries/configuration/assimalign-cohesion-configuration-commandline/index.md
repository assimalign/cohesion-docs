# Assimalign.Cohesion.Configuration.CommandLine

Loads process arguments into the Cohesion configuration model.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Configuration](../index.md)

## Scope

The options capture arguments and optional switch mappings. Builder extensions register the
provider; argument normalization and key projection stay inside the provider so callers consume
ordinary configuration entries.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Configuration`](../../configuration/assimalign-cohesion-configuration/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `ConfigurationBuilderExtensions` | `src/Extensions/ConfigurationBuilderExtensions.cs` |
| `ConfigurationCommandLineOptions` | `src/ConfigurationCommandLineOptions.cs` |
| `ConfigurationCommandLineProvider` | `src/ConfigurationCommandLineProvider.cs` |

## Sources

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.CommandLine/src/Assimalign.Cohesion.Configuration.CommandLine.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.CommandLine/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.CommandLine/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.CommandLine/src`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.CommandLine/src/Extensions/ConfigurationBuilderExtensions.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.CommandLine/src/ConfigurationCommandLineOptions.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.CommandLine/src/ConfigurationCommandLineProvider.cs`.
