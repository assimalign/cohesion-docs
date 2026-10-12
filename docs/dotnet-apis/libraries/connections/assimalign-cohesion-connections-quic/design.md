# Assimalign.Cohesion.Connections.Quic design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Connections.Quic`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Each QUIC stream surfaces as a `MultiplexedStream` with an explicit direction. The driver owns
connection and stream lifecycle, while stream typing and HTTP settings belong above it. Availability follows
`System.Net.Quic`, so callers must account for platform support.

## Connection features (#1452)

The driver references `Connections.Tls`, `Connections.StreamLimits` and `Connections.StreamAbort`.
Its lazy `ConfigureFeatures` hook registers separate TLS and stream-limit objects, each returning
its contract's pinned `FeatureName`. Distinct objects matter because one feature has only one name.
The existing public facts properties remain on `QuicMultiplexedConnection`; the TLS and limits
contracts are implemented only by the two dedicated objects. Keeping both contracts on the
connection would give it two incompatible required feature names through the single
`IConnectionFeature.Name` member. Each QUIC stream registers itself only for `DirectionalAbort`.
Stream collections never fall back to their parent connection.

Applications read `connection.Tls`, `connection.StreamLimits` and `stream.DirectionalAbort`,
which continue through layers that override `InnerFeatures`. Features allocate only on first
access. Cache a discovered feature when using it repeatedly on a connection or stream.

Adding `Features` to the three connection interfaces breaks implementers of 10.0.0-preview.1
under owner decisions 15 and 56; the guided bases cover subclasses. Moving these optional
contracts breaks no released API because they postdate preview.1. Removing
`ITlsConnectionInfo` and `IMultiplexedConnectionStreamLimits` from the public concrete connection
is an additional public-surface change required by the one-object, one-name rule, recorded for
owner review under escalations. These implementations also postdate preview.1; existing public
facts properties remain available, while consumers discover the contracts through the features.

## Each stream half ends on its own

The stream pipes are created with `leaveOpen: true` (#1330), and the halves a holder sees end only
their own direction:

- **Completing `Output`** flushes what the pipe holds and sends the FIN
  (`QuicStream.CompleteWrites`), and the stream stays readable until the peer ends its side.
  Completing it with an error sends `RESET_STREAM` with `DefaultStreamErrorCode` instead, so an
  abnormal end never looks like a complete one, and so does a completion whose own final flush
  fails (a write still in flight on the stream, for instance), which would otherwise put a FIN after
  partial data. The completion is the first signal for the sending direction, so a later
  `AbortWrite` does nothing and `Abort` stops only the receiving direction: `QuicStream` keeps the
  direction open until the peer acknowledges the FIN, and would put `RESET_STREAM` on the wire after
  it, letting the peer discard data it was told was complete (an HTTP/3 tunnel or request body that
  fails after the response's FIN reaches this).
- **Completing `Input`** stops the receiving direction with `DefaultStreamErrorCode`
  (`STOP_SENDING`), unless it has already ended (read to its end, or stopped with a code), and
  leaves the sending direction alone.

Once every usable half has been completed the stream is finished, so the `QuicStream` is disposed in
the background (it waits for the stream's shutdown, which a graceful FIN reaches once the peer has
the data), and the stream's `DisposeAsync` waits for that same disposal. That keeps the
connection-first teardown intact: a bidirectional stream disposed by its connection still delivers
its FIN before the `CONNECTION_CLOSE`, for as long as the connection's `CloseDeliveryTimeout`
allows. Neither completion signals the stream's `ConnectionClosed` or changes its `State`: a half
the holder ends is a half-close, not an abort. Before #1330 the pipes were created with
`leaveOpen: false`, so completing either one disposed the `QuicStream` synchronously, which ended
both directions and blocked the completing thread until the peer acknowledged the FIN.

## A FIN already observed (#1427)

Each stream's root `IMultiplexedStream.IsInputCompleted` checks the existing
`QuicStream.ReadsClosed` task for successful completion and excludes a locally aborted read.
.NET 10 completes that task when its final buffered bytes are copied to the caller, before
returning that final nonzero read. Thus a stream-backed `PipeReader` can expose the fact without
waiting for another read to return zero. The consumer must still check for unread pipe bytes:
a FIN after data is not an empty input until those bytes are consumed.

The implementation uses [the runtime's completion behavior](https://github.com/dotnet/runtime/blob/v10.0.0/src/libraries/System.Net.Quic/src/System/Net/Quic/QuicStream.cs#L284-L301),
with real-QUIC tests pinning it. There is no prefetch task, extra read buffer, or speculative read;
late FINs remain conservative until the runtime observes them. Moving the completion fact to the
stream root (#1453) adds no HTTP dependency: every byte-stream multiplexer supports a half-close.
The incremental HTTP/3 body signal it improves was introduced after 10.0.0-preview.1; the root's
stream return type changes are recorded under "Breaking changes" below.

## Disposal waits for stream delivery, within a bound

A graceful `DisposeAsync` completes the bidirectional streams first, because their write halves
carry application data (an HTTP/3 response, for example) whose FIN and delivery acknowledgement must
precede the connection close, or the tail of the data could be lost. Then it closes the connection
(`CONNECTION_CLOSE` carrying `DefaultCloseErrorCode`), and only then releases the unidirectional
streams, which HTTP/3 designates critical.

The bidirectional streams are disposed together, and their waits for delivery share one bound,
`CloseDeliveryTimeout` (five seconds by default, on both options types). A peer that withholds
stream credit for a response's tail while it keeps the connection alive with PINGs never
acknowledges the FIN, and MsQuic's send buffering has already completed every write, so nothing
above the driver sees a stall; without the bound the disposal, and the host's connection slot, would
wait for as long as the peer chose. When the bound runs out the connection close ends every stream
still delivering, which completes its disposal, and the tail is lost. An abort after disposal started
ends the delivery wait immediately with disposal's chosen default close code, so a shorter host
shutdown budget can still force teardown. Before #1330 the wait happened on the exchange's own thread,
inside the blocking disposal of the `QuicStream`; the bound was added with the half-close, which
moved it into this phase.

All remaining disposals start together and share a second `CloseDeliveryTimeout` bound, including
bidirectional streams still delivering and streams accepted after the first snapshot. Repeated
disposal waits for the same native release, so no separate unbounded sweep retries that wait
afterward; native connection disposal finishes teardown (#1424).

## Completed and aborted streams are released

Stream and connection dispose are idempotent. Once the background
`QuicStream` release finishes, the driver untracks its wrapper and reports
`StreamClosed` exactly once (#1424), whether it was released by completing
both usable halves or by explicit disposal. Until that release finishes it
stays tracked, so connection teardown still waits for delivery within
`CloseDeliveryTimeout`. Completing just one half neither untracks the stream
nor changes its state or cancellation signal; completing both halves still
leaves those signals alone. Explicit abort/disposal retains its existing
state and cancellation behavior. Aborting a stream also starts its pipe and native
cleanup, so reset requests do not retain wrappers either. Concurrent disposals
wait for the same native release instead of skipping a delivery already in progress.

The driver owns this transition rather than asking HTTP/3 to await stream
disposal: every protocol that completes both halves needs the same release,
and waiting in an exchange would bypass the connection's delivery bound.
This fixes preview.1's retention of one wrapper (pipes, CTS and TCS) per
request until connection teardown. A remote unauthenticated client can
retain managed memory by repeatedly requesting on one opt-in HTTP/3
connection (CWE-401; availability impact). The current-streams gauge now
falls at release instead of remaining inflated until connection teardown;
that diagnostic behavior change is recorded against preview.1 on decision
15's terms.

## Inbound stream allowance (#1425)

`QuicMultiplexedConnection` registers an `IMultiplexedConnectionStreamLimits` feature from
`Connections.StreamLimits`, available through `connection.StreamLimits`.
Its `InboundBidirectionalStreamLimit` starts at the inbound bidirectional concurrency configured
for the connection and increases once per inbound bidirectional stream whose disposal starts,
saturating at QUIC's maximum of 2^60 streams. Outbound streams and unidirectional streams do not
change it. The listener freezes its bidirectional limit at bind time and uses that same snapshot
in every handshake and accepted wrapper; mutations of the original option after binding cannot
make those two disagree. Factory-created wrappers take the actual client options' inbound limit
(currently the BCL client default of zero).

The value is a **conservative cumulative upper bound**, not an exact snapshot of the current
native MAX_STREAMS value. .NET 10 requests
[`DELAY_ID_FC_UPDATES`](https://github.com/dotnet/runtime/blob/v10.0.0/src/libraries/System.Net.Quic/src/System/Net/Quic/QuicConnection.cs#L644-L663),
which defers replenishment until the native stream handle closes. The driver increments before
starting native disposal, including cleanup after a wrapper-construction failure, so a concurrent
consumer never rejects a stream against already-replenished but unrecorded credit. The callback
is cached per connection and guarded by the stream's existing one-shot release latch.

During pending disposal the bound can include credit not advertised yet. This permits some
references beyond the exact native limit, rather than falsely rejecting legitimate references
on a long-lived connection after stream credit is replenished. `System.Net.Quic` offers no public
getter for the local cumulative allowance; its stream-capacity callback reports the opposite
direction. Protocols retain their own bounded storage as a backstop. No reflection, native calls,
or HTTP-specific semantics are added. The facet is an additive API relative to
10.0.0-preview.1; changing the bound listener's bidirectional option is now explicitly ineffective.

## Handshake facts

QUIC runs a TLS 1.3 handshake inside its own (RFC 9001), so a QUIC connection is a TLS-terminating
connection in the contracts' sense. `QuicMultiplexedConnection` registers an `ITlsConnectionInfo`
feature from `Connections.Tls`, available through `connection.Tls`, reporting what that handshake
negotiated: the ALPN application protocol, the TLS version (always TLS 1.3), the cipher suite, and the peer's certificate (the client's on a server-side connection,
present when the listener's `ServerAuthenticationOptions` requested one). The values are captured
when the connection is wrapped, after `System.Net.Quic` has completed the handshake, and are the
same for the lifetime of the connection; streams do not inherit the connection's features. The
driver reads the values and never branches on them, so it stays free of protocol semantics.

Reading `QuicConnection.RemoteCertificate` hands the certificate's ownership to the reader (a
`QuicConnection` no longer disposes a certificate it has exposed), so `QuicMultiplexedConnection`
disposes the peer certificate after it has disposed the QUIC connection.

## A failed handshake never ends the accept

`System.Net.Quic` runs each inbound connection's handshake in the background. When one fails, it
reports the failure from the next `QuicListener.AcceptConnectionAsync`, as an
`AuthenticationException` or a `QuicException`. Causes include a client certificate the policy
refuses, a handshake that exceeds its timeout, and an error from the connection-options callback.
The listener stays usable.

`QuicConnectionListener.AcceptAsync` treats each such exception as that one connection's: it
reports `HandshakeFailed` (Warning: `listenerId`, `exceptionType`, `exceptionMessage`) and accepts
the next connection (#1304). Only the listener's disposal and the caller's own cancellation leave
`AcceptAsync`. Before #1304 a single client without a required certificate stopped the HTTP/3
endpoint. Kestrel's QUIC transport makes the same call.

Pending handshakes are bounded by `Backlog`, which becomes `QuicListenerOptions.ListenBacklog`: it
counts connections whose handshake is in progress plus those waiting to be accepted;
`System.Net.Quic` refuses new connections beyond it. `MaxConcurrentHandshakes` belongs to the
TCP TLS layer and does not control this backlog.

## The inbound handshake timeout is configurable

`QuicConnectionListenerOptions.HandshakeTimeout` (#1311) is copied into
`QuicServerConnectionOptions.HandshakeTimeout` by the options factory used by the listener's
actual connection-options callback. Its ten-second default preserves the previous
`System.Net.Quic` default. Positive values bound each handshake, and
`Timeout.InfiniteTimeSpan` disables that timeout. Other negative values are rejected on assignment;
remaining runtime validation belongs to `System.Net.Quic`. The connection-options factory takes
the frozen bidirectional stream allowance from bind time, preserving the snapshot described under
[Inbound stream allowance](#inbound-stream-allowance-1425).

Zero is passed through unchanged. Although the BCL property describes zero as choosing the
native default, [.NET 10's server path starts its managed cancellation timer with that value](https://github.com/dotnet/runtime/blob/v10.0.0/src/libraries/System.Net.Quic/src/System/Net/Quic/QuicListener.cs#L207-L223),
so zero can expire immediately; callers should use `Timeout.InfiniteTimeSpan` to disable it.
Web.Hosting maps `TlsServerOptions.HandshakeTimeout` into this option for its TLS-options
overload and binds the endpoint's configured timeout. Its TLS non-positive/disabled convention
maps to `Timeout.InfiniteTimeSpan`. A timeout still drops only the failed connection, as #1304
requires. This option is additive relative to 10.0.0-preview.1 and leaves default behavior intact.

## `ConnectionClosed` on an abandoned stream

A stream's `ConnectionClosed` fires on its own `Abort` and `DisposeAsync`, and also when the stream
ends underneath it (#1329). The stream watches `QuicStream.ReadsClosed` and `WritesClosed` and
signals when either faults with anything but `QuicError.OperationAborted`: a peer `RESET_STREAM` or
`STOP_SENDING` (`StreamAborted`), or the loss of the connection. Faults from this end's own
operations (`OperationAborted`) and a half that ends cleanly signal nothing. The check runs on the
thread pool, never on the QUIC event thread that completed the task. A consumer learns that the peer
abandoned the stream without reading or writing, which is how an HTTP/3 server fires
`RequestCancelled` for a request the client cancelled.

## Application error codes

The connection and stream roots carry the coded close and whole-stream reset (#1453).
The driver also registers the `Connections.StreamAbort` directional-abort feature, so a
protocol may end one direction independently where QUIC supports it. QUIC validates its own
code range, 0 to 2^62 - 1; the generic roots do not impose QUIC's range on other transports.

| Call | `System.Net.Quic` call | Frame |
| --- | --- | --- |
| stream `AbortRead(errorCode)` | `QuicStream.Abort(QuicAbortDirection.Read, errorCode)` | `STOP_SENDING` |
| stream `AbortWrite(errorCode)` | `QuicStream.Abort(QuicAbortDirection.Write, errorCode)` | `RESET_STREAM` |
| stream `Abort(errorCode, reason)` | Both directional aborts, then stream lifecycle teardown | `STOP_SENDING` and `RESET_STREAM` for directions still open |
| connection `Abort(errorCode, reason)` | `QuicConnection.CloseAsync(errorCode)` | `CONNECTION_CLOSE` |

`Abort(Exception?)` closes the connection with `DefaultCloseErrorCode`, and
`Abort(long errorCode, Exception?)` on the multiplexed root closes with the caller's code.
The first abort or disposal decides the close code under the state lock, before any delivery wait:
a later abort interrupts disposal's delivery wait while retaining its default code, and disposal
retains a preceding abort's code even if it overtakes that abort before its asynchronous close
starts. `QuicConnection` sends one `CONNECTION_CLOSE`, and a later close request has no effect on
the wire.

- **A direction ends once.** `QuicStream.Abort` skips a direction that
  has already ended (read to its end, aborted, or completed and
  acknowledged), so a stream `Abort(Exception?)` or disposal after a
  coded abort sends the default code only for a direction still open.
  The root `Abort(errorCode, reason)` performs both directional aborts and ends the
  lifecycle in one call, retaining any direction's earlier code or clean completion.
  A sending direction whose FIN is queued but not
  yet acknowledged is still open to `QuicStream`, so the driver tracks
  `Output`'s completion itself and skips that direction too.
- **A half abort is not the stream ending.** It changes no `State` and
  does not fire the stream's `ConnectionClosed`: the `ReadsClosed` or
  `WritesClosed` fault it causes is `OperationAborted`, which the
  peer-closure watch ignores (see "Lifecycle and teardown").
- **After `AbortRead`, every read fails.** The contract says so, and the
  in-memory driver does it, but the pipe `PipeReader.Create` builds over
  the `QuicStream` returns octets it has buffered and the holder has not
  examined without reading the stream. So a readable stream's `Input` is
  a thin delegating reader that checks a flag `AbortRead` sets before the
  stream is aborted, and fails `ReadAsync`, `TryRead`, and
  `ReadAtLeastAsync` with `QuicException(OperationAborted)`, the error
  `QuicStream` gives a read the abort overtakes. A read already waiting
  on the stream fails in `QuicStream` itself. Everything else passes
  through, so a read costs one flag check.
- **Aborting a stream that has ended does nothing.** A disposed stream,
  or one whose connection is gone, has nothing to tell the peer, so the
  driver swallows `ObjectDisposedException` and `QuicException` there. A
  code outside 0 to 2^62 - 1 throws `ArgumentOutOfRangeException` before
  the stream is touched.
- **Completing a half uses the default code; abort first for another.**
  Completing `Input` on a stream the peer is still sending stops it with
  `DefaultStreamErrorCode`, and completing `Output` with an error resets
  with it. A protocol that wants its own code calls `AbortRead` or
  `AbortWrite` first, and the completion then leaves that direction alone.
  Completing `Output` cleanly needs nothing first: it sends the FIN and
  leaves reading open (#1330), so the peer may keep sending. An abort after
  it cannot turn the FIN into a reset.

## Diagnostics

The diagnostics tests link the canonical test-only `build/TestSupport/EventSourceRecorder.cs`
with MSBuild `Compile`/`Link` (see `.claude/rules/testing.md`); it records
events and counter publication without adding a shipped dependency.

The driver reports through its own internal event source, named for the assembly:
`Assimalign.Cohesion.Connections.Quic` (`Internal/EventSource/QuicConnectionEventSource.cs`), per the
repository EventSource convention (`.claude/rules/event-source.md`). Tools enable it by name
(`dotnet-trace`, `dotnet-counters`); an application forwards it into its logging with
`Assimalign.Cohesion.Logging.EventSource`, where the source name becomes the log category.

| Id | Event | Level | Payload |
| --- | --- | --- | --- |
| 1 | `ListenerBound` | Informational | `listenerId`, `endPoint` |
| 2 | `ListenerClosed` | Informational | `listenerId` |
| 3 | `ConnectionOpened` | Informational | `connectionId`, `listenerId` (empty when dialed), `localEndPoint`, `remoteEndPoint` |
| 4 | `ConnectionClosed` | Informational | `connectionId` |
| 5 | `StreamOpened` | Verbose | `streamId`, `connectionId`, `direction` |
| 6 | `StreamClosed` | Verbose | `streamId` |
| 7 | `HandshakeFailed` | Warning | `listenerId`, `exceptionType`, `exceptionMessage` — an inbound connection whose handshake failed, which the listener dropped |

Counters: `current-connections`, `total-connections`, `connections-per-second`, `current-streams`,
and `streams-per-second`.

- Streams report at Verbose and apart from connections: HTTP/3 opens one per request, and folding them
  into the connection events (as the retired shared forwarder did) made every stream look like a
  connection.
- `Abort` and `DisposeAsync` both end a connection or stream; whichever runs first reports the close,
  once, behind an `Interlocked` flag, so the `current-*` gauges stay exact when an aborted object is
  disposed later — or never.
- Lifetimes use `Opened`/`Closed`/`Bound` names rather than `Start`/`Stop`: they begin and end on
  different async flows, which EventSource's activity tracking would mis-nest.
- TLS handshake events come from the runtime's own `System.Net.Security` and `System.Net.Quic`
  sources; forward them by adding those prefixes to the forwarder.
- `HandshakeFailed` is the one handshake event this driver raises, because dropping the connection
  is its decision. It is a warning (the connection is lost, the listener recovered) and carries the
  exception's type and message, never a certificate or key material. A dropped connection never
  reports `ConnectionOpened`: `System.Net.Quic` disposed it before this driver wrapped it.

## Breaking changes

These are behavior breaks against 10.0.0-preview.1, which shipped this
driver with stream pipes created with `leaveOpen: false`. They are
accepted on the terms of decision 15 of the HTTP/Web program plan: a
break is accepted during the previews and recorded here.

1. **Completing `Output` no longer releases the stream or stops
   reading (#1330).** It used to dispose the `QuicStream`, which also
   stopped the receiving direction with `DefaultStreamErrorCode`. Now it
   ends only the sending direction: a consumer that completes only
   `Output` leaves the receiving direction open, so the peer may keep
   sending up to the stream window, and the `QuicStream` stays alive
   until `Input` is completed too or the stream is disposed.
2. **Completing `Output` with an error sends `RESET_STREAM`, not a
   FIN.** `Output.Complete(exception)` used to end the direction
   gracefully, so an abnormal end looked complete. It now resets with
   `DefaultStreamErrorCode`, and so does a completion whose final flush
   fails.
3. **Completing `Input` no longer ends the sending direction.** It used to
   dispose the `QuicStream`, which ended both directions; it now stops
   only the receiving direction (`STOP_SENDING` with
   `DefaultStreamErrorCode`, unless it has already ended).
4. **Disposal waits for the delivery of streams already completed.** A
   stream whose halves were both completed is released in the
   background, and its `DisposeAsync`, and therefore the connection's
   `DisposeAsync`, waits for the peer to acknowledge its data and FIN.
   Before, that wait blocked the thread that completed the pipe instead,
   and a later disposal of the stream, or of its connection, found the
   `QuicStream` already disposed and returned at once. The connection's
   wait is bounded by `CloseDeliveryTimeout` (five seconds by default);
   a single stream's is not.
5. **The inbound bidirectional limit is fixed when the listener binds (#1425).**
   Changing the original `MaxBidirectionalStreamCount` option after binding used to
   change the value used by subsequent handshakes. It now leaves the bound listener's
   allowance unchanged, keeping its handshake and generic limit facet consistent.
6. **Multiplexed stream return types change (#1453).** The connection's typed accept/open
   members now return `ValueTask<MultiplexedStream>` instead of `ValueTask<Connection>`;
   the interface returns `ValueTask<IMultiplexedStream>`. Assigning the awaited stream to
   `IConnection` still compiles, but existing binaries and implementations must migrate.
   `Abort(long, Exception?)` is now required on the multiplexed connection root and guided
   base. The new stream root requires whole-stream reset and `IsInputCompleted`.
   Retired optional contracts postdate preview.1, so their removal breaks nothing released.
7. **The first disposal selects the connection close (#1453).** A coded abort made while
   disposal waits for stream delivery cannot replace its default close code. The abort
   still interrupts the delivery wait and changes the state to `Aborted`, preserving bounded
   host shutdown. Earlier code allowed the later abort to choose the wire close. The coded
   overload postdates preview.1, so this code-selection rule changes only unreleased behavior.

Completing a half also no longer blocks the completing thread until the
peer acknowledges the FIN; that is not expected to break a consumer.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.Connections`,
`Assimalign.Cohesion.Connections.Tls`, `Assimalign.Cohesion.Connections.StreamAbort`, and
`Assimalign.Cohesion.Connections.StreamLimits`; `Assimalign.Cohesion.Connections` also supplies
shared source. The [overview](index.md#dependencies) distinguishes project
references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src/Assimalign.Cohesion.Connections.Quic.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src`.
