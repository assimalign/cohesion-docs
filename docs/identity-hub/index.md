# IdentityHub

IdentityHub hosts a minimal OpenID Connect issuer with persisted signing keys and code-first audience and client declarations.

> **Status:** Partial. The issuer is implemented; full account, consent, recovery, and federation flows are outside this runtime.

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
| [`Assimalign.Cohesion.IdentityHub.Hosting`](../dotnet-apis/resources/identity-hub/assimalign-cohesion-identityhub-hosting/index.md) | OpenID Connect issuer, persistent signing keys, and host lifecycle. |
| [`Assimalign.Cohesion.IdentityHub.Hosting.ApplicationModel`](../dotnet-apis/resources/identity-hub/assimalign-cohesion-identityhub-hosting-applicationmodel/index.md) | `builder.Services.AddResource(name, ...)`: the issuer as a managed resource instance. |
| [`Assimalign.Cohesion.IdentityHub.Models`](../dotnet-apis/resources/identity-hub/assimalign-cohesion-identityhub-models/index.md) | Legacy directory persistence models using canonical IdentityModel contracts. |

## Hosting model

`IdentityHubApplication.CreateBuilder(args)` returns the concrete builder. Every built host
registers one internal HTTP issuer after user services, so it starts last and stops first. The
issuer publishes OpenID Connect discovery, a JSON Web Key Set (JWKS), and token endpoints, plus
public health probes. Signing uses persisted ES256 keys.

The endpoint is resolved from `--endpoint`, then `https://127.0.0.1:8443`; state from `--data`, then
the content root. Plain HTTP is permitted only on loopback in `Local`. An ephemeral self-signed
certificate is allowed only on loopback in `Local`; every other HTTPS binding fails closed with an
explicit configuration error. TLS material is always distinct from the issuer signing key.

## Managed resource

The issuer becomes a managed resource instance with one registration in its `Program.cs`:
`builder.Services.AddResource("identity", resource => resource.UseInProcess())`. When the
environment names a platform the resource opted into, it publishes its manifest, restores the state
its gateway kept, and runs the gateway's actions. IdentityHub exposes no actions of its own yet; the
application can add its own with `AddAction`. A gateway declares the resource generically:
`builder.AddResource("IdentityHub", "identity")`. See
[ApplicationModel](../dotnet-apis/libraries/application-model/index.md).

## SDK and framework

`Assimalign.Cohesion.Sdk.IdentityHub` delivers the runtime family and the resource integration
through `Assimalign.Cohesion.App.IdentityHub`. See the
[IdentityHub SDK reference](../dotnet-apis/sdks/sdk-identity-hub/index.md).

## Issuer and client registry

The issuer accepts only `openid` or an empty scope. Client authentication accepts exactly one of
HTTP Basic or form credentials, constrains the requested `audience` or `resource` to the
registration, and issues ES256 bearer JWTs. The browser device-approval page uses a fixed subject
and is available only on a loopback Local endpoint. It is not a general account-login,
subject-selection, or consent experience.

| Declaration | Identity and ordering |
|---|---|
| `AddAudience` | Declares an audience name. |
| `AddClient` | Declares a client identifier; referenced audiences must be declared. |

Each `AddAudience` and `AddClient` declaration is a registration. The host resolves and validates
them once, before any lifecycle service is created, so an undeclared client audience fails `Build`.

## Getting started

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.IdentityHub.Hosting;
using Assimalign.Cohesion.IdentityHub.Hosting.ApplicationModel;

IdentityHubApplicationBuilder builder = IdentityHubApplication.CreateBuilder(args);
builder.Services.AddResource("identity", resource => resource.UseInProcess());

await using IdentityHubApplication application = builder.Build();
await application.RunAsync();
```

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area** — `cohesion/resources/IdentityHub/README.md`.
- **Hosting** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Hosting/docs/OVERVIEW.md` and `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Hosting/docs/DESIGN.md`.
- **Resource integration** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Hosting.ApplicationModel/docs/OVERVIEW.md`.
- **Package boundaries** — `cohesion/.claude/rules/resource-areas.md`.
