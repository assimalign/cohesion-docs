# Assimalign.Cohesion.Configuration.FileSystem

Shares file access, watching, and reload behavior between configuration formats.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Configuration](../index.md)

## Scope

Format providers derive from `FileSystemConfigurationProvider` and parse content without duplicating
watch management. Options identify the abstract file system and path and govern optional files,
reload delays, and load-exception handling.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Configuration`](../../configuration/assimalign-cohesion-configuration/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.FileSystem`](../../file-system/assimalign-cohesion-filesystem/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `ConfigurationBuilderExtensions` | `src/Extensions/ConfigurationBuilderExtensions.cs` |
| `ConfigurationFileLoadExceptionContext` | `src/ConfigurationFileLoadExceptionContext.cs` |
| `FileSystemConfigurationOptions` | `src/FileSystemConfigurationOptions.cs` |
| `FileSystemConfigurationProvider` | `src/FileSystemConfigurationProvider.cs` |

## Sources

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.FileSystem/src/Assimalign.Cohesion.Configuration.FileSystem.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.FileSystem/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.FileSystem/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.FileSystem/src`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.FileSystem/src/Extensions/ConfigurationBuilderExtensions.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.FileSystem/src/ConfigurationFileLoadExceptionContext.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.FileSystem/src/FileSystemConfigurationOptions.cs`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.FileSystem/src/FileSystemConfigurationProvider.cs`.
