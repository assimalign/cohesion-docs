# GqlPathPattern

The `GqlPathPattern` type is part of the documented `Assimalign.Cohesion.Database.Graph.Language` API.

> **Status:** Partial.

A finite chain of nodes joined by individual relationship patterns.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Documented behavior

`GqlQueryExpression` groups finite match paths, a predicate, insertions, deletion variables and
projections. `GqlPathPattern` holds ordered nodes and relationships; their immutable metadata
captures labels, direction, type and scalar properties. [GqlProjection](gql-projection.md) holds a
value expression and optional alias; [GqlVariableExpression](gql-variable-expression.md) represents
a bare binding. Typed logical/unary, IN/list, null/truth, string and parameter nodes describe WHERE and RETURN
expressions. Property maps retain GqlExpression values. Programmatically constructed ASTs receive planner validation too.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph.Language`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/Assembly/Assimalign.Cohesion.Database.Graph.Language/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlPathPattern.cs`.
