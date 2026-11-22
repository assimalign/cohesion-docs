# StorageTempRunReader

Reads a sealed run with one private page and caller-owned record memory.

Namespace: `Assimalign.Cohesion.Database.Storage`.

## Signature

```csharp
public sealed class StorageTempRunReader : IDisposable
{
    public bool TryRead(Span<byte> destination, out int length,
        CancellationToken cancellationToken = default);
    public void Dispose();
}
```

## Behavior and exceptions

StorageTempRun.OpenReader creates an independent reader. Supply at least MaximumRecordLength
bytes of destination. TryRead returns true for a record, including an empty record, and false
at end; length is zero at end or on a read failure. A short destination throws ArgumentException
without failing the reader. Pages, record boundaries and EOF are validated before accepting data.

Every read failure after argument validation permanently fails this reader, even cancellation
before any bytes are consumed. Integrity failures and subsequent retries use CK05004; retries
perform no I/O and emit no further damage event/counter. Temporary I/O uses CK05005. Cancellation
first throws OperationCanceledException. A disposed reader throws ObjectDisposedException;
disposed run/statement checks fail the reader on their first read, then retries use CK05004.
Dispose releases the private page; other readers can still use the run.

## Related reference

[Run](storage-temp-run.md) · [Temporary exception](storage-temp-exception.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/StorageTempRunReader.cs`.

