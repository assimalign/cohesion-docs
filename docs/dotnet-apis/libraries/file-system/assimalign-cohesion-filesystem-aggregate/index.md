# Assimalign.Cohesion.FileSystem.Aggregate

Routes a virtual file-system tree across mounted providers.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[FileSystem](../index.md)

## Scope

Routing selects the longest matching mount prefix. Paths above mounts appear as synthetic read-only
directories, and watch events are remapped from provider paths into aggregate paths. Mount ownership
controls which providers are disposed with the aggregate.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.FileSystem`](../../file-system/assimalign-cohesion-filesystem/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `AggregateFileSystem` | `src/AggregateFileSystem.cs` |
| `AggregateFileSystemBuilder` | `src/AggregateFileSystemBuilder.cs` |
| `AggregateFileSystemOptions` | `src/AggregateFileSystemOptions.cs` |
| `AggregateFileSystemExtensions` | `src/Extensions/AggregateFileSystemExtensions.cs` |

## Sources

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Aggregate/src/Assimalign.Cohesion.FileSystem.Aggregate.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Aggregate/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Aggregate/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/FileSystem/README.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Aggregate/src`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Aggregate/src/AggregateFileSystem.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Aggregate/src/AggregateFileSystemBuilder.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Aggregate/src/AggregateFileSystemOptions.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Aggregate/src/Extensions/AggregateFileSystemExtensions.cs`.
