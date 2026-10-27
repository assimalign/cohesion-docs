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

This remote result has no Diagnostics member. Embedded Documents execution instead returns
DatabaseResult with a non-null Diagnostics collection, empty when nothing is reported. Remote
failures throw [DocumentClientException](document-client-exception.md), whose Diagnostics snapshot
is also never null.

Materialized results own scalar and JSON values. Lone SELECT/RETURNING *, VALUE and RAW set IsDocumentQuery
even when empty; Documents then contains bare JSON values, also exposed as the row's single JsonElement.
VALUE/RAW names that column `value`; lone star names it `document`. Top-level VALUE MISSING becomes
JSON null before DISTINCT and completion counts on every transport. Positional
projections leave Documents empty and use header ordinals to preserve MISSING fields. Projected
numbers are decimal; objects/arrays are owned JsonElement values. AffectedCount is -1 for reads,
zero for DDL, and inserted/deleted count for INSERT/DELETE and matched count for UPDATE. No disposal is needed. An out-of-range row index throws
ArgumentOutOfRangeException. Construction is internal.
Without RETURNING, DELETE and UPDATE have no columns, rows or Documents and report deleted/matched count in
AffectedCount, including zero. UPDATE counts unchanged matches too and advances their versions.
The [partial-update reference](../../../../database/document/language/statements/update.md) gives
path, NULL/MISSING and failure behavior.

With [RETURNING](../../../../database/document/language/clauses/returning.md), Count equals the
nonnegative AffectedCount, including zero; Columns still describes the projection on empty input.
INSERT/UPSERT/UPDATE project post-images, DELETE pre-images. Named projections expose rows;
lone star and VALUE/RAW expose bare Documents as well. See
[client compatibility](design.md#returning-and-client-compatibility) before using an older client.

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
