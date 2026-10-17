# DatabasePlanNode

Describes one chosen model operator without defining a shared executable algebra.

Namespace: `Assimalign.Cohesion.Database.Execution`.

## Signature

```csharp
public sealed class DatabasePlanNode
{
    public required int NodeId { get; init; }
    public int? ParentId { get; init; }
    public required string Operator { get; init; }
    public string? Target { get; init; }
    public string? Index { get; init; }
    public string? Detail { get; init; }
}
```

## Behavior and exceptions

NodeId identifies a node within the description; ParentId is null for a root. Optional target,
index and detail are model-defined; a seek can include its index and bounds. The initializer
stores supplied values without validating tree structure or invoking an executor. There are no
additional constructor validation exceptions. Required members must be set by C# callers.

## Example

```csharp
var seek = new DatabasePlanNode {
    NodeId = 2, ParentId = 1, Operator = "IndexSeek", Target = "dbo.t",
    Index = "by_age", Detail = "age >= 20 AND age < 40"
};
await using var description = DatabasePlanDescription.Create([seek]);
// One eight-column row; estimated_rows and actual_rows are NULL.
```

## Related reference

[DatabasePlanDescription](database-plan-description.md) · [Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Execution/src/DatabasePlanNode.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlExplainTests.cs`.
