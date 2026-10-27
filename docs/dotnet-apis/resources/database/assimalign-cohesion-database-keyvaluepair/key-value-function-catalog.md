# KeyValueFunctionCatalog

Frozen engine-local registrations visible to all engine sessions.

Namespace: `Assimalign.Cohesion.Database.KeyValuePair`.

## Signature

```csharp
public sealed class KeyValueFunctionCatalog : IReadOnlyCollection<KeyValueFunction>
{
    public int Count { get; }
    public IReadOnlyList<KeyValueFunction> GetOverloads(KeyValueFunctionName name);
    public IEnumerator<KeyValueFunction> GetEnumerator();
}
```

## Behavior and exceptions

The engine exposes Functions after build. Lookup compares ordinally ignoring case and returns
immutable overloads in registration order, or an empty list, never null. Count includes overloads; direct Create starts empty.
There is no public constructor or mutation. Default/invalid lookup names throw ArgumentException.

The identifier type lives in Assimalign.Cohesion.Database.KeyValuePair.Protocol.

## Example

```csharp
var builder = KeyValueDatabaseEngine.CreateBuilder("functions");
builder.Functions.Add(KeyValueFunction.Create<long, long>("f1", value => value));
await using var engine = await builder.BuildAsync(CancellationToken.None);
Console.WriteLine(engine.Functions.GetOverloads("F1").Count); // 1
```

## Related reference

[Assembly](index.md) · [KeyValueFunctionCollection](key-value-function-collection.md) · [KeyValueFunction](key-value-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/Functions/KeyValueFunctionCatalog.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueFunctionTests.cs`.
