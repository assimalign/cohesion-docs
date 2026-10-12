# Assimalign.Cohesion.Connections.Quic

Adapts QUIC connections and streams to Cohesion multiplexed connection contracts.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Connections](../index.md)

## Scope

Each QUIC stream surfaces as a `MultiplexedStream` with an explicit direction. The driver owns
connection and stream lifecycle, while stream typing and HTTP settings belong above it. Availability follows
`System.Net.Quic`, so callers must account for platform support.

`QuicMultiplexedConnection` reports its TLS 1.3 handshake (the ALPN protocol, the cipher suite, the
peer certificate) through the `Tls` feature from `Connections.Tls`, so HTTP/3 can show the session
to handlers. See the [design](design.md#handshake-facts).

A stream's `ConnectionClosed` also fires when the peer abandons the stream (`RESET_STREAM` or
`STOP_SENDING`) or the QUIC connection is lost, which is how an HTTP/3 server fires
`RequestCancelled` for a request its client cancelled (#1329). See the
[design](design.md#connectionclosed-on-an-abandoned-stream).

The connection and its streams carry the caller's QUIC application error codes through root
`Abort(code, reason)` calls: a connection close sends `CONNECTION_CLOSE`, and a whole-stream reset
sends `STOP_SENDING` and `RESET_STREAM` (#1453). The `DirectionalAbort` feature still ends one
direction alone. The options' default codes, which match the default HTTP/3 ALPN, apply only where
the caller gives none. See the [design](design.md#application-error-codes).

Each half of a stream ends on its own (#1330). Completing `Output` sends the FIN and leaves the
stream readable until the peer ends its side; completing it with an error sends `RESET_STREAM`
instead. Completing `Input` stops the receiving direction (`STOP_SENDING` with
`DefaultStreamErrorCode`) and leaves the sending direction alone. The QUIC stream is released once
both halves are complete. Disposing a connection waits at most `CloseDeliveryTimeout` (five seconds
by default, on both options types) for its streams' data and FINs to be acknowledged before it
closes the connection. These are behavior breaks against 10.0.0-preview.1. See the
[design](design.md#each-stream-half-ends-on-its-own).

Streams expose root `IsInputCompleted`: a clean transport end already observed,
without another read. Check the input pipe for buffered bytes as well before treating it as empty.

The connection exposes `connection.StreamLimits.InboundBidirectionalStreamLimit`, a
conservative cumulative upper bound on the peer's bidirectional stream allowance. It grows as
inbound bidirectional streams begin disposal, unlike the fixed concurrent limit configured by
`MaxBidirectionalStreamCount`. Set that option before binding the listener; the bound listener
uses its captured value for both native handshakes and the facet. Retain a local storage cap when
using the facet to validate references, because pending disposals may include unadvertised credit.

Listener options expose `HandshakeTimeout` (ten seconds by default); `Timeout.InfiniteTimeSpan`
disables that timeout. See the [design](design.md#the-inbound-handshake-timeout-is-configurable).

## Usage

```csharp
using System;
using System.IO.Pipelines;
using System.Net;

using Assimalign.Cohesion.Connections;
using Assimalign.Cohesion.Connections.Quic;

QuicConnectionListenerOptions options = new();
options.EndPoint = new IPEndPoint(IPAddress.Loopback, 4433);
options.ServerAuthenticationOptions.ServerCertificate = certificate;

// Bound each incoming handshake (ten seconds by default). InfiniteTimeSpan disables this timeout.
options.HandshakeTimeout = TimeSpan.FromSeconds(5);

// Disposing a connection waits this long, at most, for its streams' data and FINs to be
// acknowledged before it closes the connection (five seconds by default).
options.CloseDeliveryTimeout = TimeSpan.FromSeconds(2);

await using QuicConnectionListener listener = new(options);
await listener.BindAsync(cancellationToken);

// CreateAsync(options, cancellationToken) remains available as construct-and-bind shorthand.
IMultiplexedConnection connection = await listener.AcceptAsync(cancellationToken);
IMultiplexedStream stream = await connection.AcceptStreamAsync(cancellationToken);

// End this side gracefully (FIN). The stream stays readable: the peer may keep sending
// until it ends its own side.
await stream.Output.CompleteAsync();
ReadResult rest = await stream.Input.ReadAsync(cancellationToken);

// Or refuse the rest of the peer's data with a code of the protocol's choosing (STOP_SENDING).
// Completing Input without one stops it with DefaultStreamErrorCode. Once both halves are
// complete the QUIC stream is released.
if (stream.DirectionalAbort is { } abort)
{
    abort.AbortRead(0x100); // H3_NO_ERROR
}

await stream.Input.CompleteAsync();

// Or reset the whole stream and end its lifecycle with one root call.
stream.Abort(0x10c); // H3_REQUEST_CANCELLED; already ended directions retain their first signal.
```

QUIC codes range from 0 to 2^62 - 1. The first connection abort or disposal chooses its close
code, including when disposal is still waiting for stream delivery. A later abort interrupts
that wait with the already chosen code, allowing a shorter shutdown budget to force teardown.
The new stream return types are binary breaks against 10.0.0-preview.1; see the
[design](design.md#breaking-changes) for the migration and accepted breaks.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections`](../../connections/assimalign-cohesion-connections/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections`](../../connections/assimalign-cohesion-connections/index.md) | `CohesionSharedSource` |
| [`Assimalign.Cohesion.Connections.Tls`](../assimalign-cohesion-connections-tls/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections.StreamAbort`](../assimalign-cohesion-connections-streamabort/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections.StreamLimits`](../assimalign-cohesion-connections-streamlimits/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `QuicConnectionFactory` | `src/QuicConnectionFactory.cs` |
| `QuicConnectionListener` | `src/QuicConnectionListener.cs` |
| `QuicConnectionListenerOptions` | `src/QuicConnectionListenerOptions.cs` |
| `QuicMultiplexedConnection` | `src/QuicMultiplexedConnection.cs` |
| `QuicConnectionFactoryOptions` | `src/QuicConnectionFactoryOptions.cs` |

## Sources

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src/Assimalign.Cohesion.Connections.Quic.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Connections/README.md`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src/QuicConnectionFactory.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src/QuicConnectionListener.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src/QuicConnectionListenerOptions.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src/QuicMultiplexedConnection.cs`.

- **Source** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Quic/src/QuicConnectionFactoryOptions.cs`.
