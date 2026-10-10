# Assimalign.Cohesion.Connections design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Connections`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

A stream connection is itself an `IDuplexPipe`, with input and output anchored to its holder.
Listeners bind explicitly and factories establish outbound connections. Capabilities describe
delivery and security; connection layers transform connections at establishment without introducing
a parallel transport abstraction.

## Handshake facts: `ITlsConnectionInfo`

`ConnectionSecurity.Tls` says a connection is secured; it cannot say what the handshake agreed,
because capabilities describe a transport class and the handshake is per connection. An application
protocol needs that agreement: HTTP chooses between HTTP/2 and HTTP/1.1 from the application
protocol ALPN selected (RFC 7301), and shows its handlers the client certificate, TLS version, and
cipher suite. `ITlsConnectionInfo` is the typed facet that carries it. A connection that terminates
TLS implements it beside its connection contract, and a consumer finds it with a type test on the
connection it holds:

- **The secured `IConnection`** the TLS layer returns (`Assimalign.Cohesion.Connections.Security`).
- **A multiplexed connection** whose transport is TLS itself (QUIC, RFC 9001).

The facet reports four values, all fixed once the handshake completes:

| Member | Meaning |
|---|---|
| `ApplicationProtocol` | the protocol ALPN selected (RFC 7301), or none when the client offered none |
| `TlsProtocol` | the negotiated TLS version |
| `CipherSuite` | the negotiated cipher suite |
| `RemoteCertificate` | the certificate the peer presented, or `null`: the client's on a server-side connection (present only when the server requested one, RFC 8446 §4.3.2), the server's on a client-side one |

The connection owns `RemoteCertificate` and disposes it with itself. Both platform stacks hand
ownership of the peer certificate to whoever reads it (`SslStream.RemoteCertificate`,
`QuicConnection.RemoteCertificate`), so an implementation that reads it to report it must release
it; a consumer that keeps the certificate past the connection copies it.

Why this shape:

- **It lives in the contracts library** so the TLS layer and the QUIC driver can each implement it
  without a new reference, and a consumer reads it while depending on the contracts alone. HTTP
  never references the TLS layer. The HTTP server transport carries the facet one hop further: it
  republishes the values on each exchange's connection info, where `Assimalign.Cohesion.Http.Tls`
  reads them, so neither HTTP package references the other.
- **Not an `Items` bag**: the facts are few, typed, and fixed at handshake time, which is exactly
  what a typed contract is for.
- **Not a member of `IConnection` or `IMultiplexedConnection`**: every driver and test double would
  have to answer a question only TLS-terminating connections can, and a QUIC stream would answer
  for its whole connection.
- **A facet, not a base type**: `IConnection` and `IMultiplexedConnection` are different shapes, and
  both have TLS-terminating implementations, so the facet extends neither.

The cost is the usual one for decorators. A layer composed above TLS that returns a new connection
hides the facet unless it implements the facet too and forwards it. A pass-through layer, which
returns the connection it was given, keeps it visible.

## Application error codes: `IMultiplexedStreamAbort` and `IMultiplexedConnectionAbort`

`Abort(Exception?)` ends a whole stream or connection and says nothing on the wire about why. A
multiplexed transport can: QUIC's `RESET_STREAM`, `STOP_SENDING`, and `CONNECTION_CLOSE` frames each
carry an application error code (RFC 9000 §19.4, §19.5, §19.19). Until #1080 the QUIC driver sent its
configured defaults in all three, so an HTTP/3 server could not send what RFC 9114 §8.1 asks of it:

- After a complete response, the server stops an unread request with `H3_NO_ERROR`. The default,
  `H3_REQUEST_CANCELLED`, made .NET's `HttpClient` fail a request whose response had arrived, and the
  HTTP/3 transport drained up to 64 KiB of upload for up to 5 seconds to avoid sending it.
- A rejected or malformed request is reset with `H3_REQUEST_REJECTED` or `H3_MESSAGE_ERROR`, so a
  client can tell a request it may retry from one it must not.
- A connection error closes with its own code, such as `H3_FRAME_UNEXPECTED`, not `H3_NO_ERROR`.

Two facets carry the code. A consumer finds each with a type test and falls back to `Abort` without
it:

| Facet | Member | QUIC frame | Implemented by |
|---|---|---|---|
| `IMultiplexedStreamAbort` | `AbortRead(errorCode)` | `STOP_SENDING` | the QUIC driver's streams, the in-memory driver's stream ends |
| `IMultiplexedStreamAbort` | `AbortWrite(errorCode)` | `RESET_STREAM` | the same |
| `IMultiplexedConnectionAbort` | `Abort(errorCode, reason)` | `CONNECTION_CLOSE` | `QuicMultiplexedConnection` |

The rules:

- **One direction at a time.** A stream's directions end separately, so a server stops the request
  direction and still ends its response with a FIN.
- **The first signal for a direction wins.** A later `Abort` or `DisposeAsync` does not signal a
  direction again. A reset with a code is therefore both `AbortWrite` and `AbortRead`, then
  `Abort(reason)`, which keeps the lifecycle (`State`, `ConnectionClosed`) on the member that already
  owns it. The same holds for the connection: the first `Abort` or disposal decides the close code.
- **A half abort leaves the holder's lifecycle alone.** It changes no `State` and does not signal the
  holder's `ConnectionClosed`. The peer's stream reports an abandoned stream, as for any reset
  (#1329).
- **After `AbortRead`, every read fails**, a read in flight included, even one that octets the
  transport has already buffered would satisfy. Both drivers fail such a read (the in-memory driver
  with `ConnectionAbortedException`, QUIC with `QuicException(OperationAborted)`), so a consumer
  tested over the in-memory driver cannot come to depend on reading data the abort discarded.
- **Codes range from 0 to 2^62 - 1**, the values a QUIC variable-length integer carries (RFC 9000
  §16). Anything else throws `ArgumentOutOfRangeException`. The in-memory driver enforces the same
  range, so a test over it fails where QUIC would.
- **The peer sees the code** as `QuicException.ApplicationErrorCode` on QUIC, and as
  `ConnectionResetException.ApplicationErrorCode` on the in-memory driver.

Why this shape:

- **Facets, not members of `IConnection` or `IMultiplexedConnection`.** A new member breaks every
  implementation outside this repository (the contracts shipped in 10.0.0-preview.1), and only a
  transport with codes can honor it: a TCP connection has none to send. `ITlsConnectionInfo` is a
  facet for the same reason.
- **Not a code carried on the reason exception.** The reason types belong to the protocol (HTTP/3's
  are internal to it), so a driver would have to recognize them, or the protocol would have to throw
  this area's exceptions at its own application. The code is wire data, so it is an argument.
- **Two named members, not a direction enum.** Each direction has its own frame and fails something
  different on the peer, its reads or its writes. `ConnectionDirection` says which halves a stream
  has, not which to abort.
- **The half-close of #1330 builds on it.** The QUIC driver's stream pipes are created with
  `leaveOpen: false`, so completing either `Input` or `Output` disposes the QUIC stream: ending one
  direction ends both, and the disposal stops an unread direction with the default code. #1330 maps
  `Output` completion to the FIN alone and `Input` completion to a read-direction abort with the
  default code. A consumer that needs another code calls `AbortRead` first, and the first signal
  wins. Until then a consumer that ends its sending direction while the peer is still sending calls
  `AbortRead` before completing `Output`, as the HTTP/3 transport does after a complete response.

## A listener contains each connection's failure

`AcceptAsync`, on both listener shapes, returns a connection that is ready to use, with any
handshake already complete. A failure that belongs to one inbound connection is the listener's to
handle: it releases that connection and accepts the next. Examples are a handshake that fails or
times out, or a client that resets before the accept. An exception from `AcceptAsync` therefore
means the listener itself can produce no more connections (it was disposed, or its endpoint failed),
or the caller canceled. A consumer such as the HTTP listener treats it as fatal.

- **The layered listener** (`listener.Use(layer)`, and so `UseTls`) runs each connection's upgrade
  on its own task, off the accept loop (#1304). `AcceptAsync` returns connections in the order
  their upgrades complete.
  - A failed or timed-out upgrade fails only its connection: the listener disposes the connection,
    reports `UpgradeFailed`, and keeps accepting.
  - At most `maxConcurrentUpgrades` connections are held at once; the default is 512, and TLS sets
    it from `TlsServerOptions.MaxConcurrentHandshakes`. At the bound the pump stops accepting, and
    new peers wait in the transport's own backlog.
  - Canceling an `AcceptAsync` call abandons only that wait. Disposal releases every connection the
    listener still holds.
- **The drivers** honor the same contract. The QUIC driver drops an inbound connection whose
  handshake failed and keeps accepting (#1304). The TCP driver skips a connection whose client
  reset it while it waited in the accept queue (#1308), and it waits and retries when the process
  runs out of descriptors or buffers, which clears once connections close (#1312).
- **A layer that fails leaves the connection to its caller**, which disposes it. The layered
  listener disposes a connection it accepted, and the layered factory a connection it dialed
  (#1309).

The layered listener reports through this library's one internal event source,
`Assimalign.Cohesion.Connections`:

| Id | Event | Level | Payload |
|---|---|---|---|
| 1 | `UpgradeFailed` | Warning | `connectionId`, `remoteEndPoint`, `exceptionType`, `exceptionMessage` |

The counters are `current-upgrades` and `failed-upgrades`. The payload is the exception's type and
message, never the peer's bytes, a certificate, or key material. An upgrade canceled because the
listener is being disposed is not reported.

The alternatives were rejected for these reasons:
- **Upgrading inside `AcceptAsync`**, the shape before #1304, serialized handshakes on the accept
  loop and turned one peer's failure into the listener's.
- **Containing the failure in the consumer** cannot work by exception type. A handshake timeout
  throws `OperationCanceledException`, the same type a disposed listener throws. Only the component
  that ran the upgrade knows which failure it was.
- **An unbounded listener** would make every stalled handshake cost a socket, its buffers, and TLS
  state, which is a memory-exhaustion target.

## When `ConnectionClosed` fires

A connection has three teardown paths: complete `Output` for a graceful half-close,
`DisposeAsync()` to close, and `Abort(Exception?)` to tear down at once, discarding in-flight data.
A multiplexed transport's streams and connections can also abort with an application error code, one
stream direction at a time (see
[Application error codes](#application-error-codes-imultiplexedstreamabort-and-imultiplexedconnectionabort)).
`ConnectionClosed` is signaled on closure, and `ConnectionState` tracks
`Idle → Opening → Open → Closing → Closed`, or `Aborted`.

A stream of a multiplexed connection also signals `ConnectionClosed` when its peer abandons the
stream — a QUIC `RESET_STREAM` or `STOP_SENDING`, or the in-memory driver's equivalents — so a
consumer learns of it without reading or writing (#1329). A half that ends cleanly does not signal
it. The contract is stated on `IConnection.ConnectionClosed`, and it is how an HTTP/3 server fires
`RequestCancelled` for a request the client cancelled while the application neither reads nor
writes, which HTTP/3 cannot otherwise see because it has no frame pump.

`ConnectionException` is the area's exception root, with `ConnectionAbortedException` and
`ConnectionResetException` for the common failures, so consumers catch one hierarchy.
`ConnectionResetException.ApplicationErrorCode` carries the code a peer reset or stopped a stream
with, when the driver reports the reset through this family: the in-memory driver does, and the QUIC
driver surfaces `System.Net.Quic`'s `QuicException`.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src/Assimalign.Cohesion.Connections.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections/src`.
