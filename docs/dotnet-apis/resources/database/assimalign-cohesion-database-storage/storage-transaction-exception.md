# StorageTransactionException

Reports invalid storage-bracket use, including foreign-storage ownership.

Namespace: `Assimalign.Cohesion.Database.Storage`.

## Signature

```csharp
public sealed class StorageTransactionException : StorageException
{
    public StorageTransactionException(string message);
    public StorageTransactionException(StorageTransactionErrorCode code, string message);
    public StorageTransactionErrorCode Code { get; }
}
```

## Behavior and exceptions

The message-only constructor sets Code to Unspecified. The coded constructor preserves the given
reason and message, adding no argument-validation exception. ForeignStorage means the supplied
bracket belongs to another Storage instance; the refusal names both storages. Validation occurs
before latch enlistment, pins, locks, pre-images or journal changes. Completed brackets and page
write conflicts remain general transaction violations. This code is an enum, with no SQLSTATE.

Page locks identify the bracket object, including its immutable Storage owner. Commit/rollback
always route to that owner. A refused foreign mutation leaves both file sets unchanged and does
not release a local bracket's locks.

Nested mutable scopes for one page on another thread also throw this exception before exposing
mutable bytes. A sole mutable handle may move across serialized continuations; moving it to
another thread while a nested scope remains open is refused. See [mutable handle ownership](design.md#confirmed-reads-and-record-space-reuse).

## Example

Adapted from StorageTransactionOwnershipTests, with two open model Storage facades first and second:

```csharp
using var foreign = first.BeginTransaction();
try {
    using var page = second.AllocatePageForWrite(foreign,
        Assimalign.Cohesion.Database.Storage.PageType.Index);
}
catch (StorageTransactionException failure) {
    Console.WriteLine(failure.Code); // ForeignStorage
}
foreign.Rollback(); // ends only first's bracket
```

No page is allocated in second by the refused call.

## Related reference

[Assembly](index.md) · [Error codes](storage-transaction-error-code.md) · [Ownership](design.md#storage-transaction-ownership)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/Exceptions/StorageTransactionException.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/tests/StorageTransactionOwnershipTests.cs`.
