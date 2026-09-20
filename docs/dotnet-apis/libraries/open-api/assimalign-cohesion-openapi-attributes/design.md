# Assimalign.Cohesion.OpenApi.Attributes design

Design decisions and ownership boundaries for `Assimalign.Cohesion.OpenApi.Attributes`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Attributes describe operations and schemas, while flat metadata records form the seam to generation.
The mapper reports invalid combinations. Compile-time discovery lives in the separate analyzer
project, keeping discovery out of the runtime model.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.OpenApi`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src/Assimalign.Cohesion.OpenApi.Attributes.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/README.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Attributes/src`.
