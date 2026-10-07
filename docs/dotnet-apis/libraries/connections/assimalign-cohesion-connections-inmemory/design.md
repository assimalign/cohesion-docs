# Assimalign.Cohesion.Connections.InMemory design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Connections.InMemory`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Paired pipes are cross-wired so both ends can exchange bytes repeatedly. Listener/factory pairs
preserve the production connection contracts while avoiding operating-system sockets. This driver
moves bytes; application protocol behavior belongs to its consumers.

## Teardown and an abandoned stream

Teardown propagates to the peer through pipe completion, and there is no background watcher.
Completing one end's `Output` makes the other end's `Input` observe the end of the data; disposing
completes both of this end's pipe halves without an error, and an abort completes its `Output` with
the reason, so the peer's next read throws it. `ConnectionClosed` fires when
**this** end is disposed or aborted. On a byte-stream pair it does not fire when the *peer* closes:
a peer close is observed by reading `Input` (which completes) or by writing `Output` (whose flush
reports completion), which is how a byte-stream consumer such as an HTTP parser already detects the
end of a connection.

The two ends of a multiplexed stream hold each other, only to signal an abandoned stream (#1329). An
end that aborts, or whose holder completes `Output` with an error (the in-memory `RESET_STREAM`) or
`Input` with an error (the in-memory `STOP_SENDING`), cancels the other end's `ConnectionClosed` at
once, on the calling thread, and leaves its state alone. That is what the QUIC driver reports for a
peer `RESET_STREAM` or `STOP_SENDING`, and it lets an HTTP/3 server fire `RequestCancelled` for a
request the client cancelled while the application neither reads nor writes. A clean completion
(`Output` without an error, the FIN) is a half-close and signals nothing. To see an errored `Input`
completion, a stream end hands out a thin delegating `PipeReader` instead of the pipe's own reader.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.Connections`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src/Assimalign.Cohesion.Connections.InMemory.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src`.
