# Assimalign.Cohesion.ApplicationModel.Definition

The logical side of the application model: what one gateway runs, declared and validated into an immutable model.

> **Status:** Implemented.

## Reference pages

- **[Design](design.md)** — One model per gateway, dependencies, settings, outputs, and extensions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

A model declares a gateway's resources with permission-carrying dependencies, settings, outputs, and
platform extensions, and names the parent gateway it joins. It references only the shared identity:
never the gateway-to-instance protocol and never a running instance.

## Public types

| Area | Types |
|---|---|
| Composition | `ApplicationModel`, `IApplicationModelBuilder`, `IApplicationModel`, `IApplicationSet`, `IApplicationModelParent` |
| Resources | `IApplicationResourceDefinition`, `ApplicationResourceDependency`, `ApplicationResourceSetting`, `ApplicationResourceOutput`, `ApplicationResourceExtension` |
| Manifests | `ApplicationModelManifest`, `ApplicationResourceManifest`, `ApplicationGatewayRole` |
| Names | `GatewayName`, `GatewayId`, `SettingName`, `OutputName`, `ExtensionName`, `Permission` |

## Usage

A child gateway's model names its parent and reads the parent's outputs:

```csharp
using Assimalign.Cohesion.ApplicationModel;

IApplicationModelBuilder east = ApplicationModel.CreateBuilder("east");
IApplicationModelParent root = east.UseParent("root");

IApplicationResourceDefinition database = east.AddResource("Database.Sql", "orders-db");
east.AddResource("Web", "orders-api")
    .DependsOn(database, "data.read", "data.write")
    .WithSetting("Database:Endpoint", database.Endpoint)
    .WithSetting("Identity:Authority", root.Endpoint("IdentityHub", "identity"))
    .WithSetting("Logging:LogLevel:Default", "Information");

IApplicationModel model = east.Build();
ApplicationModelManifest manifest = model.Manifest;
```

A gateway program declares its model through
[`ApplicationGatewayBuilder`](../assimalign-cohesion-applicationmodel-gateway/index.md), which uses
these types and builds the gateway that runs it. Manifest data becomes the same model through
`ApplicationModel.Create(manifest)`.

## Dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Core` | `CohesionProjectReference` |
| `Assimalign.Cohesion.ApplicationModel` | `CohesionProjectReference` |

[Parent: ApplicationModel](../index.md)

## Sources

- **Primary source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Definition/docs/OVERVIEW.md`.
- **Design** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Definition/docs/DESIGN.md`.
- **Project** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Definition/src/Assimalign.Cohesion.ApplicationModel.Definition.csproj`.
