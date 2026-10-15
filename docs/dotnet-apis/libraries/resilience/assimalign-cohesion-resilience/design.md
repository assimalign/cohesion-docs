# Assimalign.Cohesion.Resilience design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Resilience`.

> **Status:** Partial.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The core composes callbacks and outcomes without embedding individual policy behavior. Strategies
wrap execution from the outside in. Concrete policies extend builders from sibling packages, while
the current core remains transitional.

## Build-generated resources (#1467)

Exception messages are held in `src/Properties/ErrorMessages.resx`. The internal `ErrorMessages` accessor is generated in the project
root namespace by MSBuild's GenerateResource task through
`build/Targets/Build.ErrorMessages.targets`. The generated source stays under
`obj/`; no checked-in designer or Visual Studio custom tool is required. Ordinary
`dotnet build`, CI and NativeAOT publication use the same embedded resources.
Existing message text is preserved.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.ObjectPool`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/src/Assimalign.Cohesion.Resilience.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience/src`.
