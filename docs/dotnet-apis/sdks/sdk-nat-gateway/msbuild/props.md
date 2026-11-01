# Props

The NatGateway props files add its framework references and declare nothing else.

## `Sdk/Sdk.props`

Sets `_CohesionResourceSdk=true` and imports the base `Sdk.props` without an inline version, so
the base pin must be present in `global.json`. Adds `FrameworkReference` items for
`Assimalign.Cohesion.App` and `Assimalign.Cohesion.App.NatGateway` unless
`CohesionAutoIncludeAppFramework=false`, then imports the area props.

## `Targets/Sdk.NatGateway.props`

The file is an empty area hook: the resource's composition lives in its `Program.cs`. It declares
no orchestration metadata, endpoints, probes, or mounts. Setting a property later does not
retroactively change a framework item already added during props evaluation; configure inclusion
before that import.

[MSBuild](index.md) · [Overview](../overview.md)

## Sources

- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.NatGateway/Sdk/Sdk.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.NatGateway/Sdk/Sdk.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.NatGateway/Targets/Sdk.NatGateway.props`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk.NatGateway/Targets/Sdk.NatGateway.targets`.
- **Source** — `cohesion/sdks/Assimalign.Cohesion.Sdk/Targets/Assimalign.Cohesion.Sdk.FrameworkReference.props`.
