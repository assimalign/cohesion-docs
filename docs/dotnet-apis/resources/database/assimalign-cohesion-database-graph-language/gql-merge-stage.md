# GqlMergeStage

Matches one complete fixed path or creates its unbound parts, then applies the selected SET branch.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlMergeStage(GqlPathPattern Path,
    IReadOnlyList<GqlSetItem> OnCreate, IReadOnlyList<GqlSetItem> OnMatch,
    Location? Location = null) : GqlStage(Location)
{
    public override string ToString();
}
```

## Behavior and exceptions

The constructor stores one Path and two ordered SET-item lists. Repeated ON clauses append items
to the corresponding list in source order. ToString renders canonical GQL with CREATE before MATCH,
preserving item order within each kind; it does not preserve interleaving between kinds. Location
is the optional inherited source span. Construction does not execute or validate the pattern.

The planner validates the path, incoming equality scope and both branches, including under EXPLAIN.
Invalid/null mandatory operands or repeated relationship variables use DatabaseException/CG05001.
Already-declared relationships use CG03007; labels/properties on repeated or incoming nodes use
CG03006. Proven/evaluated invalid equalities use CG03004, and NULL/absent/incompatible bound nodes
CG03005. Textual bare list/map equalities retain parser CK00001. Required-property failures after
ON CREATE use CG03003. These codes have no assigned SQLSTATE. Session execution rejects parser
errors with DatabaseParseException; inspect parser diagnostics before consuming a recovered tree.

Execution runs ON MATCH for every complete match, or creates every unbound part and runs ON CREATE
once. Snapshot conflicts use DatabaseTransactionAbortedException/CG04024; ended statement/session
execution uses CK04002 while the database is live. See [MERGE](../../../../database/graph/language/statements/merge.md)
for creation direction, scope, NULL behavior, concurrency and result counts.

## Example

Adapted from GqlMergeParserTests:

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "MERGE (n:P) ON CREATE SET n.x=1 ON MATCH SET n.x=n.x+10 " +
    "ON CREATE SET n.x=n.x+2 ON MATCH SET n.x=n.x*2 RETURN n.x")).GqlExpression;
var merge = (GqlMergeStage)query.Stages[0];
Console.WriteLine(merge.OnCreate.Count); // 2
Console.WriteLine(merge.OnMatch.Count);  // 2
Console.WriteLine(GqlLanguageProfile.Instance.Supports(GqlClauses.Merge)); // True
```

In a fresh database, executing this query twice returns `3`, then `26`, as checked by
GraphMergeFixTests. Each execution affects one entity.

## Related reference

[GqlStage](gql-stage.md) · [GqlPathPattern](gql-path-pattern.md) · [GqlSetItem](gql-set-item.md) · [Assembly](index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlStage.Merge.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlMergeParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphMergeFixTests.cs`.
