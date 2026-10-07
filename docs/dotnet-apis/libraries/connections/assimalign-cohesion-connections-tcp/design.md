# Assimalign.Cohesion.Connections.Tcp design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Connections.Tcp`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

One socket data path serves the supported endpoint forms. Binding is explicit, and Unix socket-file
lifetime belongs to the listener. Shared pipe plumbing is compiled from the connection contracts
project rather than exposed as driver-specific public infrastructure.

## A client that resets before the accept

A client can connect and then reset (RST) while its connection waits in the accept queue. How the
platform reports it:
- **Windows** fails that accept with `ConnectionReset`.
- **BSD-derived stacks**, and Linux in some cases, fail it with `ConnectionAborted`.
- **Linux** usually returns the connection anyway, already reset.

`AcceptAsync` treats those two errors as the queued connection's own, reports `AcceptSkipped`, and
accepts the next connection (#1308). This is the contracts' `AcceptAsync` rule: a failure that
belongs to one inbound connection never escapes, because a consumer such as the HTTP accept loop
treats whatever escapes as the listener's end. Before #1308 one reset stopped an HTTP endpoint.

Neither error can recur without a new connection, so the retry cannot spin. Errors that leave the
listening socket unable to accept still escape, such as running out of descriptors.

The driver's event source, `Assimalign.Cohesion.Connections.Tcp`, reports the skip as event 10:

| Id | Event | Level | Payload |
|---|---|---|---|
| 10 | `AcceptSkipped` | Verbose | `listenerId`, `socketError` (`ConnectionReset` or `ConnectionAborted`) |

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.Connections`,
`Assimalign.Cohesion.Connections`. The [overview](index.md#dependencies) distinguishes project
references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/src/Assimalign.Cohesion.Connections.Tcp.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/src`.
