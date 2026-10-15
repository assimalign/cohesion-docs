# Assimalign.Cohesion.Sdk.IoTHub

This SDK adds IoTHub framework references and resource defaults to a Cohesion executable.

> **Status:** Implemented. This describes the SDK build layer; resource runtime capabilities are documented separately.

Choose `Assimalign.Cohesion.Sdk.IoTHub` for a IoTHub application. It imports
`Assimalign.Cohesion.Sdk` and adds `Assimalign.Cohesion.App.IoTHub` alongside
`Assimalign.Cohesion.App`.

## Minimal project

Save the project declaration in a `.csproj` file and the pins in `global.json`. Supply a
`Program.cs` entry point; this declaration alone does not start a domain service.

```xml
<Project Sdk="Assimalign.Cohesion.Sdk.IoTHub">
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
    "Assimalign.Cohesion.Sdk.IoTHub": "10.0.1-preview.3"
  }
}
```

`CohesionApplicationModel` remains `disabled` until the consumer enables it. The area’s
`Assimalign.Cohesion.Sdk.IoTHub.Tasks` packaging project exists, but contains no task implementation
classes; shared tasks run from the base SDK.

## Reference

- **Overview** — [Properties, items, outputs, and validation](overview.md).
- **MSBuild** — [Import order](msbuild/index.md).
- **Task assembly** — [Packaging assembly](assimalign-cohesion-sdk-iothub-tasks/index.md).
- **Resource** — [IoTHub documentation](../../../iot-hub/index.md) and
  [API reference](../../resources/iot-hub/index.md).
- **SDK family** — [All SDKs](../index.md).

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IoTHub/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IoTHub/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IoTHub/Targets/Sdk.IoTHub.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.IoTHub/Targets/Sdk.IoTHub.targets`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.IoTHub/Tasks/src/Assimalign.Cohesion.Sdk.IoTHub.Tasks.csproj`
- **Source** — `cohesion/global.json`
- **Source** — `cohesion/sdks/README.md`
