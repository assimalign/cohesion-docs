# MSBuild

The VpnGateway SDK surrounds the consumer project with base imports and area defaults.

## Evaluation order

1. `Assimalign.Cohesion.Sdk.VpnGateway/Sdk/Sdk.props` begins.
2. `Assimalign.Cohesion.Sdk/Sdk/Sdk.props` enters the base SDK.
3. `Targets/Assimalign.Cohesion.Sdk.Defaults.props` supplies empty-value defaults before Microsoft
   defaults run.
4. `Microsoft.NET.Sdk/Sdk/Sdk.props` evaluates the Microsoft SDK props and consumer `Directory.Build.props`.
5. `Targets/Build.Version.props` loads the static version snapshot generated when the SDK was packed.
6. `Targets/Assimalign.Cohesion.Sdk.Common.props` sets the SDK marker and intermediate path.
7. `Targets/Sdk.Resource.props` registers manifest tasks and resource metadata defaults.
8. `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.props` registers the settings task.
9. `Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props` registers all frameworks and
   conditionally adds `Assimalign.Cohesion.App`.
10. `Assimalign.Cohesion.Sdk.VpnGateway/Sdk/Sdk.props` adds the area framework reference, then
    imports `Targets/Sdk.VpnGateway.props`.
11. The consumer project body evaluates.
12. `Assimalign.Cohesion.Sdk.VpnGateway/Sdk/Sdk.targets` imports the base targets.
13. `Assimalign.Cohesion.Sdk/Sdk/Sdk.targets` captures image publish inputs and supplies
    self-contained host-runtime defaults for enabled Debug resources.
14. `Microsoft.NET.Sdk/Sdk/Sdk.targets` loads Microsoft build targets.
15. `Targets/Assimalign.Cohesion.Sdk.PinValidation.targets` registers pin validation.
16. `Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets` wires settings generation and cleanup.
17. `Targets/Assimalign.Cohesion.Sdk.ApplicationModel.Build.targets` imports `Sdk.Resource.targets`
    , preserves the container extension chain, then imports `Sdk.Image.targets`.
18. `Targets/Assimalign.Cohesion.Sdk.NameOnly.ProjectReference.targets` converts Cohesion reference items.

`Targets/Sdk.VpnGateway.targets` is an empty file and is not imported by `Sdk/Sdk.targets`.
Evaluation order sets values and registers targets; target execution follows dependencies and hooks,
not file order.

- **Props** — [Every props file](props.md).
- **Targets** — [Target wiring](targets.md).
- **Tasks** — [Task ownership and tests](tasks.md).

[SDK home](../index.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.VpnGateway/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.VpnGateway/Sdk/Sdk.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.VpnGateway/Targets/Sdk.VpnGateway.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.VpnGateway/Targets/Sdk.VpnGateway.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Sdk/Sdk.targets`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.ApplicationModel.Build.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.Common.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.Defaults.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.NameOnly.ProjectReference.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.PinValidation.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.ResourceManifest.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.props`
- **Source** —
  `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.StronglyTypedSettings.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Image.targets`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.props`
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Sdk.Resource.targets`
