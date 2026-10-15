# ResourceCommand

The `ResourceCommand` type belongs to `Assimalign.Cohesion.SecretStore.Client`.

> **Status:** Partial.

`ResourceCommand` is the immutable command envelope accepted by
`ISecretStoreClient.SendCommandAsync`.

## API

See the [source-backed usage examples](examples/index.md).

`Id` is the idempotency identifier carried in the request body. `Kind`, `Owner`, and `Key` carry
the area-defined command identity, while `Payload` carries its bytes. The constructor raises
`ArgumentException` when `id`, `kind`, `owner`, or `key` is null, empty, or whitespace. An empty
payload is permitted.

The command transport serializes the envelope with source-generated, camel-case JSON metadata;
`Payload` appears as a base64 string.

## Links

- **[Assembly** — overview](index.md)
- **Detail** — [ISecretStoreClient](i-secret-store-client.md)
- **[Project** — design](design.md)

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.Client/docs/Assembly/Assimalign.Cohesion.SecretStore.Client/ResourceCommand/OVERVIEW.md`.
