# DatabaseClientException

A shared client failure with a wire category and inherited structured diagnostics.

Namespace: `Assimalign.Cohesion.Database.Client`.

## Added constructor

```csharp
public DatabaseClientException(ProtocolErrorCode code, string message,
    IReadOnlyList<ProtocolDiagnostic> diagnostics, Exception? innerException);
public ProtocolErrorCode Code { get; }
```

## Behavior and exceptions

The added overload maps protocol Warning/Error entries into Database.Types.Diagnostic and the
DatabaseException carrier, preserving nullable offsets and one-based line. The root copies and
orders entries, with unlocated entries last. Message retains the Error frame text. Existing
constructors without diagnostics remain available. A null diagnostic list is rejected by the
sequence mapping with ArgumentNullException. Model SQL/GQL exceptions preserve these entries
through their inner DatabaseException. Malformed frames fail the exchange and discard its transport.

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
