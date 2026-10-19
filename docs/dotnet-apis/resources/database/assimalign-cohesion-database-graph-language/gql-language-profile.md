# GqlLanguageProfile

Declares the executable GQL clause set and lexical vocabulary.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public static class GqlLanguageProfile
{
    public static QueryLanguageProfile Instance { get; }
}
```

## Behavior and exceptions

Instance is the shared immutable GQL profile. Its clauses advertise CASE alongside matching,
projection, ORDER BY, OFFSET/SKIP, LIMIT, mutations, SHOW, EXPLAIN and label expressions. CASE/WHEN/THEN/ELSE/END and
COALESCE/NULLIF are keywords; the function table contains count, sum, avg, min, max, collect_list
and collect, plus core scalar signatures/aliases and cast. GqlClauses.QuantifiedPath enables
quantified edges/subpaths. GqlClauses.GroupBy advertises ISO GROUP BY. COALESCE and NULLIF parse into
case-abbreviation nodes, without registry function calls. Reserved vocabulary is larger than the
executable profile; recognized excluded constructs produce COHDBL001. NULLS, FIRST and LAST
are contextual words outside Keywords; first and last remain usable variable names.

The static accessor introduces no validation exceptions. GqlQueryParser returns diagnostics for
malformed text, including GQL0002 for malformed conditional/composite syntax and GQL0006 for
duplicate map keys. Semantic checks belong to the engine, including COHDBG003 parameter bounds,
COHDBG013 evaluated value bounds and COHDBG016 for unnegotiated possible composite wire results.
See [GQL diagnostics](../../../../database/graph/language/diagnostics.md).

## Example

```csharp
Console.WriteLine(GqlLanguageProfile.Instance.Supports(GqlClauses.Case)); // True
Console.WriteLine(GqlLanguageProfile.Instance.Functions.Contains("substring")); // True
var parsed = new GqlQueryParser().Parse("RETURN CASE 42 WHEN 17, 42 THEN 1 END");
Console.WriteLine(parsed.Diagnostics.Count); // 0
```

The RETURN executes as one Int64 value, 1.

GqlClauses.Set and GqlClauses.Remove are enabled, bringing execution to 26 of 33 declared clauses.
They admit property/map/label updates through [GqlSetStage](gql-set-stage.md) and
[GqlRemoveStage](gql-remove-stage.md); MERGE remains COHDBL001.

## Related reference

[Assembly](index.md) · [GqlCaseExpression](gql-case-expression.md) · [GqlCoalesceExpression](gql-coalesce-expression.md) · [GqlNullIfExpression](gql-null-if-expression.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlLanguageProfile.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlLanguageProfileTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphValueTests.cs`.
