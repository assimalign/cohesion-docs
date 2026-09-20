# Assimalign.Cohesion.OpenApi.Validation design

Design decisions and ownership boundaries for `Assimalign.Cohesion.OpenApi.Validation`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Diagnostics have stable codes and severity rather than requiring callers to interpret exception
text. Rule contracts allow additional validators to compose with the default rule set. Version
placement follows the root capability matrix.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.OpenApi`,
`Assimalign.Cohesion.OpenApi.Serialization`. The [overview](index.md#dependencies) distinguishes
project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/src/Assimalign.Cohesion.OpenApi.Validation.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/README.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Validation/src`.
