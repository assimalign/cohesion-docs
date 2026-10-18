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

NodeId identifies a node within the description; ParentId is null for a root. Optional target, index
and detail are model-defined; a seek can include its index and bounds. The initializer stores
supplied values without validating tree structure or invoking an executor. There are no additional
constructor validation exceptions. Required members must be set by C# callers.

SQL N-way descriptions emit a JoinStep child for every join, carrying its kind and scan or
IndexSeek access. GQL standalone RETURN uses a Return root; Project describes each output and
Distinct groups those projections when requested. These are model vocabulary additions without
changes to DatabasePlanNode or the eight-column result shape.

SQL derived inputs add DerivedTable with a child query plan. OQL VALUE/RAW uses ProjectValue
for ungrouped projection, with Distinct above projection/Group when requested. GQL adds ordered
Match/OptionalMatch nodes; OptionalMatch detail names newly nullable variables and its filter and
matches-nothing flags. These descriptions explain planning without executing the constructs.
OQL DELETE adds a Delete root and its CollectionScan/IndexSeek child, with filter/access detail.

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
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlNWayJoinTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphReturnTests.cs`.
