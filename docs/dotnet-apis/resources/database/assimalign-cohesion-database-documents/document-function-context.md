# DocumentFunctionContext

Borrowed database and cancellation context without a session or storage handle.

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public readonly ref struct DocumentFunctionContext
{
    public DocumentFunctionContext(DatabaseName database, CancellationToken cancellationToken,
        DateTimeOffset? transactionTimestamp = null);
    public DatabaseName Database { get; }
    public CancellationToken CancellationToken { get; }
    public DateTimeOffset? TransactionTimestamp { get; }
}
```

## Behavior and exceptions

Use the context only during the callback. It exposes no engine, mutable transaction or
transport. A derived core can observe CancellationToken; typed delegates do not receive it.
TransactionTimestamp is the captured transaction start, or null outside a transaction.
Construction/access has no validation exceptions; ThrowIfCancellationRequested throws
OperationCanceledException when the statement token is canceled.

## Example

```csharp
var context = new DocumentFunctionContext(default, CancellationToken.None);
Console.WriteLine(context.CancellationToken.CanBeCanceled); // False
```

## Related reference

[Assembly](index.md) · [DocumentArguments](document-arguments.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Functions/DocumentFunctionContext.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentRegisteredFunctionTests.cs`.
