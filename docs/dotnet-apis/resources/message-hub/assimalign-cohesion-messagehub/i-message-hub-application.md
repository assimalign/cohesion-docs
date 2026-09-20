# IMessageHubApplication

The `IMessageHubApplication` type belongs to `Assimalign.Cohesion.MessageHub`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.MessageHub` Assembly: `Assimalign.Cohesion.MessageHub`

## Purpose and surface

The root contracts are hosting-free (O34): `IMessageHubApplication` exposes `Context`, `StartAsync`
, and `StopAsync`; `IMessageHubApplicationContext` exposes `ContentRootPath`.
`IMessageHubApplicationBuilder` exposes `Build()`; it currently declares no area-specific verbs.
The root and feature packages reference no `Assimalign.Cohesion.Hosting*` library; `COHRES004`
enforces the boundary.

`StartAsync(CancellationToken)` starts the application; `StopAsync(CancellationToken)` drains and
stops it. Lifecycle failures propagate to the caller. The contract carries no host identity, runner,
or disposal members.

## Hosting implementation

`MessageHubApplication.CreateBuilder(args)` returns the public concrete
`MessageHubApplicationBuilder`; its `Build()` returns the public
`MessageHubApplication : Host<MessageHubApplicationContext>`. The public
`MessageHubApplicationContext` implements `IMessageHubApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

## Usage

See the [source-backed usage examples](examples/index.md).

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/MessageHub/Assimalign.Cohesion.MessageHub/docs/Assembly/Assimalign.Cohesion.MessageHub/IMessageHubApplication/OVERVIEW.md`.
