# Assimalign.Cohesion.Sdk.MessageHub

This SDK adds MessageHub framework references and resource defaults to a Cohesion executable.

> **Status:** Implemented. This describes the SDK build layer; resource runtime capabilities are documented separately.

Choose `Assimalign.Cohesion.Sdk.MessageHub` for a MessageHub application. It imports
`Assimalign.Cohesion.Sdk` and adds `Assimalign.Cohesion.App.MessageHub` alongside
`Assimalign.Cohesion.App`.

## Minimal project

Save the project declaration in a `.csproj` file and the pins in `global.json`. Supply a
`Program.cs` entry point; this declaration alone does not start a domain service.

```xml
<Project Sdk="Assimalign.Cohesion.Sdk.MessageHub">
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
    "Assimalign.Cohesion.Sdk.MessageHub": "10.0.1-preview.3"
  }
}
```

`CohesionApplicationModel` remains `disabled` until the consumer enables it. The area’s
`Assimalign.Cohesion.Sdk.MessageHub.Tasks` packaging project exists, but contains no task
implementation classes; shared tasks run from the base SDK.

## Reference

- **Overview** — [Properties, items, outputs, and validation](overview.md).
- **MSBuild** — [Import order](msbuild/index.md).
- **Task assembly** — [Packaging assembly](assimalign-cohesion-sdk-messagehub-tasks/index.md).
- **Resource** — [MessageHub documentation](../../../message-hub/index.md) and
  [API reference](../../resources/message-hub/index.md).
- **SDK family** — [All SDKs](../index.md).

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.MessageHub/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.MessageHub/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.MessageHub/Targets/Sdk.MessageHub.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.MessageHub/Targets/Sdk.MessageHub.targets`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.MessageHub/Tasks/src/Assimalign.Cohesion.Sdk.MessageHub.Tasks.csproj`
- **Source** — `cohesion/global.json`
- **Source** — `cohesion/sdks/README.md`
