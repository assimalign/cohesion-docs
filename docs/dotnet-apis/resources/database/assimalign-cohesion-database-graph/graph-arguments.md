# GraphArguments

Borrowed stack-only arguments for one callback.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public readonly ref struct GraphArguments
{
    public GraphArguments(ReadOnlySpan<object?> values, GraphFunctionContext context = default);
    public int Count { get; }
    public GraphFunctionContext Context { get; }
    public object? this[int index] { get; }
    public bool IsNull(int index);
    public T Get<T>(int index);
}
```

## Behavior and exceptions

Values and context are borrowed for the call only; do not retain or mutate them. Ordinals
are zero-based; out-of-range access throws IndexOutOfRangeException. Wrong-kind typed access
throws InvalidCastException; NULL is valid only for nullable/reference T.
Get<T> requires the exact CLR type and performs no numeric widening.

## Example

```csharp
var arguments = new GraphArguments(new object?[] { 42L });
Console.WriteLine(arguments.Get<long>(0)); // 42
```

## Related reference

[Assembly](index.md) · [GraphFunction](graph-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Functions/GraphArguments.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphFunctionRegistrationTests.cs`.
