# RETURN

RETURN completes a read query or projects values after insertion.

> **Status:** Implemented.

## Syntax

```syntaxsql
MATCH path [ , ...n ] [ WHERE predicate ]
RETURN variable [ . property_name ] [ AS alias ] [ , ...n ]
```

For insertion followed by projection, see [INSERT](insert.md) and [CREATE](create.md).

## Supported clauses

| Construct | Support |
|---|---|
| `MATCH` with optional `WHERE` before `RETURN` | Supported |
| `RETURN` after insertion | Supported |
| `RETURN` after deletion | Recognized, not supported (`COHDBL001`) |
| `DISTINCT`, wildcard projection, functions | Recognized, not supported (`COHDBL001`) |

## Arguments

- **`variable`** — a node or relationship variable bound by the query.
- **`property_name`** — one scalar property on that variable.
- **`alias`** — an optional result-column alias.

## Remarks

`RETURN` is a projection clause rather than a standalone way to evaluate literal expressions.
Read variables require a `MATCH` binding. See the [RETURN clause](../clauses/return.md) for result
values and projection limits.

## Examples

The parser corpus includes:

```sql
MATCH (a) WHERE a.age >= 18 AND a.name <> 'Bob' RETURN a.name AS name
```

## See also

- **[Statements](index.md)** — statement navigation.
- **[RETURN clause](../clauses/return.md)** — projection reference.
- **[MATCH](match.md)** — query bindings.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.

