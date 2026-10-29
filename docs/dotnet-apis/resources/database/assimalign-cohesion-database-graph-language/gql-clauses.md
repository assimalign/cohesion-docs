# GqlClauses

The `GqlClauses` type is part of the documented `Assimalign.Cohesion.Database.Graph.Language` API.

> **Status:** Partial.

The clause names a GQL `Assimalign.Cohesion.Database.Language.QueryLanguageProfile` may enable.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Changed member

```csharp
public static class GqlClauses
{
    public const string QuantifiedPath = "QUANTIFIED PATH";
    public const string PathMode = "PATH MODE";
    public const string PathSelector = "PATH SELECTOR";
    public const string GroupBy = "GROUP BY";
    public const string Let = "LET";
    public const string Filter = "FILTER";
    public const string For = "FOR";
    public const string Unwind = "UNWIND";
    public const string With = "WITH";
    public const string Next = "NEXT";
    public const string SetOperation = "UNION";
    public const string UnionDistinct = "UNION DISTINCT";
    public const string UnionAll = "UNION ALL";
    public const string Set = "SET";
    public const string Remove = "REMOVE";
    public const string Merge = "MERGE";
    public const string ListComprehension = "LIST COMPREHENSION";
    public const string ListSlice = "LIST SLICING";
    public const string Reduce = "REDUCE";
    public const string ListPredicate = "LIST PREDICATE";
    public const string Exists = "EXISTS";
    public const string PatternComprehension = "PATTERN COMPREHENSION";
}
```

GroupBy is enabled by GqlLanguageProfile.Instance. It admits explicit ISO RETURN names/expressions
and GROUP BY (); implicit grouping uses nonaggregate RETURN keys when aggregates are present.
The constant adds no exceptions. Malformed syntax uses CG00002 and invalid grouped scope uses
CG05001. These grouping codes have no assigned SQLSTATE.

```csharp
Console.WriteLine(GqlLanguageProfile.Instance.Supports(GqlClauses.GroupBy)); // True
```

See [GROUP BY](../../../../database/graph/language/clauses/group-by.md) for result examples and
[GqlQueryExpression.GroupBy](gql-query-expression.md) for the parsed key list.

The six composition names are enabled in GqlLanguageProfile.Instance. FOR/UNWIND share
GqlForStage; WITH/RETURN share GqlReturnStage. NEXT marks a segment boundary. The constants
add no exceptions. [Composition](../../../../database/graph/language/clauses/next.md) documents
scope, NULL handling and diagnostics; standalone ordering/page stages remain CK00001.

```csharp
Console.WriteLine(GqlLanguageProfile.Instance.Supports(GqlClauses.Let)); // True
Console.WriteLine(GqlLanguageProfile.Instance.Supports(GqlClauses.Unwind)); // True
Console.WriteLine(GqlLanguageProfile.Instance.Supports(GqlClauses.Next)); // True
```

QuantifiedPath enables [quantified edges and subpaths](../../../../database/graph/language/patterns.md).
The constant adds no exceptions; `Instance.Supports(GqlClauses.QuantifiedPath)` returns true.

Set and Remove are enabled in GqlLanguageProfile.Instance. They name ordered update stages
following an incoming clause; leading SET/REMOVE remains CG00002. The constants add no exceptions.
See [SET](../../../../database/graph/language/clauses/set.md) and [REMOVE](../../../../database/graph/language/clauses/remove.md).

Merge is enabled for one whole fixed path and repeated ON CREATE/ON MATCH SET clauses. The
constant adds no exceptions. Parsing produces [GqlMergeStage](gql-merge-stage.md); execution and
diagnostics are described by [MERGE](../../../../database/graph/language/statements/merge.md).

```csharp
Console.WriteLine(GqlLanguageProfile.Instance.Supports(GqlClauses.Merge)); // True
```

Exists is enabled for braced pattern blocks, represented by
[GqlExistsExpression](gql-exists-expression.md). Full-query bodies remain CK00001.

## Documented behavior

Reduce and ListPredicate are enabled by GqlLanguageProfile.Instance and add no exceptions.
They parse into [GqlReduceExpression](gql-reduce-expression.md) and
[GqlListPredicateExpression](gql-list-predicate-expression.md), with
[GqlListPredicateKind](gql-list-predicate-kind.md) selecting all/any/none/single.

```csharp
Console.WriteLine(GqlLanguageProfile.Instance.Supports(GqlClauses.Reduce)); // True
Console.WriteLine(GqlLanguageProfile.Instance.Supports(GqlClauses.ListPredicate)); // True
```

`GqlQueryParser` derives from the shared `QueryParser` and returns `GqlQueryStatement`, whose
diagnostics describe malformed or unsupported input. `GqlLanguageProfile.Instance` advertises only
executable clauses. `GqlClauses` names lexical capability keys, including deferred clauses.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph.Language`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## PathMode

PathMode is enabled in GqlLanguageProfile.Instance and adds no accessor exceptions. It names
the outer [WALK/TRAIL/SIMPLE/ACYCLIC](gql-path-mode.md) capability. Nested modes and unselected
unbounded WALK even with LIMIT still use CK00001. PathSelector is enabled and admits
[selectors](gql-path-selector.md) and inline WHERE execution under statement budgets.

```csharp
Console.WriteLine(GqlLanguageProfile.Instance.Supports(GqlClauses.PathMode)); // True
```

PatternComprehension is enabled by GqlLanguageProfile.Instance and produces
[GqlPatternComprehensionExpression](gql-pattern-comprehension-expression.md). Reading the constant
throws no exceptions. `Instance.Supports(GqlClauses.PatternComprehension)` returns true.

SetOperation, UnionDistinct and UnionAll are enabled. They admit read-only
[composite queries](gql-composite-query.md), including NEXT pipelines, with matching columns
and shared work/memory budgets. Reading the constants adds no exceptions.

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/Assembly/Assimalign.Cohesion.Database.Graph.Language/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlClauses.cs`.
