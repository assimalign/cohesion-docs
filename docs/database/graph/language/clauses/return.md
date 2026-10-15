# RETURN

RETURN projects bound nodes, relationships, or their scalar properties in source order.

> **Status:** Implemented.

## Syntax

```syntaxsql
RETURN projection [ , ...n ]

<projection> ::= variable [ "." property_name ] [ AS alias ]
```

## Supported clauses

| Construct | Support |
|---|---|
| Bound node/relationship variables | Supported |
| Single property access | Supported |
| `AS` aliases | Supported |
| `DISTINCT`, wildcard projection, functions | Recognized, not supported (`COHDBL001`) |
| `ORDER BY`, `LIMIT`, `OFFSET`, `SKIP` | Recognized, not supported (`COHDBL001`) |

## Arguments

- **`variable`** — a graph variable bound by matching or insertion.
- **`property_name`** — one property key; nested property access is not accepted.
- **`alias`** — an optional result-column name.

## Remarks

Projections preserve source order. Whole-element projections expose `GraphNode` or
`GraphRelationship` objects through `QueryRow.GetValue`; property projections expose scalars.
The execution corpus verifies that an absent property projects null.

`RETURN` may follow a read match or insertion, but cannot follow deletion. It cannot be appended
to `SHOW`. Literal evaluation, arithmetic, and aggregation are not projection forms.

## Examples

The execution corpus uses this query against its seeded Alice node:

```sql
MATCH (a:Person {name: 'Alice'}) RETURN a.name AS person, a.age, a.absent
```

The fixture returns `Alice`, `42`, and null.

## See also

- **[Clauses](index.md)** — clause navigation.
- **[RETURN statement composition](../statements/return.md)** — complete read syntax.
- **[INSERT](../statements/insert.md)** — projection after insertion.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GqlProfileExecutionTests.cs`.

