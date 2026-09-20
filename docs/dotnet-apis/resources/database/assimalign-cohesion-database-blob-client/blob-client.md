# BlobClient

The `BlobClient` type is part of the documented `Assimalign.Cohesion.Database.Blob.Client` API.

> **Status:** Partial.

Creates pooled clients for the Blob message family.

Namespace: `Assimalign.Cohesion.Database.Blob.Client`.

## Documented behavior

`BlobClient.Create(BlobClientOptions)` returns an `IBlobClient`. Options require shared
`DatabaseConnectionSettings` and an `IConnectionFactory`; creating a client performs no I/O.
`ConnectAsync` rents an authenticated `IBlobConnection` bound to the selected database.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Blob.Client`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Client/docs/Assembly/Assimalign.Cohesion.Database.Blob.Client/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Client/src/BlobClient.cs`.
