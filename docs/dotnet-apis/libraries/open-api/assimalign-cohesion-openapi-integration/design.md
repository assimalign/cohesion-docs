# Assimalign.Cohesion.OpenApi.Integration design

Design decisions and ownership boundaries for `Assimalign.Cohesion.OpenApi.Integration`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Endpoint sources feed description providers without introducing a Web or ApiManager dependency.
Import/export implementations compose serialization and version transforms. Retargeted exports
report losses instead of silently claiming equivalent documents.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.OpenApi`,
`Assimalign.Cohesion.OpenApi.Attributes`, `Assimalign.Cohesion.OpenApi.Generation`,
`Assimalign.Cohesion.OpenApi.Serialization`, `Assimalign.Cohesion.OpenApi.Versioning`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/src/Assimalign.Cohesion.OpenApi.Integration.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/README.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Integration/src`.
