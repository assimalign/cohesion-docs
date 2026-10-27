# GraphFunction

Immutable metadata shared by native and application scalar/aggregate functions.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public abstract class GraphFunction
{
    public GraphFunctionName Name { get; }
    public IReadOnlyList<GraphFunctionType> Parameters { get; }
    public GraphFunctionType ReturnType { get; }
    public GraphNullBehavior NullBehavior { get; }
}
```

## Behavior and exceptions

Derive [GraphScalarFunction](graph-scalar-function.md) or
[GraphAggregateFunction](graph-aggregate-function.md); the base constructor is not an external
extension point. Metadata copies parameter shapes. Register with
GraphDatabaseEngine.CreateBuilder(name).Functions.Add(function) before its one build attempt.
Build freezes the engine-local catalog before storage/workers; engine.Functions exposes it.
Registrations are configuration and are not persisted. Direct Create uses native functions only.

Names compare ordinally ignoring case. Native names/aliases, grammar names and reserved calls
cannot be replaced. Application overloads must have different arities and the same scalar or
aggregate kind. No determinism or volatility property is exposed and no purity is promised;
each aggregate call appearance owns independent state and argument evaluation. EXPLAIN validates metadata without
invoking callbacks. Unknown names use CG00010 at engine binding; a standalone parser requires
allowCustomFunctions: true to admit custom names.

One function instance serves every session of its engine; keep it stateless or thread-safe.
Callbacks are trusted application code: external effects cannot be rolled back, and private CPU,
temporary allocations and I/O cannot be sandboxed or preempted by the engine. Do not reenter an
engine from a callback. Same-session reentry is refused before execution (CK07003); another
session can wait indefinitely for a lock held by the invoking statement. Borrowed arguments
and context must not be retained or mutated. Typed delegates do not receive the statement token;
derive a function and observe its borrowed context when cancellation is needed.

Unexpected callback failures, including application I/O and timeout, use CG05021 and retain
the original InnerException. DatabaseException, OutOfMemoryException,
InsufficientExecutionStackException and actual statement cancellation retain their identity.
These function conditions have no assigned SQLSTATE. Completed wire failures use
ExecutionFailure and leave the connection reusable.

Dynamic input shape/conversion failures use CG02013 before the callback; provably incompatible
query kinds use CG03003. Invalid result or accumulator contracts use CG05021. Strict scalar
ReturnType conservatively becomes nullable when it has inputs, even if the callback declares
a non-NULL result.

## Example

```csharp
var builder = GraphDatabaseEngine.CreateBuilder("custom");
builder.Functions.Add(GraphScalarFunction.Create<long, long>("developer_echo", value => value));
await using var engine = await builder.BuildAsync();
var database = await engine.CreateDatabaseAsync("graph", CancellationToken.None);
await using var session = await database.CreateSessionAsync(CancellationToken.None);
await using var result = (DatabaseResultSet)await session.ExecuteAsync("RETURN developer_echo(42)",
    cancellationToken: CancellationToken.None); // one Int64 cell: 42
```

## Related reference

[Assembly](index.md) · [GraphFunctionCatalog](graph-function-catalog.md) · [GraphDatabaseEngineBuilder](graph-database-engine-builder.md)

## Function identifier

[GraphFunctionName](../assimalign-cohesion-database-graph-language/graph-function-name.md) supplies the identifier grammar and case-insensitive equality.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Functions/GraphFunction.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphFunctionRegistrationTests.cs`.
