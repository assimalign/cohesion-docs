# IVpnGatewayApplication

The `IVpnGatewayApplication` type belongs to `Assimalign.Cohesion.VpnGateway`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.VpnGateway` Assembly: `Assimalign.Cohesion.VpnGateway`

## Purpose and surface

The root contracts are hosting-free (O34): `IVpnGatewayApplication` exposes `Context`, `StartAsync`
, and `StopAsync`; `IVpnGatewayApplicationContext` exposes `ContentRootPath`.
`IVpnGatewayApplicationBuilder` exposes `Build()`; it currently declares no area-specific verbs.
The root and feature packages reference no `Assimalign.Cohesion.Hosting*` library; `COHRES004`
enforces the boundary.

`StartAsync(CancellationToken)` starts the application; `StopAsync(CancellationToken)` drains and
stops it. Lifecycle failures propagate to the caller. The contract carries no host identity, runner,
or disposal members.

## Hosting implementation

`VpnGatewayApplication.CreateBuilder(args)` returns the public concrete
`VpnGatewayApplicationBuilder`; its `Build()` returns the public
`VpnGatewayApplication : Host<VpnGatewayApplicationContext>`. The public
`VpnGatewayApplicationContext` implements `IVpnGatewayApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

## Usage

See the [source-backed usage examples](examples/index.md).

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/VpnGateway/Assimalign.Cohesion.VpnGateway/docs/Assembly/Assimalign.Cohesion.VpnGateway/IVpnGatewayApplication/OVERVIEW.md`.
