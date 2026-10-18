# DocumentResultSet

Namespace: `Assimalign.Cohesion.Database.Documents.Client`.

## Signature

```csharp
public sealed class DocumentResultSet : IReadOnlyList<DocumentRow>
{
    public IReadOnlyList<DocumentColumn> Columns { get; }
    public long AffectedCount { get; }
    public bool IsDocumentQuery { get; }
    public IReadOnlyList<JsonElement> Documents { get; }
    public int Count { get; }
    public DocumentRow this[int index] { get; }
}
```

## Behavior and exceptions

Materialized results own scalar and JSON values. Lone SELECT *, VALUE and RAW set IsDocumentQuery
even when empty; Documents then contains bare JSON values, also exposed as the row's single JsonElement.
VALUE/RAW names that column `value`; lone star names it `document`. Top-level VALUE MISSING becomes
JSON null before DISTINCT and completion counts on every transport. Positional
projections leave Documents empty and use header ordinals to preserve MISSING fields. Projected
numbers are decimal; objects/arrays are owned JsonElement values. AffectedCount is -1 for reads,
zero for DDL, and inserted/deleted count for INSERT/DELETE. No disposal is needed. An out-of-range row index throws
ArgumentOutOfRangeException. Construction is internal.
DELETE has no columns, rows or Documents and reports deleted count in AffectedCount, including zero.

## Example

```csharp
var rows = await connection.QueryAsync("SELECT * FROM items");
Console.WriteLine(rows.IsDocumentQuery); // True, even with no rows
foreach (var json in rows.Documents) Console.WriteLine(json.GetRawText());
var missing = await connection.QueryAsync("SELECT DISTINCT RAW MISSING FROM items");
// For any nonempty items collection: one Documents entry with ValueKind Null.
```

## Related reference

[DocumentRow](document-row.md) · [DocumentColumn](document-column.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/src/DocumentResultSet.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentClientWireTests.cs`.
