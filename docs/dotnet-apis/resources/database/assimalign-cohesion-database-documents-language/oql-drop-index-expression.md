# OqlDropIndexExpression

Removes an index from an explicitly named collection.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlDropIndexExpression : OqlExpression
{
    public OqlDropIndexExpression(string indexName, string collection, Location? location = null);
    public OqlDropIndexExpression(string indexName, string collection, bool ifExists,
        Location? location = null);
    public string IndexName { get; }
    public string Collection { get; }
    public bool IfExists { get; }
}
```

## Behavior and exceptions

Construction stores arguments without executing or additional validation exceptions. The preceding
constructor sets IfExists to false. True makes an absent index a no-op; snapshot conflicts still
fail. An existing schema-owned index raises DatabaseObjectLockedException. Both successful removal
and conditional absence return zero affected documents. See [DROP INDEX](../../../../database/document/language/statements/drop-index.md).

## Example

```csharp
var drop = (OqlDropIndexExpression)((OqlQueryStatement)new OqlQueryParser().Parse(
    "DROP INDEX IF EXISTS absent ON items")).OqlExpression;
Console.WriteLine(drop.IfExists); // True
Console.WriteLine(drop.Collection); // items
```

Executing this against an existing items collection without that index succeeds as a no-op.

## Related reference

[Assembly](index.md) · [Design](design.md) · [OqlCreateIndexExpression](oql-create-index-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlDropIndexExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentQueryTests.CompositeIndexes.cs`.
