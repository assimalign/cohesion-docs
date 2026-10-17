# OqlInsertExpression

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlInsertExpression : OqlExpression
{
    public OqlInsertExpression(string collection, string? alias,
        IReadOnlyList<(OqlExpression Key, OqlExpression Value)> values, OqlSelectExpression? select,
        string? keyName, string? valueName, Location? location = null);
    public string Collection { get; }
    public string? Alias { get; }
    public IReadOnlyList<(OqlExpression Key, OqlExpression Value)> Values { get; }
    public OqlSelectExpression? Select { get; }
    public string? KeyName { get; }
    public string? ValueName { get; }
}
```

## Behavior and exceptions

Retains Collection, optional Alias, ordered Values pairs, or Select plus KeyName/ValueName. Names
bind to SELECT outputs case-sensitively; null ValueName stores the entire projection object.
Planning rejects invalid contexts with COHDOC001. Execution requires nonblank string keys and JSON
values (COHDOC008), exact numbers (COHDOC007/22003), and no visible duplicate key
(DocumentDuplicateKeyException/COHDOC012).

These AST constructors retain child references and do not execute a query. Parser-produced lists are
read-only snapshots. Always inspect statement diagnostics before planning; execution performs
semantic validation and uses DatabaseException.Diagnostics for coded failures.

## Example

```csharp
var node = new OqlInsertExpression("items", null,
    [(new OqlLiteralExpression("a"), new OqlLiteralExpression(1m))],
    null, null, null);
Console.WriteLine(node.Values.Count); // 1
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [Language reference](../../../../database/document/language/statements/insert.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlInsertExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlDmlParserTests.cs`.
