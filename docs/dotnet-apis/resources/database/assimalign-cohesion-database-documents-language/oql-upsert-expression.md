# OqlUpsertExpression

Retains the whole-document replacement statement's VALUES pairs or SELECT source.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlUpsertExpression : OqlExpression
{
    public OqlUpsertExpression(string collection, string? alias,
        IReadOnlyList<(OqlExpression Key, OqlExpression Value)> values,
        OqlSelectExpression? select, string? keyName, string? valueName,
        Location? location = null);
    public string Collection { get; }
    public string? Alias { get; }
    public IReadOnlyList<(OqlExpression Key, OqlExpression Value)> Values { get; }
    public OqlSelectExpression? Select { get; }
    public string? KeyName { get; }
    public string? ValueName { get; }
}
```

## Behavior and exceptions

The constructor retains arguments and child references without executing or validating a query.
Parser lists are read-only snapshots. VALUES has ordered key/value pairs; SELECT has output names
bound case-sensitively. Null ValueName stores the entire projected object. The engine validates
all parameters and expressions before writes, including for EXPLAIN or an empty source.

[UPSERT](../../../../database/document/language/statements/upsert.md) inserts or replaces whole
JSON values, returning one affected count per input pair. Invalid scope/binding uses CD05001,
invalid key/value/depth CD02008, missing parameters CD01005 and lossy decimal values CD02007
(22003). Repeated keys throw DocumentDuplicateKeyException/CD03012. OPTIONS, RETURNING and
nested SELECT in VALUES remain CK00001. Parser diagnostics become DatabaseParseException at
execution. EXPLAIN describes Upsert/UpsertSelect without writes.

The statement capability is `OqlClauses.Upsert` (`public const string Upsert = "UPSERT"`).

## Example

Adapted from OqlDmlParserTests; parsing needs no collection:

```csharp
var parsed = (OqlQueryStatement)new OqlQueryParser().Parse(
    "UPSERT INTO items AS u (KEY, VALUE) VALUES ('a', {'name':'new'})");
var upsert = (OqlUpsertExpression)parsed.OqlExpression;
Console.WriteLine(upsert.Collection);   // items
Console.WriteLine(upsert.Alias);        // u
Console.WriteLine(upsert.Values.Count); // 1
```

## Related reference

[Assembly](index.md) · [OqlInsertExpression](oql-insert-expression.md) · [OqlExplainExpression](oql-explain-expression.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlUpsertExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlDmlParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentUpsertTests.cs`.
