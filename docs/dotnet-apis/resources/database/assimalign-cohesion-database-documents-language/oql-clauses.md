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
    // Other query, expression and DDL clause constants are unchanged.
}
```

## Behavior and exceptions

These transaction constants are accepted by OqlLanguageProfile.Instance. START maps to Begin;
there is no separate Start constant. Profile support describes grammar admission, not transport
support. Wire controls require negotiated transaction-status (CD07002 when absent). Reading constants throws
no exceptions.

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
