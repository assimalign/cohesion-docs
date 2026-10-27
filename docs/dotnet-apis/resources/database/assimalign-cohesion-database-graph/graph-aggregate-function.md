# GraphAggregateFunction

Creates fresh aggregate state for each group and call appearance.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public abstract class GraphAggregateFunction : GraphFunction
{
    protected GraphAggregateFunction(GraphFunctionName name,
        IEnumerable<GraphFunctionType> parameters, GraphFunctionType returnType,
        GraphNullBehavior nullBehavior = GraphNullBehavior.ReturnsNullOnNullInput);
    public GraphAggregateAccumulator CreateAccumulator(scoped in GraphFunctionContext context);
    protected abstract GraphAggregateAccumulator CreateAccumulatorCore(
        scoped in GraphFunctionContext context);
    public static GraphAggregateFunction Create<TState, T1, TResult>(GraphFunctionName name,
        Func<TState> seed, Func<TState, T1, TState> step, Func<TState, TResult> finish,
        Func<TState, long> retainedBytes,
        GraphNullBehavior nullBehavior = GraphNullBehavior.ReturnsNullOnNullInput);
}
```

## Behavior and exceptions

Each appearance owns one fresh accumulator per group, including independent argument
evaluation. Strict NULL inputs skip AddCore. The typed factory requires an honest nonnegative
retained-size callback including referenced values; initial, post-step and post-finish state
charges use the statement memory budget (CG05020). Finish runs for empty input: the factory's
finish callback determines its identity. DISTINCT uses ordinary Graph aggregate semantics.

Null/reused state and external callback or result-contract failures use CG05021. Cancellation
uses OperationCanceledException. Metadata guards match GraphScalarFunction; null callbacks
throw ArgumentNullException. Private callback work remains trusted.

## Example

```csharp
var builder = GraphDatabaseEngine.CreateBuilder("aggregates");
builder.Functions.Add(GraphAggregateFunction.Create<long, long, long>("developer_sum",
    () => 0L, (state, value) => state + value, state => state, _ => 8));
await using var engine = await builder.BuildAsync();
var database = await engine.CreateDatabaseAsync("graph", CancellationToken.None);
await using var session = await database.CreateSessionAsync(CancellationToken.None);
await using var result = (DatabaseResultSet)await session.ExecuteAsync(
    "FOR v IN [1,2,2,NULL] RETURN developer_sum(v), developer_sum(DISTINCT v)",
    cancellationToken: CancellationToken.None); // one row: 5, 3
```

## Related reference

[Assembly](index.md) · [GraphAggregateAccumulator](graph-aggregate-accumulator.md) · [GraphFunction](graph-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Functions/GraphAggregateFunction.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphFunctionRegistrationTests.cs`.
