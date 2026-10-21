# SqlTimeoutException

Reports a server-side statement or lock timeout after statement rollback.

Namespace: `Assimalign.Cohesion.Database.Sql`.

## Signature

```csharp
public sealed class SqlTimeoutException : DatabaseException
{
    public string SqlState { get; }
}
```

## Behavior and exceptions

There is no public constructor. Statement expiration produces one Error diagnostic, COHSQLE043,
and SqlState 57014 (query_canceled). An individual lock-wait expiration produces COHSQLE044 and
55P03 (lock_not_available), retaining TransactionLockTimeoutException as its inner cause. Available
statement spans locate the diagnostic. A later statement deadline does not relabel an earlier
lock timeout. Caller cancellation retains OperationCanceledException.

The session and explicit transaction remain usable after the failed statement's effects and new
resources are rolled back/released. Earlier grants and upgrades survive until transaction end;
offline/abandoned writers retain locks for recovery. A started commit is never canceled. Remote
calls instead throw SqlClientException with ExecutionFailure and the existing negotiated diagnostics;
SqlState is not a new wire field.

## Example

In the blocked unique-key [session example](sql-database-session.md#example), catch the failure:

```csharp
catch (SqlTimeoutException failure) {
    Console.WriteLine(failure.SqlState);            // 55P03
    Console.WriteLine(failure.Diagnostics[0].Code); // COHSQLE044
}
```

The transaction remains active and can commit a subsequent successful statement.

## Related reference

[Assembly](index.md) · [Engine options](sql-database-engine-options.md) · [Session](sql-database-session.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Exceptions/SqlTimeoutException.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTimeoutConformanceTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlTimeoutRegressionTests.cs`.
