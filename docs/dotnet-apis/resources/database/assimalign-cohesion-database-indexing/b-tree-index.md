# BTreeIndex

Latest live-entry lookup and snapshot-visible range cursors.

Namespace: `Assimalign.Cohesion.Database.Indexing`.

## Signature

```csharp
public sealed class BTreeIndex
{
    public const int MaxKeyLength = 1024;
    public const int MaxPayloadLength = 1024;
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
| IndexCorruptionException | Lookup reaches a damaged tree page (CK06102) |

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

## Leaf payloads

```csharp
public ValueTask InsertAsync(TransactionContext transaction, IndexKey key, ulong entryReference,
    ReadOnlyMemory<byte> payload, CancellationToken cancellationToken = default);
public ValueTask InsertVersionAsync(StorageTransaction transaction, IndexKey key, ulong entryReference,
    TransactionSequence writer, TransactionSequence deleter, ReadOnlyMemory<byte> payload,
    CancellationToken cancellationToken = default);
public ValueTask<bool> ReplacePayloadAsync(StorageTransaction transaction, IndexKey key,
    ulong entryReference, ReadOnlyMemory<byte> payload, CancellationToken cancellationToken = default);
```

The original no-payload insert overloads remain. Payload is opaque and copied during the call;
MaxPayloadLength is 1,024 bytes independently of MaxKeyLength. Empty payload adds no entry bytes.
Order, uniqueness, entry identity and internal separators ignore it. InsertAsync retains unique-key
checks; the InsertVersionAsync builder still owns uniqueness validation for explicit stamps.

ReplacePayloadAsync targets the newest key/reference mapping in the caller-owned physical bracket.
It returns false if absent and true for an accepted replacement, including identical bytes. A
nonzero writer **or deleter** refuses with IndexException: replacement requires a live physical
entry with zero stamps. Such an entry is unversioned: replacement is visible at latch release,
a snapshot does not pin the payload, and rolling back the bracket restores it.

An identical replacement still takes the page write lock and can incur a pre-image/WAL; it skips
MarkDirty when equal. Equal-length replacement overwrites in place. Growth can compact or split,
keeping the old entry until replacement fits. Cancellation is honored before the page change and
between splits. A failed split may leave structural changes that misroute seeks until rollback;
the caller must roll back its physical bracket after failure.

Null transaction throws ArgumentNullException; oversized payload or duplicate insert identity
throws IndexException. ReplacePayloadAsync also uses IndexException for stamped mappings.
A competing physical page write lock throws StorageTransactionException even for identical bytes.
Cancellation throws OperationCanceledException. No new diagnostic identity is allocated. See
[BTreeCursor.CurrentPayload](b-tree-cursor.md) and [page format 3](design.md#page-format-3).

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

## Bounded range counts

```csharp
public BTreeRangeCount CountRange(TransactionSnapshot snapshot, IndexKeyRange range,
    int entryLimit = 512, int pageLimit = 32, CancellationToken cancellationToken = default);
```

Counts visible entries without retaining keys under the tree read latch. Physical limits include invisible entries, stopping keys, descent pages and empty leaves. Limits below one throw ArgumentOutOfRangeException; cancellation and tree corruption retain their usual exceptions. Keep the caller's snapshot pinned. The returned public readonly record struct BTreeRangeCount has positional members Count (long), IsComplete (bool), EntriesExamined (int) and PagesVisited (int). IsComplete=false identifies a lower bound. See BTreeRangeCountTests in the source branch.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Indexing/src/BTreeIndex.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Indexing/src/BTreeIndexManagerOptions.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Indexing/tests/BTreeIndexTests.cs`.
