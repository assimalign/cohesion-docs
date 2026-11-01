# Assimalign.Cohesion.Sdk

The base SDK supplies compiler defaults, framework registration, and shared build tasks.

> **Status:** Implemented.

Use this SDK for a Cohesion library or as the inherited base of a resource or gateway SDK. It chains
to `Microsoft.NET.Sdk` and generates no application-model sources: a resource makes itself a
managed instance in its `Program.cs`, and a gateway declares its model in its own. For domain
functionality, choose the matching [area SDK](../index.md).

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
entry point, and adds an explicit `FrameworkReference` to `Assimalign.Cohesion.App` when it needs
the hosting kernel. Resource-area SDKs select executable output and reference App together with
their area framework; the Gateway SDK references the gateway package instead.

- **Overview** — [Consumer properties, items, and diagnostics](overview.md).
- **MSBuild** — [Import graph and build files](msbuild/index.md).
- **Task assembly** — [`Assimalign.Cohesion.Sdk.Tasks`](assimalign-cohesion-sdk-tasks/index.md).
- **Framework** — [App kernel, producers, and membership examples](../../libraries/app/index.md).
- **Family** — [All SDKs](../index.md).

## Sources

- **Source** — `cohesion/sdks/README.md`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/OVERVIEW.md`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Tasks/docs/DESIGN.md`.
