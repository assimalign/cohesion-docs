# Assimalign.Cohesion.FileSystem.IsolatedStorage design

Design decisions and ownership boundaries for `Assimalign.Cohesion.FileSystem.IsolatedStorage`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The runtime chooses the storage location, while provider options control store lifetime and polling.
Watches are polling-based; rename registrations are accepted but do not fire. `RemoveStoreOnDispose`
is an explicit cleanup choice.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.FileSystem`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.IsolatedStorage/src/Assimalign.Cohesion.FileSystem.IsolatedStorage.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.IsolatedStorage/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.IsolatedStorage/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/FileSystem/README.md`.

- **Source** — `cohesion/libraries/FileSystem/Assimalign.Cohesion.FileSystem.IsolatedStorage/src`.
