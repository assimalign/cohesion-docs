# Assimalign.Cohesion.OpenApi design

Design decisions and ownership boundaries for `Assimalign.Cohesion.OpenApi`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

`OpenApiVersionCapabilities` centralizes field availability for the supported 3.0.4, 3.1.2, and
3.2.0 lines. The format-neutral node tree carries extension and example values. Serialization and
service-runtime dependencies stay outside the root model.

## Dependency boundary

The project declares no explicit Cohesion or external package references. Shared repository build
properties still apply.

## Sources

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/src/Assimalign.Cohesion.OpenApi.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/README.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi/src`.
