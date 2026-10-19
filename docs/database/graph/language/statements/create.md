# CREATE

CREATE is Cohesion's compatibility spelling for graph insertion.

> **Status:** Implemented.

## Syntax

```syntaxsql
[ MATCH match_path [ , ...n ] [ WHERE predicate ] ]
CREATE insertion_path [ , ...n ]
[ RETURN [ DISTINCT | ALL ] { * | expression [ AS alias ] [ , ...n ] } ]
```

## Supported clauses

| Construct | Support |
|---|---|
| Node/path insertion with scalar expression properties | Supported |
| Preceding `MATCH` and `WHERE` | Supported |
| Following `RETURN` | Supported |
| `CREATE DATABASE`, `CREATE GRAPH` | Recognized, not supported (`COHDBL001`) |

## Arguments

- **`insertion_path`** — a node/path pattern with scalar expression properties from the incoming scope; created relationships require a type and direction.
- **`expression`** — a [RETURN value expression](../clauses/return.md), with optional alias.

## Remarks

This is a Cohesion compatibility extension, not the standard insertion spelling.
`CREATE` and `INSERT` compile to the same `Creates` abstract syntax tree and transaction path.
Use [INSERT](insert.md) when choosing the profile's standard insertion verb.
Its RETURN/DISTINCT ordering and pre-write validation apply here too: duplicate names use
COHDBG015, and entity columns through wire Execute use COHDBG001 before mutation.

This syntax does not manage databases, graphs, schemas, labels, or indexes.
Session-bound schema operations are provided by `GraphSchema.Open`.

[Parameters](../expressions/parameters.md) bind in node and relationship maps; only runtime scalar values execute.

## Examples

The compatibility case in the parser corpus is:

```sql
CREATE (a:Person {name: 'Alice'}) RETURN a
```

Embedded rows contain the inserted GraphNode in column `a`. Wire Execute rejects this entity
projection before writing; `CREATE (a:Person {name:'Alice'}) RETURN a.name AS name` returns
one scalar row `(Alice)` through either surface.

## See also

- **[Statements](index.md)** — statement navigation.
- **[INSERT](insert.md)** — insertion semantics.
- **[Graph](../../index.md)** — session and schema APIs.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.
