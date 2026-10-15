# Assimalign.Cohesion.Connections.Udp

Provides message-oriented User Datagram Protocol (UDP) connections.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Connections](../index.md)

## Purpose

The message-oriented UDP driver for the Cohesion Connections contracts. It produces
`IDatagramConnection`s: a server socket bound to a local endpoint that receives from any peer, or a
client socket connected to one remote peer. Datagrams keep their boundaries — one send is one message,
one receive is one message — and derive from the common `IConnection` lifecycle without exposing a byte pipe.
`Id`, `State`, `ConnectionClosed`, and `Abort` are available on every datagram connection.
Disposal cancels the closed token before returning; abort preserves the `Aborted` state.

## Scope

- `UdpConnectionFactory` — `Bind`/`BindAsync` for the server role and `Connect` for the client role.
  UDP has no listener/accept lifecycle, so one factory serves both.
- `UdpBindOptions` / `UdpConnectOptions` — endpoint, dual-mode IPv6, address reuse (bind only), and
  socket buffer sizes.
- IPv4 and IPv6 only.

## Dependencies

- `Assimalign.Cohesion.Connections` — the `IDatagramConnection` contract, the `DatagramConnection`
  guided base, `DatagramReceiveResult`, and `ConnectionCapabilities`.
- `Assimalign.Cohesion.Core`.
- `System.Net.Sockets` from the shared framework. No third-party packages, no reflection.

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections`](../../connections/assimalign-cohesion-connections/index.md) | `CohesionProjectReference` |

## Usage

```csharp
UdpConnectionFactory factory = new();

await using IDatagramConnection server = factory.Bind(new IPEndPoint(IPAddress.Loopback, 0));
await using IDatagramConnection client = factory.Connect(server.LocalEndPoint!);

await client.SendAsync(payload, server.LocalEndPoint!, cancellationToken);
DatagramReceiveResult result = await server.ReceiveAsync(buffer, cancellationToken);
```

## Diagnostics

Datagram connection lifecycle events and counters are reported through the driver's internal event
source, `Assimalign.Cohesion.Connections.Udp`. Enable it by name in `dotnet-trace` /
`dotnet-counters`, or forward it into an application's logging with
`Assimalign.Cohesion.Logging.EventSource`. See [design](design.md#diagnostics).

## Principal public types

| Type | Source file |
|---|---|
| `UdpBindOptions` | `src/UdpBindOptions.cs` |
| `UdpConnectionFactory` | `src/UdpConnectionFactory.cs` |
| `UdpConnectOptions` | `src/UdpConnectOptions.cs` |

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Udp/src/Assimalign.Cohesion.Connections.Udp.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Udp/src`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Udp/src/UdpBindOptions.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Udp/src/UdpConnectionFactory.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Udp/src/UdpConnectOptions.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Udp/docs/DESIGN.md`.
