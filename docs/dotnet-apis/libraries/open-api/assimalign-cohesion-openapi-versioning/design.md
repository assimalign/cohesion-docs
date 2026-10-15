# Assimalign.Cohesion.OpenApi.Versioning design

Design decisions and ownership boundaries for `Assimalign.Cohesion.OpenApi.Versioning`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Transforms create a separate document and report construct changes and untranslatable content.
Serialization supplies deep-copy behavior and validation analyzes target-version fit. Callers must
inspect diagnostics before treating a retargeted document as equivalent.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.OpenApi`,
`Assimalign.Cohesion.OpenApi.Serialization`, `Assimalign.Cohesion.OpenApi.Validation`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Versioning/src/Assimalign.Cohesion.OpenApi.Versioning.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Versioning/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Versioning/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/OpenApi/README.md`.

- **Source** — `cohesion/libraries/OpenApi/Assimalign.Cohesion.OpenApi.Versioning/src`.
