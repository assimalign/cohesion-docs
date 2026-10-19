# GqlStringPredicateOperator

## Signature

```csharp
public enum GqlStringPredicateOperator { StartsWith, EndsWith, Contains }
```

## Behavior and exceptions

Selects ordinal, case-sensitive text predicates. Non-string or NULL inputs yield unknown. Undefined
enum values are invalid AST shapes and fail planning with COHDBG001.

## Example

```csharp
var op = GqlStringPredicateOperator.Contains;
var node = new GqlStringPredicateExpression(new GqlLiteralExpression("Alice"),
    op, new GqlLiteralExpression("lic"));
Console.WriteLine(node.Operator); // Contains
```

## Related reference

[Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlStringPredicateOperator.cs`.
