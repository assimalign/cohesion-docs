# ConfigurationStore

ConfigurationStore persists named configuration namespaces and serves snapshots and declarative mutations.

> **Status:** Partial. Namespace storage and commands are implemented; replication and the broader versioning program remain deferred.

## What it is

ConfigurationStore holds ordinary configuration as nullable string values in named namespaces.
Its durable files are plain JSON under the `data` volume. Use [SecretStore](../secret-store/index.md)
for protected secret material.

See the [ConfigurationStore API reference](../dotnet-apis/resources/configuration-store/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.ConfigurationStore`](../dotnet-apis/resources/configuration-store/assimalign-cohesion-configurationstore/index.md) | Application contracts, namespace declarations, and loader abstraction. |
| [`Assimalign.Cohesion.ConfigurationStore.ApplicationModel`](../dotnet-apis/resources/configuration-store/assimalign-cohesion-configurationstore-applicationmodel/index.md) | Typed resource, singleton stateful planner, and namespace/value command declarations. |
| [`Assimalign.Cohesion.ConfigurationStore.Client`](../dotnet-apis/resources/configuration-store/assimalign-cohesion-configurationstore-client/index.md) | Namespace listing, snapshot reads, and observed command delivery. |
| [`Assimalign.Cohesion.ConfigurationStore.Hosting`](../dotnet-apis/resources/configuration-store/assimalign-cohesion-configurationstore-hosting/index.md) | Durable JSON store, bootstrap verification, and HTTP protocol host. |

## Hosting model

`ConfigurationStoreApplication.CreateBuilder(args)` returns the concrete builder. Its host appends
an HTTP protocol service after explicit user services, so the listener starts last and drains first.
Enabled resources bind the ambient `api` endpoint and use the `data` mount; a plain application
uses `--endpoint` or a loopback default. The default plane serves health, endpoint discovery,
and stop routes alongside the store protocol.

Gateway bootstrap credentials use ES256 verification against a durable trusted-issuer set.
Configuration and Secret mounts are gateway-materialized `ResourceContext` inputs; namespace
storage itself is ordinary JSON, not a protected secret mount.

The SDK's `api` endpoint defaults to HTTPS and a `tls` Secret mount. The enabled host consumes
the shared certificate bundle through its endpoint certificate accessor; explicit HTTP endpoints
are also supported.

## Application model

`AddConfigurationStore(manifest, options)` returns `IConfigurationStoreResourceDescriptor` over a
`ConfigurationStoreResource`. The planner requires exactly one `api` endpoint, one persistent
`data` Volume, and one stable `StatefulSet` replica. `ConfigurationStoreResourceOptions.Storage.Size`
overrides capacity; replication beyond one is rejected. The plan contains a sized claim, an API
service, and a headless governing service.

The descriptor exposes `AddNamespace`, `SetValue`, and `RemoveValue`. The default control plane
advertises these wire kinds and the hosting module registers their runtime handlers.

## SDK and framework

`Assimalign.Cohesion.Sdk.ConfigurationStore` delivers the runtime family through
`Assimalign.Cohesion.App.ConfigurationStore`. The area's `.ApplicationModel` package is NuGet-only
and added to enabled resource executables; it remains outside the runtime shared framework.
The `.Client` package is also NuGet-only.

See the [ConfigurationStore SDK reference](../dotnet-apis/sdks/sdk-configuration-store/index.md).

## Namespaces, snapshots, and mutations

Builder-time `AddNamespace` and `IConfigurationNamespaceBuilder.Set` seed missing durable data.
Writes replace JSON atomically. `IConfigurationStoreClient.ListNamespacesAsync` reads namespace
names; `GetNamespaceAsync` returns an `IReadOnlyDictionary<string, string?>` snapshot.
A snapshot read does not imply the planned immutable revision, promotion, or watch facilities.

| Descriptor verb | Wire kind | Ownership key |
|---|---|---|
| `AddNamespace` | `configurationstore.add-namespace` | Namespace name |
| `SetValue` | `configurationstore.set-value` | Namespace and value key |
| `RemoveValue` | `configurationstore.remove-value` | Namespace and value key |

Namespace declarations persist their owner and original seed. Reapplying the same declaration
succeeds even when later value commands change its contents. A foreign owner or changed seed is
rejected; resource-seeded namespaces are not silently adopted. Remove owned value declarations
before deleting their namespace. Value keys cannot contain `/`. Value commands can target namespace
names containing `/`, but the `AddNamespace` descriptor verb requires a single-segment name and
rejects `/` in both that name and seed keys.

Command transport authenticates before dispatch. Owner/issuer mismatch returns HTTP 403, missing
namespaces return 404, unsupported kinds return 501, and other ownership refusals return 409.
Deleting a set declaration removes the value. Deleting a remove-value declaration releases
ownership without restoring a historical value.

## Getting started

The `cohesion-configurationstore` template supplies this minimal `Program.cs`.

```csharp
using Assimalign.Cohesion.ConfigurationStore;
using Assimalign.Cohesion.ConfigurationStore.Hosting;

ConfigurationStoreApplicationBuilder builder = ConfigurationStoreApplication.CreateBuilder(args);

await using ConfigurationStoreApplication application = builder.Build();
await application.RunAsync();
```

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area** — `cohesion/resources/ConfigurationStore/README.md`.
- **Hosting** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.Hosting/docs/OVERVIEW.md` and `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.Hosting/docs/DESIGN.md`.
- **Application model** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.ApplicationModel/docs/OVERVIEW.md` and `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.ApplicationModel/docs/DESIGN.md`.
- **Client** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.Client/docs/OVERVIEW.md`.
- **Runtime contract** — `cohesion/docs/RUNTIME_CONTRACT.md`.
- **Package boundaries** — `cohesion/.claude/rules/resource-areas.md`.
- **Template** — `cohesion/tooling/templates/Assimalign.Cohesion.Templates/src/content/cohesion-configurationstore/Program.cs`.
- **Supporting source** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore/docs/OVERVIEW.md`.
- **Supporting source** — `cohesion/docs/programs/SERVICE_STORY_REQUIREMENTS.md`.
- **Command declarations** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.ApplicationModel/src/Extensions/ConfigurationStoreResourceCommandExtensions.cs`.
