# IEventHubApplicationBuilder

The `IEventHubApplicationBuilder` type belongs to `Assimalign.Cohesion.EventHub`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.EventHub` Assembly: `Assimalign.Cohesion.EventHub`

## Purpose

`IEventHubApplicationBuilder` is the public composition seam for an event hub application. It
exposes `Build()` returning `IEventHubApplication`.

## Surface and behavior

- **`Build()`** — creates a configured event hub application.

The current filler builder has no area feature or service registrations by default. Services
registered on the concrete Hosting builder start in registration order and stop in reverse order.
The public concrete `EventHubApplicationBuilder` lives in `Assimalign.Cohesion.EventHub.Hosting`.

## Exceptions

The concrete Hosting builder's `AddService` throws `ArgumentNullException` for a null service or
factory. `Build()` throws `InvalidOperationException` when a service factory returns null.

## Usage

See the [source-backed usage examples](examples/index.md).

## Hosting-free application contract (O34)

The root contracts are hosting-free (O34): `IEventHubApplication` exposes `Context`, `StartAsync`,
and `StopAsync`; `IEventHubApplicationContext` exposes `ContentRootPath`.
`IEventHubApplicationBuilder` exposes `Build()`; it currently declares no area-specific verbs. The
root and feature packages reference no `Assimalign.Cohesion.Hosting*` library; `COHRES004` enforces
the boundary.

`EventHubApplication.CreateBuilder(args)` returns the public concrete `EventHubApplicationBuilder`;
its `Build()` returns the public `EventHubApplication : Host<EventHubApplicationContext>`. The
public `EventHubApplicationContext` implements `IEventHubApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

Background-work registration (`AddService`) is available only on the concrete Hosting builder.

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/EventHub/Assimalign.Cohesion.EventHub/docs/Assembly/Assimalign.Cohesion.EventHub/IEventHubApplicationBuilder/OVERVIEW.md`.
