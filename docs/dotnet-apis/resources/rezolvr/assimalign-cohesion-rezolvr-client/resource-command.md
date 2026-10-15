# ResourceCommand

The `ResourceCommand` type belongs to `Assimalign.Cohesion.Rezolvr.Client`.

> **Status:** Partial.

Immutable transport value constructed from `id`, `kind`, `owner`, `key`, and payload bytes. Id,
Kind, Owner and `Key` must be nonblank; invalid values throw argument exceptions. Payload is sent as
base64 in the HTTP envelope. It represents the area command payload, not a JSON manifest command
entry. The manifest always advertises bare string kinds.

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/Rezolvr/Assimalign.Cohesion.Rezolvr.Client/docs/Assembly/Assimalign.Cohesion.Rezolvr.Client/ResourceCommand/OVERVIEW.md`.
