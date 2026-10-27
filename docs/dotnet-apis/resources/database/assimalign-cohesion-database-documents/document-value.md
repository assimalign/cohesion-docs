# DocumentValue

A function value preserving MISSING separately from NULL.

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public readonly struct DocumentValue
{
    public static DocumentValue Null { get; }
    public static DocumentValue Missing { get; }
    public bool IsNull { get; }
    public bool IsMissing { get; }
    public DatabaseType Type { get; }
    public static DocumentValue FromBoolean(bool value);
    public static DocumentValue FromInt64(long value);
    public static DocumentValue FromDecimal(decimal value);
    public static DocumentValue FromString(string? value);
    public static DocumentValue FromJson(JsonElement value);
    public bool AsBoolean();
    public long AsInt64();
    public decimal AsDecimal();
    public string AsString();
    public JsonElement AsJson();
}
```

## Behavior and exceptions

Default is NULL. NULL and MISSING both expose DatabaseType.Null; use IsNull/IsMissing to
distinguish them. FromString(null) returns NULL. FromJson clones and normalizes its input,
including primitive JSON into canonical document values; AsJson reads arrays/objects only.
Borrowed argument values must not be retained beyond the call.

Wrong-kind access throws InvalidCastException. AsInt64 also accepts an integral decimal and
throws OverflowException outside Int64. AsDecimal accepts Int64 or Decimal. JSON numeric
normalization failures use DatabaseException (CD02007 / 22003 for numeric domain overflow).

## Example

```csharp
Console.WriteLine(DocumentValue.Null.IsNull); // True
Console.WriteLine(DocumentValue.Missing.IsMissing); // True
Console.WriteLine(DocumentValue.FromInt64(2).AsDecimal()); // 2
```

## Related reference

[Assembly](index.md) · [DocumentFunction](document-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/ValueObjects/DocumentValue.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentRegisteredFunctionTests.cs`.
