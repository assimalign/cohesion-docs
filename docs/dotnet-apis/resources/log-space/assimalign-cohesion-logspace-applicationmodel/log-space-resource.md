# LogSpaceResource

The `LogSpaceResource` type is part of the documented `Assimalign.Cohesion.LogSpace.ApplicationModel` API.

> **Status:** Partial.

Represents a manifest-backed LogSpace workload in an application graph.

Namespace: `Assimalign.Cohesion.LogSpace.ApplicationModel`.

## Documented behavior

`LogSpaceResource` inherits `PlannedResource`; `LogSpaceResourceOptions` inherits `ResourceOptions`
. `ILogSpaceResourceDescriptor` exposes the typed resource and ordinary dependency edges.
`AddLogSpace` extends `IApplicationBuilder`; `LogSpaceResourceControlPlane.Create` returns an
isolated `IResourceControlPlane` with no command kinds.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.LogSpace.ApplicationModel`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.ApplicationModel/docs/Assembly/Assimalign.Cohesion.LogSpace.ApplicationModel/OVERVIEW.md`.
- **Source** — `cohesion/resources/LogSpace/Assimalign.Cohesion.LogSpace.ApplicationModel/src/LogSpaceResource.cs`.
