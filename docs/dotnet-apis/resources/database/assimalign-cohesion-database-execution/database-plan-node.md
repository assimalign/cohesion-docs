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
OQL DELETE adds a Delete root and UPDATE an Update root, each with a CollectionScan/IndexSeek child
and filter/access detail. GQL ordering/paging adds SortPage with offset/limit and SortKey children
with expression/direction/NULL placement. No new public node members are required.

SQL correlation adds CorrelatedSubqueries with complete child plans. Non-recursive WITH adds With,
Materialize, CteScan and Constant; reachable materializations precede the enclosing input. These
operators retain this public node shape and execute no data statement during EXPLAIN.

OQL [UPSERT](../../../../database/document/language/statements/upsert.md) adds Upsert or UpsertSelect
with its SELECT child. These use the existing public node members.

GQL [GROUP BY](../../../../database/graph/language/clauses/group-by.md) adds GroupAggregate. Its
Detail reports `keys=...; aggregates=...`; the node is described without reading matches.

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
