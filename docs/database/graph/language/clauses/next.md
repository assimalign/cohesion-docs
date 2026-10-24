# NEXT

Starts the next segment of one eagerly evaluated GQL statement.

> **Status:** Implemented. Applies after RETURN or a modifying segment.

## Syntax

```syntaxsql
RETURN projection [ return_tail ] NEXT next_segment
modifying_segment NEXT next_segment
```

## Remarks

All stages run in source order in one statement transaction. LET/FILTER/FOR, MATCH and writes can
compose; RETURN … NEXT passes its projected rows to the following segment. Only aliases and
bare-variable projections carry referenceable names; star carries every current name in order.
Unaliased computed outputs cannot be named later. NULL values remain rows. GQL has no MISSING value.
See [WITH](with.md) for projection scope and duplicate carried bindings.

NEXT after a modifying segment without RETURN starts with **one empty binding and no scope**.
It does not carry the previous write's per-row bindings. A statement sees its own earlier writes
under Snapshot and ReadCommitted; an unknown label empties its MATCH stage, rather than skipping
unrelated later stages. A write anywhere takes the writer lock before the first stage. Later
paging does not reduce earlier mutations. Failure rolls back the statement under session
transaction rules.

Only final columns govern wire result admission. Read-only composed queries can use ExecutePaths
for one mandatory entity/path final column; any write stage is refused by that path API. Intermediate
collect/UNWIND is permitted. Final lists of entities remain unsupported over the wire (CG05001; #1662).

## Diagnostics

| Code | Meaning and trigger |
| --- | --- |
| CG00002 | Leading/trailing NEXT, NEXT after a bare MATCH, or WITH immediately followed by NEXT |
| CG05001 | Reference to a name omitted by a projection or invalid path/wire result shape |
| CG03003 | Reference to a deleted variable or a value used as an entity |
| CG01015 | Duplicate carried name |
| CK00001 | NEXT YIELD, UNION, CALL, FOREACH, standalone ordering/paging, or immediate RETURN after DELETE |

These codes have no assigned SQLSTATE. See [Diagnostics](../diagnostics.md).

## Examples

```sql
LET x = 1 RETURN x AS next NEXT LET y = next + 1 RETURN y;
```

Result: `2`.

```sql
INSERT (:Fresh {k:1}) NEXT MATCH (a:Fresh) RETURN a.k;
```

On an initially empty graph, result: `1`. The insert commits with the statement. The equivalent
INSERT followed directly by MATCH also executes in source order and sees the inserted node.

[SET](set.md) and [REMOVE](remove.md) participate in the same stage order. Later MATCH stages
see their working copies and introduced labels; carried entity references refresh through lists,
maps and paths. One final version publishes per updated entity.

## See also

[WITH](with.md) · [LET](let.md) · [FOR/UNWIND](for.md) · [RETURN](../statements/return.md) · [Clauses](index.md) · [GqlStage .NET API](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph-language/gql-stage.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Execution/GraphPlanExecutor.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlLinearCompositionTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/tests/GraphLinearCompositionWireTests.cs`.
