# OqlSelectExpression

A query over one collection in the session database, with duplicate removal and bare-value output.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlSelectExpression : OqlExpression
{
    public OqlSelectExpression(string collection, string? alias,
        IReadOnlyList<OqlProjection> projections, OqlExpression? predicate,
        IReadOnlyList<OqlExpression> groupBy, OqlExpression? having,
        IReadOnlyList<OqlOrdering> orderBy, bool isDistinct, bool isValue,
        Location? location = null);
    public string Collection { get; }
    public string? Alias { get; }
    public IReadOnlyList<OqlProjection> Projections { get; }
    public OqlExpression? Predicate { get; }
    public IReadOnlyList<OqlExpression> GroupBy { get; }
    public OqlExpression? Having { get; }
    public IReadOnlyList<OqlOrdering> OrderBy { get; }
    public bool IsDistinct { get; }
    public bool IsValue { get; }
}
```

## Behavior and exceptions

IsDistinct controls duplicate removal after projection and before ordering. IsValue represents
VALUE and RAW, which share one bare-value result with metadata column `value`. A top-level MISSING
becomes NULL before deduplication and counting on every transport. Named projections keep MISSING.
DISTINCT uses structural collation equality and retains the first representative; objects containing
NULL can deduplicate. See [SELECT](../../../../database/document/language/statements/select.md).

Construction stores arguments and does not execute. Engine validation rejects an empty projection,
or IsValue with multiple expressions, an alias or star, with DatabaseException `COHDOC001`.
Under IsDistinct, an ORDER BY expression outside the projection after alias rewriting raises
`COHDOC014`. Duplicate projected names use `COHDOC003`. Inspect parser diagnostics before planning.

## Example

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "SELECT DISTINCT RAW x FROM items ORDER BY x");
var select = (OqlSelectExpression)statement.OqlExpression;
Console.WriteLine(select.IsDistinct); // True
Console.WriteLine(select.IsValue);    // True
```

For documents `{"x":null}`, `{}`, `{}`, execution returns one bare NULL row; IsMissing(0)
is false and IsNull(0) is true in process and through the client.

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlSelectExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlProjectionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentProjectionWireTests.cs`.
