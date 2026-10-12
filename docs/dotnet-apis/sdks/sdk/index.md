# Assimalign.Cohesion.Sdk

The base SDK supplies compiler defaults, framework registration, and shared build tasks.

Use this SDK for a Cohesion library or as the inherited base of a resource SDK. A direct executable
can explicitly reference `Assimalign.Cohesion.App`. For domain functionality, choose the matching
[area SDK](../index.md).

## Minimal consumer

```xml
<Project Sdk="Assimalign.Cohesion.Sdk">
</Project>
```

```json
{
  "sdk": {
    "version": "10.0.300",
    "rollForward": "latestFeature"
  },
  "msbuild-sdks": {
    "Assimalign.Cohesion.Sdk": "10.0.1-preview.3"
  }
}
```

The base SDK defaults to library output. An executable sets `OutputType=Exe`, supplies a `Program.cs`
entry point, and adds an explicit `FrameworkReference` when it needs the App hosting kernel.
Resource-area SDKs select executable output and reference App together with their area framework.
The base `Common.props` defaults `CohesionApplicationModel` to `disabled`. For resource generation,
set `CohesionApplicationModel=enabled` and choose an area SDK or explicitly import
`Assimalign.Cohesion.Sdk.ApplicationModel` through `Sdk.targets` after the enabling property in the
project body. ApplicationModel owns the resource props, targets, manifest contribution template,
and `Assimalign.Cohesion.Sdk.ApplicationModel.Tasks` assembly.

- **Overview** — [Consumer properties, items, generated artifacts, and diagnostics](overview.md).
- **MSBuild** — [Import graph and build files](msbuild/index.md).
- **Task assembly** — [`Assimalign.Cohesion.Sdk.Tasks`](assimalign-cohesion-sdk-tasks/index.md).
- **Framework** — [App kernel, producers, and membership examples](../../libraries/app/index.md).
- **Family** — [All SDKs](../index.md).

## Sources

- **Source** — `cohesion/sdks/README.md`
- **Source** — `cohesion/global.json`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.Common.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApplicationModel/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApplicationModel/Targets/Sdk.Resource.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApplicationModel/Targets/Sdk.Resource.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApplicationModel/Targets/Assimalign.Cohesion.Sdk.ResourceManifest.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/OVERVIEW.md`
