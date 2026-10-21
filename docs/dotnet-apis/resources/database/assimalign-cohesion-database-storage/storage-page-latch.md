# StoragePageLatch

Coordinates structure reads, writes and physical page rollback.

Namespace: `Assimalign.Cohesion.Database.Storage`.

## Signature

```csharp
public sealed class StoragePageLatch
{
    public static StoragePageLatch Create();
    public void EnterRead();
    public void EnterRead(CancellationToken cancellationToken);
    public void ExitRead();
    public void EnterWrite();
    public void ExitWrite();
}
```

## Behavior and exceptions

Readers hold the latch shared; writers and physical rollback hold it exclusively. A structure
passes the same latch to Storage.OpenPageForWrite and AllocatePageForWrite so rollback restores
its pages under that latch. The latch is non-recursive and each hold must exit on its acquiring
thread. Do not await while holding it or roll back an enlisted transaction under a read hold.

EnterRead(CancellationToken) checks cancellation before acquisition and between waits of about
10 milliseconds. Cancellation takes no shared hold, including when it races successful acquisition.
A token that cannot cancel waits indefinitely, as does the original parameterless EnterRead.
The token controls acquisition only; after success the caller releases with ExitRead.

| Exception | Trigger |
| --- | --- |
| OperationCanceledException | Tokened read acquisition is canceled; no hold remains |
| LockRecursionException | The current thread attempts an incompatible recursive acquisition |
| SynchronizationLockException | ExitRead/ExitWrite on a thread without the corresponding hold |

## Example

This follows the successful tokened read path in StoragePageLatchTests:

```csharp
var latch = StoragePageLatch.Create();
latch.EnterRead(cancellationToken);
try
{
    // Read the structure synchronously while writers and rollback are excluded.
}
finally
{
    latch.ExitRead();
}
```

A canceled token throws before entering the try block, so no ExitRead is required. Tests also
cancel queued readers while the writer remains held and verify that no shared hold survives.

## Related reference

[Assembly](index.md) · [Design](design.md) · [BTreeCursor](../assimalign-cohesion-database-indexing/b-tree-cursor.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/StoragePageLatch.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/tests/StoragePageLatchTests.cs`.
