# DatabaseConnection

An authenticated, family-bound rental with observable protocol agreement.

Namespace: `Assimalign.Cohesion.Database.Client`.

## Changed members

```csharp
public ProtocolVersion ServerVersion { get; private set; }
public IReadOnlyList<string> Capabilities { get; private set; }
```

## Behavior and exceptions

The shared handshake advertises the selected family's capabilities. Ready applies the negotiated
version and validated intersection. Empty Ready means 2.0 with no capabilities; different majors,
unsolicited agreements, invalid names or malformed UTF-8 fail the handshake and discard the
transport. Failures surface as DatabaseClientException with the wire category; cancellation
remains OperationCanceledException. The properties are read-only to consumers and throw no
additional validation exception. Each model client selects its family at composition time.

## Example

With an authenticated connection rented from the shared client:

```csharp
Console.WriteLine(connection.ServerVersion); // 2.1 against a current endpoint
foreach (string name in connection.Capabilities) Console.WriteLine(name);
// SQL/GQL can include diagnostics; Documents, Blob and Key-Value do not.
```

## Related reference

[Assembly](index.md) · [Negotiation](../assimalign-cohesion-database-protocol/design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/src/DatabaseConnection.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/tests/ProtocolNegotiationTests.cs`.
