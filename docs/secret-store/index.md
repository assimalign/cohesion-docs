# SecretStore

SecretStore persists protected secrets and private certificate authority state.

> **Status:** Partial. Protected storage and private certificates work for a loopback standalone host; authenticated remote access, automatic Platform enrollment, and public certificate issuance remain deferred.

## What it is

SecretStore is the operational resource for secret bytes and private certificate workflows. The
host protects durable files on its `data` volume and issues named private certificate authority
(CA) leaves on first resolution of `certs/<name>`. Programs read it through
`Assimalign.Cohesion.SecretStore.Client`.

See the [SecretStore API reference](../dotnet-apis/resources/secret-store/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.SecretStore`](../dotnet-apis/resources/secret-store/assimalign-cohesion-secretstore/index.md) | Application contracts and secret/certificate-authority declarations. |
| [`Assimalign.Cohesion.SecretStore.Client`](../dotnet-apis/resources/secret-store/assimalign-cohesion-secretstore-client/index.md) | Secret byte and PEM certificate reads. |
| [`Assimalign.Cohesion.SecretStore.Hosting`](../dotnet-apis/resources/secret-store/assimalign-cohesion-secretstore-hosting/index.md) | Protected repository, private CA, and HTTP/1 protocol listener. |
| [`Assimalign.Cohesion.SecretStore.Hosting.ApplicationModel`](../dotnet-apis/resources/secret-store/assimalign-cohesion-secretstore-hosting-applicationmodel/index.md) | `builder.Services.AddResource(name, ...)`: the store as a managed resource instance. |

## Hosting model

`SecretStoreApplication.CreateBuilder(args)` returns the concrete builder. The host appends an
HTTP/1 endpoint service after caller services, so it starts last and drains first. Endpoint
precedence is `--endpoint`, then `https://127.0.0.1:8443`; data precedence is `--data`, then
`<content-root>/data`. Plaintext HTTP is allowed only on loopback in `Local`.

The host has no bootstrap authentication, so it may bind only to loopback, including over HTTPS.
Its `/cohesion/v1` routes accept only transport peers at `127.0.0.1`, `::1`, or IPv4-mapped
`127.0.0.1`; other peers receive `403`, and forwarded headers do not affect the check. Bare
`/healthz`, `/readyz`, and `/livez` remain open for probes. For HTTPS the host obtains its transport
identity from its private certificate authority. The volume's file-system access controls protect
the colocated DataProtection key ring.

## Managed resource

A store becomes a managed resource instance with one registration in its `Program.cs`:
`builder.Services.AddResource("secrets", resource => resource.UseInProcess())`. When the
environment names a platform the resource opted into, it publishes its manifest, restores the state
its gateway kept, and runs the gateway's actions. SecretStore exposes no actions of its own yet; the
application can add its own with `AddAction`. A gateway declares the resource generically:
`builder.AddResource("SecretStore", "secrets")`. Run one instance until replication exists. See
[ApplicationModel](../dotnet-apis/libraries/application-model/index.md).

## SDK and framework

`Assimalign.Cohesion.Sdk.SecretStore` delivers the runtime family and the resource integration
through `Assimalign.Cohesion.App.SecretStore`. The `.Client` package is NuGet-only. See the
[SecretStore SDK reference](../dotnet-apis/sdks/sdk-secret-store/index.md).

## Protected secrets

Builder declarations seed missing durable values. Secrets, CA keys, pending enrollment keys, and
leaf bundles are protected at rest. `ISecretStoreClient.GetSecretAsync` reads secret bytes and
`GetCertificateAsync` reads PEM bundles.

## Private certificate authority

`AddCertificateAuthority` controls standalone root identity, initial key material, pending
Platform enrollment, root pinning, and self-seeding. Durable authority state wins on restart.
Explicit request/sign/complete enrollment routes exist, but an operator must drive them. A
configured Platform enrollment endpoint needs the ambient application and resource names, which a
plain host does not have, so the host fails to start with it.

Resolution renews named leaves; the returned PEM bundle includes the leaf, private key, and issuer
chain. The transport leaf is checked at listener startup, so a continuously running host must
restart before its expiry. Public-CA issuance through `certs/public`, automatic Platform
enrollment, external key management/wrapping, and protected-record rewrap migration remain
unimplemented.

## Getting started

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.SecretStore.Hosting;
using Assimalign.Cohesion.SecretStore.Hosting.ApplicationModel;

SecretStoreApplicationBuilder builder = SecretStoreApplication.CreateBuilder(args);
builder.Services.AddResource("secrets", resource => resource.UseInProcess());

await using SecretStoreApplication application = builder.Build();
await application.RunAsync();
```

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area** — `cohesion/resources/SecretStore/README.md`.
- **Hosting** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.Hosting/docs/OVERVIEW.md` and `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.Hosting/docs/DESIGN.md`.
- **Resource integration** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.Hosting.ApplicationModel/docs/OVERVIEW.md`.
- **Client** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.Client/docs/OVERVIEW.md`.
- **Package boundaries** — `cohesion/.claude/rules/resource-areas.md`.
