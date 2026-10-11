# Assimalign.Cohesion.Connections.Quic design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Connections.Quic`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Each QUIC stream surfaces as a `Connection` with an explicit direction. The driver owns connection
and stream lifecycle, while stream typing and HTTP settings belong above it. Availability follows
`System.Net.Quic`, so callers must account for platform support.

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
still delivering, which completes its disposal, and the tail is lost. An abort of the connection
during the wait closes it at once too. Before #1330 the wait happened on the exchange's own thread,
inside the blocking disposal of the `QuicStream`; the bound was added with the half-close, which
moved it into this phase.

## Handshake facts

QUIC runs a TLS 1.3 handshake inside its own (RFC 9001), so a QUIC connection is a TLS-terminating
connection in the contracts' sense. `QuicMultiplexedConnection` implements `ITlsConnectionInfo` and
reports what that handshake negotiated: the ALPN application protocol, the TLS version (always
TLS 1.3), the cipher suite, and the peer's certificate (the client's on a server-side connection,
present when the listener's `ServerAuthenticationOptions` requested one). The values are the same
on every stream of the connection; the streams themselves do not implement the facet. The driver
reads the values and never branches on them, so it stays free of protocol semantics.

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
counts connections whose handshake is in progress plus those waiting to be accepted. The handshake
timeout is `System.Net.Quic`'s default of 10 seconds. `TlsServerOptions.HandshakeTimeout` and
`MaxConcurrentHandshakes` belong to the TCP TLS layer and do not apply here.

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

The driver implements the contracts library's two code-carrying facets (#1080), so a protocol
chooses the code each abort puts on the wire:

| Call | `System.Net.Quic` call | Frame |
| --- | --- | --- |
| stream `AbortRead(errorCode)` | `QuicStream.Abort(QuicAbortDirection.Read, errorCode)` | `STOP_SENDING` |
| stream `AbortWrite(errorCode)` | `QuicStream.Abort(QuicAbortDirection.Write, errorCode)` | `RESET_STREAM` |
| connection `Abort(errorCode, reason)` | `QuicConnection.CloseAsync(errorCode)` | `CONNECTION_CLOSE` |

`QuicMultiplexedConnection` implements `IMultiplexedConnectionAbort`, and each stream implements
`IMultiplexedStreamAbort`. `Abort(Exception?)` closes the connection with `DefaultCloseErrorCode`, and
`Abort(long errorCode, Exception?)` with the caller's code. The first abort or disposal decides the
close: `QuicConnection` sends one `CONNECTION_CLOSE`, and a later close request has no effect on the
wire.

- **A direction ends once.** `QuicStream.Abort` skips a direction that has already ended (read to
  its end, aborted, or completed and acknowledged), so a stream `Abort(Exception?)` or disposal
  after a coded abort sends the default code only for a direction still open. That is how the HTTP/3
  transport resets with a code: both directions, then `Abort(reason)`. A sending direction whose FIN
  is queued but not yet acknowledged is still open to `QuicStream`, so the driver tracks `Output`'s
  completion itself and skips that direction too.
- **A half abort is not the stream ending.** It changes no `State` and does not fire the stream's
  `ConnectionClosed`: the `ReadsClosed` or `WritesClosed` fault it causes is `OperationAborted`, which
  the peer-closure watch ignores (see
  [`ConnectionClosed` on an abandoned stream](#connectionclosed-on-an-abandoned-stream)).
- **After `AbortRead`, every read fails.** The contract says so, and the in-memory driver does it,
  but the pipe `PipeReader.Create` builds over the `QuicStream` returns octets it has buffered and
  the holder has not examined without reading the stream. So a readable stream's `Input` is a thin
  delegating reader that checks a flag `AbortRead` sets before the stream is aborted, and fails
  `ReadAsync`, `TryRead`, and `ReadAtLeastAsync` with `QuicException(OperationAborted)`, the error
  `QuicStream` gives a read the abort overtakes. A read already waiting on the stream fails in
  `QuicStream` itself. Everything else passes through, so a read costs one flag check.
- **Aborting a stream that has ended does nothing.** A disposed stream, or one whose connection is
  gone, has nothing to tell the peer, so the driver swallows `ObjectDisposedException` and
  `QuicException` there. A code outside 0 to 2^62 - 1 throws `ArgumentOutOfRangeException` before
  the stream is touched.
- **Completing a half uses the default code; abort first for another.** Completing `Input` on a
  stream the peer is still sending stops it with `DefaultStreamErrorCode`, and completing `Output`
  with an error resets with it. A protocol that wants its own code calls `AbortRead` or `AbortWrite`
  first, and the completion then leaves that direction alone. Completing `Output` cleanly needs
  nothing first: it sends the FIN and leaves reading open (#1330), so the peer may keep sending. An
  abort after it cannot turn the FIN into a reset.

The options' default codes (`DefaultStreamErrorCode`, `H3_REQUEST_CANCELLED`, and
`DefaultCloseErrorCode`, `H3_NO_ERROR`, matching the default HTTP/3 ALPN) apply only where the caller
gives no code: the stream default on a stream's `Abort(Exception?)`, its disposal, the completion of
its `Input`, and the completion of its `Output` with an error; the close default on the connection's
`Abort(Exception?)` and disposal. A listener or factory serving a different ALPN protocol overrides
the codes alongside `ApplicationProtocols`.

## Breaking changes

These are behavior breaks against 10.0.0-preview.1, which shipped this driver with stream pipes
created with `leaveOpen: false`. They are accepted on the terms of decision 15 of the HTTP/Web
program plan: a break is accepted during the previews and recorded here.

1. **Completing `Output` no longer releases the stream or stops reading (#1330).** It used to
   dispose the `QuicStream`, which also stopped the receiving direction with
   `DefaultStreamErrorCode`. Now it ends only the sending direction: a consumer that completes only
   `Output` leaves the receiving direction open, so the peer may keep sending up to the stream
   window, and the `QuicStream` stays alive until `Input` is completed too or the stream is
   disposed.
2. **Completing `Output` with an error sends `RESET_STREAM`, not a FIN.**
   `Output.Complete(exception)` used to end the direction gracefully, so an abnormal end looked
   complete. It now resets with `DefaultStreamErrorCode`, and so does a completion whose final flush
   fails.
3. **Completing `Input` no longer ends the sending direction.** It used to dispose the `QuicStream`,
   which ended both directions; it now stops only the receiving direction (`STOP_SENDING` with
   `DefaultStreamErrorCode`, unless it has already ended).
4. **Disposal waits for the delivery of streams already completed.** A stream whose halves were
   both completed is released in the background, and its `DisposeAsync`, and therefore the
   connection's `DisposeAsync`, waits for the peer to acknowledge its data and FIN. Before, that
   wait blocked the thread that completed the pipe instead, and a later disposal of the stream, or
   of its connection, found the `QuicStream` already disposed and returned at once. The
   connection's wait is bounded by `CloseDeliveryTimeout` (five seconds by default); a single
   stream's is not.

Completing a half also no longer blocks the completing thread until the peer acknowledges the FIN;
that is not expected to break a consumer.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`, `Assimalign.Cohesion.Connections`,
`Assimalign.Cohesion.Connections`. The [overview](index.md#dependencies) distinguishes project
references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src/Assimalign.Cohesion.Connections.Quic.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src`.
