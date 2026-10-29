# GqlQueryExpression

A finite graph query or read-only catalog statement in one database.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed class GqlQueryExpression : GqlExpression
{
    public GqlQueryExpression(IReadOnlyList<GqlStage> stages, Location? location = null);
    public GqlQueryExpression(GqlCompositeQuery composite, Location? location = null);
    public GqlCompositeQuery? Composite { get; }
    public override string ToString();
    public IReadOnlyList<GqlStage> Stages { get; }
    public IReadOnlyList<GqlSetItem> SetItems { get; }
    public IReadOnlyList<GqlRemoveItem> RemoveItems { get; }
    public IReadOnlyList<GqlQuerySegment> Segments { get; }
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
    public IReadOnlyList<GqlExpression>? GroupBy { get; init; }
    public IReadOnlyList<GqlSortItem> OrderBy { get; init; } // defaults to []
    public GqlExpression? Offset { get; init; }
    public GqlExpression? Limit { get; init; }
    public bool IsExplain { get; private set; }
    public GqlCatalogSurface? CatalogSurface { get; init; }
}
```

## Behavior and exceptions

For linear queries, Stages is the planner/executor input. The stage constructor rejects a null list with
ArgumentNullException, retains the supplied list, and initializes compatibility summaries:
MatchClauses collects match stages, Creates/DeleteVariables collect writes, and return flags/tails
come from a final GqlReturnStage. Supply per-stage tails on GqlReturnStage for explicit pipelines.
The original constructors expose a single-segment stage facade without changing call sites.
Segments splits at NEXT and excludes the boundaries. See [GqlStage](gql-stage.md) for all leaves.

The composite constructor throws ArgumentNullException for null and retains the binary
[GqlCompositeQuery](gql-composite-query.md). A standalone composite exposes an empty Stages list;
Composite is null for a linear pipeline. A composite/NEXT pipeline uses
[GqlCompositeStage](gql-composite-stage.md) among its stages. ToString preserves semantic grouping
and NEXT precedence. Operand column, shape and read-only checks occur during planning;
[Composite queries](../../../../database/graph/language/clauses/union.md) lists the diagnostics and execution rules.

Planning validates each incoming scope before execution. Missing mandatory stage operands and
invalid boundaries use CG05001; text leading/trailing NEXT or WITH immediately followed by NEXT
uses CG00002. LET/FOR rebinding and deleted-variable reuse use CG03003. Only aliased/bare-variable
projection names carry forward; duplicate carried names use CG01015. WITH WHERE runs after its
sort/page and has its own DISTINCT/grouping scope. All these codes have no assigned SQLSTATE.

[MatchClauses](gql-match-clause.md) preserves source order and each clause's predicate and optional
flag. The original constructor wraps its matches/predicate in one mandatory clause, except an empty
match list with no predicate creates no clauses. Matches and Predicate expose data only for exactly
one mandatory clause; otherwise they return an empty list and null. The planner consumes Stages.
Malformed clause lists or forward predicate references fail with CG05001.

SetQuantifier defaults to [All](gql-set-quantifier.md). ReturnStar is separate from Projections:
star requires an empty explicit list and at least one named binding. Expansion follows first
declarations, including a named path before its elements, skipping anonymous/repeated references.
Projections now hold [value expressions](gql-projection.md), so empty Matches/Creates with
projections represents a standalone RETURN evaluated once.

Construction stores the supplied values; validation belongs to the planner. Invalid quantifiers,
star shapes or DISTINCT without items, unbound names/parameters, and SHOW composed with return
flags fail CG05001 before execution. Duplicate final names use CG01015 (no SQLSTATE), even
under EXPLAIN or on empty matches. Path requests reject invalid projection shapes with CG05001
first. DISTINCT compares complete projected tuples using exact numeric values, ordinal strings,
NULL grouping and entity/path identities; arithmetic promotion is unchanged. Mutations run for
every binding before output deduplication.

[OrderBy](gql-sort-item.md) retains priority-ordered keys; Offset and Limit are literal or
parameter expressions, with null meaning no offset/limit clause. Text SKIP populates Offset.
Counts require integral CLR values from zero through Int64.MaxValue. Invalid bound counts use
CG02013 before reads/writes, unsupported/non-finite CLR inputs CG03003 and missing parameters
CG05001. These codes have no assigned SQLSTATE. Malformed sort shapes or hidden DISTINCT
references also use CG05001. Sorting follows DISTINCT, then offset and limit; it applies to
supported path returns too and never reduces mutations or match/candidate bounds.

The parser sets IsExplain; callers cannot set it directly. Planning returns the shared eight-column
description without data execution. Parser errors remain on statement.Diagnostics.

GroupBy is null for implicit grouping, an empty list for GROUP BY (), or explicit grouping
names/expressions. Plain names resolve to nonaggregate RETURN names first. Explicit keys must
match the distinct nonaggregate projection keys; mismatches and aggregate aliases use CG05001.
[GqlAggregateExpression](gql-aggregate-expression.md) holds aggregate calls. Grouping occurs before
projection/DISTINCT/order/page. ExecutePaths refuses final GroupBy/aggregate projections with
CG05001, but intermediate collect/UNWIND can carry proven entity kinds to one mandatory final
entity/path column. Any write stage excludes ExecutePaths. Entity lists cannot be final wire
results (CG05001; #1662), even with composite negotiation.

The explicit grouping capability is `GqlClauses.GroupBy` (`public const string GroupBy = "GROUP BY"`).

## Example

```csharp
var composed = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "LET x = 1 RETURN x NEXT LET y = x + 1 RETURN y")).GqlExpression;
Console.WriteLine(composed.Stages.Count); // 5
Console.WriteLine(composed.Segments.Count); // 2
```

Execution returns one row containing 2.

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

See [GqlSortItem](gql-sort-item.md#example) for AST inspection of directions, NULL placement
and parameterized paging.

```csharp
var grouped = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a) RETURN a.active AS active, count(*) AS n GROUP BY active")).GqlExpression;
Console.WriteLine(grouped.GroupBy!.Count); // 1
var emptyKeys = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a) RETURN count(*) GROUP BY ()")).GqlExpression;
Console.WriteLine(emptyKeys.GroupBy!.Count); // 0
```

SetItems and RemoveItems flatten their respective stages in source order into a fresh array on
each access. Execution consumes Stages, preserving interleaving with other clauses. See
[GqlSetStage](gql-set-stage.md), [GqlRemoveStage](gql-remove-stage.md) and their item variants.

```csharp
var updates = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a) SET a.age = 5 REMOVE a.old RETURN a.age")).GqlExpression;
Console.WriteLine(updates.SetItems.Count);    // 1
Console.WriteLine(updates.RemoveItems.Count); // 1
```

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph.Language`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/Assembly/Assimalign.Cohesion.Database.Graph.Language/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlQueryExpression.cs`.
- **Execution and validation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphReturnTests.cs`.
