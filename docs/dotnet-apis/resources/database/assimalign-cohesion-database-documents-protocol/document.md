# Document

A stored document: identity, version, and UTF-8 JSON content.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.Documents.Protocol`.

Assembly: `Assimalign.Cohesion.Database.Documents.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Documents.Protocol`.

## Signature

```csharp
public readonly record struct Document(DocumentId Id, DocumentVersion Version, ReadOnlyMemory<byte> Content);
```

## Behavior

This type now belongs to the model Protocol package. Its namespace and assembly identity changed;
recompile callers and update imports. Wire and disk formats are unchanged. No compatibility assembly
or type forwarder is provided.

Content is UTF-8 JSON. Id is scoped to a collection; Version supports optimistic concurrency.

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md) · [DocumentId](document-id.md) · [DocumentVersion](document-version.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Protocol/src/ValueObjects/Document.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents.Protocol/docs/DESIGN.md`.
