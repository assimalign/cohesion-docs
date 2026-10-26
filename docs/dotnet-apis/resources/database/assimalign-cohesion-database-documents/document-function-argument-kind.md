# DocumentFunctionArgumentKind

Valued JSON-kind masks accepted by document parameters.

Namespace: `Assimalign.Cohesion.Database.Documents`.

## Signature

```csharp
[Flags]
public enum DocumentFunctionArgumentKind : byte
{
    None = 0, Boolean = 1, Number = 2, String = 4, Array = 8, Object = 16,
    Json = Array | Object,
    Any = Boolean | Number | String | Array | Object
}
```

## Behavior and exceptions

NULL/MISSING are governed separately by DocumentNullBehavior. Masks may combine valued
kinds. None or unknown bits in function signatures throw ArgumentException. An empty mask
list accepts Any; the last mask repeats for trailing parameters. Json means array/object,
excluding primitive JSON values. Known incompatible literals fail planning; valued stored
mismatches yield scalar NULL or skipped aggregate inputs.

## Example

```csharp
var kinds = DocumentFunctionArgumentKind.Array | DocumentFunctionArgumentKind.Object;
Console.WriteLine(kinds == DocumentFunctionArgumentKind.Json); // True
```

## Related reference

[Assembly](index.md) · [DocumentFunction](document-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Functions/DocumentFunctionArgumentKind.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentRegisteredFunctionTests.cs`.
