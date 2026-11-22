# OqlSetOperationExpression

Combines two query values with a SQL++ set operation.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public sealed class OqlSetOperationExpression : OqlQueryExpression
{
    public OqlSetOperationExpression(OqlQueryExpression left, OqlQueryExpression right,
        OqlSetOperation operation, bool isAll = false, IReadOnlyList<OqlOrdering>? orderBy = null,
        OqlExpression? limit = null, OqlExpression? offset = null, Location? location = null);
    public OqlQueryExpression Left { get; }
    public OqlQueryExpression Right { get; }
    public OqlSetOperation Operation { get; }
    public bool IsAll { get; }
}

public enum OqlSetOperation { Union, Intersect, Except }
```

## Behavior and exceptions

Left and Right retain query operands, including parenthesized local ordering/paging. All operators
have equal, left-associative precedence. IsAll selects multiset behavior: UNION concatenates,
INTERSECT keeps min(left count, right count), and EXCEPT keeps max(left count - right count, 0).
Without ALL each result value appears once. Inherited OrderBy, Limit and Offset apply to the compound. Inherited IsValue
is true only when both operands are value queries. A null ordering list means no ordering.

Null operands throw ArgumentNullException; an undefined operation throws ArgumentOutOfRangeException.
Construction does not execute. All three operators execute in every query position, including
expression subqueries, derived FROM, correlated operands and INSERT/UPSERT SELECT. All-RAW/VALUE compound ordering uses
CD01037; unbound members, META or star use CD01038. See [set-operation behavior](../../../../database/document/language/statements/select.md#set-operations).

## Example

Following OqlSetOperationParserTests:

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse(
    "SELECT x AS n FROM [1] AS x UNION ALL SELECT y AS n FROM [2] AS y ORDER BY n");
var compound = (OqlSetOperationExpression)statement.OqlExpression;
Console.WriteLine(compound.Operation);     // Union
Console.WriteLine(compound.IsAll);         // True
Console.WriteLine(compound.OrderBy.Count); // 1
```

## Related reference

[Assembly](index.md) · [Query family](oql-query-expression.md) · [OqlSelectExpression](oql-select-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlQueryExpression.SetOperation.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlSetOperationParserTests.cs`.
