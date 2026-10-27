# GraphDatabaseEngineBuilder

Configures one graph engine and opens or creates its declared databases during Build.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public sealed class GraphDatabaseEngineBuilder
{
    public string Name { get; }
    public GraphDatabaseEngineOptions Options { get; }
    public GraphFunctionCollection Functions { get; }
    public GraphDatabaseEngineBuilder AddDatabase(string name);
    public GraphDatabaseEngineBuilder AddServer(Action<GraphDatabaseServerOptions> configure);
    public GraphDatabaseEngineBuilder AddServer(Func<GraphDatabaseEngine, DatabaseServer> factory);
    public GraphDatabaseEngineBuilder AddWorker(Func<GraphDatabaseEngine, DatabaseEngineWorker> factory);
    public GraphDatabaseEngine Build();
    public ValueTask<GraphDatabaseEngine> BuildAsync(CancellationToken cancellationToken = default);
}
```

## Behavior and exceptions

GraphDatabaseEngine.CreateBuilder(name) returns the concrete builder that replaces
IGraphDatabaseEngineBuilder. Options are copied at build. Worker factories run before server
factories, and the engine owns their products. Declared databases open or are created in declaration
order; a later construction failure disposes the engine and completed products. A second build
attempt or a registration after building begins throws InvalidOperationException. Invalid names or
null factories throw argument exceptions; BuildAsync observes cancellation.

## Example

```csharp
var builder = GraphDatabaseEngine.CreateBuilder("people-engine");
builder.AddDatabase("people");
await using var engine = await builder.BuildAsync();
var graph = await engine.OpenDatabaseAsync("people"); // already opened by Build
```

## Related reference

[Graph API](index.md) · [GraphDatabase](graph-database.md) · [Design](design.md)

## Function registration

Add [GraphScalarFunction](graph-scalar-function.md) and
[GraphAggregateFunction](graph-aggregate-function.md) through Functions before Build/BuildAsync.
The engine exposes the frozen [GraphFunctionCatalog](graph-function-catalog.md). Add after
construction begins throws InvalidOperationException, including after a failed build attempt.
Functions serve every engine session and must be stateless or thread-safe; registration is
configuration, not persisted schema. See the [complete registration example](graph-function.md).

## Sources

- **Builder** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphDatabaseEngineBuilder.cs`.
- **Composition contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/OVERVIEW.md`.
