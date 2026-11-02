# Assimalign.Cohesion.Sdk.ApiManager

This SDK adds the ApiManager shared framework and the App hosting kernel to a Cohesion executable.

> **Status:** Implemented. This describes the SDK build layer; resource runtime capabilities are documented separately.

Choose `Assimalign.Cohesion.Sdk.ApiManager` for an ApiManager application. It imports `Assimalign.Cohesion.Sdk` and adds
`Assimalign.Cohesion.App.ApiManager` alongside `Assimalign.Cohesion.App`. The application's composition,
including whether it is a managed resource, lives in its `Program.cs`; the SDK generates nothing.

## Minimal project

Save the project declaration in a `.csproj` file and the pins in `global.json`. Supply a
`Program.cs` entry point.

```xml
<Project Sdk="Assimalign.Cohesion.Sdk.ApiManager">
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
    "Assimalign.Cohesion.Sdk.ApiManager": "10.0.1-preview.3"
  }
}
```

`App.ApiManager` carries `Assimalign.Cohesion.ApiManager.Hosting.ApplicationModel`, so the application calls
`builder.Services.AddResource(name, resource => ...)` with no package reference. It references
only the platform packages it opts into, such as
`Assimalign.Cohesion.ApplicationModel.Hosting.InProcess`. The area's
`Assimalign.Cohesion.Sdk.ApiManager.Tasks` packaging project contains no task implementation classes.

## Reference

- **Overview** — [Properties, items, outputs, and validation](overview.md).
- **MSBuild** — [Import order](msbuild/index.md).
- **Task assembly** — [Packaging assembly](assimalign-cohesion-sdk-apimanager-tasks/index.md).
- **Resource** — [ApiManager documentation](../../../api-manager/index.md) and
  [API reference](../../resources/api-manager/index.md).
- **SDK family** — [All SDKs](../index.md).

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApiManager/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApiManager/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApiManager/Targets/Sdk.ApiManager.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApiManager/Targets/Sdk.ApiManager.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApiManager/Tasks/src/Assimalign.Cohesion.Sdk.ApiManager.Tasks.csproj`.
- **Source** — `cohesion/sdks/README.md`.
