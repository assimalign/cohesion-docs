# KeyValueProtocol

The immutable KeyValuePair endpoint message family.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.KeyValuePair.Protocol`.

Assembly: `Assimalign.Cohesion.Database.KeyValuePair.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.KeyValuePair.Protocol`.

## Signature

```csharp
public static class KeyValueProtocol
{
    public static ProtocolMessageFamily Family { get; }
}
```

## Behavior

This type now belongs to the model Protocol package. Its namespace and assembly identity changed;
recompile callers and update imports. Wire and disk formats are unchanged. No compatibility assembly
or type forwarder is provided.

The KeyValuePair family owns identifiers 5–9. Importing this namespace also brings its
`ProtocolMessageType` extension identifiers into scope.

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md) · [KeyValueProtocolMessageType](key-value-protocol-message-type.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Protocol/src/KeyValueProtocol.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Protocol/docs/DESIGN.md`.
