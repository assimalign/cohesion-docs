# IEmailHubApplication

The `IEmailHubApplication` type belongs to `Assimalign.Cohesion.EmailHub`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.EmailHub` Assembly: `Assimalign.Cohesion.EmailHub`

## Purpose and surface

The root contracts are hosting-free (O34): `IEmailHubApplication` exposes `Context`, `StartAsync`,
and `StopAsync`; `IEmailHubApplicationContext` exposes `ContentRootPath`.
`IEmailHubApplicationBuilder` exposes `Build()`; it currently declares no area-specific verbs. The
root and feature packages reference no `Assimalign.Cohesion.Hosting*` library; `COHRES004` enforces
the boundary.

`StartAsync(CancellationToken)` starts the application; `StopAsync(CancellationToken)` drains and
stops it. Lifecycle failures propagate to the caller. The contract carries no host identity, runner,
or disposal members.

## Hosting implementation

`EmailHubApplication.CreateBuilder(args)` returns the public concrete `EmailHubApplicationBuilder`;
its `Build()` returns the public `EmailHubApplication : Host<EmailHubApplicationContext>`. The
public `EmailHubApplicationContext` implements `IEmailHubApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

## Usage

See the [source-backed usage examples](examples/index.md).

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/EmailHub/Assimalign.Cohesion.EmailHub/docs/Assembly/Assimalign.Cohesion.EmailHub/IEmailHubApplication/OVERVIEW.md`.
