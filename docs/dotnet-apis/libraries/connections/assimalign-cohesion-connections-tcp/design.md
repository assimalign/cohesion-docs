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

Neither error can recur without a new connection, so the retry cannot spin.

## A network error pending on a queued connection

Linux `accept(2)` passes network errors already pending on the new socket back as the accept's
error, and its manual says to retry them like `EAGAIN`. On Linux, `ENETDOWN`, `ENETUNREACH`,
`EHOSTDOWN`, `EHOSTUNREACH`, `ENOPROTOOPT` and `EOPNOTSUPP` arrive as `NetworkDown`,
`NetworkUnreachable`, `HostDown`, `HostUnreachable`, `ProtocolOption` and `OperationNotSupported`,
and are skipped the same way, raising `AcceptSkipped`. On Windows the same values mean the listening
socket failed, so there they still escape.

`EOPNOTSUPP` also means a listening socket that is not a stream socket, which can never accept, so
the skip applies only to a stream listener; otherwise an inherited datagram descriptor would spin.
The other two errors the manual lists, `EPROTO` and `ENONET`, are not skipped: .NET reports them as
the generic `SocketError.SocketError`, which they share with `ENOMEM`, so they are backed off instead
(see [Classification works from `SocketError` alone](#classification-works-from-socketerror-alone)).
#1312's acceptance criterion asks for all eight to be skipped; these two are the recorded exception.

## An accepted socket that cannot be set up

After the accept, the listener reads the socket's local endpoint, sets `TCP_NODELAY` on a TCP socket,
and wraps it in a connection. Those calls can fail for the one connection: on macOS, setting
`TCP_NODELAY` fails with `EINVAL` once the client has reset the connection, and a client can reset
right after the accept. A `SocketException` there closes the socket, which nothing else owns yet,
raises `AcceptedConnectionDropped`, and accepts the next connection; any other exception closes the
socket and escapes.

Only the accept call itself sits inside the accept-error filters above. They once covered the set-up
as well, so a set-up error that matched one was skipped without closing the socket, and one that
matched none stopped the listener.

## Running out of descriptors or buffers waits and retries

`TooManyOpenSockets` (`EMFILE`/`ENFILE`, `WSAEMFILE`) and `NoBufferSpaceAvailable` (`ENOBUFS`,
`WSAENOBUFS`) are transient: the endpoint is healthy again once connections close. Before #1312 they
escaped, so a client that held enough connections open stopped the endpoint until the host
restarted. Retrying at once would spin for as long as the exhaustion lasts, which is the known
problem with retrying every accept error. So `AcceptAsync` waits and retries, on the schedule Go's
`net/http` server uses: 5 ms, doubling with each consecutive failure, at most 1 s.

- **The schedule belongs to one `AcceptAsync` call**, so it starts over after every successful
  accept.
- **The wait links the caller's token to the listener's disposal**, so cancelling or disposing ends
  it at once, and the call then throws `OperationCanceledException` or `ObjectDisposedException` as it
  would have without the wait.
- **Each wait raises `AcceptBackoff`**, at most once a second per listener, with the number of waits
  it held back since the previous report, so sustained or flapping exhaustion cannot flood a trace.

## Classification works from `SocketError` alone

On Unix, .NET maps the native `errno` through a fixed table, discards it, and reports any value
outside the table as the generic `SocketError.SocketError`. `ENOMEM` and Linux's `ENOSR` arrive that
way, and so do `EPROTO` and `ENONET`, two of the pending network errors above. The value cannot tell
them apart, so on Unix the generic value is backed off for a stream listener: that never spins while
memory is short, and it costs a misclassified network error one short wait. Windows reports Winsock
codes, each of which has its own `SocketError`, so there the generic value escapes.

`TcpAcceptErrors` holds the classification and `TcpAcceptBackoff` the schedule and the report limit;
an internal constructor replaces the accept so tests can fail it with errors a real socket cannot be
made to report on demand. Every other accept error leaves the listening socket unable to accept, and
escapes.

## Accept events

The driver's event source, `Assimalign.Cohesion.Connections.Tcp`, reports what happened to an
accept:

| Id | Event | Level | Payload |
|---|---|---|---|
| 10 | `AcceptSkipped` | Verbose | `listenerId`, `socketError` (`ConnectionReset`/`ConnectionAborted`, and on Linux a pending network error): a queued connection that failed before the accept |
| 11 | `AcceptBackoff` | Warning | `listenerId`, `socketError`, `delayMilliseconds`, `unreportedBackoffs`: an accept failed and is retried after the delay. `socketError` names the cause: `TooManyOpenSockets` (out of descriptors), `NoBufferSpaceAvailable` (out of buffers), or, on Unix, `SocketError`, an `errno` .NET does not name, which is `ENOMEM`, `ENOSR`, `EPROTO` or `ENONET`; the message names no cause of its own. At most one per listener per second; `unreportedBackoffs` counts the waits held back since the previous report |
| 12 | `AcceptedConnectionDropped` | Verbose | `listenerId`, `socketError`: the listener closed a socket it had accepted because setting it up failed, typically because the client reset it right after the accept |

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
