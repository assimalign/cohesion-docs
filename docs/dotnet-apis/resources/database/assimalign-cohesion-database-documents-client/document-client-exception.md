# DocumentClientException

Namespace: `Assimalign.Cohesion.Database.Documents.Client`.

## Signature

```csharp
public sealed class DocumentClientException : DatabaseException
{
    public DocumentClientException(ProtocolErrorCode code, string message,
        Exception? innerException = null, IReadOnlyList<Diagnostic>? diagnostics = null);
    public ProtocolErrorCode Code { get; }
}
```

## Behavior and exceptions

Retains the protocol category and server's composed CODE: message. Inherited Diagnostics copies
and orders supplied entries; null means an empty collection. It preserves parser and engine
codes/spans rather than extracting codes from Message. Shared dial/handshake failures remain inner
exceptions. No extra validation exception is introduced by this constructor.
Connection reuse follows fully decoded model Failure completion, independently of exception ancestry.

## Example

```csharp
try { await connection.ExecuteAsync("DROP COLLECTION absent"); }
catch (DocumentClientException failure) {
    Console.WriteLine(failure.Code);                // ExecutionFailure
    Console.WriteLine(failure.Diagnostics[0].Code); // COHDOC002
}
// A subsequent QueryAsync on this connection can execute.
```

## Related reference

[DatabaseException](../assimalign-cohesion-database/database-exception.md) · [OQL codes](../../../../database/document/language/diagnostics.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/src/Exceptions/DocumentClientException.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentClientDiagnosticTests.cs`.
