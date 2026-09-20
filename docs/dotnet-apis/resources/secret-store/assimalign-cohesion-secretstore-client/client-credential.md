# ClientCredential

The `ClientCredential` type belongs to `Assimalign.Cohesion.SecretStore.Client`.

> **Status:** Partial.

`ClientCredential` carries the opaque Bearer token attached to every SecretStore request.

## API

See the [source-backed usage examples](examples/index.md).

The constructor rejects a null, empty, or whitespace token with `ArgumentException`. The client
does not parse, validate, refresh, or persist the token. The token is not exposed by the public
surface, and `ToString()` always redacts it.

Credentials are bound when `SecretStoreClient.Create` constructs a client. Item 25 can create a new
lightweight client for each reconciled credential while the underlying transport remains shared.

## Links

- **[Assembly** — overview](index.md)
- **Detail** — [SecretStoreClient](secret-store-client.md)
- **[Project** — design](design.md)

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.Client/docs/Assembly/Assimalign.Cohesion.SecretStore.Client/ClientCredential/OVERVIEW.md`.
