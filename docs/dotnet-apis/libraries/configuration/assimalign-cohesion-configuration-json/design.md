# Assimalign.Cohesion.Configuration.Json design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Configuration.Json`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Object members and array elements become composite configuration paths. The file provider reuses the
shared file-system lifecycle, while stream loading is a separate entry point. Registration remains
on `ConfigurationBuilderExtensions`.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Configuration.FileSystem`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Json/src/Assimalign.Cohesion.Configuration.Json.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Json/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Json/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.Json/src`.
