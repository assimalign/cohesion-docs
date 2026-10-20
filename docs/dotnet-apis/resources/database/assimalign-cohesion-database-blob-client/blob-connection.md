# BlobConnection

A leased connection bound to one Blob database, allowing one exchange at a time.

Namespace: `Assimalign.Cohesion.Database.Blob.Client`.

## Signature

```csharp
public sealed class BlobConnection : IAsyncDisposable
{
    public string Database { get; }
    public bool IsOpen { get; }
    public ValueTask<long> UploadAsync(string container, string name, Stream source,
        string contentType = "application/octet-stream", long length = -1, bool overwrite = true,
        CancellationToken cancellationToken = default);
    public ValueTask<Stream> DownloadAsync(string container, string name, CancellationToken cancellationToken = default);
    public ValueTask<bool> DeleteAsync(string container, string name, CancellationToken cancellationToken = default);
    public ValueTask<BlobProperties?> GetPropertiesAsync(string container, string name, CancellationToken cancellationToken = default);
    public IAsyncEnumerable<BlobProperties> GetBlobsAsync(string container, string? prefix = null,
        CancellationToken cancellationToken = default);
    public ValueTask DisposeAsync();
}
```

## Behavior and exceptions

BlobClient.ConnectAsync wraps a fresh shared lease. Dispose streams and enumerators before another
operation and dispose connections before the client. Disposal cancels and joins active work, then
returns a healthy session or closes a broken one. Later operations with valid arguments throw
ObjectDisposedException; repeated disposal cannot touch a later rental. Database is captured at
rental time and remains readable after return; IsOpen returns false.

The shared pool does not reset explicit transactions (known gap #1540): end or roll back any
explicit transaction before returning its connection. Blob exposes no explicit wire transaction API.

Null streams throw ArgumentNullException; unreadable streams or invalid lengths throw
ArgumentException. Overlap throws InvalidOperationException; cancellation throws
OperationCanceledException. BlobClientException carries server or transfer failure. Failed or
abandoned transfers discard the lease; subsequent cleanup stays on that original lease.
Properties returns null and Delete returns false for an absent object; a missing download fails.

## Example

With a rented connection, an existing images container and a readable source stream:

```csharp
long committed = await connection.UploadAsync("images", "cover", source, contentType: "image/png");
await using (Stream download = await connection.DownloadAsync("images", "cover"))
{
    await download.CopyToAsync(destination);
}
Console.WriteLine(await connection.DeleteAsync("images", "cover")); // True
```

The copied content is the committed upload. See [streaming usage](../../../../database/blob/streaming-client.md)
for complete setup, bounded buffering and uncertain acknowledgement outcomes.

## Related reference

[Client](blob-client.md) · [Design](design.md) · [Shared lease](../assimalign-cohesion-database-client/database-connection.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Client/src/BlobConnection.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Client/docs/OVERVIEW.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Client/tests/BlobClientTests.cs`.
