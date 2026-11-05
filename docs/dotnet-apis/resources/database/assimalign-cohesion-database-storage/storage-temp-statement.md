# StorageTempStatement

Owns and accounts all temporary runs of one statement.

Namespace: `Assimalign.Cohesion.Database.Storage`.

## Signature

```csharp
public sealed class StorageTempStatement : IDisposable
{
    public long Quota { get; }
    public long PhysicalBytes { get; }
    public long PeakBytes { get; }
    public StorageTempRun CreateRun(CancellationToken cancellationToken = default);
    public void Dispose();
}
```

## Behavior and exceptions

Created by StorageTempSpace.CreateStatement. Quota is a physical-byte limit; PhysicalBytes
includes unwritten tails, and PeakBytes is the largest simultaneous reservation in this lifetime.
CreateRun creates an empty append-only run, checking cancellation before opening its file.
Unavailable storage throws StorageTempException with CK05005 and the I/O cause. A disposed
statement or space throws ObjectDisposedException; cancellation throws OperationCanceledException.

Dispose deletes all owned runs, including partially written ones, and releases their quotas.
It continues after close failures and throws AggregateException after all runs have been released.
Dispose the statement after success, refusal or cancellation. This is the substrate only;
no query operator uses it yet.

## Related reference

[Space](storage-temp-space.md) · [Run](storage-temp-run.md) · [Temporary refusals](storage-temp-exception.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/StorageTempStatement.cs`.

