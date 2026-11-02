# OqlSubqueryExpression

A SELECT used as an array-valued expression.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlSubqueryExpression : OqlExpression
{
    public OqlSubqueryExpression(OqlSelectExpression query, Location? location = null);
    public OqlSelectExpression Query { get; }
}
```

## Behavior and exceptions

Construction retains Query and throws ArgumentNullException if it is null. Location is the
complete parenthesized source span. Query includes the projection mode: named projections
yield objects; IsValue represents RAW/VALUE. Execution returns an owned JSON array, including
an empty array for zero rows, and shares the outer statement's snapshot and budget.

Direct IN subqueries require RAW/VALUE (CD00014). Correlation, derived FROM and DML subqueries
report CK00001. Shared memory exhaustion reports CD05018; over-deep projections report CD05015.
These codes have no assigned SQLSTATE. See [evaluation rules](../../../../database/document/language/expressions/subqueries.md#evaluation).

## Example

From OqlSubqueryParserTests:

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "SELECT (SELECT RAW n FROM d) FROM c");
var select = (OqlSelectExpression)statement.OqlExpression;
var nested = (OqlSubqueryExpression)select.Projections[0].Expression;
Console.WriteLine(nested.Query.IsValue);    // True
Console.WriteLine(nested.Query.Collection); // d
```

Check statement.Diagnostics before planning. An IN subquery occupies OqlInExpression.Array,
with an empty Values list. OqlClauses.Subquery and OqlClauses.Exists are admitted by the profile.

## Related reference

[Assembly](index.md) · [OqlSelectExpression](oql-select-expression.md) · [OqlInExpression](oql-in-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlSubqueryExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlSubqueryParserTests.cs`.
