# DatabaseConnection

One rental lease over an authenticated, family-bound pooled session.

Namespace: `Assimalign.Cohesion.Database.Client`.

## Signature

```csharp
public sealed class DatabaseConnection : IAsyncDisposable
{
    public string Database { get; }
    public string Principal { get; }
    public ProtocolVersion ServerVersion { get; }
    public ProtocolMessageFamily Family { get; }
    public IReadOnlyList<string> Capabilities { get; }
    public bool IsOpen { get; }
    public ValueTask<TResult> ExecuteAsync<TResult>(DatabaseProtocolExchange<TResult> exchange,
        CancellationToken cancellationToken = default);
    public ValueTask<Stream> ExecuteStreamingAsync(DatabaseStreamingExchange exchange,
        CancellationToken cancellationToken = default);
    public ValueTask AbortAsync();
    public ValueTask DisposeAsync();
}
```

## Behavior and exceptions

The shared handshake advertises the selected family's capabilities. Ready applies the negotiated
version and validated intersection. Empty Ready means 2.0 with no capabilities; different majors,
unsolicited agreements, invalid names or malformed UTF-8 fail the handshake and discard the
transport. Failures surface as DatabaseClientException with the wire category; cancellation remains
OperationCanceledException. Each model client selects its family at composition time.

Every DatabaseClient.RentAsync returns a new public lease. An internal connector holds the pooled
transport and authenticated session. Disposing the lease permanently ends it, even if that session
is rented again. Database, Principal, ServerVersion, Family, Capabilities and execution methods
then throw ObjectDisposedException; IsOpen returns false. Even an invalid exchange argument on a
disposed lease is rejected as disposed before argument validation.

One exchange runs at a time; overlapping admission throws InvalidOperationException. A null
exchange throws ArgumentNullException and a different family instance throws ArgumentException.
Disposal cancels and joins admitted work before pooling a healthy connector or closing a broken
one. Repeated DisposeAsync or AbortAsync awaits the original return, including while it is still
running, and cannot touch a later rental. AbortAsync on a live lease discards its session.
If a cancellation callback throws, disposal still joins the exchange and releases the pool slot;
the teardown failure (including AggregateException for callback failures) reaches every disposer
awaiting that original return.

SQL, Graph and Documents negotiate server-reported transaction status and retained session state. Pool return rolls back active/failed work, then runs the family reset only if Ready still reports retained state (SQL uses DISCARD ALL). Reuse requires Idle and no retained state. Missing/invalid status, failed reset or a legacy transaction-capable peer causes connector discard.

Pool shutdown closes idle connectors and connectors returned during shutdown. Waiting renters fail
with ObjectDisposedException without waiting for outstanding leases to return.

## Queued terminal errors

Under the lease's single-operation admission, the connector checks buffered input or a closed
transport before writing a new model request. It consumes a queued shared terminal Error once,
retains its code in DatabaseClientException and discards the connector. Streaming startup uses
the same gate and preflight. IsOpen remains false for a physically closed transport; logical
admission permits this final Error read. No I/O runs under the lease lock, and buffer inspection
preserves consumed/examined positions for fragmented frames. Unexpected or malformed terminal
frames use ProtocolViolation; clean EOF without an Error remains Internal. Rent-time pings are
not implemented.

During startup, clean EOF reports Internal with the message: "The server closed the connection
during the startup handshake before sending the expected response." Failed handshakes release
the pool slot. TLS terminal delivery remains limited by #1086; see
[the protocol close contract](../assimalign-cohesion-database-protocol/design.md#terminal-delivery-and-frame-writes).

## Example

With an authenticated connection rented from the shared client:

```csharp
Console.WriteLine(connection.ServerVersion); // 2.1 against a current endpoint
foreach (string name in connection.Capabilities) Console.WriteLine(name);
// SQL/GQL can include diagnostics; Documents, Blob and Key-Value do not.
```

With a client configured for a one-session pool, the lease regression tests check this lifetime:

```csharp
var first = await client.RentAsync();
await first.DisposeAsync();
await using var next = await client.RentAsync();
Console.WriteLine(ReferenceEquals(first, next)); // False, even if the session was reused
Console.WriteLine(first.IsOpen);                 // False
await first.DisposeAsync();                    // joins the original return
await first.AbortAsync();                      // next remains usable
// Reading first.Database now throws ObjectDisposedException.
```

## Related reference

[Assembly](index.md) · [Negotiation](../assimalign-cohesion-database-protocol/design.md)

[SqlConnection](../assimalign-cohesion-database-sql-client/sql-connection.md) ·
[DocumentConnection](../assimalign-cohesion-database-documents-client/document-connection.md) ·
[GraphConnection](../assimalign-cohesion-database-graph-client/graph-connection.md) ·
[KeyValueConnection](../assimalign-cohesion-database-keyvaluepair-client/key-value-connection.md) ·
[BlobConnection](../assimalign-cohesion-database-blob-client/blob-connection.md)

## Sources

- **Terminal/EOF conformance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/tests/DatabaseClientTerminalErrorTests.cs` and `DatabaseClientHandshakeFailureTests.cs` in the same directory.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/src/DatabaseConnection.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/tests/ProtocolNegotiationTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/tests/DatabaseConnectionLeaseTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/tests/DatabaseClientShutdownTests.cs`.
