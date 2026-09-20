# DELETE

DELETE removes matched relationships or nodes without implicitly deleting incident relationships.

> **Status:** Implemented.

## Syntax

```syntaxsql
MATCH path [ , ...n ] [ WHERE predicate ]
DELETE variable [ , ...n ]
```

## Supported clauses

| Construct | Support |
|---|---|
| `MATCH` with optional `WHERE` | Supported |
| Comma-separated bound node/relationship variables | Supported |
| `RETURN` after deletion | Recognized, not supported (`COHDBL001`) |

## Arguments

- **`variable`** — an existing bound node or relationship variable.
- **`predicate`** — an optional scalar filter on the matched bindings.

## Remarks

Plain `DELETE` refuses a node that still has incident relationships. Explicitly selected
relationships are deleted before node connectivity is checked, even if a node appears first in
the deletion list. A mutation failure rolls back the owning logical transaction.

Use [DETACH DELETE](detach-delete.md) for cascading deletion. The typed `DeleteNodeAsync` API
also cascades, so its behavior differs from plain GQL `DELETE`.

## Examples

The parser corpus deletes an explicit relationship with its source node:

```sql
MATCH (a)-[r]->(b) DELETE r, a
```

Other incident relationships would still prevent deletion of `a`.

## See also

- **[Statements](index.md)** — statement navigation.
- **[DETACH DELETE](detach-delete.md)** — cascading behavior.
- **[Diagnostics](../diagnostics.md)** — restricted-deletion errors.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.

