# GraphClientException

## Signature

```csharp
public sealed class GraphClientException : DatabaseException
{
    public GraphClientException(ProtocolErrorCode code, string message, Exception? innerException = null);
    public ProtocolErrorCode Code { get; }
}
```

## Behavior and exceptions

Copies diagnostics from an inner DatabaseException, otherwise retains an empty list. Code preserves
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

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/src/Exceptions/GraphClientException.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphWireDiagnosticTests.cs`.
