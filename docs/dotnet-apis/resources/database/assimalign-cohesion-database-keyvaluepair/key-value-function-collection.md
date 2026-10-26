# KeyValueFunctionCollection

Builder-owned function registrations, mutable before construction begins.

Namespace: `Assimalign.Cohesion.Database.KeyValuePair`.

## Signature

```csharp
public sealed class KeyValueFunctionCollection : IReadOnlyCollection<KeyValueFunction>
{
    public int Count { get; }
    public KeyValueFunctionCollection Add(KeyValueFunction function);
    public bool Contains(KeyValueFunctionName name);
    public IEnumerator<KeyValueFunction> GetEnumerator();
}
```

## Behavior and exceptions

Get this collection from the engine builder's Functions property; it has no public constructor.
Add returns the collection. Build freezes registration before storage/workers, including after
a failed build attempt. Later Add or duplicate signatures throw InvalidOperationException.
Null functions throw ArgumentNullException; reserved names throw ArgumentException.
Contains refuses a default name with ArgumentException.
GET, PUT, DELETE, EXISTS, SCAN, KEYSPACES and CALL are reserved. Different exact primitive
signatures may share a name; a duplicate name and parameter-type sequence is refused.
NULL-compatible overloads can be ambiguous at execution (CV01021); no numeric widening occurs.

## Example

```csharp
var builder = KeyValueDatabaseEngine.CreateBuilder("functions");
builder.Functions.Add(KeyValueFunction.Create<long, long>("f1", value => value));
await using var engine = await builder.BuildAsync(CancellationToken.None);
Console.WriteLine(engine.Functions.GetOverloads("F1").Count); // 1
```

## Related reference

[Assembly](index.md) · [KeyValueFunctionCatalog](key-value-function-catalog.md) · [KeyValueFunction](key-value-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/Functions/KeyValueFunctionCollection.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueFunctionTests.cs`.
