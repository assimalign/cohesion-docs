# Assimalign.Cohesion.FileSystem.InMemory design

Design decisions and ownership boundaries for `Assimalign.Cohesion.FileSystem.InMemory`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The provider exposes the same file-system contracts as persistent stores, with a configurable quota
and synchronous change notifications. Its locking coordinates directory operations. Data lifetime is
bounded by the in-process provider rather than durable storage.

## Root containment

Every path-taking operation resolves its path before it walks the tree and refuses it with
`FileSystemException` (`FileSystemErrorCode.PathOutsideRoot`) unless it is the root or lies under it
on a segment boundary — the family rule in the
[FileSystem design](../assimalign-cohesion-filesystem/design.md#root-containment) (#1180).

- **Resolution is lexical.** A relative path is taken from the root; `.` and `..` are resolved, and
  `..` at the namespace root stays there (`/..` is `/`), so with the default root of `/` a leading
  `..` cannot leave it.
- **The comparison is ordinal**, ignoring case when `IgnoreCase` is set. `CultureInfo` keeps
  governing entry lookups only, because a culture-aware prefix match cannot decide a segment
  boundary.
- **The resolved path is rebuilt from the root's own text**, so walks start from the stored root.

Until #1180 the provider resolved paths with `FileSystemPath.Merge`, which confines nothing. Nothing
outside an in-memory tree can leak, but with a root such as `/data` the sibling-prefix path `/datax`
aliased the entry `/data/x` (`Exists("/datax")` was `true`), and moving through that alias
deadlocked, because the walk shared-locked the entry it then tried to lock exclusively, while a
leading `..` resolved to unrelated entries. A path no longer than the root is now the root itself,
which also fixes resolving the parent of a root-level entry under any root other than `/`.

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

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.InMemory/tests/InMemoryFileSystemContainmentTests.cs`.
