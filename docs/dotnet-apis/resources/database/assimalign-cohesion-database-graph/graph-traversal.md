# GraphTraversal

The `GraphTraversal` type is part of the documented `Assimalign.Cohesion.Database.Graph` API.

> **Status:** Partial.

A traversal specification: where to start, which relationships to follow, in which direction, and
how deep.

Namespace: `Assimalign.Cohesion.Database.Graph`.

## Documented behavior

- **`GraphTraversal`** — selects a start, direction, optional type and maximum depth. `TraverseAsync`
  yields distinct visited nodes, excluding the start.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/Assembly/Assimalign.Cohesion.Database.Graph/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/GraphTraversal.cs`.
