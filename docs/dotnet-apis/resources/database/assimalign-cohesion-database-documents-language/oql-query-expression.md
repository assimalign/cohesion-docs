# OqlQueryExpression

A query block or compound query with ordering and paging in its own scope.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public abstract class OqlQueryExpression : OqlExpression
{
    public IReadOnlyList<OqlOrdering> OrderBy { get; }
    public bool IsValue { get; }
    public OqlExpression? Limit { get; }
    public OqlExpression? Offset { get; }
}
```

## Behavior and exceptions

The private protected constructor closes the family to external derivation. Its sealed leaves are
[OqlSelectExpression](oql-select-expression.md) and [OqlSetOperationExpression](oql-set-operation-expression.md).
OrderBy, Limit and Offset belong to this query scope. IsValue is true when every query block returns
one RAW or VALUE expression.

OqlQueryStatement.OqlExpression retains the broader OqlExpression type because writes, DDL and
transaction statements have their own roots. OqlSubqueryExpression.Query and INSERT/UPSERT Select
use OqlQueryExpression. UNION, INTERSECT and EXCEPT, with and without ALL, execute in every query
position. Inspect statement diagnostics before planning; compound-order subqueries remain CK00001.

## Example

Following OqlSetOperationParserTests:

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "SELECT RAW x FROM [1] AS x UNION ALL SELECT RAW y FROM [2] AS y");
var query = (OqlQueryExpression)statement.OqlExpression;
Console.WriteLine(query.IsValue); // True
```

## Related reference

[Assembly](index.md) · [Subqueries](oql-subquery-expression.md) · [SELECT](../../../../database/document/language/statements/select.md#set-operations)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryExpression.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryStatement.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlSetOperationParserTests.cs`.
