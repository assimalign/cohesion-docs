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
executable profile; recognized excluded constructs produce CK00001. NULLS, FIRST and LAST
are contextual words outside Keywords; first and last remain usable variable names.

The static accessor introduces no validation exceptions. GqlQueryParser returns diagnostics for
malformed text, including CG00002 for malformed conditional/composite syntax and CG00006 for
duplicate map keys. Semantic checks belong to the engine, including CG03003 parameter bounds,
CG02013 evaluated value bounds and CG07016 for unnegotiated possible composite wire results.
See [GQL diagnostics](../../../../database/graph/language/diagnostics.md).

## Example

```csharp
Console.WriteLine(GqlLanguageProfile.Instance.Supports(GqlClauses.Case)); // True
Console.WriteLine(GqlLanguageProfile.Instance.Functions.Contains("substring")); // True
var parsed = new GqlQueryParser().Parse("RETURN CASE 42 WHEN 17, 42 THEN 1 END");
Console.WriteLine(parsed.Diagnostics.Count); // 0
```

The RETURN executes as one Int64 value, 1.

GqlClauses.Set, GqlClauses.Remove and GqlClauses.PathMode are enabled.
They admit property/map/label updates through [GqlSetStage](gql-set-stage.md) and
[GqlRemoveStage](gql-remove-stage.md); MERGE remains CK00001.

## Related reference

[Assembly](index.md) · [GqlCaseExpression](gql-case-expression.md) · [GqlCoalesceExpression](gql-coalesce-expression.md) · [GqlNullIfExpression](gql-null-if-expression.md) · [Design](design.md)

## Path modes and arrows

GqlClauses.PathMode admits outer WALK/TRAIL/SIMPLE/ACYCLIC. Omitted mode resolves to Trail.
Instance.RecognizeLeftArrow and Instance.ToLexerOptions().RecognizeLeftArrow are true,
retaining GQL's incoming arrow; SQL/OQL leave this option false.

```csharp
Console.WriteLine(GqlLanguageProfile.Instance.Supports(GqlClauses.PathMode)); // True
Console.WriteLine(GqlLanguageProfile.Instance.RecognizeLeftArrow); // True
```

See [GqlPathMode](gql-path-mode.md) for mode scope and refusal codes.

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlLanguageProfile.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlLanguageProfileTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphValueTests.cs`.
