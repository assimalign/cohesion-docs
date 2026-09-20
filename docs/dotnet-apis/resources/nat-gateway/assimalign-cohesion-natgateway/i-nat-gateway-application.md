# INatGatewayApplication

The `INatGatewayApplication` type belongs to `Assimalign.Cohesion.NatGateway`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.NatGateway` Assembly: `Assimalign.Cohesion.NatGateway`

## Purpose and surface

The root contracts are hosting-free (O34): `INatGatewayApplication` exposes `Context`, `StartAsync`
, and `StopAsync`; `INatGatewayApplicationContext` exposes `ContentRootPath`.
`INatGatewayApplicationBuilder` exposes `Build()`; it currently declares no area-specific verbs.
The root and feature packages reference no `Assimalign.Cohesion.Hosting*` library; `COHRES004`
enforces the boundary.

`StartAsync(CancellationToken)` starts the application; `StopAsync(CancellationToken)` drains and
stops it. Lifecycle failures propagate to the caller. The contract carries no host identity, runner,
or disposal members.

## Hosting implementation

`NatGatewayApplication.CreateBuilder(args)` returns the public concrete
`NatGatewayApplicationBuilder`; its `Build()` returns the public
`NatGatewayApplication : Host<NatGatewayApplicationContext>`. The public
`NatGatewayApplicationContext` implements `INatGatewayApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

## Usage

See the [source-backed usage examples](examples/index.md).

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/NatGateway/Assimalign.Cohesion.NatGateway/docs/Assembly/Assimalign.Cohesion.NatGateway/INatGatewayApplication/OVERVIEW.md`.
