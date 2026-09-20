# ISecretStoreClient

The `ISecretStoreClient` type belongs to `Assimalign.Cohesion.SecretStore.Client`.

> **Status:** Partial.

`ISecretStoreClient` is the asynchronous contract for reading secret material and submitting
commands to a SecretStore endpoint.

## API

- **`GetSecretAsync(string path, CancellationToken)`** — returns the stored bytes from
  `GET /cohesion/v1/secrets?path=...`.
- **`GetCertificateAsync(string name, CancellationToken)`** — returns PEM text from
  `GET /cohesion/v1/certificates?name=...`.
- **`SendCommandAsync(ResourceCommand command, CancellationToken)`** — submits the command with
  `POST /cohesion/v1/commands`.

All methods propagate cancellation. Blank paths or names raise `ArgumentException`; transport and
non-success status failures raise `HttpRequestException`. An empty certificate raises
`InvalidDataException`, and command serialization may raise `JsonException`.

## Usage

See the [source-backed usage examples](examples/index.md).

`Create` instances through `SecretStoreClient`; the HTTP implementation is internal.

## Links

- **[Assembly** — overview](index.md)
- **[Project** — overview](index.md)
- **[Project** — design](design.md)

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.Client/docs/Assembly/Assimalign.Cohesion.SecretStore.Client/ISecretStoreClient/OVERVIEW.md`.
