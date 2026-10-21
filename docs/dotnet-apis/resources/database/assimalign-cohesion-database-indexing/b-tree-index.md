# BTreeIndex

Latest live-entry lookup and snapshot-visible range cursors.

Namespace: `Assimalign.Cohesion.Database.Indexing`.

## Signature

```csharp
public sealed class BTreeIndex
{
    public const int MaxKeyLength = 1024;
    public bool IsUnique { get; }
    public ValueTask<ulong?> FindLiveEntryAsync(
        TransactionContext transaction, IndexKey key,
        CancellationToken cancellationToken = default);
}
```

The signature shows the key limit, lookup and its relevant property. BTreeIndexManager creates indexes;
BTreeIndex has no public constructor.

## Behavior and exceptions

FindLiveEntryAsync returns the opaque reference of the latest live entry for the complete key,
or null for an absent key, tombstone or rolled-back insert. A rolled-back delete restores the
original reference. It sees the caller's own writes and applies no snapshot filter.

When the manager supplies a LockManager, the method acquires
`LockResource.Entry(objectId, key.Hash())` in Exclusive mode before taking the tree's read latch.
The transaction holds that key lock until commit/rollback, stabilizing arbitration over committed
or own entries. An already-held lock is re-granted. Call before entering a physical apply bracket,
so a lock wait occurs outside the apply gate. A manager configured without a LockManager omits
that acquisition; callers must provide the coordination needed for latest-state decisions.

| Exception | Trigger |
| --- | --- |
| ArgumentNullException | transaction is null |
| InvalidOperationException | The index is not unique |
| OperationCanceledException | Cancellation before lookup or during key-lock acquisition |
| TransactionDeadlockException | Key-lock acquisition closes a wait cycle |
| IndexCorruptionException | Lookup reaches a damaged tree page (COHDBI002) |

The lookup changes no page layout. SQL uses it for [ON CONFLICT](../../../../database/sql/language/clauses/on-conflict.md)
arbitration and translates kernel failures at its model boundary.

## Encoded key limit

`MaxKeyLength` is the public maximum encoded write-key length in bytes. The count includes all
components, NULL markers and collation transforms; entry-reference and writer-stamp tiebreakers
are stored separately. `InsertAsync` and the stamp-preserving `InsertVersionAsync` accept exactly
1,024 bytes and throw `IndexException` for 1,025 bytes before inserting that key. Models can use
the constant to validate their final write keys before storage mutation.

With an active writer and index from the caller's storage/coordinator composition, this adapts
the boundary test:

```csharp
var accepted = new IndexKey(new byte[BTreeIndex.MaxKeyLength]);
await index.InsertAsync(writer, accepted, 42);
try
{
    await index.InsertAsync(writer, new IndexKey(new byte[BTreeIndex.MaxKeyLength + 1]), 43);
}
catch (IndexException)
{
    Console.WriteLine("Key exceeds 1,024 bytes.");
}
```

Commit the writer: a range cursor returns only the 1,024-byte key with entry reference `42`.

## Range cursors

```csharp
public BTreeCursor OpenCursor(TransactionContext transaction, IndexKeyRange range,
    bool reverse = false, CancellationToken cancellationToken = default);
public BTreeCursor OpenCursor(TransactionSnapshot snapshot, IndexKeyRange range,
    bool reverse = false, CancellationToken cancellationToken = default);
```

Opening captures the snapshot and copies range bounds without reading pages. Both overloads
throw ArgumentNullException for a null transaction/snapshot and OperationCanceledException for a
canceled token. The opening token applies throughout enumeration alongside each MoveNextAsync
token. Page/storage failures surface during moves. Keep the caller's snapshot pin alive, collect
mutation targets before writes, and dispose the cursor on every exit. See
[BTreeCursor](b-tree-cursor.md) for batching, lifetime, exceptions and a range example.

## Lookup example

This adapts the own-write/tombstone assertions from BTreeIndexTests. The caller supplies a unique
index and active transaction from its storage/coordinator composition, then commits or rolls back
that transaction after the helper returns.

```csharp
static async Task InspectAsync(BTreeIndex index, TransactionContext writer,
    CancellationToken cancellationToken)
{
    IndexKey key = IndexKey.FromInt64(42);
    await index.InsertAsync(writer, key, 100, cancellationToken);
    Console.WriteLine(await index.FindLiveEntryAsync(writer, key, cancellationToken)); // 100
    await index.DeleteAsync(writer, key, 100, cancellationToken);
    Console.WriteLine(await index.FindLiveEntryAsync(writer, key, cancellationToken) is null); // True
}
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Indexing/src/BTreeIndex.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Indexing/src/BTreeIndexManagerOptions.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Indexing/tests/BTreeIndexTests.cs`.
