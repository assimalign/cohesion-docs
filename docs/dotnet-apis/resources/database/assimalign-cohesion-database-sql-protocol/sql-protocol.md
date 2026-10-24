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
recompile callers and update imports. Wire and disk formats are unchanged. No compatibility assembly
or type forwarder is provided.

The Sql family owns identifiers 5–9 and advertises the shared diagnostics and transaction-status capabilities. Importing
this namespace also brings its `ProtocolMessageType` extension identifiers into scope.

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md) · [SqlProtocolMessageType](sql-protocol-message-type.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Protocol/src/SqlProtocol.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Protocol/docs/DESIGN.md`.
