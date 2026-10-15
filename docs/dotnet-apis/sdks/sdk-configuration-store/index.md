# Assimalign.Cohesion.Sdk.ConfigurationStore

This SDK adds ConfigurationStore framework references and resource defaults to a Cohesion executable.

> **Status:** Implemented. This describes the SDK build layer; resource runtime capabilities are documented separately.

Choose `Assimalign.Cohesion.Sdk.ConfigurationStore` for a ConfigurationStore application. It imports
`Assimalign.Cohesion.Sdk` and adds `Assimalign.Cohesion.App.ConfigurationStore` alongside
`Assimalign.Cohesion.App`.

## Minimal project

Save the project declaration in a `.csproj` file and the pins in `global.json`. Supply a
`Program.cs` entry point; this declaration alone does not start a domain service.

```xml
<Project Sdk="Assimalign.Cohesion.Sdk.ConfigurationStore">
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
    "Assimalign.Cohesion.Sdk.ConfigurationStore": "10.0.1-preview.3"
  }
}
```

`CohesionApplicationModel` remains `disabled` until the consumer enables it. The area’s
`Assimalign.Cohesion.Sdk.ConfigurationStore.Tasks` packaging project exists, but contains no task
implementation classes; shared tasks run from the base SDK.

## Reference

- **Overview** — [Properties, items, outputs, and validation](overview.md).
- **MSBuild** — [Import order](msbuild/index.md).
- **Task assembly** — [Packaging assembly](assimalign-cohesion-sdk-configurationstore-tasks/index.md).
- **Resource** — [ConfigurationStore documentation](../../../configuration-store/index.md) and
  [API reference](../../resources/configuration-store/index.md).
- **SDK family** — [All SDKs](../index.md).

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ConfigurationStore/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ConfigurationStore/Sdk/Sdk.targets`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.ConfigurationStore/Targets/Sdk.ConfigurationStore.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.ConfigurationStore/Targets/Sdk.ConfigurationStore.targets`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.ConfigurationStore/Tasks/src/Assimalign.Cohesion.Sdk.ConfigurationStore.Tasks.csproj`
- **Source** — `cohesion/global.json`
- **Source** — `cohesion/sdks/README.md`
