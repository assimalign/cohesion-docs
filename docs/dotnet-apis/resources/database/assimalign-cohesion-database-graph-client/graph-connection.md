# GraphConnection

Namespace: `Assimalign.Cohesion.Database.Graph.Client`.

## Changed signatures

```csharp
public ValueTask<GraphResultSet> QueryAsync(string statement,
    IReadOnlyDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default);
public ValueTask<long> ExecuteAsync(string statement,
    IReadOnlyDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default);
public GraphPathResult QueryPathsAsync(string statement,
    IReadOnlyDictionary<string, object?>? parameters = null, CancellationToken cancellationToken = default);
```

## Behavior and exceptions

One authenticated rental runs one active exchange. QueryAsync materializes scalar/SHOW/EXPLAIN rows;
ExecuteAsync returns affected entity count (-1 for rows). QueryPathsAsync streams one bound path,
node or relationship from read-only MATCH. Parameters preserve runtime scalar types and bind
case-sensitive bare names in WHERE and maps. Missing values, collisions and invalid types reach
the same engine diagnostics (COHDBG001/014/003) as embedded requests.

Blank statements throw ArgumentException; disposed rentals ObjectDisposedException; overlapping
exchanges InvalidOperationException. Unfinished cancellation throws OperationCanceledException and
discards the connection. GraphClientException retains server category and diagnostics. A terminal
rejection before scalar/path output preserves reuse. Early path disposal discards it. Explicit
wire transactions are unsupported.

## Example

```csharp
var bindings = new Dictionary<string, object?> { ["name"] = "Ada", ["weight"] = 2.5m };
await connection.ExecuteAsync("INSERT (:P {name:$name})-[:T {weight:@weight}]->(:P)", bindings);
var rows = await connection.QueryAsync(
    "MATCH (a:P {name:@name})-[r:T]->(b) WHERE r.weight = $weight RETURN a.name, r.weight", bindings);
Console.WriteLine(rows[0][0]); // Ada; weight remains Decimal 2.5
```

## Related reference

[GraphPathResult](graph-path-result.md) · [GraphResultSet](graph-result-set.md) · [Parameters](../../../../database/graph/language/expressions/parameters.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/src/GraphConnection.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphClientWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphParameterTests.cs`.
