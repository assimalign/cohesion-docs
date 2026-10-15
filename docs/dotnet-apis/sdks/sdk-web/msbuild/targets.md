# Targets

The Web layer delegates target execution to the base and ApplicationModel SDKs.

## `Sdk/Sdk.targets`

The Web SDK imports the matching ApplicationModel SDK for an enabled resource, or the base targets
directly otherwise. The ApplicationModel import uses the exact version frozen into the Web SDK at
pack time and supplies the base targets transitively:

```xml
<Import Project="Sdk.targets" Sdk="Assimalign.Cohesion.Sdk.ApplicationModel" Version="$(CohesionVersion)" Condition="'$(CohesionApplicationModel)' == 'enabled'" />
<Import Sdk="Assimalign.Cohesion.Sdk" Project="Sdk.targets" Condition="'$(_CohesionApplicationModelSdkImported)' != 'true'" />
```

There are no area `Target` declarations, `BeforeTargets`, `AfterTargets`,
`DependsOnTargets`, `Inputs`, or `Outputs` attributes.

## `Targets/Sdk.Web.targets`

This file contains an empty `Project` element. It is packaged but not imported by the area SDK. It
contributes no build hook.

## Inherited execution

The base SDK supplies settings generation and pin validation. The ApplicationModel SDK owns resource
manifests, generated resource APIs, certificates, images, and orchestration targets. Its import after
the project body sees the consumer’s final `CohesionApplicationModel` choice, while placing enabled
resource-host defaults before `Microsoft.NET.Sdk.targets`.

[MSBuild](index.md) · [Tasks](tasks.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Tasks/docs/DESIGN.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Targets/Sdk.Web.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApplicationModel/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/README.md`
