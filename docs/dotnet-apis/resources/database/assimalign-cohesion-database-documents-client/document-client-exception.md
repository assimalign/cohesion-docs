# DocumentClientException

A Documents client failure retaining its wire category and explicit structured diagnostics.

Namespace: `Assimalign.Cohesion.Database.Documents.Client`.

## Signature

```csharp
public sealed class DocumentClientException : Exception
{
    public DocumentClientException(ProtocolErrorCode code, string message,
        Exception? innerException = null, IReadOnlyList<Diagnostic>? diagnostics = null);
    public IReadOnlyList<Diagnostic> Diagnostics { get; }
    public ProtocolErrorCode Code { get; }
}
```

## Behavior and exceptions

DocumentClientException derives directly from System.Exception. Catch DocumentClientException for
remote document failures; catching DatabaseException no longer handles them.

Retains the protocol category and server's composed CODE: message. Locally declared Diagnostics defensively copies
explicitly supplied entries into a read-only snapshot, stably ordered by offset with unlocated
entries last; a null argument produces an empty collection. Diagnostics itself is never null.
It does not infer diagnostics from the inner exception. It preserves parser and engine codes/spans
rather than extracting codes from Message. Shared dial/handshake failures remain inner exceptions.
No extra validation exception is introduced by this constructor. Connection reuse follows fully
decoded model Failure completion, independently of exception ancestry.

## Example

```csharp
try { await connection.ExecuteAsync("DROP COLLECTION absent"); }
catch (DocumentClientException failure) {
    Console.WriteLine(failure.Code);                // ExecutionFailure
    Console.WriteLine(failure.Diagnostics[0].Code); // CD01002
}
// A subsequent QueryAsync on this connection can execute.
```

## Related reference

[DatabaseException](../assimalign-cohesion-database/database-exception.md) · [OQL codes](../../../../database/document/language/diagnostics.md)

## Terminal errors

A queued shared terminal Error is consumed before the next request writes, including after
transport close. Code retains the peer's category and the connector is discarded. A clean handshake
EOF uses Internal with a startup-specific message; unexpected/malformed terminal frames use
ProtocolViolation. See [shared admission](../assimalign-cohesion-database-client/database-connection.md#queued-terminal-errors).

## Sources

- **Terminal conformance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentClientTerminalErrorTests.cs`.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/src/Exceptions/DocumentClientException.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Client/tests/DocumentClientDiagnosticTests.cs`.
