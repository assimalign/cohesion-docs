# StorageTempSortReader

Streams a stable final merge, owning its input runs and borrowed key/value buffers.

Namespace: `Assimalign.Cohesion.Database.Storage`.

## Signature

```csharp
public sealed class StorageTempSortReader : IDisposable
{
    public ReadOnlyMemory<byte> Key { get; private set; }
    public ReadOnlyMemory<byte> Value { get; private set; }
    public long BufferBytes { get; }
    public StorageTempSortReader OpenReader();
    public bool MoveNext(CancellationToken cancellationToken = default);
    public void Dispose();
}
```

## Behavior and exceptions

StorageTempSorter.Complete returns the reader. MoveNext returns true for a sorted record and
false at end without rewriting the final merge. Key and Value are borrowed until the next
MoveNext or disposal; copy them to retain them. Instances require serialized use. Exhausted input
runs close and return quota as a directly consumed owning merge proceeds. Forked cursors leave
runs owned by the unstarted template. Statement disposal closes all inputs.

Damaged pages/records or temporary I/O throw StorageTempException (CK05004/CK05005). A disposed
reader, statement or input run throws ObjectDisposedException. Cancellation throws
OperationCanceledException and releases owned runs. Dispose releases private buffers and every
owned input, continuing after close failures and then reporting AggregateException.

## OpenReader

Opens an independent cursor over an unstarted owning final merge. Each cursor has private
buffers and requires serialized use; different cursors may advance independently. The first
fork releases the template workspace, leaving it as a file owner. The owner must stay alive
and unstarted until all cursors finish; MoveNext on a forked template refuses. A started reader
or a fork cannot open another cursor (InvalidOperationException). Disposing a cursor releases
only its buffers; disposing the owner or statement invalidates every cursor.

BufferBytes is the page-and-record workspace required by one active cursor:
`run count * (8192 + maximumRecordLength)`. It describes the required buffer size even when
the ownership-only template has released its workspace.

## Related reference

[Sorter](storage-temp-sorter.md) · [Statement](storage-temp-statement.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/StorageTempSortReader.cs`.

