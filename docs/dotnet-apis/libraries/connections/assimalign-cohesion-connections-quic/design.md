# Assimalign.Cohesion.Connections.Quic design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Connections.Quic`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Each QUIC stream surfaces as a `Connection` with an explicit direction. The driver owns connection
and stream lifecycle, while stream typing and HTTP settings belong above it. Availability follows
`System.Net.Quic`, so callers must account for platform support.

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
  its end, completed, or aborted), so a stream `Abort(Exception?)` or disposal after a coded abort
  sends the default code only for a direction still open. That is how the HTTP/3 transport resets
  with a code: both directions, then `Abort(reason)`.
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
- **Abort the read direction before completing `Output` on an unread stream.** Completing either
  pipe disposes the `QuicStream` (`leaveOpen: false`), and the disposal stops a read direction that
  is still open with `DefaultStreamErrorCode`. A protocol that ends its sending direction while the
  peer is still sending therefore calls `AbortRead` with its own code first. #1330 separates the two
  directions, so that completing `Output` sends the FIN and leaves reading open.

The options' default codes (`DefaultStreamErrorCode`, `H3_REQUEST_CANCELLED`, and
`DefaultCloseErrorCode`, `H3_NO_ERROR`, matching the default HTTP/3 ALPN) apply only where the caller
gives no code: the stream default on a stream's `Abort(Exception?)`, its disposal, and the completion
of its `Input` or `Output`; the close default on the connection's `Abort(Exception?)` and disposal.
A listener or factory serving a different ALPN protocol overrides the codes alongside
`ApplicationProtocols`.

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
