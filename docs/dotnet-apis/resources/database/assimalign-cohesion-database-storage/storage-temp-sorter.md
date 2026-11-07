# StorageTempSorter

A stable external merge sorter over already encoded byte-comparable keys.

Namespace: `Assimalign.Cohesion.Database.Storage`.

## Signature

```csharp
public sealed class StorageTempSorter : IDisposable
{
    public static StorageTempSorter Create(StorageTempStatement statement,
        long memoryGrant, int maximumRecordLength);
    public static StorageTempSorter Create(StorageTempStatement statement,
        long memoryGrant, int maximumRecordLength, int maximumOpenRuns);
    public static StorageTempSorter CreateWithSortedPrefix(StorageTempStatement statement,
        long memoryGrant, int maximumRecordLength,
        IEnumerable<(ReadOnlyMemory<byte> Key, ReadOnlyMemory<byte> Value)> sortedPrefix,
        CancellationToken cancellationToken = default);
    public static StorageTempSorter CreateWithSortedPrefix(StorageTempStatement statement,
        long memoryGrant, int maximumRecordLength,
        IEnumerable<(ReadOnlyMemory<byte> Key, ReadOnlyMemory<byte> Value)> sortedPrefix,
        int maximumOpenRuns, CancellationToken cancellationToken = default);
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
Eligible SQL root ORDER BY and ordered DISTINCT, including CTAS source roots, use this sorter.

memoryGrant is 64 KiB through Int32.MaxValue bytes. maximumRecordLength is at least 16 and
bounds key + value + 16 framing bytes; the remaining workspace must support a two-way merge.
Invalid bounds throw ArgumentOutOfRangeException; a null statement throws ArgumentNullException.
FanIn is derived from grant and record size, capped at 16. Fixed buffers fit the grant; the
FileStream objects, handles, paths and run registry entries grow with live runs outside it.
The maximumOpenRuns overload bounds simultaneously open runs, including a merge-output slot;
it must be at least FanIn + 1. Overloads without it default to 1,024. SQL sets 64 per sort; staged
input/output runs and the directory lease are additional, and concurrent statements add up.

RunsCreated counts initial sorted runs; MergePasses is the maximum stages traversed by an output
row. Complete transfers at most FanIn runs to a reader retaining count × (8192 + maximum record
length) bytes. The final merge streams without writing another file. Dispose deletes remaining
intermediates; a successfully returned reader owns its runs.

Append after Complete, or repeated Complete, throws InvalidOperationException; disposed owners
throw ObjectDisposedException. StorageTempException covers quotas, floor, integrity, unavailable
I/O and oversized encoded records (CK05006). Cancellation/failure disposes intermediate files.
Disposal continues after close failures and can throw AggregateException.

## CreateWithSortedPrefix

Drains ascending byte keys with stable ties into the first run before allocating the arena.
The prefix enumeration must release retained input as it advances and before completion.
Equal prefix keys precede later appends. Cancellation is checked before each record and arena
allocation. Null statement/prefix throws ArgumentNullException; invalid grant, record bound or
run ceiling throws ArgumentOutOfRangeException. Temp refusal retains StorageTempException;
failure/cancellation disposes the prefix run.

Complete returns the owning merge reader. For independent cursors, use that reader's
[OpenReader](storage-temp-sort-reader.md#openreader), keeping the owner alive and unstarted.

## Related reference

[Statement](storage-temp-statement.md) · [Sort reader](storage-temp-sort-reader.md) · [Temporary exception](storage-temp-exception.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/StorageTempSorter.cs`.

