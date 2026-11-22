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
    public long Position { get; }
    public void Append(ReadOnlySpan<byte> record, CancellationToken cancellationToken = default);
    public void Seal(CancellationToken cancellationToken = default);
    public StorageTempRunReader OpenReader(long memoryGrant);
    public StorageTempRunReader OpenReader(long memoryGrant, long position);
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

## Saved positions

Save Position immediately before Append and pass that opaque value to the two-argument
OpenReader after sealing. It seeks to that record without scanning earlier records, including
headers or payloads crossing pages. The end position is valid and reads EOF. Positions belong
only to their run; do not calculate offsets. A value that is not a saved record boundary has
undefined behavior: it can refuse as damage or return misframed records.

The reader validates every visited page's identity, ordinal, used length and checksum, plus
tail and EOF. A failed read permanently poisons that reader. Position throws
ObjectDisposedException after run/statement disposal. OpenReader requires at least 8,192 B,
a sealed live run, and a position from zero through the end; invalid grants/ranges throw
ArgumentOutOfRangeException, unsealed runs InvalidOperationException. No format version changes.

## Related reference

[Statement](storage-temp-statement.md) · [Reader](storage-temp-run-reader.md) · [Temporary storage](design.md#disposable-query-storage)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/StorageTempRun.cs`.

