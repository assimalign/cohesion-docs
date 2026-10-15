# MSBuild

The Web SDK surrounds the consumer project with shared imports and area defaults.

## Evaluation order

1. `Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.props` sets `_CohesionResourceSdk=true` before importing the base SDK.
2. The base `Sdk/Sdk.props` imports shared defaults before `Microsoft.NET.Sdk/Sdk/Sdk.props`, then loads the packed version snapshot, common defaults, settings task registration, and framework registrations.
3. Web adds `Assimalign.Cohesion.App` and `Assimalign.Cohesion.App.Web` framework references unless `CohesionAutoIncludeAppFramework=false`.
4. `Targets/Sdk.Web.props` supplies Web resource metadata, static-asset copying, the typed-handler interceptor namespace, and the conditional `EventSourceSupport=true` default.
5. The consumer project body evaluates and can override conditional property defaults and item metadata.
6. Web’s `Sdk/Sdk.targets` imports `Assimalign.Cohesion.Sdk.ApplicationModel` at `$(CohesionVersion)` when `CohesionApplicationModel=enabled`. Otherwise it imports the base targets directly.
7. For enabled resources, ApplicationModel supplies early resource-host properties and resource targets, imports the base targets, then imports its late build targets. The base import brings in Microsoft’s targets after the early resource-host properties are set.

`Targets/Sdk.Web.targets` is an empty file and is not imported by `Sdk/Sdk.targets`. Evaluation
order sets values and registers targets; target execution follows dependencies and hooks, not file
order.

- **Props** — [Every props file](props.md).
- **Targets** — [Target wiring](targets.md).
- **Tasks** — [Task ownership and tests](tasks.md).

[SDK home](../index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Tasks/docs/DESIGN.md`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Targets/Sdk.Web.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Targets/Sdk.Web.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.ApplicationModel/Sdk/Sdk.targets`
