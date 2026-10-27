# GraphNullBehavior

Controls whether NULL reaches a Graph callback.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public enum GraphNullBehavior { ReturnsNullOnNullInput, CalledOnNullInput }
```

## Behavior and exceptions

ReturnsNullOnNullInput is the default. Any NULL returns scalar NULL or skips an aggregate row
before the callback, after argument expressions have evaluated. CalledOnNullInput passes NULL
to the core. Typed called-on-NULL parameters must be nullable/reference types or factory
creation throws ArgumentException. GQL has no MISSING value. Undefined policies are refused
by function constructors with ArgumentOutOfRangeException.

## Example

```csharp
var function = GraphScalarFunction.Create<string?, string>("developer_null",
    value => value ?? "missing", GraphNullBehavior.CalledOnNullInput);
var arguments = new GraphArguments(new object?[] { null });
Console.WriteLine(function.Invoke(in arguments)); // missing
```

## Related reference

[Assembly](index.md) · [GraphFunction](graph-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Functions/GraphNullBehavior.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphFunctionRegistrationTests.cs`.
