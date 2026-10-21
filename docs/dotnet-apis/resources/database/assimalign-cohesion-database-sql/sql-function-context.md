# SqlFunctionContext

The database, input collation, transaction timestamp and cancellation token for one function invocation.

Namespace: `Assimalign.Cohesion.Database.Sql`.

## Signature

```csharp
public readonly ref struct SqlFunctionContext
{
    public SqlFunctionContext(DatabaseName database, Collation? collation,
        CancellationToken cancellationToken);
    public SqlFunctionContext(DatabaseName database, Collation? collation,
        CancellationToken cancellationToken, DateTimeOffset transactionTimestamp);
    public DateTimeOffset? TransactionTimestamp { get; }
    public DatabaseName Database { get; }
    public Collation Collation { get; }
    public CancellationToken CancellationToken { get; }
}
```

## Behavior and exceptions

Scalars read this context through SqlArguments.Context; aggregate accumulator creation receives it
directly. The ref struct cannot be retained beyond the call. A null collation selects Collation.Binary.
The default context has an empty database, binary collation, a never-canceled token and a null
TransactionTimestamp. The three-argument constructor also leaves the timestamp null; the
four-argument constructor supplies the exact timestamp. Construction
and property reads introduce no validation exceptions.

Input collation is resolved during planning: an explicit argument COLLATE, then a column's collation,
then the database default. Implementations can observe the token during long work. A scalar's
OperationCanceledException propagates as cancellation only when this context's token is canceled;
otherwise it is a function failure, COHSQLE007 (38000). Ordinary implementation exceptions are
wrapped with that code, while DatabaseException propagates unchanged; stack and allocation failures
retain their special handling.

TransactionTimestamp exposes the instant captured at BEGIN/API transaction start or automatic
statement start, matching NOW() and CURRENT_TIMESTAMP. It stays stable across every statement of
an explicit transaction; each autocommit statement receives its own start. Scalars read it through
SqlArguments.Context. Aggregates receive it during accumulator creation and row accumulation,
including DISTINCT and ordered inputs. SQL function bodies and table/domain CHECK calls retain
the same instant. A null value means no statement or transaction context, as in a default or
direct three-argument context.

## Example

The function tests construct public contexts to supply cancellation:

```csharp
using var cancellation = new CancellationTokenSource();
var context = new SqlFunctionContext("db", null, cancellation.Token);
Console.WriteLine(context.Collation == Collation.Binary); // True
Console.WriteLine(context.CancellationToken.IsCancellationRequested); // False
Console.WriteLine(context.TransactionTimestamp is null); // True
cancellation.Cancel();
context.CancellationToken.ThrowIfCancellationRequested(); // OperationCanceledException
```

This application function follows SqlFunctionTimestampTests. Register it before building the
engine, then execute each statement on the same session:

```csharp
var builder = SqlDatabaseEngine.CreateBuilder("function-timestamp");
builder.Functions.Add(new TimestampValueFunction());
await using var engine = await builder.BuildAsync(CancellationToken.None);
var database = await engine.CreateDatabaseAsync("db", CancellationToken.None);
await using var session = await database.CreateSessionAsync(CancellationToken.None);
await session.ExecuteAsync("BEGIN", cancellationToken: CancellationToken.None);
await using var rows = (DatabaseResultSet)await session.ExecuteAsync(
    "SELECT timestamp_value() = now(), timestamp_value() = CURRENT_TIMESTAMP",
    cancellationToken: CancellationToken.None);
await foreach (var row in rows.GetRowsAsync(CancellationToken.None))
{
    Console.WriteLine(row.GetValue(0)); // True
    Console.WriteLine(row.GetValue(1)); // True
}
await session.ExecuteAsync("COMMIT", cancellationToken: CancellationToken.None);

sealed class TimestampValueFunction : SqlScalarFunction
{
    public TimestampValueFunction()
        : base("timestamp_value", [], SqlType.TimestampTz, SqlFunctionVolatility.Stable) { }

    protected override SqlValue InvokeCore(scoped in SqlArguments arguments)
        => SqlValue.FromDateTimeOffset(arguments.Context.TransactionTimestamp!.Value);
}
```

Import System, System.Threading, Assimalign.Cohesion.Database.Execution and
Assimalign.Cohesion.Database.Sql. Repeating the SELECT before COMMIT keeps the same instant.
For direct function tests, the four-argument constructor accepts a DateTimeOffset explicitly.

## Related reference

[Assembly](index.md) · [SqlFunctionCollection](sql-function-collection.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlFunctionTimestampTests.cs`.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Functions/SqlFunctionContext.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Functions/SqlScalarFunction.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlFunctionCancellationTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlFunctionExtensibilityTests.Contracts.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlT1TemporalTests.cs`.
