# IdentityHub

IdentityHub hosts a minimal OpenID Connect issuer with persisted signing keys and resource commands.

> **Status:** Partial. The issuer and command registry are implemented; full account, consent, recovery, and federation flows are outside this runtime.

## What it is

IdentityHub is the identity service area. The current host supplies discovery, signing-key
publication, client-credentials tokens, and a Local device-authorization flow. Its larger intended
scope includes directories, principals, sessions, federation, and provisioning.

Canonical identity contracts belong to the
[IdentityModel library](../dotnet-apis/libraries/identity-model/index.md).
The `IdentityHub.Models` assembly retains legacy directory data transfer objects and identifier
types while referring to those canonical subject and credential contracts.

See the [IdentityHub API reference](../dotnet-apis/resources/identity-hub/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.IdentityHub`](../dotnet-apis/resources/identity-hub/assimalign-cohesion-identityhub/index.md) | Hosting-free application contracts and code-first audience/client declarations. |
| [`Assimalign.Cohesion.IdentityHub.ApplicationModel`](../dotnet-apis/resources/identity-hub/assimalign-cohesion-identityhub-applicationmodel/index.md) | Manifest-backed resource, singleton stateful planner, and typed identity commands. |
| [`Assimalign.Cohesion.IdentityHub.Client`](../dotnet-apis/resources/identity-hub/assimalign-cohesion-identityhub-client/index.md) | Thin authenticated client for applying and deleting resource commands. |
| [`Assimalign.Cohesion.IdentityHub.Hosting`](../dotnet-apis/resources/identity-hub/assimalign-cohesion-identityhub-hosting/index.md) | OpenID Connect issuer, persistent signing keys, and resource control plane. |
| [`Assimalign.Cohesion.IdentityHub.Models`](../dotnet-apis/resources/identity-hub/assimalign-cohesion-identityhub-models/index.md) | Legacy directory persistence models using canonical IdentityModel contracts. |

## Hosting model

`IdentityHubApplication.CreateBuilder(args)` returns the concrete builder. The host publishes
OpenID Connect discovery, a JSON Web Key Set (JWKS), and token endpoints, plus public health
probes and the bootstrap-authenticated resource control plane. Signing uses persisted ES256 keys.

The generated manifest supplies the `https` endpoint and persistent `data` volume. Production
Transport Layer Security (TLS) reads a PEM leaf, private key, and optional chain from the
materialized `tls` Secret mount. A self-signed fallback exists only for loopback `Local`.
Non-Local HTTPS fails without its certificate. Other non-persistent Configuration and Secret
mounts pass through the planner and are delivered through `ResourceContext`.

## Application model

`AddIdentityHub(manifest, options)` creates an `IdentityHubResource` and returns
`IIdentityHubResourceDescriptor`. The planner requires a singleton `StatefulSet`, an `https`
endpoint over Transmission Control Protocol (TCP), and exactly one sized persistent `data`
Volume. Extra endpoints and non-persistent mounts pass through.

Runtime `AddAudience` and `AddClient` declarations live in the root package; descriptor verbs
with the same names declare `identityhub.add-audience` and `identityhub.add-client` commands.
The descriptor retains its typed command surface through `DependsOn` chaining.

## SDK and framework

`Assimalign.Cohesion.Sdk.IdentityHub` delivers the runtime family through
`Assimalign.Cohesion.App.IdentityHub`. The area's `.ApplicationModel` package is NuGet-only
and added to enabled resource executables; it remains outside the runtime shared framework.
The `.Client` package is also NuGet-only.

See the [IdentityHub SDK reference](../dotnet-apis/sdks/sdk-identity-hub/index.md).

## Issuer and client registry

The issuer accepts only `openid` or an empty scope. Its browser device-approval page uses a fixed
subject and is available only on a loopback Local endpoint. It is not a general account-login,
subject-selection, or consent experience.

Command declarations are stored atomically in `registry.json` beside the signing key under the
data path. Startup restores registry ownership before serving; token issuance uses the combined
builder-declared and command-declared registry. Discovery and JWKS remain the same issuer surface.

| Command | Identity and ordering |
|---|---|
| `AddAudience` | Owns the audience name; removal is refused while a client references it. |
| `AddClient` | Owns the client identifier; referenced audiences must already exist. |

A client's `credentialSource` names a Secret mount, optionally with a `mount:` prefix. The model
stores the reference, not credential bytes. Hosting reads and hashes the mounted value during
registry initialization or update, so the mount is required again after restart. Conflicting
resource seeds and changed client declarations are refused until the owned declaration is deleted.

`IdentityHubCommandClient.Create` accepts a full control-plane URI and bearer credential.
`SendCommandAsync` and `DeleteCommandAsync` return a `ResourceCommandObservation` with status and
detail. Token acquisition and secret resolution belong to the gateway.

## Getting started

The `cohesion-identityhub` template supplies this minimal `Program.cs`.

```csharp
using Assimalign.Cohesion.IdentityHub;
using Assimalign.Cohesion.IdentityHub.Hosting;

IdentityHubApplicationBuilder builder = IdentityHubApplication.CreateBuilder(args);

await using IdentityHubApplication application = builder.Build();
await application.RunAsync();
```

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area** — `cohesion/resources/IdentityHub/README.md`.
- **Hosting** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Hosting/docs/OVERVIEW.md` and `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Hosting/docs/DESIGN.md`.
- **Application model** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.ApplicationModel/docs/OVERVIEW.md` and `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.ApplicationModel/docs/DESIGN.md`.
- **Client** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Client/docs/OVERVIEW.md`.
- **Runtime contract** — `cohesion/docs/RUNTIME_CONTRACT.md`.
- **Package boundaries** — `cohesion/.claude/rules/resource-areas.md`.
- **Template** — `cohesion/tooling/templates/Assimalign.Cohesion.Templates/src/content/cohesion-identityhub/Program.cs`.
- **Supporting source** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Models/docs/OVERVIEW.md`.
