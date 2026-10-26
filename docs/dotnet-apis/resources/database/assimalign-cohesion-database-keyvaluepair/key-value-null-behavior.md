# KeyValueNullBehavior

Whether a NULL argument skips the callback or reaches it.

Namespace: `Assimalign.Cohesion.Database.KeyValuePair`.

## Signature

```csharp
public enum KeyValueNullBehavior { ReturnsNullOnNullInput, CalledOnNullInput }
```

## Behavior and exceptions

ReturnsNullOnNullInput is the default and returns NULL without entering the body if an input
is NULL. CalledOnNullInput invokes once with NULL. Typed parameters must support NULL or
creation throws ArgumentException. Undefined policy values throw ArgumentOutOfRangeException.
Overload resolution precedes strict short-circuiting; multiple NULL-compatible overloads still
use CV01021. Key-Value has no MISSING value.

## Example

```csharp
var function = KeyValueFunction.Create<long?, long>("null_value",
    value => value ?? 7L, KeyValueNullBehavior.CalledOnNullInput);
var arguments = new KeyValueArguments(new object?[] { null });
Console.WriteLine(function.Invoke(in arguments)); // 7
```

## Related reference

[Assembly](index.md) · [KeyValueFunction](key-value-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/src/Functions/KeyValueNullBehavior.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueFunctionTests.cs`.
