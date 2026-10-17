# GraphResultSet

Materialized graph scalar rows with negotiated statement diagnostics.

Namespace: `Assimalign.Cohesion.Database.Graph.Client`.

## Signature

```csharp
public sealed class GraphResultSet : IReadOnlyList<GraphRow>
{
    public IReadOnlyList<GraphColumn> Columns { get; }
    public IReadOnlyList<ProtocolDiagnostic> Diagnostics { get; }
    public long AffectedCount { get; }
    public int Count { get; }
    public GraphRow this[int index] { get; }
}
```

## Behavior and exceptions

Construction is internal. Rows own their values and retain runtime scalar types. Diagnostics holds
the negotiated Warning/Error list at completed result return; it is empty on a baseline 2.0 exchange
or without diagnostics. Unknown-name warnings do not turn success into failure. An invalid row index
throws ArgumentOutOfRangeException. Statement failures throw GraphClientException instead of
returning this successful result.

RETURN columns use explicit aliases or normalized source text. The engine rejects duplicate final
names with COHDBG015 before output, including collisions between aliases and implicit names. Scalar
standalone/computed RETURN and DISTINCT use this same container; retained values keep their runtime
types. This adds no result frame or public member.

## Example

```csharp
var result = await connection.QueryAsync("MATCH (n:Absent) RETURN n.name");
Console.WriteLine(result.Count); // 0 when the label is absent
Console.WriteLine(result.Diagnostics[0].Code); // COHDBG010 with diagnostics negotiated
```

## Related reference

[Connection](graph-connection.md) · [Path completion](graph-path-result.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/src/GraphResultSet.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphWireDiagnosticTests.cs`.
