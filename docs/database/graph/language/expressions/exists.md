# EXISTS

EXISTS returns TRUE when a braced graph pattern finds a match, otherwise FALSE. NOT EXISTS negates it.

## Syntax

```syntaxsql
EXISTS { pattern [ , ...n ] [ WHERE predicate ] }
EXISTS { [ OPTIONAL ] MATCH pattern [ WHERE predicate ] ... }
NOT EXISTS { pattern [ WHERE predicate ] }
```

## Correlation and scope

Outer variables are arguments to the inner pattern. Node, relationship and path positions keep
the supplied outer entity identities. Scalar outer values are available in property maps and
predicates. This includes iterator variables and locals of an enclosing EXISTS. New names
declared inside the braces stay local; referencing them outside is an invalid scope (CG05001).
Iterator variables shadow enclosing names only in their bodies.

A NULL outer binding in a pattern position matches nothing, even under OPTIONAL MATCH:
EXISTS is FALSE and NOT EXISTS is TRUE. The result is always Boolean. An uncorrelated block
also executes, for example `RETURN EXISTS { (:Person) }`. Inner execution shares the outer
operation, snapshot, update overlay, parameters and statement budget, stops at its first match,
and closes that inner stream without changing the outer bindings.

EXISTS works in Boolean and value expressions: WHERE/FILTER, inline pattern predicates,
RETURN, CASE, ordering, grouping expressions, aggregate arguments, list comprehensions and
list predicates. Ordinary grouping rules still apply to correlated outer references.
Inner predicates retain MATCH's prohibition on aggregates.

The explicit form accepts successive MATCH or OPTIONAL MATCH clauses. It is a pattern block,
not a full query body: RETURN, WITH, NEXT, UNION, CALL, ordering, paging and writes inside it
remain refused. `EXISTS ((a))`, COUNT/COLLECT blocks and bare pattern predicates such as
`WHERE (a)->(b)` are also refused. Parentheses around a complete braced EXISTS expression
are ordinary expression grouping.

## Diagnostics

None of these codes has an assigned SQLSTATE.

| Code | Meaning and trigger |
| --- | --- |
| CG00013 | Empty or unbraced EXISTS operand, including `EXISTS { MATCH }` |
| CG01019 | Outer binding has a statically proven incompatible node/relationship/path kind |
| CG02016 | Runtime correlated argument has an incompatible entity kind |
| CG05027 | Malformed/missing correlated subplan or missing argument, including malformed direct ASTs |
| CG05001 | Invalid lexical or grouped scope |
| CK00001 | Full query body, parenthesized EXISTS operand, COUNT/COLLECT block, bare pattern predicate or unsupported inner vocabulary |

## Example

The friend-recommendation test uses two undirected KNOWS steps and excludes existing friends.
Execute these separately in a fresh graph:

```sql
INSERT (a:Person {id:1})-[:KNOWS]->(b:Person {id:2}),
       (b)-[:KNOWS]->(c:Person {id:3});
MATCH (p:Person {id:1})-[:KNOWS]-(f:Person)-[:KNOWS]-(c:Person)
WHERE c.id <> p.id AND NOT EXISTS { MATCH (p)-[:KNOWS]-(c) }
RETURN c.id, f.id;
```

Returns `(3,2)`: person 3 is reachable through friend 2 and is not already a friend of person 1.
The test also uses RETURN DISTINCT over all starting people to deduplicate multiple mutual friends.

## See also

[Expressions](index.md) · [WHERE](../clauses/where.md) · [MATCH](../statements/match.md) · [GqlExistsExpression](../../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph-language/gql-exists-expression.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphExistsTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlExistsParserTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Execution/GraphPlanExecutor.Correlation.cs`.
