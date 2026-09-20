# Assimalign.Cohesion.Logging.Console design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Logging.Console`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The provider separates routing from formatting and allows a custom formatter. Rendering is skipped
when an entry cannot be emitted. Console output remains a sink concern, leaving the structured event
contract in the logging foundation.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Logging`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Console/src/Assimalign.Cohesion.Logging.Console.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Console/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Console/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Console/src`.
