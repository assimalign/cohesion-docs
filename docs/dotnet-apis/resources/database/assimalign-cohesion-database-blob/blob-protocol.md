# BlobProtocol

The `BlobProtocol` type is part of the documented `Assimalign.Cohesion.Database.Blob` API.

> **Status:** Partial.

Defines the Blob message family and its bounded content payload size.

Namespace: `Assimalign.Cohesion.Database.Blob`.

## Documented behavior

Immutable `Family` and the 65,536-byte `MaxChunkLength` constant

Sequential `SendAsync` and `ReceiveAsync` over a channel bound to `BlobProtocol.Family`

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Blob`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/docs/Assembly/Assimalign.Cohesion.Database.Blob/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/src/Protocol/BlobProtocol.cs`.
