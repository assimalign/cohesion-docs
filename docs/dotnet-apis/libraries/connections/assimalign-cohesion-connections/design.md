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
  never references the TLS layer.
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
  reset it while it waited in the accept queue (#1308).
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
`ConnectionClosed` is signaled on closure, and `ConnectionState` tracks
`Idle → Opening → Open → Closing → Closed`, or `Aborted`.

A stream of a multiplexed connection also signals `ConnectionClosed` when its peer abandons the
stream — a QUIC `RESET_STREAM` or `STOP_SENDING`, or the in-memory driver's equivalents — so a
consumer learns of it without reading or writing (#1329). A half that ends cleanly does not signal
it. The contract is stated on `IConnection.ConnectionClosed`, and it is how an HTTP/3 server fires
`RequestCancelled` for a request the client cancelled while the application neither reads nor
writes, which HTTP/3 cannot otherwise see because it has no frame pump.

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
