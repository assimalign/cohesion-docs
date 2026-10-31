# BlobTransferCompleteMessage

The `BlobTransferCompleteMessage` type is part of the documented `Assimalign.Cohesion.Database.Blob.Protocol` API.

> **Status:** Partial.

Ends a transfer with the actual byte count, or acknowledges a completed upload operation.

Namespace: `Assimalign.Cohesion.Database.Blob.Protocol`.

Assembly: `Assimalign.Cohesion.Database.Blob.Protocol.dll`. NuGet package: `Assimalign.Cohesion.Database.Blob.Protocol`.

## Documented behavior

Actual content count at transfer completion. For automatic uploads, the final server message
follows commit; for borrowed host transactions it confirms operation completion, with publication
waiting for host commit. See [host transactions](../../../../database/blob/operations.md#host-transactions).

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Blob.Protocol`](index.md).
- **Design** — [Lifetime and implementation decisions](../assimalign-cohesion-database-blob/design.md).
- **Examples** — [Source-backed usage](../assimalign-cohesion-database-blob/examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/docs/Assembly/Assimalign.Cohesion.Database.Blob/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Protocol/src/BlobTransferCompleteMessage.cs`.
