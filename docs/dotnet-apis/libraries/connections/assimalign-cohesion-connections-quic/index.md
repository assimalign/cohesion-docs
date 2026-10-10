# Assimalign.Cohesion.Connections.Quic

Adapts QUIC connections and streams to Cohesion multiplexed connection contracts.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Connections](../index.md)

## Scope

Each QUIC stream surfaces as a `Connection` with an explicit direction. The driver owns connection
and stream lifecycle, while stream typing and HTTP settings belong above it. Availability follows
`System.Net.Quic`, so callers must account for platform support.

`QuicMultiplexedConnection` reports its TLS 1.3 handshake (the ALPN protocol, the cipher suite, the
peer certificate) through the contracts' `ITlsConnectionInfo`, so HTTP/3 can show the session to
handlers. See the [design](design.md#handshake-facts).

A stream's `ConnectionClosed` also fires when the peer abandons the stream (`RESET_STREAM` or
`STOP_SENDING`) or the QUIC connection is lost, which is how an HTTP/3 server fires
`RequestCancelled` for a request its client cancelled (#1329). See the
[design](design.md#connectionclosed-on-an-abandoned-stream).

The connection and its streams carry the caller's QUIC application error codes through the
contracts' `IMultiplexedConnectionAbort` (`CONNECTION_CLOSE`) and `IMultiplexedStreamAbort`
(`STOP_SENDING`, `RESET_STREAM`) (#1080). The options' default codes, which match the default HTTP/3
ALPN, apply only where the caller gives none. See the
[design](design.md#application-error-codes).

## Usage

```csharp
using System.Net;

using Assimalign.Cohesion.Connections;
using Assimalign.Cohesion.Connections.Quic;

QuicConnectionListenerOptions options = new();
options.EndPoint = new IPEndPoint(IPAddress.Loopback, 4433);
options.ServerAuthenticationOptions.ServerCertificate = certificate;

await using QuicConnectionListener listener = new(options);
await listener.BindAsync(cancellationToken);

// CreateAsync(options, cancellationToken) remains available as construct-and-bind shorthand.
IMultiplexedConnection connection = await listener.AcceptAsync(cancellationToken);
IConnection stream = await connection.AcceptStreamAsync(cancellationToken);

// Refuse the rest of the peer's data with a code of the protocol's choosing (STOP_SENDING),
// then end this side gracefully.
if (stream is IMultiplexedStreamAbort abort)
{
    abort.AbortRead(0x100); // H3_NO_ERROR
}

await stream.Output.CompleteAsync();
```

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections`](../../connections/assimalign-cohesion-connections/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Connections`](../../connections/assimalign-cohesion-connections/index.md) | `CohesionSharedSource` |

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
