# Assimalign.Cohesion.FileSystem

Defines a common file-system contract and named provider composition.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

- **[`IFileSystemFile`](i-file-system-file.md)** — type reference.

- **[`IFileSystemFileHandle`](i-file-system-file-handle.md)** — type reference.

[FileSystem](../index.md)

## Scope

Interface contracts allow different backing stores to expose files, directories, enumeration, and
events. The factory composes providers by name. Shared provider contract tests define the portable
behavior; provider-specific watch timing remains documented separately.

A provider rooted somewhere confines every path-taking operation to that root (#1180). The path is
resolved first — an empty path is the root, a relative path is taken from the root, and `.` and `..`
segments are resolved — and must then equal the root or lie under it on a segment boundary, so
`/srv/public2/x` does not lie under `/srv/public`. Anything else throws `FileSystemException` with
`FileSystemErrorCode.PathOutsideRoot` before the backing store is touched; `Exists` throws too rather
than answering `false`. The physical and in-memory providers enforce this, the isolated-storage
provider does not yet, and the aggregate provider delegates to its mounts. See the
[design](design.md#root-containment).

`FileSystemPath.Merge` navigates; it does not confine. It joins a relative path onto a base and
applies leading `..` segments, which may climb above the base but never above its root (drive,
leading `/`, or UNC share), so no provider uses it to resolve incoming paths.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `FileSystemErrorCode` | `src/Exceptions/FileSystemErrorCode.cs` |
| `FileSystemEvent` | `src/FileSystemEvent.cs` |
| `FileSystemEvent<T>` | `src/FileSystemEvent.T.cs` |
| `FileSystemException` | `src/Exceptions/FileSystemException.cs` |
| `FileSystemFactory` | `src/FileSystemFactory.cs` |
| `FileSystemFactoryBuilder` | `src/FileSystemFactoryBuilder.cs` |
| `IFileSystem` | `src/Abstractions/IFileSystem.cs` |
| `IFileSystemDirectory` | `src/Abstractions/IFileSystemDirectory.cs` |
| `IFileSystemEventToken` | `src/Abstractions/IFileSystemEventToken.cs` |
| `IFileSystemFactory` | `src/Abstractions/IFileSystemFactory.cs` |
| `IFileSystemFile` | `src/Abstractions/IFileSystemFile.cs` |
| `IFileSystemFileHandle` | `src/Abstractions/IFileSystemFileHandle.cs` |
| `IFileSystemInfo` | `src/Abstractions/IFileSystemInfo.cs` |
| `FileSystemEnumerationOptions` | `src/FileSystemEnumerationOptions.cs` |
| `FileSystemEventType` | `src/FileSystemEventType.cs` |
| `FileSystemExtensions` | `src/Extensions/FileSystemExtensions.cs` |
| `FileSystemExceptionExtensions` | `src/Extensions/FileSystemExceptionExtensions.cs` |

## Sources

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/Assimalign.Cohesion.FileSystem.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/FileSystem/README.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/Exceptions/FileSystemErrorCode.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/FileSystemEvent.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/FileSystemEvent.T.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/Exceptions/FileSystemException.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/FileSystemFactory.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/FileSystemFactoryBuilder.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/Abstractions/IFileSystem.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/Abstractions/IFileSystemDirectory.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/Abstractions/IFileSystemEventToken.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/Abstractions/IFileSystemFactory.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/Abstractions/IFileSystemFile.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/Abstractions/IFileSystemFileHandle.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/Abstractions/IFileSystemInfo.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/FileSystemEnumerationOptions.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/FileSystemEventType.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/Extensions/FileSystemExtensions.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/Extensions/FileSystemExceptionExtensions.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/docs/COMPATIBILITY.md`.
