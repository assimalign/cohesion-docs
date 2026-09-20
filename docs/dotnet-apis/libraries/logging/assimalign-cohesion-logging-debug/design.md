# Assimalign.Cohesion.Logging.Debug design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Logging.Debug`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The default gate requires an attached debugger. Options provide a writer seam for deterministic
tests and alternate output. The provider follows the same entry and scope contracts as other logging
sinks.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Logging`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Debug/src/Assimalign.Cohesion.Logging.Debug.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Debug/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Debug/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Logging/Assimalign.Cohesion.Logging.Debug/src`.
