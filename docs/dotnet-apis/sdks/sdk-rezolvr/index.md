# Assimalign.Cohesion.Sdk.Rezolvr

This SDK adds Rezolvr framework references and resource defaults to a Cohesion executable.

> **Status:** Implemented. This describes the SDK build layer; resource runtime capabilities are documented separately.

Choose `Assimalign.Cohesion.Sdk.Rezolvr` for a Rezolvr application. It imports
`Assimalign.Cohesion.Sdk` and adds `Assimalign.Cohesion.App.Rezolvr` alongside
`Assimalign.Cohesion.App`.

## Minimal project

Save the project declaration in a `.csproj` file and the pins in `global.json`. Supply a
`Program.cs` entry point; this declaration alone does not start a domain service.

```xml
<Project Sdk="Assimalign.Cohesion.Sdk.Rezolvr">
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
    "Assimalign.Cohesion.Sdk.Rezolvr": "10.0.1-preview.3"
  }
}
```

`CohesionApplicationModel` remains `disabled` until the consumer enables it. The area’s
`Assimalign.Cohesion.Sdk.Rezolvr.Tasks` packaging project exists, but contains no task
implementation classes; shared tasks run from the base SDK.

## Reference

- **Overview** — [Properties, items, outputs, and validation](overview.md).
- **MSBuild** — [Import order](msbuild/index.md).
- **Task assembly** — [Packaging assembly](assimalign-cohesion-sdk-rezolvr-tasks/index.md).
- **Resource** — [Rezolvr documentation](../../../rezolvr/index.md) and
  [API reference](../../resources/rezolvr/index.md).
- **SDK family** — [All SDKs](../index.md).

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Rezolvr/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Rezolvr/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Rezolvr/Targets/Sdk.Rezolvr.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Rezolvr/Targets/Sdk.Rezolvr.targets`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk.Rezolvr/Tasks/src/Assimalign.Cohesion.Sdk.Rezolvr.Tasks.csproj`
- **Source** — `cohesion/global.json`
- **Source** — `cohesion/sdks/README.md`
