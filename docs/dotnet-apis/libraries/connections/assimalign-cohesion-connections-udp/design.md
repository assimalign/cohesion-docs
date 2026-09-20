# Assimalign.Cohesion.Connections.Udp design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Connections.Udp`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The factory creates `IDatagramConnection` instances rather than pretending datagrams are ordered
streams. The project references the connection contracts and Core. There is no separate project
design document; the factory and datagram implementation define the current behavior.

There is no separate project DESIGN.md in this checkout. The project file, source, and area
documentation are the available design evidence.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.Connections`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Udp/src/Assimalign.Cohesion.Connections.Udp.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Udp/src`.
