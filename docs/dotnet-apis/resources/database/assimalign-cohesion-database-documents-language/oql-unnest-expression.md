# OqlUnnestExpression

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlUnnestExpression : OqlExpression
{
    public OqlUnnestExpression(OqlExpression collection, string variable,
        bool isLeft = false, Location? location = null);
    public OqlExpression Collection { get; }
    public string Variable { get; }
    public bool IsLeft { get; }
}
```

## Behavior and exceptions

Collection evaluates in the preceding binding scope. IsLeft preserves empty/non-array input
with a MISSING variable; false emits only array elements. Actual null elements remain NULL.
The constructor retains arguments without validation or evaluation. The engine rejects duplicate
or empty UNNEST names with DatabaseException/COHDOC023; parsed duplicates use OQL0010.
Invalid META/aggregate use gives COHDOC001. These codes have no assigned SQLSTATE.

## Example

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "SELECT t FROM items d LEFT UNNEST d.tags t");
var binding = ((OqlSelectExpression)statement.OqlExpression).Unnest[0];
Console.WriteLine(binding.Variable); // t
Console.WriteLine(binding.IsLeft);   // True
```

For {"tags":[]} this yields one MISSING cell; for {"tags":[null]} one NULL cell.
This follows OqlArrayParserTests and DocumentQueryTests.Arrays.

## Related reference

[Assembly](index.md) · [OqlSelectExpression](oql-select-expression.md) · [UNNEST](../../../../database/document/language/clauses/unnest.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlUnnestExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlArrayParserTests.cs`.
