# Assimalign.Cohesion.FileSystem design

Design decisions and ownership boundaries for `Assimalign.Cohesion.FileSystem`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Interface contracts allow different backing stores to expose files, directories, enumeration, and
events. The factory composes providers by name. Shared provider contract tests define the portable
behavior; provider-specific watch timing remains documented separately.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src/Assimalign.Cohesion.FileSystem.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/FileSystem/README.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem/src`.
