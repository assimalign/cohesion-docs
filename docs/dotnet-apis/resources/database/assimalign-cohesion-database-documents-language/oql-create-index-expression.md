# OqlCreateIndexExpression

An index definition over ordered scalar paths and an optional DISTINCT ARRAY comprehension.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlCreateIndexExpression : OqlExpression
{
    public OqlCreateIndexExpression(string indexName, string collection,
        OqlPathExpression path, Location? location = null);
    public OqlCreateIndexExpression(string indexName, string collection,
        IReadOnlyList<OqlPathExpression> paths, bool isUnique, bool ifNotExists,
        Location? location = null, bool includeMissing = false);
    public OqlCreateIndexExpression(string indexName, string collection,
        IReadOnlyList<OqlExpression> keys, bool isUnique, bool ifNotExists,
        Location? location = null, bool includeMissing = false);
    public IReadOnlyList<OqlExpression> Keys { get; }
    public string IndexName { get; }
    public string Collection { get; }
    public OqlPathExpression Path { get; }
    public IReadOnlyList<OqlPathExpression> Paths { get; }
    public bool IsUnique { get; }
    public bool IncludeMissing { get; }
    public bool IfNotExists { get; }
}
```

## Behavior and exceptions

The single-path constructor supplies false for both flags. Keys is a read-only copy. Each
OqlComprehensionExpression key denotes DISTINCT ARRAY (IsFirst must be false). Paths is a
read-only scalar list when all keys are paths, otherwise empty; use Keys for an array index.
Path returns Paths[0] and therefore cannot be used for an array index.
A null list throws ArgumentNullException. Construction does not validate its
count; Path on an empty list throws ArgumentOutOfRangeException. Planning refuses a count outside
1..32 with DatabaseException/CD03022 and duplicate paths with CD05001. Text uses CD00009
at the 33rd key and CD00002 for duplicates. Neither count code has an assigned SQLSTATE.

Engine binding validates the restricted source/projection/WHEN grammar before catalog writes:
CD01028 for a wrong element variable, CD02027 for an unsupported constant (including direct AST
Single/Double), CD03027 for an excluded expression, CD03028 for a predicate above 32 levels/128
nodes, and CD03025 for multiple/UNIQUE arrays. All are DatabaseException diagnostics with source
locations when available and no assigned SQLSTATE.

IfNotExists preserves any existing same-named definition. Build/write UNIQUE failures use
[DocumentConstraintViolationException](../assimalign-cohesion-database-documents/document-constraint-violation-exception.md),
CD03020 (23505) or CD02021. See [CREATE INDEX](../../../../database/document/language/statements/create-index.md)
for ownership, format and NULL/MISSING rules.

## Example

IncludeMissing defaults to false and applies only to the leading scalar key. Text places it
inside the key list: `(a INCLUDE MISSING, b)`. A leading array key with the flag uses CD03025;
misplaced text uses CD00002. COHESION_SCHEMA.INDEXES renders the suffix in PATH/PATHS[0]
and exposes INCLUDE_MISSING. These refusals have no assigned SQLSTATE.

```csharp
var included = (OqlCreateIndexExpression)((OqlQueryStatement)new OqlQueryParser().Parse(
    "CREATE INDEX ix ON c (a INCLUDE MISSING, b)")).OqlExpression;
Console.WriteLine(included.IncludeMissing); // True
```

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "CREATE UNIQUE INDEX IF NOT EXISTS uq ON items (a, b)");
var create = (OqlCreateIndexExpression)statement.OqlExpression;
Console.WriteLine(create.Paths.Count); // 2
Console.WriteLine(create.IsUnique);    // True
Console.WriteLine(create.IfNotExists); // True
```

The array parser tests also cover:

```csharp
var arrayStatement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "CREATE INDEX ix ON items (category, DISTINCT ARRAY v.price FOR v IN variants END)");
var arrayCreate = (OqlCreateIndexExpression)arrayStatement.OqlExpression;
Console.WriteLine(arrayCreate.Keys.Count);  // 2
Console.WriteLine(arrayCreate.Paths.Count); // 0
Console.WriteLine(arrayCreate.Keys[1] is OqlComprehensionExpression); // True
```

## Related reference

[Assembly](index.md) · [Design](design.md) · [OqlDropIndexExpression](oql-drop-index-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlCreateIndexExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlCompositeIndexParserTests.cs` and `OqlArrayIndexParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/src/Internal/Planning/DocumentPlanner.cs`.
