# GqlProjection

The `GqlProjection` type is part of the documented `Assimalign.Cohesion.Database.Graph.Language` API.

> **Status:** Partial.

A projected graph element or one of its scalar properties.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Documented behavior

`GqlQueryExpression` groups finite match paths, a predicate, insertions, deletion variables and
projections. `GqlPathPattern` holds ordered nodes and relationships; their immutable metadata
captures labels, direction, type and scalar properties. `GqlProjection` selects a bound element or
property with an optional alias. Literal, property and binary expression classes describe the
bounded comparison grammar. Programmatically constructed ASTs receive planner validation too.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph.Language`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/Assembly/Assimalign.Cohesion.Database.Graph.Language/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlProjection.cs`.
