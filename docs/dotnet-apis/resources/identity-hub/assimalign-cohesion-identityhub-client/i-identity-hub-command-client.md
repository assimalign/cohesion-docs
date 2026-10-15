# IIdentityHubCommandClient

The `IIdentityHubCommandClient` type belongs to `Assimalign.Cohesion.IdentityHub.Client`.

> **Status:** Partial.

Disposable command protocol contract. `SendCommandAsync(ResourceCommand, CancellationToken)` and
`DeleteCommandAsync(ResourceCommand, CancellationToken)` return a ValueTask of
`ResourceCommandObservation`. Cancellation and transport failures propagate; server refusals retain
their status and detail. The owner and key are passed through unchanged.

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Client/docs/Assembly/Assimalign.Cohesion.IdentityHub.Client/IIdentityHubCommandClient/OVERVIEW.md`.
