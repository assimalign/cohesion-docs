# Assimalign.Cohesion.FileSystem.IsolatedStorage

Provides file-system access backed by .NET isolated storage.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[FileSystem](../index.md)

## Scope

The runtime chooses the storage location, while provider options control store lifetime and polling.
Watches are polling-based; rename registrations are accepted but do not fire. `RemoveStoreOnDispose`
is an explicit cleanup choice.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.FileSystem`](../../file-system/assimalign-cohesion-filesystem/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `IsolatedStorageFileSystem` | `src/IsolatedStorageFileSystem.cs` |
| `IsolatedStorageFileSystemOptions` | `src/IsolatedStorageFileSystemOptions.cs` |
| `IsolatedStorageFileSystemExtensions` | `src/Extensions/IsolatedStorageFileSystemExtensions.cs` |

## Sources

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.IsolatedStorage/src/Assimalign.Cohesion.FileSystem.IsolatedStorage.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.IsolatedStorage/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.IsolatedStorage/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/FileSystem/README.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.IsolatedStorage/src`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.IsolatedStorage/src/IsolatedStorageFileSystem.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.IsolatedStorage/src/IsolatedStorageFileSystemOptions.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.IsolatedStorage/src/Extensions/IsolatedStorageFileSystemExtensions.cs`.
