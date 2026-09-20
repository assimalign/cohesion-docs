# Assimalign.Cohesion.FileSystem.Physical design

Design decisions and ownership boundaries for `Assimalign.Cohesion.FileSystem.Physical`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The provider delegates file operations to `System.IO` and reports capacity from the drive containing
its root. Change notifications use `FileSystemWatcher`, so latency follows the platform. Rooting and
provider ownership are explicit construction choices.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.FileSystem`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/src/Assimalign.Cohesion.FileSystem.Physical.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/FileSystem/README.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/src`.
