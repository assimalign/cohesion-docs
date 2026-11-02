# Assimalign.Cohesion.Sdk.NatGateway

This SDK adds the NatGateway shared framework and the App hosting kernel to a Cohesion executable.

> **Status:** Implemented. This describes the SDK build layer; resource runtime capabilities are documented separately.

Choose `Assimalign.Cohesion.Sdk.NatGateway` for a NatGateway application. It imports `Assimalign.Cohesion.Sdk` and adds
`Assimalign.Cohesion.App.NatGateway` alongside `Assimalign.Cohesion.App`. The application's composition,
including whether it is a managed resource, lives in its `Program.cs`; the SDK generates nothing.

## Minimal project

Save the project declaration in a `.csproj` file and the pins in `global.json`. Supply a
`Program.cs` entry point.

```xml
<Project Sdk="Assimalign.Cohesion.Sdk.NatGateway">
</Project>
```

```json
{
  "sdk": {
    "version": "10.0.300",
    "rollForward": "latestFeature"
  },
  "msbuild-sdks": {
    "Assimalign.Cohesion.Sdk": "10.0.1-preview.3",
    "Assimalign.Cohesion.Sdk.NatGateway": "10.0.1-preview.3"
  }
}
```

`App.NatGateway` carries `Assimalign.Cohesion.NatGateway.Hosting.ApplicationModel`, so the application calls
`builder.Services.AddResource(name, resource => ...)` with no package reference. It references
only the platform packages it opts into, such as
`Assimalign.Cohesion.ApplicationModel.Hosting.InProcess`. The area's
`Assimalign.Cohesion.Sdk.NatGateway.Tasks` packaging project contains no task implementation classes.

## Reference

- **Overview** — [Properties, items, outputs, and validation](overview.md).
- **MSBuild** — [Import order](msbuild/index.md).
- **Task assembly** — [Packaging assembly](assimalign-cohesion-sdk-natgateway-tasks/index.md).
- **Resource** — [NatGateway documentation](../../../nat-gateway/index.md) and
  [API reference](../../resources/nat-gateway/index.md).
- **SDK family** — [All SDKs](../index.md).

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.NatGateway/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.NatGateway/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.NatGateway/Targets/Sdk.NatGateway.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.NatGateway/Targets/Sdk.NatGateway.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.NatGateway/Tasks/src/Assimalign.Cohesion.Sdk.NatGateway.Tasks.csproj`.
- **Source** — `cohesion/sdks/README.md`.
