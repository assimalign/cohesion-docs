# Assimalign.Cohesion.IdentityHub.Client

Create an `IIdentityHubCommandClient` with `IdentityHubCommandClient.Create` and dispose it when command delivery finishes. `SendCommandAsync` applies a declaration; `DeleteCommandAsync` removes it.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`IdentityHubCommandClient`](identity-hub-command-client.md)** — Documented public type.
- **[`IIdentityHubCommandClient`](i-identity-hub-command-client.md)** — Documented public type.
- **[`ResourceCommand`](resource-command.md)** — Documented public type.
- **[`ResourceCommandObservation`](resource-command-observation.md)** — Documented public type.

`Create` an `IIdentityHubCommandClient` with `IdentityHubCommandClient.Create` and dispose it when
command delivery finishes. `SendCommandAsync` applies a declaration; `DeleteCommandAsync` removes
it. Both accept a package-local `ResourceCommand` and return a `ResourceCommandObservation`.

The observation preserves `Status` and `Detail`. `Applied` and `Deleted` are successful outcomes;
`Rejected` identifies an HTTP refusal. An empty successful handler response is still successful.
Network failures and cancellation propagate to the caller.

`Use` a full control-plane URI and an opaque bearer credential issued for the target resource. The
factory validates the URI synchronously; it does not establish a connection. The client does not
acquire tokens, resolve secret sources, or create declarations. Those are gateway responsibilities.

The only dependency is Core for endpoint validation. JSON envelopes use `Utf8JsonWriter` and
observations use `JsonDocument`, with no reflection-based serialization or Hosting dependency.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Core` | `CohesionProjectReference` |

[Parent: IdentityHub](../index.md)

## Sources

- **Primary source** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Client/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Client/src/Assimalign.Cohesion.IdentityHub.Client.csproj`.
