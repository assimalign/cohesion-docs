# GqlPathMode

Identity-reuse policy for one complete matched path.

Namespace: `Assimalign.Cohesion.Database.Graph.Language`.

## Signature

```csharp
public enum GqlPathMode : byte { Trail, Walk, Simple, Acyclic }
```

## Behavior and exceptions

Trail forbids repeated relationship identities and permits repeated nodes. Walk permits both
but requires bounded quantifiers. Simple forbids repeated nodes except closure of the final node
onto the first; it can reuse a relationship for that closure. Acyclic forbids all repeated nodes.
An omitted GqlPathPattern.Mode is null and resolves to Trail.

Only an outer MATCH path may specify a mode. Nested modes or unbounded Walk (even with LIMIT)
report CK00001 during parsing/planning. Undefined direct AST values and insertion modes use
CG05001. Every mode retains hop/work (CG05004) and memory (CG05020) limits.

## Example

```csharp
var statement = (GqlQueryStatement)new GqlQueryParser().Parse(
    "MATCH p = WALK PATHS (a)->{1,3}(b) RETURN p");
Console.WriteLine(statement.GqlExpression.Matches[0].Mode); // Walk
```

## Related reference

[Assembly](index.md) · [GqlPathPattern](gql-path-pattern.md) · [Path modes](../../../../database/graph/language/patterns.md#path-modes)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlPathModeParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlPathModeTests.cs`.
