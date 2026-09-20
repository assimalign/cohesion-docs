# ApiManagerResource

The `ApiManagerResource` type is part of the documented `Assimalign.Cohesion.ApiManager.ApplicationModel` API.

> **Status:** Partial.

Represents a manifest-backed ApiManager workload in an application graph.

Namespace: `Assimalign.Cohesion.ApiManager.ApplicationModel`.

## Documented behavior

`ApiManagerResource` inherits `PlannedResource`; `ApiManagerResourceOptions` inherits
`ResourceOptions`. `IApiManagerResourceDescriptor` exposes the typed resource and ordinary
dependency edges. `AddApiManager` extends `IApplicationBuilder`;
`ApiManagerResourceControlPlane.Create` returns an isolated `IResourceControlPlane` with no command
kinds.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.ApiManager.ApplicationModel`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/ApiManager/Assimalign.Cohesion.ApiManager.ApplicationModel/docs/Assembly/Assimalign.Cohesion.ApiManager.ApplicationModel/OVERVIEW.md`.
- **Source** — `cohesion/resources/ApiManager/Assimalign.Cohesion.ApiManager.ApplicationModel/src/ApiManagerResource.cs`.
