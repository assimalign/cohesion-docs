# GraphDatabaseEngineOptions

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Execution ceilings

```csharp
public long QueryMemoryBudget { get; set; }
public long QueryWorkBudget { get; set; }
```

Both must be positive; engine creation rejects nonpositive values with ArgumentOutOfRangeException and snapshots valid options. QueryMemoryBudget bounds accounted retained execution bytes; QueryWorkBudget bounds physical work across one statement. Configure them before engine creation, or on GraphDatabaseEngineBuilder.Options before BuildAsync. These are statement ceilings, not process working-set limits.

```csharp
var builder = GraphDatabaseEngine.CreateBuilder("people");
builder.Options.QueryMemoryBudget = 32L * 1024 * 1024;
builder.Options.QueryWorkBudget = 500_000;
builder.AddDatabase("people");
await using var engine = await builder.BuildAsync();
```

[GraphQueryLimits](../assimalign-cohesion-database-graph-protocol/graph-query-limits.md) can lower the ceilings for an embedded request or typed traversal. Remote statements use the server's engine configuration. See [streaming execution](design.md#streaming-execution-budgets).

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphDatabaseEngineOptions.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphDatabaseSession.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlExecutionBudgetTests.cs`.
