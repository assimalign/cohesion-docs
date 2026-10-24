# StorageCorruptionException

Reports an integrity violation in a storage file, with a stable code and optional page location.

Namespace: `Assimalign.Cohesion.Database.Storage`.

## Signature

```csharp
public sealed class StorageCorruptionException : StorageException
{
    public const string ErrorCode = "CK06003";
    public StorageCorruptionException(string message);
    public StorageCorruptionException(PageId pageId, string message);
    public StorageCorruptionException(PageId pageId, string message, Exception inner);
    public StorageCorruptionException(PageId pageId, int slotIndex, string message,
        Exception? inner = null);
    public int? SlotIndex { get; }
    public string Code { get; }
    public PageId? PageId { get; }
    public static StorageCorruptionException? Find(Exception? error);
}
```

## Behavior and exceptions

Code always returns ErrorCode. PageId is null for the message-only constructor and otherwise
retains the supplied page. Examples of detected damage include a checksum mismatch, a nonzero
Free page, a nonzero page with no checksum, a malformed header and a broken recovery LSN chain.
The storage exception has no SQLSTATE member; SQL and Key-Value Pair map CK06003 to XX001
at the engine boundary. Constructors add no validation exceptions.

Find returns the first storage corruption in the supplied exception, its inner chain or any
AggregateException branch; it returns null for null or no match. Its iterative, reference-based
visited set handles deep/shared chains without a recursive depth cutoff.

Storage users can catch this child-kernel exception directly. Engine operations translate it to
[DatabaseCorruptionException](../assimalign-cohesion-database/database-corruption-exception.md),
retaining the cause and adding database context. Corruption fails the operation; it is never a
successful missing-record result.

The page-scoped overload with inner retains the underlying cause. If a header half is unreadable
and the other has a damaged slot with no valid survivor, PageId is zero and Code is CK06003;
Find still discovers it through an aggregate. Two unreadable halves or a valid survivor without
journal coverage instead preserve I/O classification. Caller-owned Page span access also throws
this exception for out-of-range overflow lengths. See [recovery](design.md#page-0-and-alternating-headers).

## Example

```csharp
using System;
using Assimalign.Cohesion.Database.Storage;

var readError = new System.IO.IOException("Header half unreadable.");
var cause = new StorageCorruptionException((PageId)0L, "The other header slot is damaged.", readError);
var wrapped = new AggregateException(new InvalidOperationException("Cleanup failed."), cause);
Console.WriteLine(StorageCorruptionException.Find(wrapped) == cause); // True
Console.WriteLine(cause.Code); // CK06003
Console.WriteLine((long)cause.PageId!.Value); // 0
Console.WriteLine(cause.InnerException == readError); // True
```

## Related reference

[Assembly](index.md) · [Page integrity and format](design.md#storage-format-4) · [DatabaseCorruptionException](../assimalign-cohesion-database/database-corruption-exception.md)

## Confirmed record failures

The record-scoped overload retains PageId, SlotIndex and the optional inner decoder failure. Its contextual message names the page and slot without adding the code prefix. Other constructors leave SlotIndex null. Model decoders use this only after confirming the private record copy against the page write sequence; an overlapping mutation retries before a decoder failure is accepted. CK06003 remains the identity.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/Exceptions/StorageCorruptionException.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/tests/StoragePageIntegrityTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database/tests/DatabaseCorruptionTests.cs`.
