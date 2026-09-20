# Assimalign.Cohesion.Amqp

Reserves the Advanced Message Queuing Protocol (AMQP) session and messaging layer.

> **Status:** Not yet implemented.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Amqp](../index.md)

## Scope

The project currently supplies an empty abstractions folder. Header negotiation and wire codecs are
implemented in `Amqp.Connections`; session and link semantics must not be inferred from that lower
layer.

## Dependencies

The project file declares no explicit Cohesion project or external package references.

## Principal public types

No implemented public type declarations were found in the project's retained source files.

## Sources

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp/src/Assimalign.Cohesion.Amqp.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp/src`.
