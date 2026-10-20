# DocumentVersion

A monotonically increasing document version used for optimistic concurrency.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.Documents.Protocol`.

Assembly: `Assimalign.Cohesion.Database.Documents.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Documents.Protocol`.

## Signature

```csharp
public readonly record struct DocumentVersion(ulong Value) : IComparable<DocumentVersion>;
```

## Behavior

This type now belongs to the model Protocol package. Its namespace and assembly identity changed;
recompile callers and update imports. Wire and disk formats are unchanged. No compatibility assembly
or type forwarder is provided.

The underlying `Value` is an unsigned 64-bit value; comparison follows that value.

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md) · [Document](document.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Protocol/src/ValueObjects/DocumentVersion.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Protocol/docs/DESIGN.md`.
