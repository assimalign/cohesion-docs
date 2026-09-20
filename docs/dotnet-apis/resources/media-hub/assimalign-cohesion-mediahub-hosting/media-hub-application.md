# MediaHubApplication

The `MediaHubApplication` type belongs to `Assimalign.Cohesion.MediaHub.Hosting`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.MediaHub.Hosting` Assembly: `Assimalign.Cohesion.MediaHub.Hosting`

## Purpose

`MediaHubApplication` is the public concrete application and creation entry point for the MediaHub
hosting module. The application, builder, and context are public; runtime options remain internal.

## Factory behavior

- **`CreateBuilder(string[] args)`** — validates the argument array and returns a `MediaHubApplicationBuilder`.
- **The entry assembly selects** — the generated control-plane registration. Enabled builders capture ResourceRuntime.Current; command-line arguments remain available for future domain composition.
- **Building the returned builder** — materializes its registered service factories once against the new MediaHub context and preserves registration order; an enabled resource also registers its private control-plane listener when its ambient endpoint is present.

## Exceptions

`CreateBuilder` throws `ArgumentNullException` when `args` is `null`.

## Usage

See the [source-backed usage examples](examples/index.md).

`RunAsync` delegates to the shared host runner. Namespaced management authenticates gateway-issued
ES256 tokens; bare probes remain public. Without a registration the host creates no listener.
Invalid managed identity or endpoint configuration fails during `Build`().

## Concrete composition (T10 / O34)

`MediaHubApplication.CreateBuilder(args)` returns the public concrete `MediaHubApplicationBuilder`;
its `Build()` returns the public `MediaHubApplication : Host<MediaHubApplicationContext>`. The
public `MediaHubApplicationContext` implements `IMediaHubApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

Background-work registration belongs to the concrete `MediaHubApplicationBuilder`:
`AddService(IHostService)` and `AddService(Func<MediaHubApplicationContext, IHostService>)`. The
factory receives the same concrete context as Web's and `Database`'s `AddService`, so hosting consumers
can use environment, state, and hosted-service members beyond the small root contract. Factories run
once per build against the same context retained by the application; the hosted-service snapshot is
installed after factory evaluation. Services start in registration order and stop in reverse. No
area-owned service abstraction is introduced.

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/MediaHub/Assimalign.Cohesion.MediaHub.Hosting/docs/Assembly/Assimalign.Cohesion.MediaHub.Hosting/MediaHubApplication/OVERVIEW.md`.
