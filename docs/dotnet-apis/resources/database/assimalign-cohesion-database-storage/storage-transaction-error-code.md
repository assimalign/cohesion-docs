# StorageTransactionErrorCode

Identifies a storage transaction violation without parsing its message.

Namespace: `Assimalign.Cohesion.Database.Storage`.

## Signature

```csharp
public enum StorageTransactionErrorCode
{
    Unspecified = 0,
    ForeignStorage = 1
}
```

## Behavior and exceptions

StorageTransactionException.Code is Unspecified for a general violation and ForeignStorage when a
mutation receives a bracket from another Storage instance. The enum adds no diagnostic string or
SQLSTATE. Ownership is checked before pins, locks, latch enlistment, pre-images and journal changes.
Equal per-storage sequence numbers do not make brackets interchangeable.

## Example

In the [foreign-bracket example](storage-transaction-exception.md#example):

```csharp
try {
    using var page = second.AllocatePageForWrite(foreign, PageType.Index);
}
catch (StorageTransactionException failure)
    when (failure.Code == StorageTransactionErrorCode.ForeignStorage) {
    Console.WriteLine("Use a transaction from the target storage.");
}
```

## Related reference

[Assembly](index.md) · [StorageTransactionException](storage-transaction-exception.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/Exceptions/StorageTransactionErrorCode.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/tests/StorageTransactionOwnershipTests.cs`.
