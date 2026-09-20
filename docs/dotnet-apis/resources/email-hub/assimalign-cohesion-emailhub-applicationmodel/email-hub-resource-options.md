# EmailHubResourceOptions

The `EmailHubResourceOptions` type is part of the documented `Assimalign.Cohesion.EmailHub.ApplicationModel` API.

> **Status:** Partial.

Provides deployer-owned planning overrides for a EmailHub resource.

Namespace: `Assimalign.Cohesion.EmailHub.ApplicationModel`.

## Documented behavior

`EmailHubResource` inherits `PlannedResource`; `EmailHubResourceOptions` inherits `ResourceOptions`
. `IEmailHubResourceDescriptor` exposes the typed resource and ordinary dependency edges.
`AddEmailHub` extends `IApplicationBuilder`; `EmailHubResourceControlPlane.Create` returns an
isolated `IResourceControlPlane` with no command kinds.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.EmailHub.ApplicationModel`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/EmailHub/Assimalign.Cohesion.EmailHub.ApplicationModel/docs/Assembly/Assimalign.Cohesion.EmailHub.ApplicationModel/OVERVIEW.md`.
- **Source** — `cohesion/resources/EmailHub/Assimalign.Cohesion.EmailHub.ApplicationModel/src/EmailHubResourceOptions.cs`.
