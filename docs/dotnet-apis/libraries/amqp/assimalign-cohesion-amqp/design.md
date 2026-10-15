# Assimalign.Cohesion.Amqp design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Amqp`.

> **Status:** Not yet implemented.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The project currently supplies an empty abstractions folder. Header negotiation and wire codecs are
implemented in `Amqp.Connections`; session and link semantics must not be inferred from that lower
layer.

There is no separate project DESIGN.md in this checkout. The project file, source, and area
documentation are the available design evidence.

## Dependency boundary

The project declares no explicit Cohesion or external package references. Shared repository build
properties still apply.

## Sources

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp/src/Assimalign.Cohesion.Amqp.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp/src`.
