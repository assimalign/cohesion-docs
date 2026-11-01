# Assimalign.Cohesion.SecretStore design

This page describes the boundaries and implementation shape of `Assimalign.Cohesion.SecretStore`.

> **Status:** Partial.

## Design intent

The area root owns the contracts that executable composition and feature packages target.
`ISecretStoreApplicationBuilder` is the contract-only builder seam, while `ISecretStoreApplication`
supplies the hosting-free application lifecycle. The root also owns the two foundational SecretStore
declarations needed by a `Program.cs`: `AddSecret` and `AddCertificateAuthority`. They are builder
members rather than Hosting-owned verbs, so any implementation of the root seam can consume the same
declaration data.

## Hosting isolation

The root contracts are hosting-free (O34): `ISecretStoreApplication` exposes `Context`,
`StartAsync`, and `StopAsync`; `ISecretStoreApplicationContext` exposes `ContentRootPath`.
`ISecretStoreApplicationBuilder` owns area declarations and `Build()`. The root and feature
packages reference no `Assimalign.Cohesion.Hosting*` library; `COHRES004` enforces the boundary.

The concrete application, builder, and context live in `Assimalign.Cohesion.SecretStore.Hosting`;
options and supporting services remain internal.

`CertificateAuthorityOptions` is composition data: it introduces no network, certificate,
configuration, or service-container dependency into the root.

## Code-first secret seeds

`AddSecret(path, value)` declares first-start seed data. An implementation snapshots the bytes at
registration and writes them only when the durable store has no version at that exact, ordinal path.
This distinction is deliberate: composition must not roll back a value already on disk. Duplicate
declarations are rejected instead of making call order an implicit overwrite policy.

## Certificate-authority seeding

`AddCertificateAuthority` captures one `CertificateAuthorityOptions` declaration. Existing durable
authority state always wins. On first start, the decision order is:

1. Use the paired `InitialCertificate` and `InitialPrivateKey` PEM material when supplied. This is
   the operator-seeded route.
2. Otherwise, when `SelfSeed` is true (the default), generate a self-signed development root.
3. Otherwise, fail configuration because no authority source exists.

The default common name is stable and can be overridden for an application-specific root. The
store enrolls with no parent authority and issues no intermediate authorities.

Compared with `10.0.0-preview.1`, `CertificateAuthorityOptions` is a source and binary break:
`PlatformEnrollmentEndpoint` and `PlatformCertificate` are removed with the gateway-mediated
Platform enrollment they configured, and `SelfSeedWhenNoPlatform` is renamed `SelfSeed`, because
with no Platform source its condition no longer qualifies anything. The default (`true`) and the
self-seeding behavior are unchanged.

## Composition lifecycle

Background-work registration belongs to the concrete `SecretStoreApplicationBuilder`:
`AddService(IHostService)` and `AddService(Func<SecretStoreApplicationContext, IHostService>)`. The
factory receives the same concrete context as Web's and `Database`'s `AddService`, so hosting consumers
can use environment, state, and hosted-service members beyond the small root contract. Factories run
once per build against the same context retained by the application; the hosted-service snapshot is
installed after factory evaluation. Services start in registration order and stop in reverse. No
area-owned service abstraction is introduced.

A managed resource is a registration the application makes, `builder.Services.AddResource(name, ...)`
from the area's `Hosting.ApplicationModel` package; the root contracts do not change for it. Plain
application defaults are described by the Hosting design.

## AOT posture

The contracts and option values require no reflection, dynamic code generation, runtime assembly
scanning, configuration binding, or container-based activation. Secret and certificate material is
carried as `ReadOnlyMemory<byte>`, leaving parsing and cryptographic ownership to the Hosting
implementation while the public root remains trimming- and NativeAOT-safe.

## Non-goals

This project does not persist or encrypt secrets, generate certificates, or expose a protocol
endpoint. Those are runtime responsibilities. Seed declarations are builder-time only; changing a
running store is the resource integration's, through the actions its application adds.

## Hosting-free application contract (O34)

`SecretStoreApplication.CreateBuilder(args)` returns the public concrete
`SecretStoreApplicationBuilder`; its `Build()` returns the public
`SecretStoreApplication : Host<SecretStoreApplicationContext>`. The public
`SecretStoreApplicationContext` implements `ISecretStoreApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

Background-work registration (`AddService`) is available only on the concrete Hosting builder.

## Declared dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Core` | `CohesionProjectReference` |

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore/docs/DESIGN.md`.
- **Source** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore/src/Assimalign.Cohesion.SecretStore.csproj`.
