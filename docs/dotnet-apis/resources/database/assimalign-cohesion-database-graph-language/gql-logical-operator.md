# GqlLogicalOperator

## Signature

```csharp
public enum GqlLogicalOperator { And, Or, Xor }
```

## Behavior and exceptions

And/Or/Xor select three-valued logical chains. OR and XOR share text precedence; XOR computes parity
unless an operand is unknown. Undefined enum values are rejected by Graph planning with CG05001.

## Example

```csharp
var node = new GqlLogicalExpression(GqlLogicalOperator.Or,
    [new GqlLiteralExpression(true), new GqlLiteralExpression(null)]);
Console.WriteLine(node.Operator); // Or
```

## Related reference

[Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlLogicalOperator.cs`.
