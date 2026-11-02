# GqlReduceExpression

An ordered fold with lexical accumulator and iterator bindings.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlReduceExpression : GqlExpression
{
    public GqlReduceExpression(string accumulator, GqlExpression initial, string variable,
        GqlExpression list, GqlExpression body, Location? location = null);
    public string Accumulator { get; }
    public GqlExpression Initial { get; }
    public string Variable { get; }
    public GqlExpression List { get; }
    public GqlExpression Body { get; }
}
```

Location is `Assimalign.Cohesion.Database.Language.Location`.

## Behavior and exceptions

Construction retains operands and location without validation or evaluation. List and Initial
use the enclosing scope; Accumulator and Variable shadow it only in Body. Execution evaluates
List first, skips Initial/Body for NULL, and otherwise folds in source order. Empty input returns
Initial. See [reduce](../../../../database/graph/language/expressions/reduce.md).

Engine validation raises DatabaseException with CG01018 for missing/empty or identical declarations,
CG05025 for a malformed body or body aggregate, CG02015 for a non-list source, and CG05001 for
unbound names/parameters. Statically incompatible predicate uses retain CG02013. Shape/type
planning above 65,536 node visits or closure steps uses CG05026. None has an assigned SQLSTATE.
Check parser diagnostics before planning; construction itself does not throw these errors.

## Example

Adapted from GqlReduceParserTests:

```csharp
var statement = (GqlQueryStatement)new GqlQueryParser().Parse(
    "RETURN reduce(a = $seed, x IN $xs | a + x)");
var expression = (GqlReduceExpression)statement.GqlExpression.Projections[0].Expression;
Console.WriteLine(expression.Accumulator); // a
Console.WriteLine(expression.Variable);    // x
```

With seed = 0 and xs = [1,2,3], execution returns scalar Int64 `6`.

## Related reference

[Assembly](index.md) · [GqlListPredicateExpression](gql-list-predicate-expression.md) · [GqlListComprehensionExpression](gql-list-comprehension-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlReduceExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlReduceParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphReduceTests.cs`.
