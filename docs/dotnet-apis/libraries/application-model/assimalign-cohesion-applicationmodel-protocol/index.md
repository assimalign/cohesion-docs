# Assimalign.Cohesion.ApplicationModel.Protocol

The protocol between a gateway and the instances of its resources: manifests, publications, action invocations, snapshots, and content serialization.

> **Status:** Implemented.

## Reference pages

- **[Design](design.md)** — Wire stability, operations, and errors.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

Every type here is part of a message the two sides exchange, or of the one seam both sides use. Both
runtimes reference this assembly, so neither loads the other's code.

## Public types

| Area | Types |
|---|---|
| Publication | `ApplicationInstanceManifest`, `ApplicationActionDescriptor`, `InstanceKey`, `DeploymentId`, `ApplicationInstancePublication`, `IApplicationInstancePublisher` |
| Invocation | `ActionName`, `ApplicationActionKind`, `ApplicationActionOperation`, `ApplicationActionContext`, `ApplicationActionResult`, `ApplicationActionStatus` |
| State | `ApplicationActionSnapshot` |
| Content | `IApplicationContentSerializer`, `JsonApplicationContentSerializer`, `ApplicationContentException` |

## Usage

An instance publishes its manifest through its platform's client, and its gateway answers with the
snapshots to restore:

```csharp
using Assimalign.Cohesion.ApplicationModel;

ApplicationInstanceManifest manifest = new(
    "Web",
    "orders-api",
    InstanceKey.New(),
    Deployment: null,
    ControlPlaneEndpoint: null,
    [new ApplicationActionDescriptor("log-level", ApplicationActionKind.ReadWrite, IsStateful: true)]);
ApplicationInstancePublication publication = await publisher.PublishAsync(manifest, cancellationToken);
```

An operator's invocation requests one operation of an action:

```csharp
using Assimalign.Cohesion.ApplicationModel;

JsonApplicationContentSerializer json = JsonApplicationContentSerializer.Create(MyJsonContext.Default);
ApplicationActionContext context = ApplicationActionContext.Create(
    ApplicationActionOperation.Write,
    json,
    json.Serialize(new LogLevelSetting("Debug")));
```

## Dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Core` | `CohesionProjectReference` |
| `Assimalign.Cohesion.ApplicationModel` | `CohesionProjectReference` |

The seams only one side uses are elsewhere: the instance's control plane and gateway client in
[Hosting](../assimalign-cohesion-applicationmodel-hosting/index.md), the gateway's instance client in
[Gateway](../assimalign-cohesion-applicationmodel-gateway/index.md). Every transport belongs to a
platform package.

[Parent: ApplicationModel](../index.md)

## Sources

- **Primary source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Protocol/docs/OVERVIEW.md`.
- **Design** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Protocol/docs/DESIGN.md`.
- **Project** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Protocol/src/Assimalign.Cohesion.ApplicationModel.Protocol.csproj`.
