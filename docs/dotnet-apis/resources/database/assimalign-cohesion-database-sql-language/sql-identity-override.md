# SqlIdentityOverride

The INSERT policy for explicit identity values.

Namespace: `Assimalign.Cohesion.Database.Sql.Language`.

## Signature

```csharp
public enum SqlIdentityOverride
{
    None,
    System,
    User
}
```

## Behavior and exceptions

None uses the column's generation policy. System represents OVERRIDING SYSTEM VALUE, permitting
explicit values for ALWAYS identities. User represents OVERRIDING USER VALUE, generating for
either mode after binding and assignment validation of supplied inputs, including NULL.
SqlInsertExpression.Overriding and SqlMergeWhenClause.Overriding expose the parsed value.
It applies to VALUES, INSERT SELECT, ON CONFLICT proposals and MERGE INSERT actions.

Reading the enum adds no exceptions. Without an override, explicit ALWAYS input reports
DatabaseException/CS03025 (428C9). USER still rejects invalid assignment types and string
conversions; for example, CAST('abc' AS INT) in a discarded VALUES/MERGE input uses CS02022
(22P02). Typed numeric casts and volatile inputs discarded in these lists do not execute;
SELECT sources still materialize normally. See [the assignment rules](../../../../database/sql/language/clauses/identity.md#assignment-and-overrides).

## Example

From the completed forms in SqlIdentityParserTests:

```csharp
var statement = (SqlQueryStatement)new SqlQueryParser().Parse(
    "INSERT INTO t OVERRIDING SYSTEM VALUE VALUES (1);");
var insert = (SqlInsertExpression)statement.SqlExpression;
Console.WriteLine(insert.Overriding == SqlIdentityOverride.System); // True
```

For an ALWAYS identity this inserts 1 explicitly without advancing its sequence.

## Related reference

[Assembly](index.md) · [SqlInsertExpression](sql-insert-expression.md) · [SqlMergeWhenClause](sql-merge-when-clause.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/src/SqlIdentityOverride.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/tests/SqlIdentityParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlIdentityReviewTests.Binding.cs`.
