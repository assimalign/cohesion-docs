# SecretStoreResourceControlPlane

The `SecretStoreResourceControlPlane` type belongs to `Assimalign.Cohesion.SecretStore.ApplicationModel`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.SecretStore.ApplicationModel`

Assembly: `Assimalign.Cohesion.SecretStore.ApplicationModel`

## Purpose

`SecretStoreResourceControlPlane` supplies the default-control-plane factory used by enabled
secret-store executables.

## `Create`

See the [source-backed usage examples](examples/index.md).

Every call returns a fresh, isolated plane accepting `cohesion.trust.add`, `secretstore.add-secret`
, and `secretstore.issue-certificate`. The plane aggregates health, readiness, and liveness,
records observed endpoints, and carries graceful-stop and command operations.

Generated `ResourceControlPlane.g.cs` registers the factory with `ResourceRuntime` and observes the
invocation's endpoints before `SecretStore.Hosting` serves the standard control-plane and
store-protocol routes under `/cohesion/v1` on `api`.

The factory does not host HTTP, persist secrets, verify credentials, or issue certificates. Those
are runtime responsibilities. `SecretStoreResourceCommandExtensions` declares the two desired-state
kinds. The manifest omits cohesion.trust.add because it is a gateway-owned trust channel, not an
application declaration. Enroll remains deferred to item 31t.

## Links

- **[Assembly** — overview](index.md)
- **[Project** — design](design.md)

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.ApplicationModel/docs/Assembly/Assimalign.Cohesion.SecretStore.ApplicationModel/SecretStoreResourceControlPlane/OVERVIEW.md`.
