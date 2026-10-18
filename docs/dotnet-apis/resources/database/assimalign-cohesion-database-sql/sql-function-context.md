# SqlFunctionContext

The database, input collation and cancellation token for one function invocation.

Namespace: `Assimalign.Cohesion.Database.Sql`.

## Signature

```csharp
public readonly ref struct SqlFunctionContext
{
    public SqlFunctionContext(DatabaseName database, Collation? collation,
        CancellationToken cancellationToken);
    public DatabaseName Database { get; }
    public Collation Collation { get; }
    public CancellationToken CancellationToken { get; }
}
```

## Behavior and exceptions

Scalars read this context through SqlArguments.Context; aggregate accumulator creation receives it
directly. The ref struct cannot be retained beyond the call. A null collation selects Collation.Binary.
The default context has an empty database, binary collation and a never-canceled token. Construction
and property reads introduce no validation exceptions.

Input collation is resolved during planning: an explicit argument COLLATE, then a column's collation,
then the database default. Implementations can observe the token during long work. A scalar's
OperationCanceledException propagates as cancellation only when this context's token is canceled;
otherwise it is a function failure, COHSQLE007 (38000). Ordinary implementation exceptions are
wrapped with that code, while DatabaseException propagates unchanged; stack and allocation failures
retain their special handling.

T1 clocks use an internal transaction-start carrier in this context. The engine captures it at
BEGIN/API transaction start or automatic statement start, and passes it through bound calls and SQL
function bodies. CURRENT_TIMESTAMP, NOW() and their date/time relatives agree for the transaction.
There is no public TransactionStart member or clock-provider option. A directly constructed public
context does not install a transaction clock.

## Example

The function tests construct public contexts to supply cancellation:

```csharp
using var cancellation = new CancellationTokenSource();
var context = new SqlFunctionContext("db", null, cancellation.Token);
Console.WriteLine(context.Collation == Collation.Binary); // True
Console.WriteLine(context.CancellationToken.IsCancellationRequested); // False
cancellation.Cancel();
context.CancellationToken.ThrowIfCancellationRequested(); // OperationCanceledException
```

Use [SQL transaction execution](../../../../database/sql/language/functions/builtin-functions.md#clock-temporal-and-uuid-functions)
for clock values; the function test clock seam remains internal.

## Related reference

[Assembly](index.md) · [SqlFunctionCollection](sql-function-collection.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Functions/SqlFunctionContext.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Functions/SqlScalarFunction.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlFunctionCancellationTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlFunctionExtensibilityTests.Contracts.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlT1TemporalTests.cs`.
