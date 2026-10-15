# DETACH DELETE

DETACH DELETE removes matched nodes together with their incident relationships.

> **Status:** Implemented.

## Syntax

```syntaxsql
MATCH path [ , ...n ] [ WHERE predicate ]
DETACH DELETE variable [ , ...n ]
```

## Supported clauses

| Construct | Support |
|---|---|
| `MATCH` with optional `WHERE` | Supported |
| Comma-separated bound variables | Supported |
| `RETURN` after deletion | Recognized, not supported (`COHDBL001`) |

## Arguments

- **`variable`** — a bound node or relationship variable selected for deletion.
- **`path`** — the finite matching pattern establishing those bindings.

## Remarks

Deleting a node tombstones its incident relationships, adjacency entries, and property-index
entries with the node atomically. The operation joins the session transaction. Any mutation failure
rolls back the owning logical transaction.

Detaching a node does not select its neighboring nodes for deletion merely because they share an
incident relationship.

## Examples

The parser corpus includes:

```sql
MATCH (a:Person) DETACH DELETE a
```

This deletes every matched `Person` node and cascades to its incident relationships.

## See also

- **[Statements](index.md)** — statement navigation.
- **[DELETE](delete.md)** — restricted deletion.
- **[Graph](../../index.md)** — transaction model.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.

