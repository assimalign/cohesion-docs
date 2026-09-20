# Assimalign.Cohesion.Configuration.FileSystem design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Configuration.FileSystem`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Format providers derive from `FileSystemConfigurationProvider` and parse content without duplicating
watch management. Options identify the abstract file system and path and govern optional files,
reload delays, and load-exception handling.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Configuration`, `Assimalign.Cohesion.FileSystem`.
The [overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.FileSystem/src/Assimalign.Cohesion.Configuration.FileSystem.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.FileSystem/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.FileSystem/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.FileSystem/src`.
