# GqlStage

The abstract record family for source-ordered operations on a graph binding table.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public abstract record GqlStage
{
    public Location? Location { get; init; }
}
```

## Behavior and exceptions

The private protected constructor limits variants to the assembly. Each sealed leaf stores its
operands; the planner validates shapes and scopes before executing any stage. Null/missing mandatory
operands use COHDBG001. Every stage materializes eagerly in source order within one transaction.

| Stage | Operation |
| --- | --- |
| [GqlMatchStage](gql-match-stage.md) | MATCH/OPTIONAL MATCH over incoming bindings |
| [GqlLetStage](gql-let-stage.md) | Sequential value assignments |
| [GqlFilterStage](gql-filter-stage.md) | Retain only TRUE predicates |
| [GqlForStage](gql-for-stage.md) | FOR/UNWIND list expansion |
| [GqlInsertStage](gql-insert-stage.md) | INSERT/CREATE per input row |
| [GqlSetStage](gql-set-stage.md) | Property/map/label assignments |
| [GqlRemoveStage](gql-remove-stage.md) | Remove properties/node labels |
| [GqlDeleteStage](gql-delete-stage.md) | DELETE/DETACH DELETE |
| [GqlReturnStage](gql-return-stage.md) | RETURN/WITH projection, grouping and tail |
| [GqlNextStage](gql-next-stage.md) | Segment boundary |

## Example

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "LET x = 1 FILTER x > 0 RETURN x")).GqlExpression;
foreach (GqlStage stage in query.Stages) Console.WriteLine(stage.GetType().Name);
// GqlLetStage, GqlFilterStage, GqlReturnStage (one per line)
```

Execution returns one row containing 1.

SET/REMOVE are [GqlSetStage](gql-set-stage.md) and [GqlRemoveStage](gql-remove-stage.md), with
ordered [GqlSetItem](gql-set-item.md)/[GqlRemoveItem](gql-remove-item.md) variants.

## Related reference

[GqlQueryExpression](gql-query-expression.md) · [GqlQuerySegment](gql-query-segment.md) · [Assembly](index.md) · [Design](design.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlStage.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlLinearCompositionParserTests.cs`.
