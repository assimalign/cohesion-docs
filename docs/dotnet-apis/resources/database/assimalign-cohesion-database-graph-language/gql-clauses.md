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
    public const string GroupBy = "GROUP BY";
    public const string Let = "LET";
    public const string Filter = "FILTER";
    public const string For = "FOR";
    public const string Unwind = "UNWIND";
    public const string With = "WITH";
    public const string Next = "NEXT";
    public const string Set = "SET";
    public const string Remove = "REMOVE";
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

## Documented behavior

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
unbounded WALK even with LIMIT still use CK00001. [Selectors](gql-path-selector.md) bind and EXPLAIN
but execution remains CK00001; they are not yet added to the executable-clause profile.

```csharp
Console.WriteLine(GqlLanguageProfile.Instance.Supports(GqlClauses.PathMode)); // True
```

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/Assembly/Assimalign.Cohesion.Database.Graph.Language/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlClauses.cs`.
