# SqlResultSet

A materialized SQL row result with negotiated diagnostics.

Namespace: `Assimalign.Cohesion.Database.Sql.Client`.

## Signature

```csharp
public sealed class SqlResultSet : IReadOnlyList<SqlRow>
{
    public IReadOnlyList<ProtocolDiagnostic> Diagnostics { get; }
    public IReadOnlyList<SqlColumn> Columns { get; }
    public int Count { get; }
    public SqlRow this[int index] { get; }
}
```

## Behavior and exceptions

The client constructs this after consuming a complete exchange. Rows own their values; columns
retain wire identity and bounds. Diagnostics contains the negotiated warning/error list and is empty
without the capability. Information, including SQL0100, is never sent. An invalid row index fails
through the underlying collection; statement failures throw SqlClientException instead of returning
a successful result. Direct construction is unavailable.

## Example

For a stored table t:

```csharp
var plan = await connection.QueryAsync("EXPLAIN SELECT id FROM t;");
Console.WriteLine(plan.Columns.Count); // 8
foreach (var diagnostic in plan.Diagnostics) Console.WriteLine(diagnostic.Code);
// Normally empty; overflow notices, when supplied by a result, retain COHDBP001.
```

## Related reference

[Assembly](index.md) · [Protocol diagnostics](../assimalign-cohesion-database-protocol/protocol-diagnostics-message.md) · [SQL EXPLAIN](../../../../database/sql/language/statements/explain.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/src/SqlResultSet.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlWireDiagnosticTests.cs`.
