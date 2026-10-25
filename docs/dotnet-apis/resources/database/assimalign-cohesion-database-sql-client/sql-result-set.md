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
    public long AffectedCount { get; }
    public SqlRow this[int index] { get; }
}
```

## Behavior and exceptions

The client constructs this after consuming a complete exchange. Rows own their values; columns
retain wire identity and bounds. Diagnostics is never null. It contains the negotiated warning/error
list and is empty when nothing was reported or without the capability. Information, including
CS00009, is never sent. An invalid row index fails
through the underlying collection; statement failures throw SqlClientException instead of returning
a successful result. Direct construction is unavailable.

AffectedCount preserves ResultComplete's long value: -1 for SELECT and n for a write,
including RETURNING. Count is the number of returned rows. Empty RETURNING writes keep typed
columns with Count/AffectedCount zero. Results own their values and survive a later ROLLBACK.
QueryAsync returns rows and count; ExecuteAsync still buffers/transfers returned rows before
discarding them; ExecuteScalarAsync reads the first cell.

## Example

For a stored table t:

```csharp
var plan = await connection.QueryAsync("EXPLAIN SELECT id FROM t;");
Console.WriteLine(plan.Columns.Count); // 8
foreach (var diagnostic in plan.Diagnostics) Console.WriteLine(diagnostic.Code);
// Normally empty; overflow notices, when supplied by a result, retain CK07001.
```

```csharp
await connection.ExecuteAsync("CREATE TABLE returned_ids (id INT GENERATED ALWAYS AS IDENTITY, name TEXT);");
var inserted = await connection.QueryAsync(
    "INSERT INTO returned_ids (name) VALUES ('x'), ('y') RETURNING id, name;");
Console.WriteLine(inserted.AffectedCount); // 2
Console.WriteLine(inserted[0].GetInt32(0)); // 1
```

This follows SqlReturningWireTests over InMemory and TCP.

## Related reference

[Assembly](index.md) · [Protocol diagnostics](../assimalign-cohesion-database-protocol/protocol-diagnostics-message.md) · [SQL EXPLAIN](../../../../database/sql/language/statements/explain.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/src/SqlResultSet.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlWireDiagnosticTests.cs`.
