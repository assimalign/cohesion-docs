# OqlComprehensionExpression

An ARRAY or FIRST mapping over one scoped array binding.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlComprehensionExpression : OqlExpression
{
    public OqlComprehensionExpression(string variable, OqlExpression collection,
        OqlExpression projection, OqlExpression? predicate = null,
        bool isFirst = false, Location? location = null);
    public string Variable { get; }
    public OqlExpression Collection { get; }
    public OqlExpression Projection { get; }
    public OqlExpression? Predicate { get; }
    public bool IsFirst { get; }
}
```

## Behavior and exceptions

IsFirst false represents ARRAY; true represents FIRST. Collection evaluates in the enclosing
scope; Projection and optional Predicate see Variable. Nested bindings can shadow outer names
and restore them at END. ARRAY keeps input order and omits mapped MISSING values. FIRST returns
the first qualifying mapping even when it is MISSING or NULL; no qualifying element yields
MISSING. Both propagate MISSING input and return NULL for NULL/non-array input. See the
[language reference](../../../../database/document/language/expressions/comprehensions.md).

Construction retains child references and throws ArgumentNullException for a null variable,
collection or projection. An empty variable is accepted by construction and refused by the
planner with DatabaseException/CD01027 before data access. Aggregates inside any child,
invalid grouped references and META on the inner binding use CD05001. Neither code has an
assigned SQLSTATE. Collection, mapping and predicate validation runs even for empty inputs.

## Example

Adapted from OqlComprehensionParserTests:

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "SELECT ARRAY (x + 1) FOR x IN d.tags WHEN x > 0 END AS a FROM items d");
var select = (OqlSelectExpression)statement.OqlExpression;
var mapping = (OqlComprehensionExpression)select.Projections[0].Expression;
Console.WriteLine(mapping.Variable); // x
Console.WriteLine(mapping.IsFirst);  // False
Console.WriteLine(mapping.Predicate is not null); // True
```

For a document with tags `[0,1,2]`, execution returns `{"a":[2,3]}`.
OqlClauses.ArrayComprehension (`ARRAY ... FOR`) and FirstComprehension (`FIRST ... FOR`)
are admitted by OqlLanguageProfile.Instance. Both forms share this AST type.

## Related reference

[Assembly](index.md) · [OqlClauses](oql-clauses.md) · [OqlArrayExpression](oql-array-expression.md) · [OqlQuantifiedExpression](oql-quantified-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlComprehensionExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlComprehensionParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/tests/DocumentQueryTests.Comprehensions.cs`.
