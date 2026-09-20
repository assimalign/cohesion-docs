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
