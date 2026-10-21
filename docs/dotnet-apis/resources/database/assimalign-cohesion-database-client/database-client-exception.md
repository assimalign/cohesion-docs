# DatabaseClientException

A shared client failure with a wire category and locally owned structured diagnostics.

Namespace: `Assimalign.Cohesion.Database.Client`.

## Signature

```csharp
public class DatabaseClientException : Exception
{
    public DatabaseClientException(ProtocolErrorCode code, string message);
    public DatabaseClientException(ProtocolErrorCode code, string message, Exception? innerException);
    public DatabaseClientException(ProtocolErrorCode code, string message,
        IReadOnlyList<ProtocolDiagnostic> diagnostics, Exception? innerException);
    public ProtocolErrorCode Code { get; }
    public IReadOnlyList<Diagnostic> Diagnostics { get; }
}
```

## Behavior and exceptions

The diagnostic overload maps protocol Warning/Error entries into Database.Types.Diagnostic,
preserving nullable offsets and one-based line numbers. Diagnostics is declared locally and holds
a defensive read-only snapshot, stably ordered by source offset with unlocated entries last.
Message retains the Error frame text. Constructors without diagnostics return an empty snapshot;
a null diagnostic list is rejected with ArgumentNullException. SQL/Graph wrappers copy these entries
from an actual DatabaseClientException or their own nested client exception. Malformed frames fail
the exchange and discard its transport.

DatabaseClientException derives directly from System.Exception. Catch it for shared-core failures;
catch the model client exception for typed APIs. Catching DatabaseException no longer catches remote
failures. Model client exceptions are siblings of this class, not its subclasses.

## Example

```csharp
var failure = new DatabaseClientException(ProtocolErrorCode.ExecutionFailure, "Rejected", [
    new ProtocolDiagnostic(ProtocolDiagnosticSeverity.Error, "COHSQLE015", "Unknown column", 7, 8, 1)
], null);
Console.WriteLine(failure.Diagnostics[0].Code); // COHSQLE015
```

## Related reference

[Assembly](index.md) · [DatabaseException](../assimalign-cohesion-database/database-exception.md) · [ProtocolDiagnostic](../assimalign-cohesion-database-protocol/protocol-diagnostic.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/src/Exceptions/DatabaseClientException.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/tests/SqlWireDiagnosticTests.cs`.
