# Assimalign.Cohesion.FileSystem.InMemory design

Design decisions and ownership boundaries for `Assimalign.Cohesion.FileSystem.InMemory`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The provider exposes the same file-system contracts as persistent stores, with a configurable quota
and synchronous change notifications. Its locking coordinates directory operations. Data lifetime is
bounded by the in-process provider rather than durable storage.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.FileSystem`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.InMemory/src/Assimalign.Cohesion.FileSystem.InMemory.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.InMemory/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.InMemory/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/FileSystem/README.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.InMemory/src`.
