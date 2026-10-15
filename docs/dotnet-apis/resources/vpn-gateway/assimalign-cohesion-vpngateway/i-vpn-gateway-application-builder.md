# IVpnGatewayApplicationBuilder

The `IVpnGatewayApplicationBuilder` type belongs to `Assimalign.Cohesion.VpnGateway`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.VpnGateway` Assembly: `Assimalign.Cohesion.VpnGateway`

## Purpose

`IVpnGatewayApplicationBuilder` is the public composition seam for a VPN gateway application. It
exposes `Build()` returning `IVpnGatewayApplication`.

## Surface and behavior

- **`Build()`** — creates a configured VPN gateway application.

Registrations retain insertion order. The shared host starts the materialized services in that order
and stops them in reverse; a builder with no registrations still produces an empty collection. The
public concrete `VpnGatewayApplicationBuilder` lives in `Assimalign.Cohesion.VpnGateway.Hosting`.

## Exceptions

The concrete Hosting builder's `AddService` throws `ArgumentNullException` for a null service or
factory. `Build()` throws `InvalidOperationException` when a factory returns null, and otherwise
propagates factory failures.

## Usage

See the [source-backed usage examples](examples/index.md).

## Hosting-free application contract (O34)

The root contracts are hosting-free (O34): `IVpnGatewayApplication` exposes `Context`, `StartAsync`
, and `StopAsync`; `IVpnGatewayApplicationContext` exposes `ContentRootPath`.
`IVpnGatewayApplicationBuilder` exposes `Build()`; it currently declares no area-specific verbs.
The root and feature packages reference no `Assimalign.Cohesion.Hosting*` library; `COHRES004`
enforces the boundary.

`VpnGatewayApplication.CreateBuilder(args)` returns the public concrete
`VpnGatewayApplicationBuilder`; its `Build()` returns the public
`VpnGatewayApplication : Host<VpnGatewayApplicationContext>`. The public
`VpnGatewayApplicationContext` implements `IVpnGatewayApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

Background-work registration (`AddService`) is available only on the concrete Hosting builder.

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/VpnGateway/Assimalign.Cohesion.VpnGateway/docs/Assembly/Assimalign.Cohesion.VpnGateway/IVpnGatewayApplicationBuilder/OVERVIEW.md`.
