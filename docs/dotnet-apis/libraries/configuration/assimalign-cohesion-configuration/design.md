# Assimalign.Cohesion.Configuration design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Configuration`.

> **Status:** Partial.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Provider registration is separated from configuration consumption. `ConfigurationBuilder` gathers
factories, while `ConfigurationManager` supports a longer-lived orchestration role. Format-specific
parsing belongs in provider packages rather than the core object model.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/src/Assimalign.Cohesion.Configuration.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration/src`.
