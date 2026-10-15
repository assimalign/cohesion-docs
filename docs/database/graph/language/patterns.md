# Graph patterns

Graph patterns describe finite chains of labeled nodes and typed relationships with literal properties.

> **Status:** Implemented.

## Syntax

```syntaxsql
<paths> ::= path [ , ...n ]
<path> ::= node [ relationship node ...n ]

<node> ::= "(" [ variable ] [ ":" label_name ...n ] [ property_map ] ")"

<relationship> ::=
    { "-" "[" relationship_body "]" "->"
    | "<-" "[" relationship_body "]" "-"
    | "-" "[" relationship_body "]" "-" }

<relationship_body> ::= [ variable ] [ ":" type_name ] [ property_map ]

<property_map> ::= "{" [ property_name ":" scalar_literal [ , ...n ] ] "}"
```

Double-quoted punctuation in this diagram denotes the literal character or arrow; diagram quotes
are not entered in the query. Optional square brackets in the diagram differ from the quoted
square brackets that surround relationship syntax.

## Supported forms

| Construct | Support | Constraint |
|---|---|---|
| Named and anonymous nodes | Supported | Node parentheses are required |
| Incoming, outgoing, undirected matching | Supported | Relationship brackets are required |
| Repeated colon labels | Supported | Cohesion convenience; one label is the portable form |
| Literal property maps | Supported | Unique keys and scalar values |
| Quantified or variable-length paths | Recognized, not supported (`COHDBL001`) | Use explicit finite chains |
| Collection-valued properties in text | Recognized, not supported (`COHDBL001`) | No list or nested map literal |

## Arguments

- **`variable`** — an optional case-sensitive node or relationship binding.
- **`label_name`** — a node label; repeated labels each have their own colon.
- **`type_name`** — an optional matching type, required for relationship insertion.
- **`property_map`** — an optional map of literal properties with ordinal, unique keys.

## Remarks

A path always contains one more node than relationship. Anonymous elements have no variable.
Incoming arrows reverse endpoints relative to consecutive pattern nodes. Matching permits an
undirected relationship, but insertion requires a direction and a type.

A path contains at most 64 relationships. Repeated nodes are permitted; a relationship identity
cannot repeat within one matched path. Comma-separated paths share variables but keep separate
relationship-identity sets. Conflicting node/relationship variable kinds fail planning.

The executor limits intermediate materialized matches to 100,000 per pattern and candidate
examination to 1,000,000 node/edge candidates per statement, including dead ends.
Overflow produces `COHDBG004`, without silent truncation. Cancellation is observed during expansion
and materialization.

## Examples

The parser corpus exercises incoming and undirected relationships in one chain:

```sql
MATCH (a:Person {name: 'Alice'})<-[r:KNOWS {weight: 2}]-(b)-[s]-(c) RETURN a, r, b.name AS friend
```

It also accepts anonymous nodes:

```sql
MATCH ()-[r:KNOWS]->() RETURN r
```

## See also

- **[Language (GQL)](index.md)** — reference hub.
- **[Syntax conventions](syntax-conventions.md)** — diagram notation.
- **[MATCH](statements/match.md)** — pattern matching.
- **[INSERT](statements/insert.md)** — insertion rules.
- **[Diagnostics](diagnostics.md)** — bound and binding failures.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlQueryParser.Patterns.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/GraphPlanner.cs`.

