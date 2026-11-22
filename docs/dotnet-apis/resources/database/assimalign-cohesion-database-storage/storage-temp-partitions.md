# StorageTempPartitions

Bounded append-only hash partition files for future joins and aggregates.

Namespace: `Assimalign.Cohesion.Database.Storage`.

## Signature

```csharp
public sealed class StorageTempPartitions : IDisposable
{
    public static StorageTempPartitions Create(StorageTempStatement statement,
        int partitionCount, long memoryGrant);
    public int PartitionCount { get; }
    public void Append(ulong hash, ReadOnlySpan<byte> record,
        CancellationToken cancellationToken = default);
    public void Seal(CancellationToken cancellationToken = default);
    public StorageTempRun? GetPartition(int partition);
    public void Dispose();
}
```

## Behavior and exceptions

Instances require serialized use. partitionCount must be positive and memoryGrant at least
partitionCount × (8192 + 512) bytes. Invalid values throw ArgumentOutOfRangeException; a null
statement throws ArgumentNullException. Files are lazy. Append copies records to hash modulo
PartitionCount; equal join/group keys must supply equal hashes. Consumers own skew, repartition
and match-flag policies. No query operator uses these partitions yet.

Seal flushes files; empty partitions stay absent. GetPartition returns the sealed run or null
without transferring ownership. An invalid ordinal throws ArgumentOutOfRangeException; access
before Seal or append after Seal throws InvalidOperationException. Disposed owners throw
ObjectDisposedException; quota/floor/I/O failures throw StorageTempException. Cancellation throws
OperationCanceledException. Failed append/seal disposes owned runs. Dispose deletes every partition
and releases quota, continuing after close failures before reporting AggregateException.

## Related reference

[Statement](storage-temp-statement.md) · [Run](storage-temp-run.md) · [Temporary exception](storage-temp-exception.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/StorageTempPartitions.cs`.

