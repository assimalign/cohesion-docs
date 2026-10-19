# DocumentColumn

Namespace: `Assimalign.Cohesion.Database.Documents.Client`.

## Signature

```csharp
public sealed record DocumentColumn(string Name, int Ordinal, DatabaseType Type);
```

## Behavior and exceptions

Describes a result column's name, zero-based ordinal and shared scalar identity. A heterogeneous or
unobserved projection can use DatabaseType.Null (unknown); MISSING does not hide an otherwise
observed scalar type. The record stores its supplied values without additional validation
exceptions; the wire decoder validates header structure.

## Example

```csharp
var column = new DocumentColumn("name", 0, DatabaseType.String);
Console.WriteLine(column.Ordinal); // 0
```

## Related reference

[Result set](document-result-set.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/src/DocumentColumn.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentMissingWireTests.cs`.
