# GraphFunctionCatalog

Frozen engine-local registrations visible to all engine sessions.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public sealed class GraphFunctionCatalog : IReadOnlyCollection<GraphFunction>
{
    public int Count { get; }
    public IReadOnlyList<GraphFunction> GetOverloads(GraphFunctionName name);
    public GraphFunction? Find(GraphFunctionName name);
    public IEnumerator<GraphFunction> GetEnumerator();
}
```

## Behavior and exceptions

The engine exposes Functions after build. Lookup compares ordinally ignoring case and returns
immutable overloads in registration order, or an empty list, never null. Count includes overloads, including native registrations.
There is no public constructor or mutation. Default/invalid lookup names throw ArgumentException.
Find returns the first registered overload or null; use GetOverloads to inspect every arity.
The identifier type lives in Assimalign.Cohesion.Database.Graph.Language.

## Example

```csharp
var builder = GraphDatabaseEngine.CreateBuilder("functions");
builder.Functions.Add(GraphScalarFunction.Create<long, long>("developer_echo", value => value));
await using var engine = await builder.BuildAsync(CancellationToken.None);
Console.WriteLine(engine.Functions.GetOverloads("DEVELOPER_ECHO").Count); // 1
```

## Related reference

[Assembly](index.md) · [GraphFunctionCollection](graph-function-collection.md) · [GraphFunction](graph-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Functions/GraphFunctionCatalog.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphFunctionRegistrationTests.cs`.
