# SqlProtocolMessageType

Message identifiers owned by the Sql endpoint.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.Sql.Protocol`.

Assembly: `Assimalign.Cohesion.Database.Sql.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Sql.Protocol`.

## Signature

```csharp
public enum SqlProtocolMessageType : byte
{
    Execute = 5,
    ResultHeader = 6,
    ResultRow = 7,
    ResultComplete = 8,
    Transaction = 9,
}
```

## Behavior

This type now belongs to the model Protocol package. Its namespace and assembly identity changed;
recompile callers and update imports. Wire and disk formats are unchanged. No compatibility assembly
or type forwarder is provided.

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md) · [SqlProtocol](sql-protocol.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Protocol/src/SqlProtocolMessageType.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Protocol/docs/DESIGN.md`.
