# StorageStream

Namespace: `Assimalign.Cohesion.Database.Storage`.

```csharp
public sealed class StorageStream : Stream, IFileSystemFileHandle
{
    public StorageStream(Stream innerStream);
    public StorageStream(IFileSystemFileHandle handle);
    public StorageStream(StorageStream stream);
    public bool SupportsDurableFlush { get; }
    public void FlushDurable();
    public void Flush(bool durable);
    public ValueTask FlushAsync(bool durable, CancellationToken cancellationToken = default);
}
```

The adapter is now sealed. Existing constructors, file factories, positional I/O and stream/handle ownership remain unchanged. SupportsDurableFlush reflects the underlying handle; durable flush delegates to it. A plain Stream cannot promise durable persistence. See [file handles and durability](design.md#file-handles-positional-io-and-durability).

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/StorageStream.cs`.
