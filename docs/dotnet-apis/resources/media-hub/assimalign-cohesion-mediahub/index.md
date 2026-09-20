# Assimalign.Cohesion.MediaHub

This project defines the public, contract-only builder and application lifecycle seam for the MediaHub area.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`IMediaHubApplication`](i-media-hub-application.md)** — Documented public type.
- **[`IMediaHubApplicationBuilder`](i-media-hub-application-builder.md)** — Documented public type.

## Summary

This project defines the public, contract-only builder and application lifecycle seam for the
MediaHub area. The implementation and creation entry point live in
`Assimalign.Cohesion.MediaHub.Hosting`.

## Public surface

- **`IMediaHubApplicationBuilder`** — owns area declarations and builds an `IMediaHubApplication`.
- **`IMediaHubApplication`** — exposes `Context`, `StartAsync`, and `StopAsync`.

The current application is a composition-only filler that is empty by default. Caller-registered
services participate in the shared ordered lifecycle; media-hub behavior remains outside this slice.

## Hosting-free application contract (O34)

The root contracts are hosting-free (O34): `IMediaHubApplication` exposes `Context`, `StartAsync`,
and `StopAsync`; `IMediaHubApplicationContext` exposes `ContentRootPath`.
`IMediaHubApplicationBuilder` exposes `Build()`; it currently declares no area-specific verbs. The
root and feature packages reference no `Assimalign.Cohesion.Hosting*` library; `COHRES004` enforces
the boundary.

`MediaHubApplication.CreateBuilder(args)` returns the public concrete `MediaHubApplicationBuilder`;
its `Build()` returns the public `MediaHubApplication : Host<MediaHubApplicationContext>`. The
public `MediaHubApplicationContext` implements `IMediaHubApplicationContext`, reading
`ContentRootPath` from the host environment. The application explicitly forwards the root lifecycle
contract to `IHost`, and consumers use the concrete application for `RunAsync` and `await using`.
Runtime options and supporting services remain internal.

Background-work registration (`AddService`) is available only on the concrete Hosting builder.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Core` | `CohesionProjectReference` |

[Parent: MediaHub](../index.md)

## Sources

- **Primary source** — `cohesion/resources/MediaHub/Assimalign.Cohesion.MediaHub/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/MediaHub/Assimalign.Cohesion.MediaHub/src/Assimalign.Cohesion.MediaHub.csproj`.
