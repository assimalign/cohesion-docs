# KeyValueFunctionContext

Borrowed database and cancellation context without a session or storage handle.

Namespace: `Assimalign.Cohesion.Database.KeyValuePair`.

## Signature

```csharp
public readonly ref struct KeyValueFunctionContext
{
    public KeyValueFunctionContext(DatabaseName database, CancellationToken cancellationToken);
    public DatabaseName Database { get; }
    public CancellationToken CancellationToken { get; }
}
```

## Behavior and exceptions

Use the context only during the callback. It exposes no engine, mutable transaction or
transport. A derived core can observe CancellationToken; typed delegates do not receive it.
No transaction timestamp is exposed.
Construction/access has no validation exceptions; ThrowIfCancellationRequested throws
OperationCanceledException when the statement token is canceled.

## Example

```csharp
var context = new KeyValueFunctionContext(default, CancellationToken.None);
Console.WriteLine(context.CancellationToken.CanBeCanceled); // False
```

## Related reference

[Assembly](index.md) · [KeyValueArguments](key-value-arguments.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/Functions/KeyValueFunctionContext.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueFunctionTests.cs`.
