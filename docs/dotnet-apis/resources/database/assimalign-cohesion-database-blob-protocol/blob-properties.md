# BlobProperties

The catalog metadata of a stored blob.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.Blob.Protocol`.

Assembly: `Assimalign.Cohesion.Database.Blob.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Blob.Protocol`.

## Signature

```csharp
public readonly record struct BlobProperties(
    string Name,
    long Length,
    string? ContentType,
    ulong ETag,
    DateTimeOffset CreatedAt,
    DateTimeOffset ModifiedAt,
    uint Checksum);
```

## Behavior

This type now belongs to the model Protocol package. Its namespace and assembly identity changed;
recompile callers and update imports. Wire and disk formats are unchanged. No compatibility assembly
or type forwarder is provided.

ETag changes on each write. Length is the content byte count; Checksum is CRC-32.

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Protocol/src/ValueObjects/BlobProperties.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Protocol/docs/DESIGN.md`.
