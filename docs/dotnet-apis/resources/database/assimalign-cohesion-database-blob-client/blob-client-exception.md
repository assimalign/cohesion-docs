# BlobClientException

The `BlobClientException` type is part of the documented `Assimalign.Cohesion.Database.Blob.Client` API.

> **Status:** Partial.

A Blob wire operation failed; the connection is no longer reusable.

Namespace: `Assimalign.Cohesion.Database.Blob.Client`.

## Documented behavior

`BlobClientException.Code` preserves a server `ProtocolErrorCode`. Every failed exchange
invalidates its connection. Cancellation throws `OperationCanceledException`; overlapping
operations throw `InvalidOperationException`. A download's lifetime cancellation remains active
after return, and later read failures remain sticky. Successful EOF requires verified completion.
See the [streaming contract](index.md) and [design](design.md) .

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Blob.Client`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Client/docs/Assembly/Assimalign.Cohesion.Database.Blob.Client/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Client/src/Exceptions/BlobClientException.cs`.
