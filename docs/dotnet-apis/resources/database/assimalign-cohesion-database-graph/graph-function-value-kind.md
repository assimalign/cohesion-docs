# GraphFunctionValueKind

The public category of a GraphFunctionType.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Signature

```csharp
public enum GraphFunctionValueKind
{
    Any, Number, Scalar, Node, Relationship, Path, List, Map,
    String, Boolean, Date, Timestamp, Integral, NonIntegral
}
```

## Behavior and exceptions

Integral identifies integral numbers; NonIntegral includes floating-point and decimal values.
Number is the numeric union, Scalar the scalar union and Any conservatively includes entities
and containers. This enum describes metadata, not an implicit coercion or overload ranking.
Reading ValueKind throws no exceptions.

## Example

```csharp
Console.WriteLine(GraphFunctionType.Of<long>().ValueKind); // Integral
Console.WriteLine(GraphFunctionType.Of<decimal>().ValueKind); // NonIntegral
```

## Related reference

[Assembly](index.md) · [GraphFunctionType](graph-function-type.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Functions/GraphFunctionValueKind.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphFunctionRegistrationTests.cs`.
