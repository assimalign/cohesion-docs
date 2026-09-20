# Assimalign.Cohesion.Logging design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Logging`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Providers consume one immutable entry model and concrete sinks remain separate. Factory filtering is
evaluated per provider. Scope entries correlate identifiers rather than creating an implicit ambient
stack, so adapters must preserve the actual correlation contract.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src/Assimalign.Cohesion.Logging.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging/src`.
