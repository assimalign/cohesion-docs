# INSERT

INSERT creates nodes and relationships from graph patterns with scalar expression properties.

> **Status:** Implemented.

## Syntax

```syntaxsql
[ MATCH match_path [ , ...n ] [ WHERE predicate ] ]
INSERT insertion_path [ , ...n ]
[ RETURN [ DISTINCT | ALL ] { * | expression [ AS alias ] [ , ...n ] } ]
```

## Supported clauses

| Clause | Support | Role |
|---|---|---|
| `MATCH` and `WHERE` | Supported | Supply existing bound elements |
| `RETURN` | Supported | Expressions, aliases, DISTINCT/ALL and named-binding star |
| [SET](../clauses/set.md), [REMOVE](../clauses/remove.md) | Supported | Update inserted or existing entities; created-then-updated counts once |
| MERGE | Recognized, not supported (COHDBL001) | Includes ON CREATE SET and ON MATCH SET |

## Arguments

- **`insertion_path`** — nodes and directed, typed relationships with optional scalar expression property maps.
- **`match_path`** — a finite pattern supplying existing variables.
- **`expression`** — a [RETURN value expression](../clauses/return.md), optionally aliased.

## Remarks

`INSERT` is the standard graph-insertion spelling in this profile. Its abstract syntax tree uses
the same `Creates` collection as `CREATE`. Relationship insertion requires a type and direction;
undirected relationship creation fails planning. Property maps contain scalar literals or [parameters](../expressions/parameters.md).
Bindings preserve runtime types; missing/invalid values and collisions use COHDBG001/003/014.

Insertion joins the session transaction. A failed mutation rolls back the owning logical
transaction, including an explicit transaction.

RETURN expressions evaluate after insertion for every matched binding. DISTINCT removes result
duplicates after all writes, without reducing insertion counts. Duplicate final column names fail
COHDBG015 before mutation. Wire Execute accepts scalar result kinds; `INSERT (a:P) RETURN a` or
`INSERT (a:P) RETURN *` fails COHDBG001 directing callers to ExecutePaths before writing. Path requests require
a read-only composed statement, so they cannot return insertion results; embedded rows can expose entities.

[FOR/UNWIND](../clauses/for.md) can supply rows to INSERT. Inline maps evaluate scalar expressions
against the incoming scope; newly inserted variables in the same stage are not available there
(COHDBG001). Lists/maps remain invalid stored properties. A later MATCH sees source-ordered writes,
including introduced labels/types. [NEXT](../clauses/next.md) after a write without RETURN starts
one empty binding with no scope.

[Parameters](../expressions/parameters.md) bind in node and relationship maps; only runtime scalar values execute.

## Examples

The parser corpus demonstrates standalone insertion and reuse of matched endpoints:

```sql
INSERT (a:Person {name: 'Alice', age: -42, active: TRUE, optional: NULL})
```

```sql
MATCH (a:Person {name: 'Alice'}), (b:Person {name: 'Bob'}) INSERT (a)-[r:KNOWS]->(b) RETURN r
```

That entity projection uses embedded rows. This scalar example runs embedded or over the wire,
with each statement executed separately in a fresh database:

```sql
INSERT (:Source), (:Source), (:Source);
MATCH (a:Source) INSERT (:Output {v:1}) RETURN DISTINCT 1 AS one;
MATCH (n:Output) RETURN n.v;
```

The second statement returns one row `(1)` with column `one` but inserts three Output nodes.
The final query returns three rows containing `1`.

## See also

- **[Statements](index.md)** — statement navigation.
- **[CREATE](create.md)** — compatibility spelling.
- **[Patterns](../patterns.md)** — nodes and relationship maps.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Planning/GraphPlanner.cs`.
- **DISTINCT after insertion** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/GraphReturnTests.cs`.
