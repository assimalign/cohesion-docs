# DocumentRow

A materialized DatabaseRow leaf with ordinal/name access and MISSING support.

Namespace: `Assimalign.Cohesion.Database.Documents.Client`.

## Signature

```csharp
public sealed class DocumentRow : DatabaseRow, IReadOnlyList<object?>
{
    public int Count { get; }
    public object? this[int index] { get; }
    public object? this[string name] { get; }
    public override bool IsNull(int ordinal);
    public override object? GetValue(int ordinal);
}
// Inherited: SupportsMissing, IsMissing(int), FieldCount and typed getters.
```

## Behavior and exceptions

SupportsMissing is true. IsMissing identifies omitted projection members; IsNull is false for
MISSING and true for explicit NULL. GetValue and indexers return null for either. Names match
ordinally; first duplicate name wins at row lookup, though duplicate wire header/projection names
are rejected. Nested values are owned JsonElement instances. Whole-document rows are never MISSING.

An invalid ordinal index throws IndexOutOfRangeException. IsMissing validates its ordinal with
ArgumentOutOfRangeException. Unknown names throw KeyNotFoundException; null names
ArgumentNullException. A typed numeric/Boolean getter on NULL/MISSING throws InvalidOperationException.
GetBytes accepts JSON/string data and returns empty memory for unknown values; other types fail.

## Example

For one stored {} document:

```csharp
var result = await connection.QueryAsync("SELECT absent AS m, NULL AS n FROM items");
var row = result[0];
Console.WriteLine(row.IsMissing(0)); // True
Console.WriteLine(row.IsNull(0));    // False
Console.WriteLine(row.IsNull(1));    // True
```

## Related reference

[DatabaseRow](../assimalign-cohesion-database-execution/database-row.md) · [Result set](document-result-set.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/src/DocumentRow.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentMissingWireTests.cs`.
