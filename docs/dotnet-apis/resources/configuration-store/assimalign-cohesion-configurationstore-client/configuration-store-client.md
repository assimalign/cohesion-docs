# ConfigurationStoreClient

The `ConfigurationStoreClient` type belongs to `Assimalign.Cohesion.ConfigurationStore.Client`.

> **Status:** Partial.

`ConfigurationStoreClient` is the factory for the package's internal HTTP protocol client.

## Factory

See the [source-backed usage examples](examples/index.md).

`Create` accepts an HTTP or HTTPS endpoint `Uri` and a non-null `ClientCredential`. It performs no
network I/O. The URI must be absolute, contain a host and valid port, and have no user information,
query, or fragment. An invalid endpoint shape or non-HTTP scheme raises `ArgumentException`; a null
endpoint or credential raises `ArgumentNullException`.

The returned client uses a process-shared BCL `HttpMessageInvoker`. Redirects and cookies are
disabled so its Bearer credential is not forwarded to another authority or mixed with ambient cookie
state. Callers do not own or dispose the shared transport.

The three-argument `Create(Uri, ClientCredential, HttpMessageInvoker)` uses a caller-owned transport,
including its TLS trust policy. The caller keeps a supplied invoker alive until requests finish and
disposes it afterward; the client does not own it. A null transport throws `ArgumentNullException`.

## Links

- **[Assembly** — overview](index.md)
- **Detail** — [IConfigurationStoreClient](i-configuration-store-client.md)
- **[Project** — design](design.md)

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.Client/src/ConfigurationStoreClient.cs`.
