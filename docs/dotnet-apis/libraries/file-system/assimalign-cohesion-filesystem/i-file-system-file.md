# IFileSystemFile

Namespace: `Assimalign.Cohesion.FileSystem` Assembly: `Assimalign.Cohesion.FileSystem`

[Assembly overview](index.md) · [Design](design.md) · [Examples](examples/index.md)

Namespace: `Assimalign.Cohesion.FileSystem`.

Assembly: `Assimalign.Cohesion.FileSystem`.

## Remarks

Namespace: `Assimalign.Cohesion.FileSystem`  
Assembly: `Assimalign.Cohesion.FileSystem`

Represents a file returned by `IFileSystem.GetFile` or `IFileSystem.CreateFile`. Extends
`IFileSystemInfo` with `Size`, `Name`, `Directory`, `Watch()`, and the existing `Open`
stream overloads. There is no public constructor.

## OpenHandle

Opens the file for positional reads and writes, length control, and durability-aware flush.
`fileMode` controls opening or creation, `fileAccess` selects reading and/or writing, and
`fileShare` controls other opens. The returned
[`IFileSystemFileHandle`](i-file-system-file-handle.md) belongs to the caller and must
be disposed with `using` or `await using`.

Providers enforce their file modes, access, sharing, and read-only rules. Opening can fail
when the file cannot be opened with those settings. Operations on the handle expose argument,
access, I/O, cancellation, and disposal errors. In particular, a handle whose
`SupportsDurableFlush` is `false` throws `NotSupportedException` on durable flush, so a
storage engine cannot silently lose a durability guarantee.

## Existing stream surface

`Open()`, `Open(FileMode)`, `Open(FileMode, FileAccess)`, and
`Open(FileMode, FileAccess, FileShare)` continue returning caller-owned `Stream` instances
with unchanged behavior. Sequential Configuration and Web.StaticFiles consumers continue to
use those methods without changes.

## Members

| Member | Responsibility |
|---|---|
| `Size` | File size; the source contract documents a sentinel for a nonexistent file. |
| `Name` | File name as `FileName`. |
| `Directory` | Containing directory. |
| `Watch()` | Registers access to file change notifications. |
| `Open()` | Opens a read stream owned by the caller. |
| `Open(FileMode)` | Opens with an explicit creation/opening mode. |
| `Open(FileMode, FileAccess)` | Adds explicit access selection. |
| `Open(FileMode, FileAccess, FileShare)` | Adds explicit sharing selection. |
| `OpenHandle(FileMode, FileAccess, FileShare)` | Returns a caller-owned random-access handle. |

## Sources

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/docs/Assembly/Assimalign.Cohesion.FileSystem/IFileSystemFile/OVERVIEW.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/Abstractions/IFileSystemFile.cs`.
