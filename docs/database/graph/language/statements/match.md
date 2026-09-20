# MATCH

MATCH binds graph variables using finite node and relationship patterns.

> **Status:** Implemented.

## Syntax

```syntaxsql
MATCH path [ , ...n ]
    [ WHERE predicate ]
    { RETURN projection [ , ...n ]
    | { INSERT | CREATE } path [ , ...n ] [ RETURN projection [ , ...n ] ]
    | [ DETACH ] DELETE variable [ , ...n ] }
```

## Supported clauses

| Clause | Support | Role |
|---|---|---|
| `WHERE` | Supported | Filter matched bindings |
| `RETURN` | Supported | Project variables or scalar properties |
| `INSERT`, `CREATE` | Supported | Mutate from matched bindings |
| `DELETE`, `DETACH DELETE` | Supported | Delete bound elements |
| `OPTIONAL MATCH`, `MANDATORY MATCH` | Recognized, not supported (`COHDBL001`) | Outside the profile |

## Arguments

- **`path`** — a node followed by zero or more relationship/node pairs; see [Patterns](../patterns.md).
- **`predicate`** — scalar comparisons joined by `AND`.
- **`projection`** — a bound variable or `variable.property`, optionally with an `AS` alias.

## Remarks

`MATCH` must be followed by projection or mutation. Comma-separated paths share variable bindings.
Repeated variable names enforce element identity equality. A variable cannot represent both a node
and a relationship.

The planner can anchor at an indexed node in the middle or at either end of a path.
Each candidate still passes label, property, and variable checks. A path uses each relationship
identity at most once, while node identities may recur. Separate paths have separate edge sets.

## Examples

The execution corpus seeds Alice, Bob, and a `KNOWS` relationship, then runs:

```sql
MATCH (a:Person {name: 'Alice'})-[r:KNOWS]->(b) RETURN b.name, r.weight
```

That fixture returns `Bob` and `2`. These are fixture-dependent results, not built-in graph data.

## See also

- **[Statements](index.md)** — statement navigation.
- **[WHERE](../clauses/where.md)** — filtering.
- **[RETURN](../clauses/return.md)** — projection.
- **[Patterns](../patterns.md)** — bounds and direction.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlProfileExecutionTests.cs`.

