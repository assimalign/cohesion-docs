# Assimalign.Cohesion.Amqp.Connections design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Amqp.Connections`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

A carrier must provide a reliable ordered byte stream; protocol identity does not select the
transport. Single-stream and multiplexed carriers share the AMQP connection surface. The server
transport owns its listener, while session, link, and messaging policy belong above this package.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.Connections`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src/Assimalign.Cohesion.Amqp.Connections.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Amqp/Assimalign.Cohesion.Amqp.Connections/src`.
