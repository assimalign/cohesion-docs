# IdentityHubResourceOptions

The `IdentityHubResourceOptions` type is part of the documented `Assimalign.Cohesion.IdentityHub.ApplicationModel` API.

> **Status:** Partial.

Provides deployer-owned planning overrides for an `IdentityHubResource`.

Namespace: `Assimalign.Cohesion.IdentityHub.ApplicationModel`.

## Documented behavior

- **`IdentityHubResource`** — — manifest-backed planned resource.
- **`IdentityHubResourceOptions`** — — replica/storage option carrier; the planner currently
  permits one replica and supports `Storage.Size`.
- **`IdentityHubResourceExtensions.AddIdentityHub(...)`** — — adds the typed resource to an
  application graph.
- **`IdentityHubResourceControlPlane.Create()`** — — creates an isolated default control plane
  accepting `identityhub.add-audience` and `identityhub.add-client`.
- **`IIdentityHubResourceDescriptor`** — retains typed resource and command/dependency surfaces.
- **`IdentityHubResourceCommandExtensions`** — supplies `AddAudience` and `AddClient` declarations.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.IdentityHub.ApplicationModel`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.ApplicationModel/docs/Assembly/Assimalign.Cohesion.IdentityHub.ApplicationModel/OVERVIEW.md`.
- **Source** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.ApplicationModel/src/IdentityHubResourceOptions.cs`.
