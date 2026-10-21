# KeyValueClientException

A key-value client failure retaining its model error kind and wire category.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.KeyValuePair.Client`.

Assembly: `Assimalign.Cohesion.Database.KeyValuePair.Client.dll`.
NuGet package: `Assimalign.Cohesion.Database.KeyValuePair.Client`.

## Signature

```csharp
public sealed class KeyValueClientException : Exception
{
    public KeyValueClientException(KeyValueClientErrorKind kind, ProtocolErrorCode code, string message);
    public KeyValueClientException(KeyValueClientErrorKind kind, ProtocolErrorCode code,
        string message, Exception? innerException);
    public IReadOnlyList<Diagnostic> Diagnostics { get; }
    public KeyValueClientErrorKind Kind { get; }
    public ProtocolErrorCode Code { get; }
    public bool ConnectionUsable { get; }
}
```

## Handling failures

This exception derives directly from `System.Exception`. Catch `KeyValueClientException` for typed
remote failures; `catch (DatabaseException)` no longer catches them. Diagnostics is locally declared
and always empty and non-null, even when the inner exception has diagnostics. The inner exception, Kind and Code
remain available.

ConnectionUsable is true for ParseFailure, ExecutionFailure, TransactionAborted and MalformedResult.
An engine write conflict maps to ExecutionFailure and may be retried as a command. Conditional
compare-and-swap misses are returned as outcomes rather than exceptions. Incomplete exchanges,
decoder failures and cancellation invalidate the pooled transport.

[Assembly](index.md) · [Design](design.md) · [Migration guide](../../../../database/model-project-migration.md)

## Terminal errors

A queued shared terminal Error is consumed before the next request writes, including after
transport close. Code retains the peer's category and the connector is discarded. A clean handshake
EOF uses Internal with a startup-specific message; unexpected/malformed terminal frames use
ProtocolViolation. See [shared admission](../assimalign-cohesion-database-client/database-connection.md#queued-terminal-errors).

## Sources

- **Terminal conformance** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Client/tests/KeyValueClientTerminalErrorTests.cs`.

- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Client/src/Exceptions/KeyValueClientException.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Client/tests/ClientExceptionDiagnosticsTests.cs`.
