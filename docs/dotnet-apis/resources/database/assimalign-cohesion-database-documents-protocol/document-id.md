# DocumentId

A strongly-typed document identity, unique within a collection.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.Documents.Protocol`.

Assembly: `Assimalign.Cohesion.Database.Documents.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Documents.Protocol`.

## Signature

```csharp
public readonly struct DocumentId : IEquatable<DocumentId>
{
    public DocumentId(string value);
    public string Value { get; }
    public static implicit operator DocumentId(string value);
}
```

## Behavior

This type now belongs to the model Protocol package. Its namespace and assembly identity changed;
recompile callers and update imports. Wire and disk formats are unchanged. No compatibility assembly
or type forwarder is provided.

Construction rejects null or whitespace with `ArgumentException`. Identity equality and hashing
are ordinal. The default value exposes an empty string.

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md) · [Document](document.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Protocol/src/ValueObjects/DocumentId.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Protocol/docs/DESIGN.md`.
