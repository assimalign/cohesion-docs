# ResourceCommandObservation

The `ResourceCommandObservation` type belongs to `Assimalign.Cohesion.SecretStore.Client`.

> **Status:** Partial.

Immutable value with Status and nullable Detail. `ObserveCommandAsync` returns Applied for a
successful POST; `DeleteCommandAsync` returns Deleted for a successful DELETE. Rejected carries the
endpoint's JSON refusal detail, or a named HTTP detail for a legacy empty response. The type
introduces no dependency on Hosting.Resources or another area client.

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/SecretStore/Assimalign.Cohesion.SecretStore.Client/docs/Assembly/Assimalign.Cohesion.SecretStore.Client/ResourceCommandObservation/OVERVIEW.md`.
