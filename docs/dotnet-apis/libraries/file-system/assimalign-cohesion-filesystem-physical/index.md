# Assimalign.Cohesion.FileSystem.Physical

Provides file-system access rooted in an operating-system directory.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[FileSystem](../index.md)

## Scope

The provider delegates file operations to `System.IO` and reports capacity from the drive containing
its root. Change notifications use `FileSystemWatcher`, so latency follows the platform. Rooting and
provider ownership are explicit construction choices.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.FileSystem`](../../file-system/assimalign-cohesion-filesystem/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `PhysicalFileSystem` | `src/PhysicalFileSystem.cs` |
| `PhysicalFileSystemOptions` | `src/PhysicalFileSystemOptions.cs` |
| `PhysicalFileSystemExtensions` | `src/Extensions/PhysicalFileSystemExtensions.cs` |

## Sources

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/src/Assimalign.Cohesion.FileSystem.Physical.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/FileSystem/README.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/src`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/src/PhysicalFileSystem.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/src/PhysicalFileSystemOptions.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Physical/src/Extensions/PhysicalFileSystemExtensions.cs`.
