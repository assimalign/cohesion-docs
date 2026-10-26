# OqlTransactionCommand

The operation represented by an OQL transaction control.

Namespace: `Assimalign.Cohesion.Database.Documents.Language`.

## Signature

```csharp
public enum OqlTransactionCommand { Begin, Commit, Rollback }
```

## Behavior and exceptions

BEGIN and START spellings parse as Begin. COMMIT and ROLLBACK parse as their corresponding
values. Enum access has no exceptions; OqlTransactionExpression refuses undefined values.
Execution uses the owning session, including over the wire. CD07002 applies only without
negotiated transaction-status.

## Example

```csharp
var statement = (OqlQueryStatement)new OqlQueryParser().Parse("COMMIT WORK");
Console.WriteLine(((OqlTransactionExpression)statement.OqlExpression).Command); // Commit
```

## Related reference

[Assembly](index.md) · [OqlTransactionExpression](oql-transaction-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Language/tests/OqlTransactionParserTests.cs`.
