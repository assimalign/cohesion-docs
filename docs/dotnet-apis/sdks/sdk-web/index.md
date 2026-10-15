# Assimalign.Cohesion.Sdk.Web

Sdk.Web builds a Cohesion HTTP application with the base SDK's executable defaults and the App and
App.Web shared frameworks. It declares the Web resource endpoint/probe defaults, enables generated
typed-handler interceptors, and copies `wwwroot/**` into build and publish output. Optional resource
orchestration is supplied through Sdk.ApplicationModel when `CohesionApplicationModel=enabled`.

Select `Assimalign.Cohesion.Sdk.Web` as the project SDK and pin both it and
`Assimalign.Cohesion.Sdk` to the same exact version in `global.json`, as described in the
[SDK consumption guide](../index.md#resolution-and-pins). Configure the server and middleware through
`WebApplication.CreateBuilder(args)` in `Program.cs`.

NativeAOT Web applications default to `EventSourceSupport=true`, preserving delivery of the Web
server's operator diagnostics. Entry-point Web hosts forward their selected Web and HTTP transport
sources to configured logging sinks. This adds no sink, and an application's filter choices apply.

To explicitly remove EventSource delivery from a native publish, set the property in the project:

```xml
<PropertyGroup>
    <EventSourceSupport>false</EventSourceSupport>
</PropertyGroup>
```

The choice also disables an in-process EventListener and EventSource forwarding in that native
application. Server behavior and counter maintenance remain unchanged. See the
[design](assimalign-cohesion-sdk-web-tasks/design.md#nativeaot-event-delivery) for import precedence, measured native size costs, and the
independent SDK-evaluation and published-binary checks.

## Minimal project

Save the project declaration in a `.csproj` file and the pins in `global.json`. Supply a
`Program.cs` entry point; this declaration alone does not start a domain service.

```xml
<Project Sdk="Assimalign.Cohesion.Sdk.Web">
</Project>
```

```json
{
  "sdk": {
    "version": "10.0.300",
    "rollForward": "latestFeature"
  },
  "msbuild-sdks": {
    "Assimalign.Cohesion.Sdk": "10.0.0-preview.1",
    "Assimalign.Cohesion.Sdk.Web": "10.0.0-preview.1"
  }
}
```

`CohesionApplicationModel` remains `disabled` until the consumer enables it. The area’s
`Assimalign.Cohesion.Sdk.Web.Tasks` packaging project exists, but contains no task implementation
classes. Shared settings tasks run from the base SDK; enabled resource orchestration uses
`Assimalign.Cohesion.Sdk.ApplicationModel`.

## Reference

- **Overview** — [Properties, items, outputs, and validation](overview.md).
- **MSBuild** — [Import order](msbuild/index.md).
- **Task assembly** — [Packaging assembly](assimalign-cohesion-sdk-web-tasks/index.md).
- **Resource** — [Web documentation](../../../web/index.md) and
  [API reference](../../resources/web/index.md).
- **SDK family** — [All SDKs](../index.md).

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Tasks/docs/OVERVIEW.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Targets/Sdk.Web.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Targets/Sdk.Web.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Tasks/src/Assimalign.Cohesion.Sdk.Web.Tasks.csproj`
- **Source** — `cohesion/global.json`
- **Source** — `cohesion/sdks/README.md`
