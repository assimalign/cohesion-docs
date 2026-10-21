# StorageCorruptionException

Reports an integrity violation in a storage file, with a stable code and optional page location.

Namespace: `Assimalign.Cohesion.Database.Storage`.

## Signature

```csharp
public sealed class StorageCorruptionException : StorageException
{
    public const string ErrorCode = "COHDBS003";
    public StorageCorruptionException(string message);
    public StorageCorruptionException(PageId pageId, string message);
    public string Code { get; }
    public PageId? PageId { get; }
    public static StorageCorruptionException? Find(Exception? error);
}
```

## Behavior and exceptions

Code always returns ErrorCode. PageId is null for the message-only constructor and otherwise
retains the supplied page. Examples of detected damage include a checksum mismatch, a nonzero
Free page, a nonzero page with no checksum, a malformed header and a broken recovery LSN chain.
No SQLSTATE is assigned to COHDBS003. Constructors add no validation exceptions.

Find returns the first storage corruption in the supplied exception, its inner chain or any
AggregateException branch; it returns null for null or no match. Its iterative, reference-based
visited set handles deep/shared chains without a recursive depth cutoff.

Storage users can catch this child-kernel exception directly. Engine operations translate it to
[DatabaseCorruptionException](../assimalign-cohesion-database/database-corruption-exception.md),
retaining the cause and adding database context. Corruption fails the operation; it is never a
successful missing-record result.

## Example

```csharp
using System;
using Assimalign.Cohesion.Database.Storage;

var cause = new StorageCorruptionException((PageId)9L, "Page 9 failed checksum verification.");
var wrapped = new AggregateException(new InvalidOperationException("Cleanup failed."), cause);
Console.WriteLine(StorageCorruptionException.Find(wrapped) == cause); // True
Console.WriteLine(cause.Code); // COHDBS003
Console.WriteLine((long)cause.PageId!.Value); // 9
```

## Related reference

[Assembly](index.md) · [Page integrity and format](design.md#storage-format-4) · [DatabaseCorruptionException](../assimalign-cohesion-database/database-corruption-exception.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/Exceptions/StorageCorruptionException.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/tests/StoragePageIntegrityTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database/tests/DatabaseCorruptionTests.cs`.
