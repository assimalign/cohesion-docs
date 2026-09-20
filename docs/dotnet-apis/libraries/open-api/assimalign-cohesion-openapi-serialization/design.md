# Assimalign.Cohesion.OpenApi.Serialization design

Design decisions and ownership boundaries for `Assimalign.Cohesion.OpenApi.Serialization`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Serialization maps between the canonical model and a format-neutral node tree. Target-version
capabilities govern emitted fields. JSON uses the base class library and YAML uses `Content.Yaml`,
preserving the service-free model boundary.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.OpenApi`, `Assimalign.Cohesion.Content.Yaml`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Serialization/src/Assimalign.Cohesion.OpenApi.Serialization.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Serialization/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Serialization/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/README.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Serialization/src`.
