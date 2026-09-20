# Assimalign.Cohesion.OpenApi.Generation design

Design decisions and ownership boundaries for `Assimalign.Cohesion.OpenApi.Generation`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Generation consumes explicit input and target-version options. Metadata can come from generated
registries or attribute mapping, so document assembly does not require runtime reflection discovery.
The resulting document remains a normal version-aware model.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.OpenApi`,
`Assimalign.Cohesion.OpenApi.Attributes`. The [overview](index.md#dependencies) distinguishes
project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Generation/src/Assimalign.Cohesion.OpenApi.Generation.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Generation/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Generation/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/README.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Generation/src`.
