# OqlClauses

Stable clause names used by the OQL language profile.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public static class OqlClauses
{
    public const string Begin = "BEGIN";
    public const string Commit = "COMMIT";
    public const string Rollback = "ROLLBACK";
    public const string ArrayComprehension = "ARRAY ... FOR";
    public const string FirstComprehension = "FIRST ... FOR";
    public const string Subquery = "SUBQUERY";
    public const string DerivedFrom = "DERIVED FROM";
    public const string ExpressionFrom = "EXPRESSION FROM";
    public const string Exists = "EXISTS";
    public const string Join = "JOIN";
    public const string LeftJoin = "LEFT JOIN";
    public const string Nest = "NEST";
    public const string LeftNest = "LEFT NEST";
    public const string Union = "UNION";
    public const string Intersect = "INTERSECT";
    public const string Except = "EXCEPT";
    public const string Merge = "MERGE";
    public const string Over = "OVER";
    public const string Window = "WINDOW";
    // Other query, expression and DDL clause constants are unchanged.
}
```

## Behavior and exceptions

These transaction constants are accepted by OqlLanguageProfile.Instance. START maps to Begin;
there is no separate Start constant. Profile support describes executable capability, not transport
support. Wire controls require negotiated transaction-status (CD07002 when absent). Reading constants throws
no exceptions.

ArrayComprehension and FirstComprehension are also admitted by the profile. Both produce
[OqlComprehensionExpression](oql-comprehension-expression.md); their retired function spellings
remain CK00001.

Subquery and Exists are admitted for [expression subqueries](oql-subquery-expression.md)
and the EXISTS unary operator. Correlated and DML forms execute.

DerivedFrom and ExpressionFrom are admitted by the profile and use
[OqlSelectExpression.Source](oql-select-expression.md). See [FROM](../../../../database/document/language/clauses/from.md).

Join, LeftJoin, Nest and LeftNest are admitted for [key JOIN/NEST](../../../../database/document/language/clauses/join.md).
They produce OqlJoinExpression bindings; general predicates remain CK00001.

Union, Intersect and Except are admitted in every query position, including their ALL forms,
and produce [OqlSetOperationExpression](oql-set-operation-expression.md).

Over and Window are supported for the eight ranking/navigation functions. Merge is a stable
name but Supports(OqlClauses.Merge) is false: its AST parses, binds and explains, while execution
refuses CD05022 until OQ-M2. See [MERGE](oql-merge-expression.md) and [windows](oql-window-expression.md).

## Example

```csharp
Console.WriteLine(OqlLanguageProfile.Instance.Supports(OqlClauses.Begin)); // True
Console.WriteLine(OqlLanguageProfile.Instance.Supports(OqlClauses.Rollback)); // True
```

## Related reference

[Assembly](index.md) · [OqlTransactionExpression](oql-transaction-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/src/OqlClauses.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlLanguageProfileTests.cs`.
