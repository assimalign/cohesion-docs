# Targets

The Web layer delegates target execution to the base SDK.

## `Sdk/Sdk.targets`

Imports `Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`, then `Targets/Sdk.Web.targets`. There are no
Web `Target` declarations or hooks.

## `Targets/Sdk.Web.targets`

An empty `Project` element.

## Inherited execution

The base SDK supplies settings generation and pin validation; see the
[base target reference](../../sdk/msbuild/targets.md).

[MSBuild](index.md) · [Tasks](tasks.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Targets/Sdk.Web.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Web/Targets/Sdk.Web.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`.
