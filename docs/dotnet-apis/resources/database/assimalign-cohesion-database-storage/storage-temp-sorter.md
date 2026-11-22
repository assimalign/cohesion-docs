# StorageTempSorter

A stable external merge sorter over already encoded byte-comparable keys.

Namespace: `Assimalign.Cohesion.Database.Storage`.

## Signature

```csharp
public sealed class StorageTempSorter : IDisposable
{
    public static StorageTempSorter Create(StorageTempStatement statement,
        long memoryGrant, int maximumRecordLength);
    public long MemoryGrant { get; }
    public int FanIn { get; }
    public long RunsCreated { get; private set; }
    public int MergePasses { get; private set; }
    public void Append(ReadOnlySpan<byte> key, ReadOnlySpan<byte> value,
        CancellationToken cancellationToken = default);
    public StorageTempSortReader Complete(CancellationToken cancellationToken = default);
    public void Dispose();
}
```

## Behavior and exceptions

The caller encodes direction, collation and NULL placement in keys. Append copies key/value;
equal keys preserve append order through all merges. Instances require serialized use.
No query operator uses this sorter on this branch.

memoryGrant is 64 KiB through Int32.MaxValue bytes. maximumRecordLength is at least 16 and
bounds key + value + 16 framing bytes; the remaining workspace must support a two-way merge.
Invalid bounds throw ArgumentOutOfRangeException; a null statement throws ArgumentNullException.
FanIn is derived from grant and record size, capped at 16. Fixed buffers fit the grant; the
FileStream objects, handles, paths and run registry entries grow with live runs outside it.
At most 64 × FanIn runs are live per sorter, plus the directory lease; concurrent statements add up.

RunsCreated counts initial sorted runs; MergePasses is the maximum stages traversed by an output
row. Complete transfers at most FanIn runs to a reader retaining count × (8192 + maximum record
length) bytes. The final merge streams without writing another file. Dispose deletes remaining
intermediates; a successfully returned reader owns its runs.

Append after Complete, or repeated Complete, throws InvalidOperationException; disposed owners
throw ObjectDisposedException. StorageTempException covers quotas, floor, integrity, unavailable
I/O and oversized encoded records (CK05006). Cancellation/failure disposes intermediate files.
Disposal continues after close failures and can throw AggregateException.

## Related reference

[Statement](storage-temp-statement.md) · [Sort reader](storage-temp-sort-reader.md) · [Temporary exception](storage-temp-exception.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/StorageTempSorter.cs`.

