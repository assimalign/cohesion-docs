# DocumentArguments

Borrowed stack-only arguments for one callback.

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public readonly ref struct DocumentArguments
{
    public DocumentArguments(ReadOnlySpan<DocumentValue> values);
    public DocumentArguments(ReadOnlySpan<DocumentValue> values, scoped in DocumentFunctionContext context);
    public int Count { get; }
    public DocumentFunctionContext Context { get; }
    public DocumentValue this[int index] { get; }
    public bool IsNull(int index);
    public bool IsMissing(int index);
    public bool GetBoolean(int index);
    public long GetInt64(int index);
    public decimal GetDecimal(int index);
    public string GetString(int index);
    public JsonElement GetJson(int index);
}
```

## Behavior and exceptions

Values and context are borrowed for the call only; do not retain or mutate them. Ordinals
are zero-based; out-of-range access throws IndexOutOfRangeException. Wrong-kind typed access
throws InvalidCastException; integer overflow also throws OverflowException.
GetJson exposes a borrowed array/object; DocumentValue preserves MISSING.

## Example

```csharp
var arguments = new DocumentArguments(new[] { DocumentValue.Missing });
Console.WriteLine(arguments.IsMissing(0)); // True
```

## Related reference

[Assembly](index.md) · [DocumentFunction](document-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Functions/DocumentArguments.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentRegisteredFunctionTests.cs`.
