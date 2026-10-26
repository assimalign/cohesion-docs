# GraphFunctionCollection

Builder-owned function registrations, mutable before construction begins.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public sealed class GraphFunctionCollection : IReadOnlyCollection<GraphFunction>
{
    public int Count { get; }
    public GraphFunctionCollection Add(GraphFunction function);
    public IEnumerator<GraphFunction> GetEnumerator();
}
```

## Behavior and exceptions

Get this collection from the engine builder's Functions property; it has no public constructor.
Add returns the collection. Build freezes registration before storage/workers, including after
a failed build attempt. Later Add or duplicate signatures throw InvalidOperationException.
Null functions throw ArgumentNullException; reserved names throw ArgumentException.

Native names/aliases, grammar names, lexical keywords and recognized unsupported call words
are reserved. Application overloads require different arities and one scalar/aggregate kind.
Same-arity type overloads are refused.

## Example

```csharp
var builder = GraphDatabaseEngine.CreateBuilder("functions");
builder.Functions.Add(GraphScalarFunction.Create<long, long>("developer_echo", value => value));
await using var engine = await builder.BuildAsync(CancellationToken.None);
Console.WriteLine(engine.Functions.GetOverloads("DEVELOPER_ECHO").Count); // 1
```

## Related reference

[Assembly](index.md) · [GraphFunctionCatalog](graph-function-catalog.md) · [GraphFunction](graph-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Functions/GraphFunctionCollection.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphFunctionRegistrationTests.cs`.
