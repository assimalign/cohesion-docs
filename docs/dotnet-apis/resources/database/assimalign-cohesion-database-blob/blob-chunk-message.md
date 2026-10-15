# BlobChunkMessage

The `BlobChunkMessage` type is part of the documented `Assimalign.Cohesion.Database.Blob` API.

> **Status:** Partial.

A nonempty content chunk, retaining its caller-owned buffer without copying.

Namespace: `Assimalign.Cohesion.Database.Blob`.

## Documented behavior

A caller-owned memory view; `ToFrame` and `Decode` validate the chunk size without copying

Metadata messages expose `Encode()` and static `Decode(ReadOnlySpan<byte>)`. Encoding or decoding
invalid values throws `ProtocolException`. Names are nonempty; strings use strict UTF-8 and each is
limited to 65,535 encoded bytes. Completion and acknowledgement counts are nonnegative.
`BlobChunkMessage.Decode` takes `ReadOnlyMemory<byte>` and retains that memory; the source memory
must remain valid until the receiver finishes consuming it or the awaited frame write completes.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Blob`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/docs/Assembly/Assimalign.Cohesion.Database.Blob/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/src/Protocol/BlobChunkMessage.cs`.
