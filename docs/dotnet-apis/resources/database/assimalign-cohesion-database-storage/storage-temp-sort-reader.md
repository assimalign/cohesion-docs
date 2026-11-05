# StorageTempSortReader

Streams a stable final merge, owning its input runs and borrowed key/value buffers.

Namespace: `Assimalign.Cohesion.Database.Storage`.

## Signature

```csharp
public sealed class StorageTempSortReader : IDisposable
{
    public ReadOnlyMemory<byte> Key { get; private set; }
    public ReadOnlyMemory<byte> Value { get; private set; }
    public bool MoveNext(CancellationToken cancellationToken = default);
    public void Dispose();
}
```

## Behavior and exceptions

StorageTempSorter.Complete returns the reader. MoveNext returns true for a sorted record and
false at end without rewriting the final merge. Key and Value are borrowed until the next
MoveNext or disposal; copy them to retain them. Instances require serialized use. Exhausted input
runs close and return quota as the merge proceeds. Statement disposal also closes all inputs.

Damaged pages/records or temporary I/O throw StorageTempException (CK05004/CK05005). A disposed
reader, statement or input run throws ObjectDisposedException. Cancellation throws
OperationCanceledException and releases owned runs. Dispose releases private buffers and every
owned input, continuing after close failures and then reporting AggregateException.

## Related reference

[Sorter](storage-temp-sorter.md) · [Statement](storage-temp-statement.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/StorageTempSortReader.cs`.

