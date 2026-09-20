# RezolvrResourceOptions

The `RezolvrResourceOptions` type is part of the documented `Assimalign.Cohesion.Rezolvr.ApplicationModel` API.

> **Status:** Partial.

Provides deployer-owned planning overrides for a Rezolvr resource.

Namespace: `Assimalign.Cohesion.Rezolvr.ApplicationModel`.

## Documented behavior

`RezolvrResource` inherits `PlannedResource`; `RezolvrResourceOptions` inherits `ResourceOptions`.
`IRezolvrResourceDescriptor` exposes the typed resource, commands, and dependency edges.
`AddRezolvr` extends `IApplicationBuilder`; `RezolvrResourceControlPlane.Create` returns an
isolated `IResourceControlPlane` accepting `rezolvr.add-a-record` and `rezolvr.add-cname-record`.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Rezolvr.ApplicationModel`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Rezolvr/Assimalign.Cohesion.Rezolvr.ApplicationModel/docs/Assembly/Assimalign.Cohesion.Rezolvr.ApplicationModel/OVERVIEW.md`.
- **Source** — `cohesion/resources/Rezolvr/Assimalign.Cohesion.Rezolvr.ApplicationModel/src/RezolvrResourceOptions.cs`.
