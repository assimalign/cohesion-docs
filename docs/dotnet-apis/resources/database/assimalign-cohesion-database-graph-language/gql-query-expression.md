# GqlQueryExpression

The `GqlQueryExpression` type is part of the documented `Assimalign.Cohesion.Database.Graph.Language` API.

> **Status:** Partial.

A finite graph query or read-only catalog statement in one database.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Documented behavior

`GqlQueryExpression` groups finite match paths, a predicate, insertions, deletion variables and
projections. `GqlPathPattern` holds ordered nodes and relationships; their immutable metadata
captures labels, direction, type and scalar properties. `GqlProjection` selects a bound element or
property with an optional alias. Typed logical/unary, IN/list, null/truth, string and parameter nodes describe WHERE
expressions. Property maps retain GqlExpression values. Programmatically constructed ASTs receive planner validation too.

## Changed member

```csharp
public bool IsExplain { get; private set; }
```

The parser sets this for EXPLAIN; callers cannot set it directly. Planning returns the shared
eight-column description without executing the data statement.

```csharp
var statement = (GqlQueryStatement)new GqlQueryParser().Parse("EXPLAIN MATCH (n) RETURN n");
Console.WriteLine(statement.GqlExpression.IsExplain); // True
```

Normal binding errors retain COHDBG001 and parser errors remain on statement.Diagnostics.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph.Language`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/Assembly/Assimalign.Cohesion.Database.Graph.Language/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlQueryExpression.cs`.
