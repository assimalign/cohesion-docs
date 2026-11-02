# SqlProtocol

The immutable Sql endpoint message family.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.Sql.Protocol`.

Assembly: `Assimalign.Cohesion.Database.Sql.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Sql.Protocol`.

## Signature

```csharp
public static partial class SqlProtocol
{
    public static ProtocolMessageFamily Family { get; }
}
```

## Behavior

This type now belongs to the model Protocol package. Its namespace and assembly identity changed;
recompile callers and update imports. The package split adds no disk format change. No compatibility assembly
or type forwarder is provided.

The Sql family owns identifiers 5–9 and advertises the shared diagnostics and transaction-status capabilities. Importing
this namespace also brings its `ProtocolMessageType` extension identifiers into scope.

Family.ResetStatement is `DISCARD ALL`. The exact two-byte statement Ready contains transaction
I/T/E and retained-state 0/1. Pool return uses the reset only when the latest Ready reports retained
state, after any needed rollback. See [Ready status](../assimalign-cohesion-database-protocol/protocol-transaction-status-message.md).

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md) · [SqlProtocolMessageType](sql-protocol-message-type.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Protocol/src/SqlProtocol.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Protocol/docs/DESIGN.md`.
