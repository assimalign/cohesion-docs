# GraphDatabaseEngineOptions

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Execution ceilings

```csharp
public long QueryMemoryBudget { get; set; } = 256L * 1024 * 1024;
public long QueryWorkBudget { get; set; } = 1_000_000;
```

Both must be positive; engine creation rejects nonpositive values with `ArgumentOutOfRangeException`
and snapshots valid options. `QueryMemoryBudget` defaults to 268,435,456 bytes (256 MiB) of accounted
retained execution state; `QueryWorkBudget` defaults to 1,000,000 physical work units per statement.
These are statement ceilings, not process working-set limits.

The default admits 100,000 one-property rows for SET, FOR … INSERT, RETURN n and QueryPaths.
The regression cases refuse ten times that workload with `CG05020`. Capacity depends on row width
and retained intermediate state. Shared containers and supported immutable leaves, including small
strings, byte arrays and boxed scalars, are charged once across operators; duplicate owner
bookkeeping charges have been removed. `TraverseAsync` uses both ceilings and refuses work beyond
1,000,000 units by default with `CG05004`; it previously had no work cap.

Raise the server engine ceilings before engine creation, or on `GraphDatabaseEngineBuilder.Options`
before `BuildAsync`, for larger workloads:

```csharp
using Assimalign.Cohesion.Database.Graph;

var builder = GraphDatabaseEngine.CreateBuilder("people");
builder.Options.QueryMemoryBudget = 512L * 1024 * 1024;
builder.Options.QueryWorkBudget = 2_000_000;
builder.AddDatabase("people");
await using var engine = await builder.BuildAsync();
```

[GraphQueryLimits](../assimalign-cohesion-database-graph-protocol/graph-query-limits.md) can lower the ceilings for an embedded request or typed traversal. Remote statements use the server's engine configuration. See [streaming execution](design.md#streaming-execution-budgets).

[UNION, EXCEPT, INTERSECT and OTHERWISE](../../../../database/graph/language/clauses/union.md) share
these ceilings across all operands and outputs. Input hash visits, retained-row output visits and
forwarding at every parent operator consume work. UNION duplicate-elimination state uses CG05030;
EXCEPT/INTERSECT retained frames, rows and hash state use CG05032. Other memory and work retain
CG05020 and CG05004. UNION ALL has no distinct table or per-operand result set; final results still materialize.

## Sources

- **Composite accounting** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphSetQueryTests.cs`.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphDatabaseEngineOptions.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphDatabaseSession.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlExecutionBudgetTests.cs`.
