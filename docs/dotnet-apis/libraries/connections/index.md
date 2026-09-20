# Connections

Connection contracts and concrete stream, multiplexed, datagram, and security drivers.

[Libraries](../index.md)

## Packages

| Package | Responsibility | Reference |
|---|---|---|
| `Assimalign.Cohesion.Connections` | Defines byte-stream, multiplexed, and datagram connection contracts. | [Overview](assimalign-cohesion-connections/index.md) |
| `Assimalign.Cohesion.Connections.InMemory` | Provides socketless stream and multiplexed connections for live protocol tests. | [Overview](assimalign-cohesion-connections-inmemory/index.md) |
| `Assimalign.Cohesion.Connections.NamedPipes` | Carries ordered byte streams over named pipes for local inter-process communication. | [Overview](assimalign-cohesion-connections-namedpipes/index.md) |
| `Assimalign.Cohesion.Connections.Quic` | Adapts QUIC connections and streams to Cohesion multiplexed connection contracts. | [Overview](assimalign-cohesion-connections-quic/index.md) |
| `Assimalign.Cohesion.Connections.Security` | Applies Transport Layer Security (TLS) to established Cohesion connections. | [Overview](assimalign-cohesion-connections-security/index.md) |
| `Assimalign.Cohesion.Connections.Tcp` | Provides socket-backed streams over TCP, Unix domain sockets, and inherited listening sockets. | [Overview](assimalign-cohesion-connections-tcp/index.md) |
| `Assimalign.Cohesion.Connections.Udp` | Provides message-oriented User Datagram Protocol (UDP) connections. | [Overview](assimalign-cohesion-connections-udp/index.md) |

## Dependencies and delivery

The repository places this area in delivery wave 2. The wave is a dependency-ordering guide, not a
release or completeness guarantee.

| Assembly | Declared dependencies |
|---|---|
| `Assimalign.Cohesion.Connections` | `Assimalign.Cohesion.Core` (CohesionProjectReference) |
| `Assimalign.Cohesion.Connections.InMemory` | `Assimalign.Cohesion.Core` (CohesionProjectReference), `Assimalign.Cohesion.Connections` (CohesionProjectReference) |
| `Assimalign.Cohesion.Connections.NamedPipes` | `Assimalign.Cohesion.Core` (CohesionProjectReference), `Assimalign.Cohesion.Connections` (CohesionProjectReference) |
| `Assimalign.Cohesion.Connections.Quic` | `Assimalign.Cohesion.Core` (CohesionProjectReference), `Assimalign.Cohesion.Connections` (CohesionProjectReference), `Assimalign.Cohesion.Connections` (CohesionSharedSource) |
| `Assimalign.Cohesion.Connections.Security` | `Assimalign.Cohesion.Core` (CohesionProjectReference), `Assimalign.Cohesion.Connections` (CohesionProjectReference) |
| `Assimalign.Cohesion.Connections.Tcp` | `Assimalign.Cohesion.Core` (CohesionProjectReference), `Assimalign.Cohesion.Connections` (CohesionProjectReference), `Assimalign.Cohesion.Connections` (CohesionSharedSource) |
| `Assimalign.Cohesion.Connections.Udp` | `Assimalign.Cohesion.Core` (CohesionProjectReference), `Assimalign.Cohesion.Connections` (CohesionProjectReference) |

## Sources

- **Source** — `cohesion/libraries/README.md`.

- **Source** — `cohesion/README.md`.

- **Source** — `cohesion/docs/programs/DELIVERY_ROADMAP.md`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Assimalign.Cohesion.Connections.csproj`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src/Assimalign.Cohesion.Connections.InMemory.csproj`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.NamedPipes/src/Assimalign.Cohesion.Connections.NamedPipes.csproj`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.NamedPipes/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.NamedPipes/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.NamedPipes/src`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src/Assimalign.Cohesion.Connections.Quic.csproj`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/src/Assimalign.Cohesion.Connections.Security.csproj`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Security/src`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/src/Assimalign.Cohesion.Connections.Tcp.csproj`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/src`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Udp/src/Assimalign.Cohesion.Connections.Udp.csproj`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Udp/src`.
