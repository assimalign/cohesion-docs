# DocumentFunctionKind

Distinguishes a row scalar from a group aggregate.

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public enum DocumentFunctionKind : byte { Scalar, Aggregate }
```

## Behavior and exceptions

Scalar computes one row value; Aggregate owns per-group state. A name cannot mix kinds in
one builder collection; Add throws InvalidOperationException for such a collision. Reading
Kind has no exceptions.

## Example

```csharp
var function = DocumentScalarFunction.Create<string, string>("slug", value => value);
Console.WriteLine(function.Kind); // Scalar
```

## Related reference

[Assembly](index.md) · [DocumentFunction](document-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Functions/DocumentFunctionKind.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentRegisteredFunctionTests.cs`.
