# Assimalign.Cohesion.Sdk.LoadBalancer

This SDK adds the LoadBalancer shared framework and the App hosting kernel to a Cohesion executable.

> **Status:** Implemented. This describes the SDK build layer; resource runtime capabilities are documented separately.

Choose `Assimalign.Cohesion.Sdk.LoadBalancer` for a LoadBalancer application. It imports `Assimalign.Cohesion.Sdk` and adds
`Assimalign.Cohesion.App.LoadBalancer` alongside `Assimalign.Cohesion.App`. The application's composition,
including whether it is a managed resource, lives in its `Program.cs`; the SDK generates nothing.

## Minimal project

Save the project declaration in a `.csproj` file and the pins in `global.json`. Supply a
`Program.cs` entry point.

```xml
<Project Sdk="Assimalign.Cohesion.Sdk.LoadBalancer">
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
    "Assimalign.Cohesion.Sdk.LoadBalancer": "10.0.1-preview.3"
  }
}
```

`App.LoadBalancer` carries `Assimalign.Cohesion.LoadBalancer.Hosting.ApplicationModel`, so the application calls
`builder.Services.AddResource(name, resource => ...)` with no package reference. It references
only the platform packages it opts into, such as
`Assimalign.Cohesion.ApplicationModel.Hosting.InProcess`. The area's
`Assimalign.Cohesion.Sdk.LoadBalancer.Tasks` packaging project contains no task implementation classes.

## Reference

- **Overview** — [Properties, items, outputs, and validation](overview.md).
- **MSBuild** — [Import order](msbuild/index.md).
- **Task assembly** — [Packaging assembly](assimalign-cohesion-sdk-loadbalancer-tasks/index.md).
- **Resource** — [LoadBalancer documentation](../../../load-balancer/index.md) and
  [API reference](../../resources/load-balancer/index.md).
- **SDK family** — [All SDKs](../index.md).

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.LoadBalancer/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.LoadBalancer/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.LoadBalancer/Targets/Sdk.LoadBalancer.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.LoadBalancer/Targets/Sdk.LoadBalancer.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.LoadBalancer/Tasks/src/Assimalign.Cohesion.Sdk.LoadBalancer.Tasks.csproj`.
- **Source** — `cohesion/sdks/README.md`.
