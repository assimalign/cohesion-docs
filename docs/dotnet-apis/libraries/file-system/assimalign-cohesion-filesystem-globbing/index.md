# Assimalign.Cohesion.FileSystem.Globbing

Matches include and exclude glob patterns over file-system abstractions.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[FileSystem](../index.md)

## Scope

`GlobMatcherBuilder` separates pattern composition from matching. Matchers operate on Cohesion paths
and file-system entries, so the same rules work across backing stores. Results encapsulate match
bookkeeping instead of exposing backend-specific traversal.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.FileSystem`](../../file-system/assimalign-cohesion-filesystem/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `GlobMatcherBuilder` | `src/GlobMatcherBuilder.cs` |
| `GlobMatcherOptions` | `src/GlobMatcherOptions.cs` |
| `GlobMatchResults` | `src/GlobMatchResults.cs` |
| `IGlobContext` | `src/Abstractions/IGlobContext.cs` |
| `IGlobMatcher` | `src/Abstractions/IGlobMatcher.cs` |
| `IGlobMatcherBuilder` | `src/Abstractions/IGlobMatcherBuilder.cs` |

## Sources

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Globbing/src/Assimalign.Cohesion.FileSystem.Globbing.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Globbing/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Globbing/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/FileSystem/README.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Globbing/src`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Globbing/src/GlobMatcherBuilder.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Globbing/src/GlobMatcherOptions.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Globbing/src/GlobMatchResults.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Globbing/src/Abstractions/IGlobContext.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Globbing/src/Abstractions/IGlobMatcher.cs`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Globbing/src/Abstractions/IGlobMatcherBuilder.cs`.
