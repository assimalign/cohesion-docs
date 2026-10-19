# GqlMatchClause

A MATCH or OPTIONAL MATCH with its own paths and filter.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlMatchClause(
    IReadOnlyList<GqlPathPattern> Paths, GqlExpression? Predicate, bool Optional);
```

## Behavior and exceptions

[GqlQueryExpression](gql-query-expression.md) evaluates MatchClauses in source order. Each clause
joins its paths on shared variable identities, then evaluates Predicate. Optional preserves an
unmatched input row with NULL for newly declared variables. Mandatory drops that input row.
The predicate can reference variables declared through its clause, never later ones.

The record stores its arguments without validation. The Graph planner rejects null clauses,
null/empty path lists, null paths, unbound predicate references and catalog composition with
DatabaseException carrying `COHDBG001`. Path requests also reject projections supplied only by
optional clauses with that code.

## Example

```csharp
var statement = (GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH (a:Person) OPTIONAL MATCH (a)-[:KNOWS]->(b) RETURN a.name, b.name");
var clauses = statement.GqlExpression.MatchClauses;
Console.WriteLine(clauses.Count); // 2
Console.WriteLine(clauses[1].Optional); // True
Console.WriteLine(statement.GqlExpression.Matches.Count); // 0 (compatibility view)
```

In the Alice/Bob/Isolated fixture, rows are `(Alice, Bob)`, `(Bob, null)` and `(Isolated, null)`
in unspecified order. See [MATCH](../../../../database/graph/language/statements/match.md).

## Related reference

[Assembly](index.md) · [Design](design.md) · [Examples](examples/index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlMatchClause.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlOptionalMatchParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlOptionalMatchTests.cs`.
