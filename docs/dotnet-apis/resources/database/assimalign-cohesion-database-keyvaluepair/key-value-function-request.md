# KeyValueFunctionRequest

A reusable scalar request with detached primitive arguments.

Namespace: `Assimalign.Cohesion.Database.KeyValuePair`.

## Signature

```csharp
public sealed class KeyValueFunctionRequest : KeyValueRequest
{
    public KeyValueFunctionRequest(KeyValueFunctionName name, IReadOnlyList<object?> arguments);
    public KeyValueFunctionName Name { get; }
    public IReadOnlyList<object?> Arguments { get; }
}
```

## Behavior and exceptions

Construction copies the argument list and binary bytes. Repeated references to the same
binary backing range share one detached snapshot; equal bytes in independent buffers remain
separate. Binary access is read-only. The request can be reused concurrently. Null argument
lists throw ArgumentNullException; default names or unsupported primitives throw ArgumentException.

Execution returns one nullable value column of the declared primitive type, one row and
AffectedCount -1. Resolution failures use CV01018–CV01021; result and callback failures use
CV02016 and CV05003. No SQLSTATE is assigned.

## Example

```csharp
byte[] bytes = { 1, 2 };
var request = new KeyValueFunctionRequest("echo", new object?[] { bytes });
bytes[0] = 9;
Console.WriteLine(((ReadOnlyMemory<byte>)request.Arguments[0]!).Span[0]); // 1
```

## Related reference

[Assembly](index.md) · [KeyValueFunction](key-value-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/KeyValueFunctionRequest.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueFunctionTests.cs`.
