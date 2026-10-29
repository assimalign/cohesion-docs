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
    // Other query, expression and DDL clause constants are unchanged.
}
```

## Behavior and exceptions

These transaction constants are accepted by OqlLanguageProfile.Instance. START maps to Begin;
there is no separate Start constant. Profile support describes grammar admission, not transport
support. Wire controls require negotiated transaction-status (CD07002 when absent). Reading constants throws
no exceptions.

ArrayComprehension and FirstComprehension are also admitted by the profile. Both produce
[OqlComprehensionExpression](oql-comprehension-expression.md); their retired function spellings
remain CK00001.

Subquery and Exists are admitted for uncorrelated [expression subqueries](oql-subquery-expression.md)
and the EXISTS unary operator. Correlated and DML forms remain CK00001.

DerivedFrom and ExpressionFrom are admitted by the profile and use
[OqlSelectExpression.Source](oql-select-expression.md). See [FROM](../../../../database/document/language/clauses/from.md).

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
