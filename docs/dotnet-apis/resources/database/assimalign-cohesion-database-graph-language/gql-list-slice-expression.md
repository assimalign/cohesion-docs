# GqlListSliceExpression

A list range with an inclusive start and exclusive end.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlListSliceExpression : GqlExpression
{
    public GqlListSliceExpression(GqlExpression operand, GqlExpression? from = null,
        GqlExpression? to = null, Location? location = null);
    public GqlExpression Operand { get; }
    public GqlExpression? From { get; }
    public GqlExpression? To { get; }
}
```

## Behavior and exceptions

Construction retains operands without validation or execution. A null From/To means an omitted
bound, distinct from an explicit GqlLiteralExpression(null). Omitted start/end means zero/list
length; negative bounds count from the end and clamp. Reversed intervals yield an empty object?[].
NULL input or explicit NULL bounds yield NULL after bound expressions evaluate.
See [list slices](../../../../database/graph/language/expressions/list-slices.md).

Planning raises DatabaseException with CG00012 when both bounds are omitted, including under
EXPLAIN; the parser still represents this shape. Known non-integer bounds, including 1.0, use
CG02014. Dynamic finite numeric bounds truncate toward zero before Int32 range validation.
Invalid source/bound kinds or ranges use CG02014; missing operands use CG05001. These codes
have no assigned SQLSTATE. Constructor acceptance alone does not establish executable validity.

## Example

```csharp
var statement = (GqlQueryStatement)new GqlQueryParser().Parse("RETURN $xs[..2]");
var slice = (GqlListSliceExpression)statement.GqlExpression.Projections[0].Expression;
Console.WriteLine(slice.From is null);     // True
Console.WriteLine(slice.To is not null);   // True
```

With xs = [0,1,2,3], execution returns one list cell `[0,1]`.

## Related reference

[Assembly](index.md) · [GqlSubscriptExpression](gql-subscript-expression.md) · [GqlListComprehensionExpression](gql-list-comprehension-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlListSliceExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlListComprehensionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphCollectionSliceAdmissionTests.cs`.
