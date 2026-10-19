# OqlArrayExpression

An ordered JSON array constructor.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlArrayExpression : OqlExpression
{
    public OqlArrayExpression(IReadOnlyList<OqlExpression> elements, Location? location = null);
    public IReadOnlyList<OqlExpression> Elements { get; }
}
```

## Behavior and exceptions

The constructor retains child references; parser-produced lists are read-only snapshots.
Evaluation preserves element order and converts MISSING to JSON null without removing positions.
Validation and grouping visit every element; parameters are bound even for an empty source or
an unevaluated branch. Elements can include objects, arrays and aggregate expressions where allowed.

A SELECT constructor result over 256 levels raises DatabaseException with `COHDOC015` at its span,
including nested stored/parameter values and the named projection wrapper. INSERT keeps its
128-level storage limit (`COHDOC008`). Malformed text and recursive parsing limits are statement
diagnostics (`OQL0002`/`OQL0005`), not constructor exceptions.

## Example

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "SELECT VALUE [1, MISSING, {x: NULL}] FROM items");
var select = (OqlSelectExpression)statement.OqlExpression;
var array = (OqlArrayExpression)select.Projections[0].Expression;
Console.WriteLine(array.Elements.Count); // 3
```

For each source document, execution returns bare `[1,null,{"x":null}]`.

## Related reference

[Assembly](index.md) · [Design](design.md) · [Array constructors](../../../../database/document/language/expressions/array-constructors.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlArrayExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlProjectionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentQueryTests.Projections.cs`.
