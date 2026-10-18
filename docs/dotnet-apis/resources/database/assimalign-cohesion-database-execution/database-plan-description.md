# DatabasePlanDescription

A materialized EXPLAIN result with the same eight columns for SQL, OQL and GQL.

Namespace: `Assimalign.Cohesion.Database.Execution`.

## Signature

```csharp
public sealed class DatabasePlanDescription : DatabaseResultSet
{
    public static DatabasePlanDescription Create(
        IEnumerable<DatabasePlanNode> nodes, IEnumerable<Diagnostic>? diagnostics = null);
    public override DatabaseResultStatus Status { get; }
    public override long AffectedCount { get; }
    public override IReadOnlyList<Diagnostic> Diagnostics { get; }
    public override IReadOnlyList<DatabaseColumn> Columns { get; }
    public override IAsyncEnumerable<DatabaseRow> GetRowsAsync(CancellationToken cancellationToken = default);
    public override ValueTask DisposeAsync();
}
```

## Behavior and exceptions

Create materializes nodes in their supplied order, expected to be depth-first pre-order, and copies
optional diagnostics. It does not sort or execute a plan. Status is Success and affected count is
-1. Columns, in order, are node_id (Int32), parent_id (nullable Int32), operator (String),
target/index/detail (nullable String), estimated_rows/actual_rows (nullable Int64). Both counts are
always NULL. Operator vocabulary belongs to each model. DisposeAsync is a completed no-op.

Null nodes or a null sequence throw ArgumentNullException. Enumeration checks cancellation and
throws OperationCanceledException. EXPLAIN retains the wrapped statement's planning errors and
provisioning admission; data execution never runs. Each model's tests guard every concrete plan
record's description mapping. Documents serializes each description row as a JSON document.

DerivedTable/Subqueries/CorrelatedSubqueries and With/Materialize/CteScan/Constant in SQL, ProjectValue/Distinct in OQL and Match/OptionalMatch/SortPage/SortKey in GQL all
retain this shape. See [DatabasePlanNode](database-plan-node.md) for model vocabulary and the
language EXPLAIN pages for supported statement forms. OQL UPDATE adds an Update root and access
child while preserving these columns. See [OQL EXPLAIN](../../../../database/document/language/statements/explain.md)
and [GQL EXPLAIN](../../../../database/graph/language/statements/explain.md).

OQL UPSERT adds Upsert/UpsertSelect operators while retaining this signature and eight columns.

## Example

```csharp
await using var plan = DatabasePlanDescription.Create([
    new DatabasePlanNode { NodeId = 0, Operator = "TableScan", Target = "dbo.t" }
]);
await foreach (var row in plan.GetRowsAsync())
    Console.WriteLine($"{row.GetInt32(0)}: {row.GetString(2)}");
// 0: TableScan
```

## Related reference

[DatabasePlanNode](database-plan-node.md) · [Assembly](index.md) · [SQL EXPLAIN](../../../../database/sql/language/statements/explain.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Execution/src/DatabasePlanDescription.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Execution/tests/DatabasePlanDescriptionTests.cs`.
