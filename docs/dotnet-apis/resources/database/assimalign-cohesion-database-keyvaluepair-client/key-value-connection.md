# KeyValueConnection

A typed byte-key rental over one authenticated pooled session.

Namespace: `Assimalign.Cohesion.Database.KeyValuePair.Client`.

## Signature

```csharp
public sealed class KeyValueConnection : IAsyncDisposable
{
    public string Database { get; }
    public bool IsOpen { get; }
    public ValueTask<KeyValueClientEntry?> GetAsync(ReadOnlyMemory<byte> key, CancellationToken cancellationToken = default);
    public ValueTask<long> PutAsync(ReadOnlyMemory<byte> key, ReadOnlyMemory<byte> value, CancellationToken cancellationToken = default);
    public ValueTask<KeyValueWriteResult> PutAsync(ReadOnlyMemory<byte> key, ReadOnlyMemory<byte> value,
        KeyValueWriteCondition condition, CancellationToken cancellationToken = default);
    public ValueTask<bool> TryDeleteAsync(ReadOnlyMemory<byte> key, CancellationToken cancellationToken = default);
    public ValueTask<bool> TryDeleteAsync(ReadOnlyMemory<byte> key, long expectedETag, CancellationToken cancellationToken = default);
    public ValueTask<bool> ExistsAsync(ReadOnlyMemory<byte> key, CancellationToken cancellationToken = default);
    public ValueTask<IReadOnlyList<KeyValueClientEntry>> ScanAsync(KeyValueScanRange? range = null,
        CancellationToken cancellationToken = default);
    public ValueTask<object?> InvokeFunctionAsync(KeyValueFunctionName name,
        IReadOnlyList<object?> arguments, CancellationToken cancellationToken = default);
    public ValueTask DisposeAsync();
}
```

## Behavior and exceptions

KeyValueClient.ConnectAsync rents a connection. Get returns null for absence; unconditional Put
returns the new etag. Conditional misses are KeyValueWriteResult or false outcomes. Scan materializes
its ordered, bounded result. One command runs at a time; overlap throws InvalidOperationException.
Cancellation throws OperationCanceledException and invalidates an unfinished exchange. Model failures
use KeyValueClientException; retryable write conflicts are ExecutionFailure.

Typed entry/write outcomes have no Diagnostics member. KeyValueClientException.Diagnostics is
always empty and non-null; this wire family keeps engine codes in Error text. Embedded command
and row results use DatabaseResult.Diagnostics, also non-null and empty when nothing is reported.

`KeyValueScanRange.Prefix`, `Start` and `End` are nullable `ReadOnlyMemory<byte>` properties.
Null or zero-length bounds mean absent, including an implicitly converted null `byte[]`.
The client omits their clauses and parameters. Combining a non-empty prefix with a non-empty
start or end throws `ArgumentException` when the returned task is awaited.

The shared pool reuses connectors through fresh DatabaseConnection leases. Disposing this wrapper
permanently rejects later commands with ObjectDisposedException, and repeated disposal is a no-op
that cannot return a later rental. Database is cached and stays readable; IsOpen returns false.
The shared lease cancels and joins admitted work before returning a healthy session or closing it.

SQL, Graph and Documents negotiate server-reported transaction status and retained session state. Pool return rolls back active/failed work, then runs the family reset only if Ready still reports retained state (SQL uses DISCARD ALL). Reuse requires Idle and no retained state. Missing/invalid status, failed reset or a legacy transaction-capable peer causes connector discard.

## Example

With a connected client, the CRUD test's operation sequence is:

```csharp
await using var connection = await client.ConnectAsync();
long etag = await connection.PutAsync("user:1"u8.ToArray(), "ada"u8.ToArray());
var entry = await connection.GetAsync("user:1"u8.ToArray());
Console.WriteLine(entry!.Value.ETag == etag); // True
Console.WriteLine(await connection.TryDeleteAsync("user:1"u8.ToArray())); // True
Console.WriteLine(await connection.GetAsync("user:1"u8.ToArray()) is null); // True
```

For a connection containing the test keys `a`, `b`, `user:1`, `user:2`, `z`:

```csharp
var rows = await connection.ScanAsync(new KeyValueScanRange
{
    Prefix = "user:"u8.ToArray(),
    Start = Array.Empty<byte>(),
    End = Array.Empty<byte>()
});
Console.WriteLine(rows.Count); // 2; command sent: SCAN PREFIX @prefix
```

## Related reference

[Key-Value client](index.md) · [Design](design.md) · [Shared lease](../assimalign-cohesion-database-client/database-connection.md)

## Queued terminal errors

Before the next admitted operation writes a request, the shared connector checks buffered input or
a closed transport and consumes a queued terminal Error once. Its original code reaches
KeyValueClientException and the connector is discarded. This also applies to streaming startup;
IsOpen continues to describe physical transport health. Unexpected/malformed terminal frames use
ProtocolViolation; clean EOF without an Error remains Internal. Rent-time pings are not implemented.
A clean handshake EOF has its own message naming the startup handshake. See
[shared admission](../assimalign-cohesion-database-client/database-connection.md#queued-terminal-errors)
and [terminal delivery/TLS limits](../assimalign-cohesion-database-protocol/design.md#terminal-delivery-and-frame-writes).

## Registered CALL functions

InvokeFunctionAsync encodes CALL name @p0 @p1 ... and returns one declared primitive or NULL.
Names use KeyValuePair.Protocol.KeyValueFunctionName; values use exact native primitives with
no numeric widening. Binary inputs are copied before the exchange and outputs are caller-owned
byte arrays. No functions are installed by a client; the server engine builder registers them.

Null argument lists throw ArgumentNullException; default names/unsupported primitive values
throw ArgumentException. Pre-encoding cancellation preserves the caller's token and leaves the
rental untouched; admitted cancellation follows the connection policy. Disposed rentals throw
ObjectDisposedException, overlap InvalidOperationException, and server/malformed scalar-result
failures KeyValueClientException. Completed function ExecutionFailure keeps the session usable.

With the tests' server-registered increment(long) callback:

```csharp
object? result = await connection.InvokeFunctionAsync("InCrEmEnT", new object?[] { 41L });
Console.WriteLine(result); // 42
```

[CALL](../../../../database/key-value-pair/commands/call.md) lists NULL and resolution failures.

## Sources

- **Terminal client conformance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Client/tests/KeyValueClientTerminalErrorTests.cs`.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Client/src/KeyValueConnection.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Client/tests/KeyValueClientTests.cs`.
