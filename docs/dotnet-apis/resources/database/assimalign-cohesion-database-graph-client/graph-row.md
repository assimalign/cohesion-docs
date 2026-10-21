# GraphRow

One materialized Graph value row, indexed by ordinal or exact column name.

Namespace: `Assimalign.Cohesion.Database.Graph.Client`.

## Signature

```csharp
public sealed class GraphRow : IReadOnlyList<object?>
{
    public int Count { get; }
    public object? this[int index] { get; }
    public object? this[string name] { get; }
    public IEnumerator<object?> GetEnumerator();
}
```

## Behavior and exceptions

Construction is internal. Values are scalars, object?[] lists, read-only ordinal
IReadOnlyDictionary<string, object?> maps or NULL. Scalar runtime types survive encoding, including
nested Decimal and unsigned numbers. Composite values require negotiated graph.composite-values;
query GraphColumn.Type remains DatabaseType.Null (unknown). There is no MISSING state.

Ordinal access throws IndexOutOfRangeException outside the row. Name access throws
ArgumentNullException for null or KeyNotFoundException for an absent exact name. For manually
supplied duplicate column metadata, the first name wins; the engine rejects duplicate final
RETURN names with COHDBG015 before output. Enumeration follows column order.

## Example

On a negotiated GraphConnection:

```csharp
var rows = await connection.QueryAsync("RETURN $m AS value",
    new Dictionary<string, object?> {
        ["m"] = new Dictionary<string, object?> { ["k"] = new object?[] { 0.1m, null } }
    });
var map = (IReadOnlyDictionary<string, object?>)rows[0]["value"]!;
var list = (object?[])map["k"]!;
Console.WriteLine(list[0]!.GetType() == typeof(decimal)); // True
Console.WriteLine(list[1] is null); // True
```

## Related reference

[Assembly](index.md) · [GraphResultSet](graph-result-set.md) · [GraphConnection](graph-connection.md) · [GraphValueCodec](../assimalign-cohesion-database-graph-protocol/graph-value-codec.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/src/GraphRow.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphCompositeWireTests.cs`.
