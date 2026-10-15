# Assimalign.Cohesion.ApplicationModel.Gateway.InProcess

Realizes composable resources as separately owned hosts inside a gateway process.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

- **[`InProcessGateway`](in-process-gateway.md)** — type reference.

- **[`InProcessGatewayExtensions`](in-process-gateway-extensions.md)** — type reference.

- **[`InProcessGatewayOptions`](in-process-gateway-options.md)** — type reference.

- **[`InProcessResourceDescriptorExtensions`](in-process-resource-descriptor-extensions.md)** — type reference.

[ApplicationModel](../index.md)

## Scope

The gateway invokes real resource entry points with isolated ambient `ResourceContext` values. The
consuming Composite supplies resource runtimes through project references; this package does not
reference area hosting modules. Offline rendering produces plan units without invoking entry points
or resolving runtime inputs.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.ApplicationModel`](../../application-model/assimalign-cohesion-applicationmodel/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.ApplicationModel.Gateway`](../../application-model/assimalign-cohesion-applicationmodel-gateway/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Hosting`](../../hosting/assimalign-cohesion-hosting/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Hosting.Resources`](../../hosting/assimalign-cohesion-hosting-resources/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `InProcessGateway` | `src/InProcessGateway.cs` |
| `InProcessGatewayExtensions` | `src/Extensions/InProcessGatewayExtensions.cs` |
| `InProcessGatewayOptions` | `src/InProcessGatewayOptions.cs` |
| `InProcessResourceDescriptorExtensions` | `src/Extensions/InProcessResourceDescriptorExtensions.cs` |

## Sources

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/src/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/ApplicationModel/README.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/src`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/src/InProcessGateway.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/src/Extensions/InProcessGatewayExtensions.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/src/InProcessGatewayOptions.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/src/Extensions/InProcessResourceDescriptorExtensions.cs`.
