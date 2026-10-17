# OqlDropCollectionExpression

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlDropCollectionExpression : OqlExpression
{
    public OqlDropCollectionExpression(string name, bool ifExists = false, Location? location = null);
    public string Name { get; }
    public bool IfExists { get; }
}
```

## Behavior and exceptions

DROP COLLECTION AST with Name and IfExists. Execution removes documents and indexes transactionally.
Missing collections fail COHDOC002 unless conditional; schema-owned collections throw
DatabaseObjectLockedException with COHDOC011 even when conditional.

These AST constructors retain child references and do not execute a query. Parser-produced lists are
read-only snapshots. Always inspect statement diagnostics before planning; execution performs
semantic validation and uses DatabaseException.Diagnostics for coded failures.

## Example

```csharp
var node = new OqlDropCollectionExpression("items", ifExists: true);
Console.WriteLine(node.IfExists); // True
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Language reference](../../../../database/document/language/statements/drop-collection.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlDropCollectionExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlCollectionDdlParserTests.cs`.
