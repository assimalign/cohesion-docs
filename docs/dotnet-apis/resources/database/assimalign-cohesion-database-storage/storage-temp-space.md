# StorageTempSpace

Owns an engine's disposable query files and accounts physical pages.

Namespace: `Assimalign.Cohesion.Database.Storage`.

## Signature

```csharp
public sealed class StorageTempSpace : IDisposable
{
    public const int PageSize = 8192;
    public static StorageTempSpace Create(FileSystemPath directory, long? engineQuota,
        Func<long> freeSpaceFloor);
    public string DirectoryPath { get; }
    public long EngineQuota { get; private set; }
    public StorageTempStatement CreateStatement(long quota);
    public void Dispose();
}
```

## Behavior and exceptions

Create sweeps owned leftovers when the directory exists; directory creation waits until the first
run. One directory belongs to one spilling engine. Null engineQuota uses min(10 GiB, 25% of free
bytes after sweeping); zero refuses growth. The required freeSpaceFloor callback supplies a
nonnegative reserve as a constant or lock-free snapshot. It must not acquire engine registry
locks or page latches. Every page checks both quotas and this live floor.

An empty/default directory throws ArgumentException, a null callback ArgumentNullException and
negative quotas ArgumentOutOfRangeException. Unavailable temporary storage warns and is retried
by the first run; EngineQuota is zero until an unavailable probe succeeds. DirectoryPath is
normalized when setup succeeds. CreateStatement on a disposed space throws ObjectDisposedException.

Dispose closes all statements and releases the directory lease, continuing after cleanup failures;
failed closes are reported in AggregateException. No journal, recovery, transactions or shared
buffer pool participates. No query operator uses this substrate yet.

## Related reference

[Assembly](index.md) · [Statement](storage-temp-statement.md) · [Directory, permissions and admission](design.md#disposable-query-storage)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/StorageTempSpace.cs`.

