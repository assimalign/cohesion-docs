# OqlCreateIndexExpression

An index definition over ordered document paths.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlCreateIndexExpression : OqlExpression
{
    public OqlCreateIndexExpression(string indexName, string collection,
        OqlPathExpression path, Location? location = null);
    public OqlCreateIndexExpression(string indexName, string collection,
        IReadOnlyList<OqlPathExpression> paths, bool isUnique, bool ifNotExists,
        Location? location = null);
    public string IndexName { get; }
    public string Collection { get; }
    public OqlPathExpression Path { get; }
    public IReadOnlyList<OqlPathExpression> Paths { get; }
    public bool IsUnique { get; }
    public bool IfNotExists { get; }
}
```

## Behavior and exceptions

The single-path constructor supplies false for both flags. Paths is a read-only copy and Path
returns Paths[0]. A null list throws ArgumentNullException. Construction does not validate its
count; Path on an empty list throws ArgumentOutOfRangeException. Planning refuses a count outside
1..32 with DatabaseException/CD03022 and duplicate paths with CD05001. Text uses CD00009
at the 33rd key and CD00002 for duplicates. Neither count code has an assigned SQLSTATE.

IfNotExists preserves any existing same-named definition. Build/write UNIQUE failures use
[DocumentConstraintViolationException](../assimalign-cohesion-database-documents/document-constraint-violation-exception.md),
CD03020 (23505) or CD02021. See [CREATE INDEX](../../../../database/document/language/statements/create-index.md)
for ownership, format and NULL/MISSING rules.

## Example

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "CREATE UNIQUE INDEX IF NOT EXISTS uq ON items (a, b)");
var create = (OqlCreateIndexExpression)statement.OqlExpression;
Console.WriteLine(create.Paths.Count); // 2
Console.WriteLine(create.IsUnique);    // True
Console.WriteLine(create.IfNotExists); // True
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [OqlDropIndexExpression](oql-drop-index-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlCreateIndexExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlCompositeIndexParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/Planning/DocumentPlanner.cs`.
