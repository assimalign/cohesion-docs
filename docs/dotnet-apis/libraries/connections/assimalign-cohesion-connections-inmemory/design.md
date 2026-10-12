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

## Multiplexed Variant

`InMemoryMultiplexedConnection` models a QUIC-shaped connection: each end owns an inbound-stream
`Channel`, and the two ends reference each other. `OpenStreamAsync` builds a cross-wired
stream pair (via `InMemoryConnection.CreateStreamPair`), returns the local end to the opener, and
enqueues the peer end on the *other* multiplexed connection's inbound channel, where
`AcceptStreamAsync` picks it up. Each stream is a `MultiplexedStream` with its own lifetime and
duplex pipe; stream capabilities report `IsMultiplexed = false` while the parent reports `true`.

Stream direction is honored: an outbound unidirectional (`WriteOnly`) stream mirrors to a
`ReadOnly` stream on the peer — the write-only end's `Input` is pre-completed (it reads nothing)
and the read-only end's `Output` throws on write, per the `IMultiplexedConnection` contract.
Opening a `ReadOnly` stream throws `ArgumentException`, since a peer cannot open a stream only the
remote side may write to.

**A parent close reaches both ends (#1451).** The pair shares one close gate. Opening a stream
registers both ends before handing either out; each parent tracks queued, accepted and locally
opened streams until their terminal teardown. A stream leaving an accept queue stays tracked.
Stream disposal, whole-stream abort and completion of both usable halves remove the end; a
directional peer notification alone does not, since the opposite half can still be live.

The first abort or disposal on either parent chooses the pair close. Coded abort uses its caller
code; no-code abort and disposal use 0. Both parents signal `ConnectionClosed`, both accept queues
end, and pending and future accepts throw `OperationCanceledException`. The initiating parent is
`Aborted` for abort or `Closed` for disposal; its peer becomes `Aborted`. Repeated closes and later
disposal preserve that first decision. Opening another stream fails with the selected loss.

Every active stream first records a `ConnectionResetException` whose `ApplicationErrorCode` is the
connection close code. Only then are queues completed and parent tokens signaled; callbacks and
pipe completion run outside the pair gate. A callback can dispose its stream without deadlocking
or replacing the recorded code with a clean end. A throwing cancellation callback does not strand
other notifications. Pipe wrappers check recorded loss before and after pending operations, so
pending and subsequent reads/writes fail as connection loss even after cleanup. Parent disposal
is not a clean stream FIN. The optional reason exception stays local; the code is the peer-facing
close reason, matching the existing reset-code mapping.

**Compatibility and triage.** This corrects behavior shipped in 10.0.0-preview.1: parent closure
previously left the peer token and accepts live, and failed to terminate accepted streams. Code
relying on those pending operations surviving closure now observes immediate cancellation/loss,
an accepted preview behavior break under decisions 15 and 56. The socketless driver has no remote
network entry point; this is an in-process lifecycle defect, not an unauthenticated remote attack.

**An end that abandons a stream signals the other end (#1329).** The two ends of a stream pair hold
each other. An end that aborts, or whose holder completes `Output` with an error (the in-memory
`RESET_STREAM`) or `Input` with an error (the in-memory `STOP_SENDING`), cancels the other end's
`ConnectionClosed` at once, on the calling thread, and leaves its state alone. That is what the QUIC
driver reports for a peer `RESET_STREAM` or `STOP_SENDING`, and it lets an HTTP/3 server fire
`RequestCancelled` for a request the client cancelled while the application neither reads nor
writes. A clean completion (`Output` without an error, the FIN) is a half-close and signals nothing.
To see an errored `Input` completion, a stream end hands out a thin delegating `PipeReader` instead of
the pipe's own reader.

**A stream end carries application error codes (#1080, #1452).** It registers itself as the
`IMultiplexedStreamAbort` feature from `Connections.StreamAbort`, available through
`stream.DirectionalAbort`: the in-memory `STOP_SENDING` and `RESET_STREAM` with a code, so a protocol
tested over this driver puts its codes where the QUIC driver puts them:

- `AbortRead(errorCode)` completes the end's receive pipe with a `ConnectionResetException` whose
  `ApplicationErrorCode` is the code, so the other end's next flush throws it. A read waiting on the
  pipe is woken first: completing a pipe reader leaves a pending read waiting for the writer, so the
  delegating reader cancels it and fails it, and every later read, with `ConnectionAbortedException`.
- `AbortWrite(errorCode)` completes the end's send pipe with the same exception, so the other end's
  reads throw it, a read in flight included.
- Both signal the other end's `ConnectionClosed`, leave this end's state alone, and do nothing for a
  direction that has already ended or that a unidirectional end lacks. A later `Abort(reason)` or
  dispose does not complete an aborted direction again, so the other end keeps seeing the code, not the
  reason. Codes outside 0 to 2^62 - 1 throw, as on the QUIC driver.
- The ends of a byte-stream pair do not register the feature: a single-stream transport has no code to
  carry. `DirectionalAbort` is available only on `IMultiplexedStream`. Stream ends derive from
  `MultiplexedStream` and compose the internal `InMemoryConnection` pipe and lifecycle mechanics.

**The multiplexed roots (#1453).** Streams expose `Abort(code, reason)`, which resets both
available directions before ending the lifecycle. A direction already ended retains its first code.
Exceptions thrown by local or peer `ConnectionClosed` registrations are isolated, as in the QUIC
driver, so abort still notifies the peer and removes the ended stream from its parent's live set.
This also applies to no-code abort, directional abort and disposal through the shared cancellation
path. Compared with 10.0.0-preview.1, callback exceptions no longer escape teardown; this behavior
break is accepted under decision 15. The coded stream root itself postdates preview.1.
`IsInputCompleted` is true when the peer completed output cleanly and this end has not aborted
reading, aborted the stream, or completed input with an error; buffered bytes may still remain.
Local input failure is recorded separately from directional read cancellation, so a later peer FIN
cannot turn failed input into clean completion. A whole-stream abort records failure even when the
holder already completed input cleanly. This corrects the unreleased root contract; input-completion
reporting did not exist in 10.0.0-preview.1.
The parent root accepts coded aborts in the same 62-bit
range as streams; the first abort or disposal decides the pair close and notifies both ends as
described above.

The stream return types change from `ValueTask<IConnection>` to `ValueTask<IMultiplexedStream>`
(and from `ValueTask<Connection>` to `ValueTask<MultiplexedStream>` on guided bases). Together
with the required coded connection abort, these break preview.1 binaries and external implementers,
accepted during previews on decisions 15 and 56. Awaited streams can still be assigned to
`IConnection`. Stream reset and completion members are new; the removed optional contracts
postdate preview.1.

## Connection features (#1452)

The stream's `ConfigureFeatures` registers it once on first access, with
`IConnectionFeature.Name` returning `IMultiplexedStreamAbort.FeatureName` (`"StreamAbort"`).
Byte-stream pairs and multiplexed parent connections have empty collections. Each multiplexed
stream owns its collection; parent features never appear on the stream. A wrapper forwards the
stream's features by overriding `InnerFeatures`, which removes the need to reimplement and forward
every optional capability. A wrapper can replace or remove a named slot when its semantics demand it.

The core `Features` additions break external implementers of the connection interfaces shipped
in 10.0.0-preview.1, accepted under owner decisions 15 and 56; subclasses inherit the guided-base
implementation. The directional-abort contract itself postdates preview.1, so its package move
breaks no released API.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.Connections`, and
[`Assimalign.Cohesion.Connections.StreamAbort`](../assimalign-cohesion-connections-streamabort/index.md). The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src/Assimalign.Cohesion.Connections.InMemory.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.InMemory/src`.
