# IGraphSchema

The `IGraphSchema` type is part of the documented `Assimalign.Cohesion.Database.Graph` API.

> **Status:** Partial.

`Database`-scoped label, relationship-type and index management bound to one session.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Documented behavior

- **`GraphSchema.Open(database, session)`** — returns `IGraphSchema` for label/type discovery, definition
  and property changes, and node-property index creation. It rejects foreign sessions.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/Assembly/Assimalign.Cohesion.Database.Graph/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Abstractions/IGraphSchema.cs`.
