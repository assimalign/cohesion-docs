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
    public override ReadOnlyMemory<byte> GetBytes(int ordinal);
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
ArgumentNullException. A typed numeric/Boolean getter on NULL/MISSING throws
InvalidOperationException.

GetBytes(0) for a bare-value row (lone star or VALUE/RAW) returns the original JSON frame bytes:
strings include JSON quotes, NULL is UTF-8 `null`, and a lone star preserves stored whitespace.
For named columns, strings return unquoted UTF-8, JSON returns encoded JSON and NULL/MISSING
returns empty memory; other named scalar types throw InvalidOperationException. This byte contract
also holds for embedded Documents rows. VALUE/RAW never exposes top-level MISSING: it is NULL.

## Example

For one stored {} document:

```csharp
var result = await connection.QueryAsync("SELECT absent AS m, NULL AS n FROM items");
var row = result[0];
Console.WriteLine(row.IsMissing(0)); // True
Console.WriteLine(row.IsNull(0));    // False
Console.WriteLine(row.IsNull(1));    // True
var bare = await connection.QueryAsync("SELECT DISTINCT VALUE absent FROM items");
Console.WriteLine(bare.Count); // 1
Console.WriteLine(bare[0].IsMissing(0)); // False
Console.WriteLine(System.Text.Encoding.UTF8.GetString(bare[0].GetBytes(0).Span)); // null
```

## Related reference

[DatabaseRow](../assimalign-cohesion-database-execution/database-row.md) · [Result set](document-result-set.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/src/DocumentRow.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentMissingWireTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentProjectionWireTests.cs`.
