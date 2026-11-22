# StorageTempRun

An append-only byte-record run, sealed before opening independent readers.

Namespace: `Assimalign.Cohesion.Database.Storage`.

## Signature

```csharp
public sealed class StorageTempRun : IDisposable
{
    public long RecordCount { get; private set; }
    public int MaximumRecordLength { get; private set; }
    public bool IsSealed { get; }
    public void Append(ReadOnlySpan<byte> record, CancellationToken cancellationToken = default);
    public void Seal(CancellationToken cancellationToken = default);
    public StorageTempRunReader OpenReader(long memoryGrant);
    public void Dispose();
}
```

## Behavior and exceptions

StorageTempStatement.CreateRun supplies the owner. Append copies the record before returning,
including records spanning pages. Empty records and empty runs are valid. Each record has a
four-byte length prefix; checked 8 KiB pages count toward the space and statement quotas.
MaximumRecordLength sizes a caller-owned reader destination. Seal flushes the tail and permits
multiple independent readers. OpenReader needs at least 8192 bytes of memoryGrant for its private page.

Append/Seal cancellation or failure releases the run's pages. Quota, floor and I/O refusals use
StorageTempException; I/O uses CK05005. Append after sealing or reading before sealing throws
InvalidOperationException. An undersized reader grant throws ArgumentOutOfRangeException;
disposed runs throw ObjectDisposedException. Dispose closes/deletes the file and releases quota;
a close I/O failure is CK05005. These are disposable files, independent of durable storage format 4.

## Related reference

[Statement](storage-temp-statement.md) · [Reader](storage-temp-run-reader.md) · [Temporary storage](design.md#disposable-query-storage)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/StorageTempRun.cs`.

