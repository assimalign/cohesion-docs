# SecretStore

SecretStore persists protected secrets, application trust, and private certificate authority state.

> **Status:** Partial. Protected storage and private certificates work; automatic Platform enrollment and public certificate issuance remain deferred.

## What it is

SecretStore is the operational resource for secret bytes, application trust, and private
certificate workflows. The host protects durable files on its `data` volume and issues named
private certificate authority (CA) leaves on first resolution of `certs/<name>`.

Gateways use the client to resolve mounts before resource startup. Resource code consumes the
materialized values through the shared runtime contract.

See the [SecretStore API reference](../dotnet-apis/resources/secret-store/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.SecretStore`](../dotnet-apis/resources/secret-store/assimalign-cohesion-secretstore/index.md) | Application contracts and secret/certificate-authority declarations. |
| [`Assimalign.Cohesion.SecretStore.ApplicationModel`](../dotnet-apis/resources/secret-store/assimalign-cohesion-secretstore-applicationmodel/index.md) | Typed resource, singleton stateful planner, and secret/certificate commands. |
| [`Assimalign.Cohesion.SecretStore.Client`](../dotnet-apis/resources/secret-store/assimalign-cohesion-secretstore-client/index.md) | Secret and PEM certificate reads, trust commands, and observed command delivery. |
| [`Assimalign.Cohesion.SecretStore.Hosting`](../dotnet-apis/resources/secret-store/assimalign-cohesion-secretstore-hosting/index.md) | Protected repository, private CA, bootstrap verification, and protocol listener. |

## Hosting model

`SecretStoreApplication.CreateBuilder(args)` captures the ambient `ResourceContext`. The host
uses its identity, environment, application trust key, `api` endpoint, and `data` mount.
Its HTTP/1 listener starts after caller services and drains before them. The default resource
plane aggregates health, readiness, liveness, observed endpoints, and graceful stop.

Endpoint precedence is the registered control-plane observation, ambient `api`, `--endpoint`,
then `https://127.0.0.1:8443`. Data precedence is the ambient `data` mount, `--data`, then
`<content-root>/data`. HTTP is allowed only on loopback in `Local`; a standalone host without a
bootstrap credential may bind only to loopback, including over HTTPS.

For HTTPS, the host first reads the Secret certificate mount named by the `api` endpoint through
the shared certificate accessor. If no bundle is present, it obtains the transport identity from
its private certificate authority. It does not generate an unrelated certificate at each start.

Managed `/cohesion/v1/*` calls verify trusted ES256 bootstrap credentials. Missing authentication
returns HTTP 401; wrong audience returns 403. Certificate and trust state is protected alongside
secrets. The volume's file-system access controls protect the colocated DataProtection key ring.

## Application model

`AddSecretStore(manifest, options)` returns `ISecretStoreResourceDescriptor` over a
`SecretStoreResource`. The planner requires a `StatefulSet`, HTTP or HTTPS `api` endpoint,
persistent `data` Volume, and one stable replica. `SecretStoreResourceOptions.Storage.Size`
overrides the sized claim; a replica count above one is rejected until replication exists.

`AddSecret` declares a source reference. `IssueCertificate` declares a certificate identity.
`SecretStoreResourceControlPlane.Create()` also accepts the gateway-owned `cohesion.trust.add`
operation, which is deliberately absent from the manifest's application command list.

## SDK and framework

`Assimalign.Cohesion.Sdk.SecretStore` delivers the runtime family through
`Assimalign.Cohesion.App.SecretStore`. The area's `.ApplicationModel` package is NuGet-only
and added to enabled resource executables; it remains outside the runtime shared framework.
The `.Client` package is also NuGet-only.

See the [SecretStore SDK reference](../dotnet-apis/sdks/sdk-secret-store/index.md).

## Protected secrets and command sources

Builder declarations seed missing durable values. Command declarations use
`secretstore.add-secret` and `secretstore.issue-certificate`. A `parameter:<name>` source resolves
through the gateway parameter provider; `<resource>:<key>` requires a declared dependency and
an available source endpoint. `literal:<value>` is rejected so secret material never enters the
desired model. Only the transient delivery envelope contains the resolved bytes.

The repository stores the protected value and its source. Unresolved references return a named
rejection. Trust grants keep their separate gateway authorization and POST-only protocol;
secret and certificate declarations support owned deletion.

## Private certificate authority

`AddCertificateAuthority` controls standalone root identity, initial key material, pending
Platform enrollment, root pinning, and self-seeding. Durable authority state wins on restart.
Explicit request/sign/complete enrollment routes exist, but a gateway or operator must drive them;
the configured Platform endpoint is not called automatically.

`IssueCertificate` preserves the requested subject and subject alternative names. A conflicting
identity is rejected until the existing declaration is deleted. Resolution renews named leaves;
the returned PEM bundle includes the leaf, private key, and issuer chain. The transport leaf is
checked at listener startup, so a continuously running host must restart before its expiry.

Public-CA issuance through `certs/public`, automatic `Enroll(platformStore)`, external key
management/wrapping, and protected-record rewrap migration remain unimplemented.

## Getting started

The `cohesion-secretstore` template supplies this minimal `Program.cs`.

```csharp
using Assimalign.Cohesion.SecretStore;
using Assimalign.Cohesion.SecretStore.Hosting;

SecretStoreApplicationBuilder builder = SecretStoreApplication.CreateBuilder(args);

await using SecretStoreApplication application = builder.Build();
await application.RunAsync();
```

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area** — `cohesion/resources/SecretStore/README.md`.
- **Hosting** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.Hosting/docs/OVERVIEW.md` and `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.Hosting/docs/DESIGN.md`.
- **Application model** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.ApplicationModel/docs/OVERVIEW.md` and `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.ApplicationModel/docs/DESIGN.md`.
- **Client** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.Client/docs/OVERVIEW.md`.
- **Runtime contract** — `cohesion/docs/RUNTIME_CONTRACT.md`.
- **Package boundaries** — `cohesion/.claude/rules/resource-areas.md`.
- **Transport certificate precedence** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.Hosting/src/Internal/SecretsEndpointService.cs`.
- **Template** — `cohesion/tooling/templates/Assimalign.Cohesion.Templates/src/content/cohesion-secretstore/Program.cs`.
