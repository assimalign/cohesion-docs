# BlobProtocol

The `BlobProtocol` type is part of the documented `Assimalign.Cohesion.Database.Blob.Protocol` API.

> **Status:** Partial.

Defines the Blob message family and its bounded content payload size.

Namespace: `Assimalign.Cohesion.Database.Blob.Protocol`.

Assembly: `Assimalign.Cohesion.Database.Blob.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Blob.Protocol`.

## Documented behavior

Immutable `Family` and the 65,536-byte `MaxChunkLength` constant

Sequential `SendAsync` and `ReceiveAsync` over a channel bound to `BlobProtocol.Family`

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Blob.Protocol`](index.md).
- **Design** — [Lifetime and implementation decisions](../assimalign-cohesion-database-blob/design.md).
- **Examples** — [Source-backed usage](../assimalign-cohesion-database-blob/examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/docs/Assembly/Assimalign.Cohesion.Database.Blob/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Protocol/src/BlobProtocol.cs`.
