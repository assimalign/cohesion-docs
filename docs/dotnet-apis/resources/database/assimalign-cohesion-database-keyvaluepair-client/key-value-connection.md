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
    public ValueTask DisposeAsync();
}
```

## Behavior and exceptions

KeyValueClient.ConnectAsync rents a connection. Get returns null for absence; unconditional Put
returns the new etag. Conditional misses are KeyValueWriteResult or false outcomes. Scan materializes
its ordered, bounded result. One command runs at a time; overlap throws InvalidOperationException.
Cancellation throws OperationCanceledException and invalidates an unfinished exchange. Model failures
use KeyValueClientException; retryable write conflicts are ExecutionFailure.

`KeyValueScanRange.Prefix`, `Start` and `End` are nullable `ReadOnlyMemory<byte>` properties.
Null or zero-length bounds mean absent, including an implicitly converted null `byte[]`.
The client omits their clauses and parameters. Combining a non-empty prefix with a non-empty
start or end throws `ArgumentException` when the returned task is awaited.

The shared pool reuses connectors through fresh DatabaseConnection leases. Disposing this wrapper
permanently rejects later commands with ObjectDisposedException, and repeated disposal is a no-op
that cannot return a later rental. Database is cached and stays readable; IsOpen returns false.
The shared lease cancels and joins admitted work before returning a healthy session or closing it.

The shared pool does not reset explicit transactions (known gap #1540): end or roll back any explicit
transaction before returning its connection. This typed client's wire transactions remain unsupported.

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

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Client/src/KeyValueConnection.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Client/tests/KeyValueClientTests.cs`.
