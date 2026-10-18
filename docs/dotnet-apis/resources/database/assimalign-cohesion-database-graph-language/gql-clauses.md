# GqlClauses

The `GqlClauses` type is part of the documented `Assimalign.Cohesion.Database.Graph.Language` API.

> **Status:** Partial.

The clause names a GQL `Assimalign.Cohesion.Database.Language.QueryLanguageProfile` may enable.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Changed member

```csharp
public static class GqlClauses
{
    public const string GroupBy = "GROUP BY";
}
```

GroupBy is enabled by GqlLanguageProfile.Instance. It admits explicit ISO RETURN names/expressions
and GROUP BY (); implicit grouping uses nonaggregate RETURN keys when aggregates are present.
The constant adds no exceptions. Malformed syntax uses GQL0002 and invalid grouped scope uses
COHDBG001. These grouping codes have no assigned SQLSTATE.

```csharp
Console.WriteLine(GqlLanguageProfile.Instance.Supports(GqlClauses.GroupBy)); // True
```

See [GROUP BY](../../../../database/graph/language/clauses/group-by.md) for result examples and
[GqlQueryExpression.GroupBy](gql-query-expression.md) for the parsed key list.

## Documented behavior

`GqlQueryParser` derives from the shared `QueryParser` and returns `GqlQueryStatement`, whose
diagnostics describe malformed or unsupported input. `GqlLanguageProfile.Instance` advertises only
executable clauses. `GqlClauses` names lexical capability keys, including deferred clauses.

## Related reference

- **Assembly** — [`Assimalign.Cohesion.Database.Graph.Language`](index.md).
- **Design** — [Lifetime and implementation decisions](design.md).
- **Examples** — [Source-backed usage](examples/index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/Assembly/Assimalign.Cohesion.Database.Graph.Language/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlClauses.cs`.
