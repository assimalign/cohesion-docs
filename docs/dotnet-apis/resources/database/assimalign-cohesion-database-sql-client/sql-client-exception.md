# SqlClientException

## Signature

```csharp
public sealed class SqlClientException : DatabaseException
{
    public SqlClientException(SqlClientErrorKind kind, ProtocolErrorCode code, string message);
    public SqlClientException(SqlClientErrorKind kind, ProtocolErrorCode code, string message, Exception? innerException);
    public SqlClientErrorKind Kind { get; }
    public ProtocolErrorCode Code { get; }
    public bool ConnectionUsable { get; }
}
```

## Behavior and exceptions

The inner-exception constructor copies diagnostics when its inner cause is a DatabaseException. The three-argument constructor has an empty list. The carrier preserves code, severity and span while Kind/Code retain client and wire categories. ConnectionUsable is true for ParseFailure, ExecutionFailure, TransactionAborted, NotAuthorized and InvalidCast; protocol/connection failures are unusable. Constructors add no new validation exception.

## Example

```csharp
try { await connection.QueryAsync("SELECT absent FROM t;"); }
catch (SqlClientException failure) {
    Console.WriteLine(failure.Diagnostics[0].Code); // COHSQLE015 with diagnostics negotiated
    Console.WriteLine(failure.ConnectionUsable); // True
}
```

## Related reference

[Assembly](index.md) · [DatabaseException](../assimalign-cohesion-database/database-exception.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/src/Exceptions/SqlClientException.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlWireDiagnosticTests.cs`.
