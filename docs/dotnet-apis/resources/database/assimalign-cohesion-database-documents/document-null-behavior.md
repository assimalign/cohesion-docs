# DocumentNullBehavior

Controls NULL and MISSING propagation into callbacks.

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
public enum DocumentNullBehavior : byte
{
    PropagateMissingOrNull,
    CalledOnNullOrMissingInput
}
```

## Behavior and exceptions

PropagateMissingOrNull is the default: MISSING wins over NULL for scalars without invoking
the core, and either unknown skips an aggregate row. CalledOnNullOrMissingInput passes every
input to the core. Use DocumentValue to distinguish the unknown states; nullable/reference
typed inputs can receive unknowns but do not preserve that distinction. Invalid enum values
throw ArgumentOutOfRangeException; incompatible typed NULL policies throw ArgumentException.

## Example

```csharp
var inspect = DocumentScalarFunction.Create<DocumentValue, string>("inspect_value",
    value => value.IsMissing ? "missing" : value.IsNull ? "null" : "valued",
    DocumentNullBehavior.CalledOnNullOrMissingInput);
var arguments = new DocumentArguments(new[] { DocumentValue.Missing });
Console.WriteLine(inspect.Invoke(in arguments).AsString()); // missing
```

## Related reference

[Assembly](index.md) · [DocumentFunction](document-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Functions/DocumentNullBehavior.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentRegisteredFunctionTests.cs`.
