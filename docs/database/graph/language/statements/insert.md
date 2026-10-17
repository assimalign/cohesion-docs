# INSERT

INSERT creates nodes and relationships from graph patterns with scalar literal or parameter properties.

> **Status:** Implemented.

## Syntax

```syntaxsql
[ MATCH match_path [ , ...n ] [ WHERE predicate ] ]
INSERT insertion_path [ , ...n ]
[ RETURN projection [ , ...n ] ]
```

## Supported clauses

| Clause | Support | Role |
|---|---|---|
| `MATCH` and `WHERE` | Supported | Supply existing bound elements |
| `RETURN` | Supported | Project inserted or matched variables |
| `SET`, `REMOVE`, `MERGE` | Recognized, not supported (`COHDBL001`) | No update or merge clause |

## Arguments

- **`insertion_path`** — nodes and directed, typed relationships with optional literal or parameter property maps.
- **`match_path`** — a finite pattern supplying existing variables.
- **`projection`** — a bound variable or scalar property, optionally aliased.

## Remarks

`INSERT` is the standard graph-insertion spelling in this profile. Its abstract syntax tree uses
the same `Creates` collection as `CREATE`. Relationship insertion requires a type and direction;
undirected relationship creation fails planning. Property maps contain scalar literals or [parameters](../expressions/parameters.md).
Bindings preserve runtime types; missing/invalid values and collisions use COHDBG001/003/014.

Insertion joins the session transaction. A failed mutation rolls back the owning logical
transaction, including an explicit transaction.

[Parameters](../expressions/parameters.md) bind in node and relationship maps; only runtime scalar values execute.

## Examples

The parser corpus demonstrates standalone insertion and reuse of matched endpoints:

```sql
INSERT (a:Person {name: 'Alice', age: -42, active: TRUE, optional: NULL})
```

```sql
MATCH (a:Person {name: 'Alice'}), (b:Person {name: 'Bob'}) INSERT (a)-[r:KNOWS]->(b) RETURN r
```

## See also

- **[Statements](index.md)** — statement navigation.
- **[CREATE](create.md)** — compatibility spelling.
- **[Patterns](../patterns.md)** — nodes and relationship maps.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/GraphPlanner.cs`.
