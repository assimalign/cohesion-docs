# GraphFunctionContext

Borrowed database and cancellation context without a session or storage handle.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public readonly ref struct GraphFunctionContext
{
    public GraphFunctionContext(DatabaseName databaseName, DateTimeOffset transactionTimestamp,
        CancellationToken cancellationToken = default);
    public DatabaseName DatabaseName { get; }
    public CancellationToken CancellationToken { get; }
    public DateTimeOffset TransactionTimestamp { get; }
}
```

## Behavior and exceptions

Use the context only during the callback. It exposes no engine, mutable transaction or
transport. A derived core can observe CancellationToken; typed delegates do not receive it.
TransactionTimestamp is the captured transaction instant.
Construction/access has no validation exceptions; ThrowIfCancellationRequested throws
OperationCanceledException when the statement token is canceled.

## Example

```csharp
var context = new GraphFunctionContext(default, DateTimeOffset.UnixEpoch);
Console.WriteLine(context.TransactionTimestamp); // Unix epoch
```

## Related reference

[Assembly](index.md) · [GraphArguments](graph-arguments.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Functions/GraphFunctionContext.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphFunctionRegistrationTests.cs`.
