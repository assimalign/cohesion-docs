# GqlQueryExpression

A finite graph query or read-only catalog statement in one database.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlQueryExpression : GqlExpression
{
    public GqlQueryExpression(IReadOnlyList<GqlPathPattern> matches, GqlExpression? predicate,
        IReadOnlyList<GqlPathPattern> creates, IReadOnlyList<string> deleteVariables,
        bool detachDelete, IReadOnlyList<GqlProjection> projections, Location? location = null);
    public GqlQueryExpression(IReadOnlyList<GqlMatchClause> matchClauses,
        IReadOnlyList<GqlPathPattern> creates, IReadOnlyList<string> deleteVariables,
        bool detachDelete, IReadOnlyList<GqlProjection> projections, Location? location = null);
    public IReadOnlyList<GqlMatchClause> MatchClauses { get; }
    public IReadOnlyList<GqlPathPattern> Matches { get; }
    public GqlExpression? Predicate { get; }
    public IReadOnlyList<GqlPathPattern> Creates { get; }
    public IReadOnlyList<string> DeleteVariables { get; }
    public bool DetachDelete { get; }
    public IReadOnlyList<GqlProjection> Projections { get; }
    public GqlSetQuantifier SetQuantifier { get; init; }
    public bool ReturnStar { get; init; }
    public bool IsExplain { get; private set; }
    public GqlCatalogSurface? CatalogSurface { get; init; }
}
```

## Behavior and exceptions

[MatchClauses](gql-match-clause.md) preserves source order and each clause's predicate and optional
flag. The original constructor wraps its matches/predicate in one mandatory clause, except an empty
match list with no predicate creates no clauses. Matches and Predicate expose data only for exactly
one mandatory clause; otherwise they return an empty list and null. The planner consumes MatchClauses.
Malformed clause lists or forward predicate references fail with COHDBG001.

SetQuantifier defaults to [All](gql-set-quantifier.md). ReturnStar is separate from Projections:
star requires an empty explicit list and at least one named binding. Expansion follows first
declarations, including a named path before its elements, skipping anonymous/repeated references.
Projections now hold [value expressions](gql-projection.md), so empty Matches/Creates with
projections represents a standalone RETURN evaluated once.

Construction stores the supplied values; validation belongs to the planner. Invalid quantifiers,
star shapes or DISTINCT without items, unbound names/parameters, and SHOW composed with return
flags fail COHDBG001 before execution. Duplicate final names use COHDBG015 (no SQLSTATE), even
under EXPLAIN or on empty matches. Path requests reject invalid projection shapes with COHDBG001
first. DISTINCT compares complete projected tuples using exact numeric values, ordinal strings,
NULL grouping and entity/path identities; arithmetic promotion is unchanged. Mutations run for
every binding before output deduplication.

The parser sets IsExplain; callers cannot set it directly. Planning returns the shared eight-column
description without data execution. Parser errors remain on statement.Diagnostics.

## Example

```csharp
var statement = (GqlQueryStatement)new GqlQueryParser().Parse(
    "EXPLAIN RETURN DISTINCT 1 + 1 AS two");
var query = statement.GqlExpression;
Console.WriteLine(query.IsExplain); // True
Console.WriteLine(query.SetQuantifier); // Distinct
Console.WriteLine(query.Projections[0].Alias); // two
```

Execution describes a Project named `two` and a Distinct operator without evaluating data.
See [GqlProjection](gql-projection.md#example) for a source-free standalone AST execution.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph.Language`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/Assembly/Assimalign.Cohesion.Database.Graph.Language/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlQueryExpression.cs`.
- **Execution and validation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphReturnTests.cs`.
