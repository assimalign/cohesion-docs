# CREATE

CREATE is Cohesion's compatibility spelling for graph insertion.

> **Status:** Implemented.

## Syntax

```syntaxsql
[ MATCH match_path [ , ...n ] [ WHERE predicate ] ]
CREATE insertion_path [ , ...n ]
[ RETURN projection [ , ...n ] ]
```

## Supported clauses

| Construct | Support |
|---|---|
| Literal node/path insertion | Supported |
| Preceding `MATCH` and `WHERE` | Supported |
| Following `RETURN` | Supported |
| `CREATE DATABASE`, `CREATE GRAPH` | Recognized, not supported (`COHDBL001`) |

## Arguments

- **`insertion_path`** — a literal node/path pattern; created relationships require a type and direction.
- **`projection`** — a bound variable or scalar property, with optional alias.

## Remarks

This is a Cohesion compatibility extension, not the standard insertion spelling.
`CREATE` and `INSERT` compile to the same `Creates` abstract syntax tree and transaction path.
Use [INSERT](insert.md) when choosing the profile's standard insertion verb.

This syntax does not manage databases, graphs, schemas, labels, or indexes.
Session-bound schema operations are provided by `GraphSchema.Open`.

## Examples

The compatibility case in the parser corpus is:

```sql
CREATE (a:Person {name: 'Alice'}) RETURN a
```

## See also

- **[Statements](index.md)** — statement navigation.
- **[INSERT](insert.md)** — insertion semantics.
- **[Graph](../../index.md)** — session and schema APIs.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.

