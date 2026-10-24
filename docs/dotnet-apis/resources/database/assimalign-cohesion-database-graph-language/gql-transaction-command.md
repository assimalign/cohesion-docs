# GqlTransactionCommand

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

```csharp
public enum GqlTransactionCommand : byte { None, Start, Commit, Rollback }
```

GqlQueryExpression.TransactionCommand returns this command for standalone parsed controls; None identifies ordinary queries. The parser recognizes START TRANSACTION, COMMIT and ROLLBACK. No new wire transaction message is added. See [transaction statements](../../../../database/graph/language/statements/transactions.md).

## Expression property

GqlQueryExpression exposes public GqlTransactionCommand TransactionCommand { get; internal init; }. External consumers read the parsed command; parser construction controls the setter.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlTransactionCommand.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlQueryExpression.cs`.
