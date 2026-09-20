# Assimalign.Cohesion.Core design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Core`.

> **Status:** Partial.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Core stays free of hosting, configuration, logging, and dependency-injection references. The frozen
resource environment contract belongs here so gateways and resources share one vocabulary. Its
project also delivers the component-integration analyzer to package consumers.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.SourceGeneration.ComponentModel`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/src/Assimalign.Cohesion.Core.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Core/README.md`.

- **Source** — `cohesion/libraries/Core/Assimalign.Cohesion.Core/src`.
