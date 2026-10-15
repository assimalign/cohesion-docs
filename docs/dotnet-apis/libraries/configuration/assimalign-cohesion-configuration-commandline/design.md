# Assimalign.Cohesion.Configuration.CommandLine design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Configuration.CommandLine`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The options capture arguments and optional switch mappings. Builder extensions register the
provider; argument normalization and key projection stay inside the provider so callers consume
ordinary configuration entries.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Configuration`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.CommandLine/src/Assimalign.Cohesion.Configuration.CommandLine.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.CommandLine/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.CommandLine/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Configuration/Assimalign.Cohesion.Configuration.CommandLine/src`.
