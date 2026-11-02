# Assimalign.Cohesion.SecretStore.Hosting

The SecretStore runtime module: `SecretStoreApplication.CreateBuilder(args)`, the concrete host, and its services.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`SecretStoreApplication`](secret-store-application.md)** — Documented public type.

## Summary

Provides the SecretStore runtime: `SecretStoreApplication.CreateBuilder(args)`, protected durable
secret/certificate storage, an ECDSA certificate authority, and the HTTP data protocol served on the
resource's `api` endpoint.

## Current behavior

- The area's resource integration is `Assimalign.Cohesion.SecretStore.Hosting.ApplicationModel`,
  which references this module; this module references no application-model library.
- Always appends an HTTP/1 endpoint service after caller-registered host services, so the endpoint
  starts last and drains first.
- Interoperates with `SecretStore.Client` for secret reads and durable leaf-certificate reads.
- Protects secrets, CA keys, and leaf bundles at rest.
- Supports a durable CA, seeded from supplied PEM material or self-signed on first start.
- Renews named leaves on resolution and checks the HTTPS transport leaf at listener startup; a
  continuously running host must restart before transport expiry until the shared TLS listener
  supports asynchronous certificate selection per handshake.

## Configuration

Endpoint precedence is `--endpoint`, then `https://127.0.0.1:8443`. Data-path precedence is
`--data`, then `<content-root>/data`. Plaintext HTTP is permitted only on loopback in `Local`. The
host takes no credentials and therefore may bind only to loopback, including when it uses HTTPS. Its
`/cohesion/v1` and `/cohesion/v1/*` routes accept only transport peers at `127.0.0.1`, `::1`, or
IPv4-mapped `127.0.0.1`; other or unknown peers receive `403`. Forwarded headers do not affect this
check. Bare `/healthz`, `/readyz`, and `/livez` remain open for probes. This tightens standalone
request authorization from 10.0.0-preview.1.

Builder-declared secrets seed missing durable entries. `AddCertificateAuthority` controls the
root common name, initial certificate/key material, and self-seeding. Durable authority state takes precedence on restart.

The persistent volume's file-system ACL protects the colocated DataProtection key ring; an external
KMS/wrapping-key integration and protected-record rewrap migration remain future work.

## Known gap

`certs/public`/ACME issuance is not implemented by this host. See [DESIGN.md](design.md) for the exact routes.

## Public types

- `SecretStoreApplication` — concrete host and creation entry point.
- `SecretStoreApplicationBuilder` — composition and background-work registration.
- `SecretStoreApplicationContext` — host environment and runtime state.

## Concrete composition (T10 / O34)

`SecretStoreApplication.CreateBuilder(args)` returns the public concrete
`SecretStoreApplicationBuilder`; its `Build()` returns the public `SecretStoreApplication :
Host<SecretStoreApplicationContext>`. The public `SecretStoreApplicationContext` implements
`ISecretStoreApplicationContext`, reading `ContentRootPath` from the host environment. The
application explicitly forwards the root lifecycle contract to `IHost`, and consumers use the
concrete application for `RunAsync` and `await using`. Runtime options and supporting services
remain internal.

Background-work registration belongs to the concrete `SecretStoreApplicationBuilder`:
`AddService(IHostService)` and `AddService(Func<SecretStoreApplicationContext, IHostService>)`. The
factory receives the same concrete context as Web's and Database's AddService, so hosting consumers
can use environment, state, and hosted-service members beyond the small root contract. Database's
root-level AddServer keeps the interface context. Factories run once per build against the same
context retained by the application; the hosted-service snapshot is installed after factory
evaluation. Services start in registration order and stop in reverse. No area-owned service
abstraction is introduced.

The base host owns the already-cancelled run semantic: one complete start and graceful stop with
fresh lifecycle tokens, normal run-observer notifications, and a final Stopped state. The concrete
application and IHost route share it. Startup failures still roll back and propagate.

## Dependency injection composition

`SecretStoreApplicationBuilder.Services` holds every registration the application runs with.
`AddService` registers an `IHostService`, and a direct `IHostService` registration joins the
lifecycle in the same order. `Build` closes registration and resolves the services once. Disposing
the application disposes every factory-created service. See
[DESIGN.md](design.md#dependency-injection-composition).

`AddSecret` accepts the root's generated `SecretStoreSeedPath` (ordinary strings convert
implicitly). It refuses blank paths and `default` during
registration. Seeds may still use `certs/`. Certificate reads share the client package's validation
predicate through internal shared source, so malformed values cannot bypass validation by supplying
raw query strings.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.SecretStore` | `CohesionProjectReference` |
| `Assimalign.Cohesion.DependencyInjection` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Connections.Security` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Connections.Tcp` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Connections` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Http` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Http.Connections` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.IdentityModel` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.IdentityModel.Token` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Security.DataProtection` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Web` | `CohesionPrivateProjectReference` |
| `Assimalign.Cohesion.Web.Hosting` | `CohesionPrivateProjectReference` |

The area's resource integration, [`Assimalign.Cohesion.SecretStore.Hosting.ApplicationModel`](../assimalign-cohesion-secretstore-hosting-applicationmodel/index.md), references this module and registers the resource service in its `Services`.

[Parent: SecretStore](../index.md)

## Sources

- **Primary source** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.Hosting/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.Hosting/src/Assimalign.Cohesion.SecretStore.Hosting.csproj`.
