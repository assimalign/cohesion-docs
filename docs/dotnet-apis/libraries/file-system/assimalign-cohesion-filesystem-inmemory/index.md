# Assimalign.Cohesion.FileSystem.InMemory

Stores files in managed memory for tests and temporary storage.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[FileSystem](../index.md)

## Scope

The provider exposes the same file-system contracts as persistent stores, with a configurable quota
and synchronous change notifications. Its locking coordinates directory operations. Data lifetime is
bounded by the in-process provider rather than durable storage.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.FileSystem`](../../file-system/assimalign-cohesion-filesystem/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `InMemoryFileSystem` | `src/InMemoryFileSystem.cs` |
| `InMemoryFileSystemOptions` | `src/InMemoryFileSystemOptions.cs` |
| `InMemoryFileSystemExtensions` | `src/Extensions/InMemoryFileSystemExtensions.cs` |
| `InMemoryFileSystemLockHandle` | `src/InMemoryFileSystemLockHandle.cs` |

## Sources

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.InMemory/src/Assimalign.Cohesion.FileSystem.InMemory.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.InMemory/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.InMemory/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/FileSystem/README.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.InMemory/src`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.InMemory/src/InMemoryFileSystem.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.InMemory/src/InMemoryFileSystemOptions.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.InMemory/src/Extensions/InMemoryFileSystemExtensions.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.InMemory/src/InMemoryFileSystemLockHandle.cs`.
