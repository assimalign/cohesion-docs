# IConfigurationStoreClient

The `IConfigurationStoreClient` type belongs to `Assimalign.Cohesion.ConfigurationStore.Client`.

> **Status:** Partial.

`IConfigurationStoreClient` is the asynchronous contract for listing and reading configuration
namespaces from a ConfigurationStore endpoint.

## API

- **`ListNamespacesAsync(CancellationToken)`** — returns a direct JSON array's namespace-name strings from
  `GET /cohesion/v1/namespaces` with no query string.
- **`GetNamespaceAsync(string name, CancellationToken)`** — returns a direct JSON object's string or null
  values from `GET /cohesion/v1/namespaces?name=...`.

All methods propagate cancellation. A blank namespace name raises `ArgumentException`; transport
failures raise `HttpRequestException`, and non-success statuses throw. Invalid, empty, or JSON
`null` namespace-list and namespace-value documents raise `JsonException`.

## Usage

See the [source-backed usage examples](examples/index.md).

`Create` instances through `ConfigurationStoreClient`; the HTTP implementation is internal.

## Links

- **[Assembly** — overview](index.md)
- **[Project** — overview](index.md)
- **[Project** — design](design.md)

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.Client/src/Abstractions/IConfigurationStoreClient.cs`.
