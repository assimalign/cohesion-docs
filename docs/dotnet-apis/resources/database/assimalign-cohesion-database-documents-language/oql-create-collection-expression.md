# OqlCreateCollectionExpression

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlCreateCollectionExpression : OqlExpression
{
    public OqlCreateCollectionExpression(string name, bool ifNotExists = false, Location? location = null);
    public string Name { get; }
    public bool IfNotExists { get; }
}
```

## Behavior and exceptions

CREATE COLLECTION AST with Name and IfNotExists. The constructor retains its inputs; execution uses the transactional collection core. Blank names fail COHDOC001, existing names COHDOC010 (42P07) unless conditional, and system names COHDOC004.

These AST constructors retain child references and do not execute a query. Parser-produced lists
are read-only snapshots. Always inspect statement diagnostics before planning; execution performs
semantic validation and uses DatabaseException.Diagnostics for coded failures.

## Example

```csharp
var node = new OqlCreateCollectionExpression("items", ifNotExists: true);
Console.WriteLine(node.IfNotExists); // True
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Language reference](../../../../database/document/language/statements/create-collection.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlCreateCollectionExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlCollectionDdlParserTests.cs`.
