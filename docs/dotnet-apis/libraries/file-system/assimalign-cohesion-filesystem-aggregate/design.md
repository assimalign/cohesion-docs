# Assimalign.Cohesion.FileSystem.Aggregate design

Design decisions and ownership boundaries for `Assimalign.Cohesion.FileSystem.Aggregate`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Routing selects the longest matching mount prefix. Paths above mounts appear as synthetic read-only
directories, and watch events are remapped from provider paths into aggregate paths. Mount ownership
controls which providers are disposed with the aggregate.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.FileSystem`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Aggregate/src/Assimalign.Cohesion.FileSystem.Aggregate.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Aggregate/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Aggregate/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/FileSystem/README.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Aggregate/src`.
