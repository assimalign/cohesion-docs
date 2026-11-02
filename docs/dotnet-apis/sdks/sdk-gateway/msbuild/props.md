# Props

The Gateway SDK's props files set the executable default and the implicit gateway package reference.

## `Sdk/Sdk.props`

Sets `OutputType=Exe` when empty, then imports the base `Sdk.props` without an inline version, so
the base pin must be present in `global.json`. It then imports `Targets/Sdk.Gateway.props`.

## `Targets/Sdk.Gateway.props`

| Item | Declaration | Condition |
|---|---|---|
| `PackageReference` | `Assimalign.Cohesion.ApplicationModel.Gateway` at `$(CohesionVersion)`, `IsImplicitlyDefined="true"` | `'$(CohesionGatewayImplicitReference)' != 'false'` |

`$(CohesionVersion)` comes from the base SDK's frozen `Build.Version.props`, which the base props
import first; this SDK's own frozen copy ships but is not imported. A consumer pins both SDKs to one
version, so the two agree. The file declares no provider, image, or manifest metadata.

[MSBuild](index.md) · [Overview](../overview.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.Gateway/Targets/Sdk.Gateway.props`.
