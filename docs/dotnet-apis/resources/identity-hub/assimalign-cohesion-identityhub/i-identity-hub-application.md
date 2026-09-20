# IIdentityHubApplication

The `IIdentityHubApplication` type belongs to `Assimalign.Cohesion.IdentityHub`.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.IdentityHub` Assembly: `Assimalign.Cohesion.IdentityHub`

## Purpose and surface

The root contracts are hosting-free (O34): `IIdentityHubApplication` exposes `Context`,
`StartAsync`, and `StopAsync`; `IIdentityHubApplicationContext` exposes `ContentRootPath`.
`IIdentityHubApplicationBuilder` owns area declarations and `Build()`. The root and feature
packages reference no `Assimalign.Cohesion.Hosting*` library; `COHRES004` enforces the boundary.

`StartAsync(CancellationToken)` starts the application; `StopAsync(CancellationToken)` drains and
stops it. Lifecycle failures propagate to the caller. The contract carries no host identity, runner,
or disposal members.

## Hosting implementation

`IdentityHubApplication.CreateBuilder(args)` returns the public concrete
`IdentityHubApplicationBuilder`; its `Build()` returns the public
`IdentityHubApplication : Host<IdentityHubApplicationContext>`. The public
`IdentityHubApplicationContext` implements `IIdentityHubApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

## Usage

See the [source-backed usage examples](examples/index.md).

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub/docs/Assembly/Assimalign.Cohesion.IdentityHub/IIdentityHubApplication/OVERVIEW.md`.
