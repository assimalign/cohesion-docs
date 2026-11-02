# Assimalign.Cohesion.Database.Hosting.ApplicationModel

The Database resource integration: `builder.Services.AddResource(name, ...)` makes a Database application a managed resource instance.

> **Status:** Implemented. The resource exposes only application-added actions and contributes no instance health yet.

## Reference pages

- **[Design](design.md)** — Composition, lifecycle, ownership, and current limits.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

The selected platform client publishes the instance's manifest, restores the gateway's snapshots,
and carries calls to its actions. Engine construction and wire listeners stay in the application's
own composition. Without a platform named in its environment, the application runs unmanaged and
`AddResource` registers nothing.

## Public types

| Type | Role |
|---|---|
| `DatabaseResourceExtensions` | `AddResource(ResourceName name, Action<DatabaseResourceOptions>? configure = null)`, an `extension(IServiceProviderBuilder)` member called on `builder.Services`. Returns the same registrations for chaining. |
| `DatabaseResourceOptions` | Sealed leaf of `ApplicationInstanceOptions`, with an internal constructor and a read-only `ResourceName Name`. The base supplies platform selection, deployment, `UseGatewayClient`, and `AddAction`; platform packages supply verbs such as `UseInProcess()`. |

`AddResource` rejects null services with `ArgumentNullException` and a default resource name with
`ArgumentException`. Options read the platform and deployment through Core's `AppEnvironment`.
`UseInProcess()` opts into a client; it does not select the platform. If the environment names a
platform without a registered client, `AddResource` throws `InvalidOperationException` immediately.

## Usage

```csharp
using System;

using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.Database;
using Assimalign.Cohesion.Database.Hosting;
using Assimalign.Cohesion.Database.Hosting.ApplicationModel;
using Assimalign.Cohesion.Database.Sql;
using Assimalign.Cohesion.Hosting;

DatabaseApplicationBuilder builder = DatabaseApplication.CreateBuilder(args);
builder.AddSql("orders-sql", sql =>
{
    sql.Options.RootPath = "./data/sql";
    sql.AddDatabase("orders");
    sql.AddServer(server => server.Listen(new Uri("cohesion-db://127.0.0.1:5740")));
});
builder.Services.AddResource("orders-db", resource => resource.UseInProcess());

await using DatabaseApplication application = builder.Build();
await application.RunAsync();
```

A gateway declares this program generically with `builder.AddResource("Database", "orders-db")`.
An in-process gateway uses a project reference naming the resource:

```xml
<ItemGroup>
  <ProjectReference Include="..\Orders.Database\Orders.Database.csproj" CohesionResource="orders-db" />
</ItemGroup>
```

The build exposes the program's entry point and registers it with the in-process gateway's
`UseInProcess()`. See [managed Database programs](../../../../database/overview.md#managed-resources)
for the gateway program and the current endpoint and readiness limits.

## Added actions

`AddAction(action, jsonContext)` accepts a supplied instance;
`AddAction(services => action, jsonContext)` creates one from the application's provider at Build.
Actions appear in registration order. Each content type needs its source-generated JSON context;
omit the context only for an action exchanging no content of its own. The
[registration example](examples/database-resource-registration-tests.md) resolves an action's
collaborator from services, and the [in-process example](examples/database-resource-in-process-tests.md)
writes and reads state through a gateway.

Duplicate action names fail Build. Factory-created actions cannot be disposable; put disposable
collaborators in services. Supplied action instances remain caller-owned. Database supplies no
catalog-management actions: database data and principals belong to each instance's durable catalog,
and the retired catalog-mutation commands have no replacement in the snapshot-restore model yet.

## Dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.DependencyInjection` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting` | `CohesionProjectReference` |
| `Assimalign.Cohesion.ApplicationModel.Hosting` | `CohesionProjectReference` |

`COHRES001` sanctions the reference to the [runtime module](../assimalign-cohesion-database-hosting/index.md)
by this project's exact name. The integration is a public member of `Assimalign.Cohesion.App.Database`,
so an `Sdk.Database` application needs no extra integration package reference. The application
references each platform it opts into, such as
[Hosting.InProcess](../../../libraries/application-model/assimalign-cohesion-applicationmodel-hosting-inprocess/index.md).
The instance contracts are
[ApplicationModel.Hosting](../../../libraries/application-model/assimalign-cohesion-applicationmodel-hosting/index.md).

## Shutdown and open questions

Gateway `StopAsync` and `Completion` leave the Database application running and accepting
connections (G37/G40). Gateway disposal stops its run, then disposes its owned platform (G42).
The owned in-process platform shuts down the Database program; a borrowed platform stays
caller-owned. The [in-process example](examples/database-resource-in-process-tests.md) uses gateway
disposal as its shutdown criterion.

The package's [seven open questions](design.md#current-limits) cover endpoints, readiness, the
startup window, Database-owned actions, instance health, the publication-to-open window, and HTTP
health. No production path starts the internal admin health service today, so managed Database
applications serve no HTTP health yet.

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting.ApplicationModel/docs/OVERVIEW.md`.
- **Public API** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting.ApplicationModel/src/DatabaseResourceOptions.cs` and `src/Extensions/DatabaseResourceExtensions.cs`.
- **Actions** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Hosting/src/ApplicationInstanceOptions.cs`.
- **Project** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting.ApplicationModel/src/Assimalign.Cohesion.Database.Hosting.ApplicationModel.csproj`.
- **Framework** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Runtime/Directory.Build.props`.
- **Design and open questions** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting.ApplicationModel/docs/DESIGN.md`.
- **Gateway lifetime** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/ApplicationGateway.cs`.
