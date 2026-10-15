# Assimalign.Cohesion.OpenApi.Fluent design

Design decisions and ownership boundaries for `Assimalign.Cohesion.OpenApi.Fluent`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Concrete builders use nested callbacks to author the model and return a document from `Build()`.
Version-gated operations reject unsupported constructs at authoring time. The package needs the
model, not serialization or service integration.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.OpenApi`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/src/Assimalign.Cohesion.OpenApi.Fluent.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/README.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Fluent/src`.
