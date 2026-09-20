# Assimalign.Cohesion.SecretStore.Client

This package is the gateway-side protocol boundary for reading secret bytes and PEM certificates from a realized SecretStore resource.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`ClientCredential`](client-credential.md)** — Documented public type.
- **[`ISecretStoreClient`](i-secret-store-client.md)** — Documented public type.
- **[`ResourceCommand`](resource-command.md)** — Documented public type.
- **[`ResourceCommandObservation`](resource-command-observation.md)** — Documented public type.
- **[`SecretStoreClient`](secret-store-client.md)** — Documented public type.

## Summary

This package is the gateway-side protocol boundary for reading secret bytes and PEM certificates
from a realized SecretStore resource. It keeps mount-source resolution out of runtime libraries and
hides its BCL HTTP implementation behind `ISecretStoreClient`.

## Public surface

- **`ISecretStoreClient`** — reads secret bytes, reads PEM certificates, and sends a generic command.
- **`SecretStoreClient.Create(Uri, ClientCredential)`** — creates the internal protocol
  implementation without performing network I/O.
- **`ClientCredential`** — carries the opaque bootstrap token and redacts it when formatted.
- **`ResourceCommand`** — is the generic command envelope; item 31c adds typed SecretStore commands.

## Usage

See the [source-backed usage examples](examples/index.md).

The certificate response is a PEM bundle containing the persistent first-issued leaf, its PKCS#8
private key, and the issuer chain. A `parameter:` source bypasses this client and is mounted by the
gateway unchanged. `certs/public` /`Certificate="public"` is reserved for a later public-CA item.

The factory accepts only HTTP or HTTPS Cohesion endpoint URIs: absolute, host-bearing values with a
valid port and no user information, query, or fragment. Each request presents the credential as a
bearer token; the package treats the token as opaque and does not acquire, parse, refresh, or
persist it.

## Observed command delivery

`CreateForControlPlane` accepts the full control-plane prefix. `ObserveCommandAsync` and
`DeleteCommandAsync` return `ResourceCommandObservation`; `SendCommandAsync` retains its original
Task-returning behavior. The new secret/certificate commands return 200 application/octet-stream on
success and JSON {status,detail} on refusal. Legacy cohesion.trust.add keeps empty 204/409/403
responses. The observation client treats an empty 2xx body as Applied, or Deleted for DELETE, and
supplies a named HTTP detail when a legacy refusal has no body. DELETE is for the new kinds; trust
grants remain POST-only. The package still has exactly one Core reference and no Hosting or Gateway
dependencies. Identity verification and grant policy belong to the endpoint.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Core` | `CohesionProjectReference` |

[Parent: SecretStore](../index.md)

## Sources

- **Primary source** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.Client/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.Client/src/Assimalign.Cohesion.SecretStore.Client.csproj`.
