# BTreeCursor

Reads snapshot-visible index entries in bounded forward or reverse batches.

Namespace: `Assimalign.Cohesion.Database.Indexing`.

## Signature

```csharp
public sealed class BTreeCursor : IAsyncDisposable
{
    public IndexKey CurrentKey { get; }
    public ulong CurrentEntryReference { get; }
    public ValueTask<bool> MoveNextAsync(CancellationToken cancellationToken = default);
    public ValueTask DisposeAsync();
}
```

[BTreeIndex.OpenCursor](b-tree-index.md#range-cursors) creates the cursor; there is no public
constructor. Opening copies bounds and captures the supplied snapshot without reading a page.

## Behavior and exceptions

The first batch copies one visible entry; later batches copy at most 64, examining at most 256
retained entries per tree read-latch hold. Invisible entries count toward that work bound;
empty leaves also release the latch between probes. Moves complete synchronously and return
with all page handles and latches released. A reverse cursor descends from its upper bound.
Entries order by key, entry reference and writer stamp, reversed for a reverse scan.

After a batch drains, the cursor resumes by the last examined logical identity, so splits,
erases and physical rollback do not leave a retained page/slot position. Keep the caller's
statement or transaction snapshot pin alive until enumeration ends. The cursor creates no pin.
Do not mutate the scanned index through the snapshot owner during enumeration; collect write
targets before applying changes. One reader is supported; concurrent moves/disposal are unsupported.

Both the opening token and each move's token apply, including buffered reads and waits for a
writer's latch. Any failed move disposes the cursor and releases its owned state. DisposeAsync
is idempotent and drops keys, bounds and snapshot references. A previously retained CurrentKey
owns its bytes and remains valid after later moves or disposal.

| Exception | Trigger |
| --- | --- |
| InvalidOperationException | CurrentKey or CurrentEntryReference before the first successful move or after exhaustion |
| ObjectDisposedException | Move or current-value access after disposal, including a failed move |
| OperationCanceledException | Either token is canceled; the cursor is disposed |
| IndexCorruptionException | A move reaches a damaged index page (CK06102); the cursor is disposed |

Other storage errors propagate from moves and also dispose the cursor. Lazy opening means page
damage is discovered when reading, rather than at OpenCursor.

## Example

With an index seeded with keys/references 0 through 99 and an active reader, this adapts
BTreeCursorTests. Keep reader active until the cursor is disposed:

```csharp
await using (var cursor = index.OpenCursor(reader.Snapshot,
    new IndexKeyRange(IndexKey.FromInt64(50), IndexKey.FromInt64(53), true, false),
    cancellationToken: cancellationToken))
{
    while (await cursor.MoveNextAsync(cancellationToken))
    {
        Console.WriteLine(cursor.CurrentEntryReference);
    }
}
```

The output is 50, 51, 52. Passing reverse: true returns 52, 51, 50, without loading the full range.

## Related reference

[Assembly](index.md) · [BTreeIndex](b-tree-index.md) · [Design](design.md) · [StoragePageLatch](../assimalign-cohesion-database-storage/storage-page-latch.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Indexing/src/BTreeCursor.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Indexing/tests/BTreeCursorTests.cs` and `BTreeCursorAllocationTests.cs`.
