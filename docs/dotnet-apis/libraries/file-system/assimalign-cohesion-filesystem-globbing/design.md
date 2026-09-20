# Assimalign.Cohesion.FileSystem.Globbing design

Design decisions and ownership boundaries for `Assimalign.Cohesion.FileSystem.Globbing`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

`GlobMatcherBuilder` separates pattern composition from matching. Matchers operate on Cohesion paths
and file-system entries, so the same rules work across backing stores. Results encapsulate match
bookkeeping instead of exposing backend-specific traversal.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.FileSystem`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Globbing/src/Assimalign.Cohesion.FileSystem.Globbing.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Globbing/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Globbing/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/FileSystem/README.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.Globbing/src`.
