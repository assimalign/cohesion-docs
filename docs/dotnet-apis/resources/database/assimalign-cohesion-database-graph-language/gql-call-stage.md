# GqlCallStage

Invokes an inline query for each incoming row and joins its returned bindings.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public sealed record GqlCallStage(GqlQueryExpression Body,
    IReadOnlyList<string>? Imports = null, bool ImportAll = false,
    bool Optional = false, Location? Location = null) : GqlStage(Location);
```

## Members

| Member | Meaning |
| --- | --- |
| Body | Inner query |
| Imports | Imported variable names; null records an omitted scope, an empty list records () |
| ImportAll | Scope clause is (*) |
| Optional | An empty invocation emits one NULL-extended row |
| Location | Source span |

## Behavior and exceptions

The positional record retains Body, Imports and flags; construction does not execute or validate
the query. Planning rejects a null body, or ImportAll together with a nonempty Imports list, with
CG05001. Imports use ordinal, case-sensitive binding names. Missing names use CG05001; duplicate
imports, rebinding or importing a deleted variable use CG03003.

Only read-only bodies with RETURN execute. A returned bare variable or aliased expression exports
its name; computed results require aliases (CG05001). Every export must be absent from the outer
scope, including imports and star expansion (CG01024). Empty invocations filter input unless
Optional is true. Inner state is local to each invocation and shares outer snapshot and budgets.
Writing/unit bodies, named procedures and CALL IN TRANSACTIONS remain CK00001. See
[CALL scope and execution](../../../../database/graph/language/clauses/next.md#inline-call-subqueries).

## Example

Following GqlCallParserTests and GqlCallSubqueryTests:

```csharp
var query = ((GqlQueryStatement)new GqlQueryParser().Parse(
    "LET x=1 CALL (x) { RETURN x+1 AS y } RETURN y")).GqlExpression;
var call = (GqlCallStage)query.Stages[1];
Console.WriteLine(call.Imports![0]); // x
Console.WriteLine(call.ImportAll);   // False
Console.WriteLine(call.Optional);    // False
```

Execution returns one row containing 2. Inspect parser diagnostics before execution.

## Related reference

[Assembly](index.md) · [GqlStage](gql-stage.md) · [GqlQueryExpression](gql-query-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlStage.Call.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlCallParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Planning/GraphPlanner.Call.cs`.

