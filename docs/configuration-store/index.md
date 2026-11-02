# ConfigurationStore

ConfigurationStore persists named configuration namespaces and serves their snapshots.

> **Status:** Partial. Namespace storage and snapshot reads are implemented; mutations over the wire, replication, and the broader versioning program remain deferred.

## What it is

ConfigurationStore holds ordinary configuration as nullable string values in named namespaces.
Its durable files are plain JSON under the `data` volume. Use [SecretStore](../secret-store/index.md)
for protected secret material.

See the [ConfigurationStore API reference](../dotnet-apis/resources/configuration-store/index.md).

## Project map

| Assembly | Responsibility |
|---|---|
| [`Assimalign.Cohesion.ConfigurationStore`](../dotnet-apis/resources/configuration-store/assimalign-cohesion-configurationstore/index.md) | Application contracts, namespace declarations, and loader abstraction. |
| [`Assimalign.Cohesion.ConfigurationStore.Client`](../dotnet-apis/resources/configuration-store/assimalign-cohesion-configurationstore-client/index.md) | Core-only client for namespace listing and snapshot reads. |
| [`Assimalign.Cohesion.ConfigurationStore.Hosting`](../dotnet-apis/resources/configuration-store/assimalign-cohesion-configurationstore-hosting/index.md) | Durable JSON store and HTTP protocol host. |
| [`Assimalign.Cohesion.ConfigurationStore.Hosting.ApplicationModel`](../dotnet-apis/resources/configuration-store/assimalign-cohesion-configurationstore-hosting-applicationmodel/index.md) | `builder.Services.AddResource(name, ...)`: the store as a managed resource instance. |

## Hosting model

`ConfigurationStoreApplication.CreateBuilder(args)` returns the concrete builder. Its host appends
an HTTP protocol service after explicit user services, so the listener starts last and drains first.
The host uses `--endpoint` or a loopback default and serves list and read operations beneath
`/cohesion/v1` alongside health and endpoint-discovery routes.

`/cohesion/v1` accepts only transport peers at `127.0.0.1`, `::1`, or IPv4-mapped `127.0.0.1`;
other peers receive `403`, regardless of forwarded headers. Bare `/healthz`, `/readyz`, and
`/livez` stay open.

## Managed resource

A store becomes a managed resource instance with one registration in its `Program.cs`:
`builder.Services.AddResource("settings", resource => resource.UseInProcess())`. When the
environment names a platform the resource opted into, it publishes its manifest, restores the state
its gateway kept, and runs the gateway's actions. ConfigurationStore exposes no actions of its own
yet; the application can add its own with `AddAction`. A gateway declares the resource generically:
`builder.AddResource("ConfigurationStore", "settings")`. See
[ApplicationModel](../dotnet-apis/libraries/application-model/index.md).

## SDK and framework

`Assimalign.Cohesion.Sdk.ConfigurationStore` delivers the runtime family and the resource
integration through `Assimalign.Cohesion.App.ConfigurationStore`. The `.Client` package is
NuGet-only. See the [ConfigurationStore SDK reference](../dotnet-apis/sdks/sdk-configuration-store/index.md).

## Namespaces and snapshots

Builder-time `AddNamespace` and `IConfigurationNamespaceBuilder.Set` seed missing durable data.
Writes replace JSON atomically. `IConfigurationStoreClient.ListNamespacesAsync` reads namespace
names; `GetNamespaceAsync` returns an `IReadOnlyDictionary<string, string?>` snapshot. A snapshot
read does not imply the planned immutable revision, promotion, or watch facilities.

## Getting started

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.ConfigurationStore.Hosting;
using Assimalign.Cohesion.ConfigurationStore.Hosting.ApplicationModel;

ConfigurationStoreApplicationBuilder builder = ConfigurationStoreApplication.CreateBuilder(args);
builder.Services.AddResource("settings", resource => resource.UseInProcess());

await using ConfigurationStoreApplication application = builder.Build();
await application.RunAsync();
```

Return to [Cohesion Documentation](../index.md).

## Sources

- **Area** — `cohesion/resources/ConfigurationStore/README.md`.
- **Hosting** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.Hosting/docs/OVERVIEW.md` and `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.Hosting/docs/DESIGN.md`.
- **Resource integration** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.Hosting.ApplicationModel/docs/OVERVIEW.md`.
- **Client** — `cohesion/resources/ConfigurationStore/Assimalign.Cohesion.ConfigurationStore.Client/docs/OVERVIEW.md`.
- **Package boundaries** — `cohesion/.claude/rules/resource-areas.md`.
