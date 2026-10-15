# Assimalign.Cohesion.Configuration.Json

Loads JSON streams and files into configuration entries.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Configuration](../index.md)

## Scope

Object members and array elements become composite configuration paths. The file provider reuses the
shared file-system lifecycle, while stream loading is a separate entry point. Registration remains
on `ConfigurationBuilderExtensions`.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Configuration.FileSystem`](../../configuration/assimalign-cohesion-configuration-filesystem/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `ConfigurationBuilderExtensions` | `src/Extensions/ConfigurationBuilderExtensions.cs` |
| `ConfigurationJsonOptions` | `src/ConfigurationJsonOptions.cs` |
| `ConfigurationJsonProvider` | `src/ConfigurationJsonProvider.cs` |
| `ConfigurationJsonStreamProvider` | `src/ConfigurationJsonStreamProvider.cs` |

## Sources

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Json/src/Assimalign.Cohesion.Configuration.Json.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Json/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Json/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Json/src`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Json/src/Extensions/ConfigurationBuilderExtensions.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Json/src/ConfigurationJsonOptions.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Json/src/ConfigurationJsonProvider.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Json/src/ConfigurationJsonStreamProvider.cs`.
