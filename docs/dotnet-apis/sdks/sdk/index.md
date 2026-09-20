# Assimalign.Cohesion.Sdk

The base SDK supplies executable defaults, framework registration, resource generation, and shared build tasks.

> **Status:** Partial. Shared SDK behavior is implemented; the image pipeline has an unspecified in-container build route.

Use this SDK for a generic Cohesion application or as the inherited base of a resource SDK. It pairs
with `Assimalign.Cohesion.App`. For domain functionality, choose the matching
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

Supply a `Program.cs` entry point. A library instead sets `OutputType=Library`. Resource generation
is disabled unless explicitly enabled; a generic enabled resource must also declare its kind and
control-plane metadata.

- **Overview** — [Consumer properties, items, generated artifacts, and diagnostics](overview.md).
- **MSBuild** — [Import graph and build files](msbuild/index.md).
- **Task assembly** — [`Assimalign.Cohesion.Sdk.Tasks`](assimalign-cohesion-sdk-tasks/index.md).
- **Family** — [All SDKs](../index.md).

## Sources

- **Source** — `cohesion/sdks/README.md`
- **Source** — `cohesion/global.json`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/OVERVIEW.md`
