# IRezolvrCommandClient

The `IRezolvrCommandClient` type belongs to `Assimalign.Cohesion.Rezolvr.Client`.

> **Status:** Partial.

Disposable command protocol contract. `SendCommandAsync(ResourceCommand, CancellationToken)` and
`DeleteCommandAsync(ResourceCommand, CancellationToken)` return a ValueTask of
`ResourceCommandObservation`. Cancellation and transport failures propagate; server refusals retain
their status and detail. The owner and key are passed through unchanged.

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/Rezolvr/Assimalign.Cohesion.Rezolvr.Client/docs/Assembly/Assimalign.Cohesion.Rezolvr.Client/IRezolvrCommandClient/OVERVIEW.md`.
