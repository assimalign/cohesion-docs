# OqlCallExpression

A scalar or aggregate call with its source span and ordered arguments.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlCallExpression : OqlExpression
{
    public OqlCallExpression(string name, IReadOnlyList<OqlExpression> arguments,
        Location? location = null);
    public string Name { get; }
    public IReadOnlyList<OqlExpression> Arguments { get; }
}
```

## Behavior and exceptions

The constructor stores its arguments without validation; supply an uppercase Name. Text parsing
normalizes the name and checks [OqlFunctions](oql-functions.md). META uses OqlMetaExpression.
Unknown text calls report CD00008; invalid arity/star reports CD00006. Programmatic invalid calls,
incompatible literal kinds, invalid aggregate placement and grouping fail planning with CD05001.
Text/session factories reject parser errors with DatabaseParseException.

Scalar calls can wrap aggregate results or appear inside aggregate arguments, but aggregates cannot
nest. Calls are not constants or index seek bounds and are refused in INSERT/UPSERT VALUES.
NULL/MISSING, lazy conditional evaluation, transaction clocks and runtime diagnostics are described
in [Scalar functions](../../../../database/document/language/functions/scalar-functions.md).

## Example

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "SELECT REPLACE(name,'a','b',2) FROM items");
var call = (OqlCallExpression)((OqlSelectExpression)statement.OqlExpression)
    .Projections[0].Expression;
Console.WriteLine(call.Name); // REPLACE
Console.WriteLine(call.Arguments.Count); // 4
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [OqlFunction](oql-function.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlCallExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlFunctionTests.cs`.
