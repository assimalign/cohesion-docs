# GqlParameterExpression

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlParameterExpression : GqlExpression
{
    public GqlParameterExpression(string name, Location? location = null);
    public string Name { get; }
}
```

## Behavior and exceptions

Name is the bare, case-sensitive parameter name. WHERE, RETURN and property maps retain runtime scalar
types. Missing bindings fail COHDBG001, normalized collisions COHDBG014, unsupported/non-finite
values COHDBG003 during planning. This constructor retains its inputs; malformed query reference
syntax is GQL0002.

The node stores an expression; evaluation belongs to the Graph engine. Check parser diagnostics
before execution. Constructor acceptance alone does not establish a valid executable AST.

## Example

```csharp
var node = new GqlParameterExpression("minimum");
Console.WriteLine(node.Name); // minimum
```

## Related reference

[Assembly](index.md) · [WHERE operators](../../../../database/graph/language/expressions/operators.md) · [Parameters](../../../../database/graph/language/expressions/parameters.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlParameterExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphParameterTests.cs`.
