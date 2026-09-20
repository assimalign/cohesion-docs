# Assimalign.Cohesion.NatGateway

This project defines the public, contract-only builder and application lifecycle seam for the NatGateway area.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`INatGatewayApplication`](i-nat-gateway-application.md)** — Documented public type.
- **[`INatGatewayApplicationBuilder`](i-nat-gateway-application-builder.md)** — Documented public type.

## Summary

This project defines the public, contract-only builder and application lifecycle seam for the
NatGateway area. The implementation and creation entry point live in
`Assimalign.Cohesion.NatGateway.Hosting`.

## Public surface

- **`INatGatewayApplicationBuilder`** — owns area declarations and builds an `INatGatewayApplication`.
- **`INatGatewayApplication`** — exposes `Context`, `StartAsync`, and `StopAsync`.

The current application is a composition-only filler that is empty by default. Caller-registered
services participate in the shared ordered lifecycle; NAT-gateway behavior remains outside this
slice.

## Hosting-free application contract (O34)

The root contracts are hosting-free (O34): `INatGatewayApplication` exposes `Context`, `StartAsync`
, and `StopAsync`; `INatGatewayApplicationContext` exposes `ContentRootPath`.
`INatGatewayApplicationBuilder` exposes `Build()`; it currently declares no area-specific verbs.
The root and feature packages reference no `Assimalign.Cohesion.Hosting*` library; `COHRES004`
enforces the boundary.

`NatGatewayApplication.CreateBuilder(args)` returns the public concrete
`NatGatewayApplicationBuilder`; its `Build()` returns the public
`NatGatewayApplication : Host<NatGatewayApplicationContext>`. The public
`NatGatewayApplicationContext` implements `INatGatewayApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

Background-work registration (`AddService`) is available only on the concrete Hosting builder.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Core` | `CohesionProjectReference` |

[Parent: NatGateway](../index.md)

## Sources

- **Primary source** — `cohesion/resources/NatGateway/Assimalign.Cohesion.NatGateway/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/NatGateway/Assimalign.Cohesion.NatGateway/src/Assimalign.Cohesion.NatGateway.csproj`.
