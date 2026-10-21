# GraphClientException

A Graph client failure with a stable wire category and structured diagnostics.

## Signature

```csharp
public sealed class GraphClientException : Exception
{
    public GraphClientException(ProtocolErrorCode code, string message, Exception? innerException = null);
    public IReadOnlyList<Diagnostic> Diagnostics { get; }
    public ProtocolErrorCode Code { get; }
}
```

## Behavior and exceptions

GraphClientException derives directly from System.Exception. Catch GraphClientException for remote
Graph failures; catching DatabaseException no longer handles them.

Diagnostics is declared locally and copies entries from an inner DatabaseClientException or nested
GraphClientException, otherwise retaining an empty list. The snapshot is read-only, defensively
copied and stably sorted by offset, with unlocated entries last. Code preserves
the wire category and Message preserves composed server text. Constructors add no new validation
exception. Connection reuse depends on terminal-response evidence: scalar and pre-output path
rejections preserve it; unfinished streams, cancellation and protocol failures discard it.

## Example

```csharp
try { await connection.QueryAsync("MATCH (n) WHERE $missing RETURN n.name"); }
catch (GraphClientException failure) {
    Console.WriteLine(failure.Diagnostics[0].Code); // COHDBG001 with diagnostics negotiated
}
```

## Related reference

[Assembly](index.md) · [DatabaseException](../assimalign-cohesion-database/database-exception.md)

## Terminal errors

A queued shared terminal Error is consumed before the next request writes, including after
transport close. Code retains the peer's category and the connector is discarded. A clean handshake
EOF uses Internal with a startup-specific message; unexpected/malformed terminal frames use
ProtocolViolation. See [shared admission](../assimalign-cohesion-database-client/database-connection.md#queued-terminal-errors).

## Sources

- **Terminal conformance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphClientTerminalErrorTests.cs`.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/src/Exceptions/GraphClientException.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphWireDiagnosticTests.cs`.
