# Unsupported GQL features

The executable GQL subset rejects recognized features outside its bounded profile.
[CASE](expressions/case.md), [COALESCE](expressions/coalesce.md), [NULLIF](expressions/nullif.md),
[list/map values](expressions/list-values.md) and [subscripts/member access](expressions/property-access.md) execute.

> **Status:** Partial. This matrix records the current executable profile and delivery limits.

## Capability matrix

These constructs are recognized but produce the capability diagnostic rather than being executed.
The examples below are rejected forms, not executable syntax diagrams.

| Construct | Support | Rejected form |
|---|---|---|
| Mandatory keyword | Recognized, not supported (`COHDBL001`) | `MANDATORY MATCH` |
| Standalone ordering and paging | Recognized, not supported (`COHDBL001`) | `MATCH (a) ORDER BY a.age RETURN a`; ordering/paging after RETURN executes |
| FOR ordinality/offset suffixes | Recognized, not supported (`COHDBL001`) | `FOR x IN xs WITH ORDINALITY i`, `WITH OFFSET i`; [FOR/UNWIND](clauses/for.md) executes |
| NEXT YIELD and FOREACH | Recognized, not supported (`COHDBL001`) | [RETURN … NEXT](clauses/next.md) and [WITH](clauses/with.md) execute |
| Property updates and merge | Recognized, not supported (`COHDBL001`) | `SET`, `REMOVE`, `MERGE` |
| Set operations | Recognized, not supported (`COHDBL001`) | `UNION` |
| Unsupported scalar/statistical calls | Recognized, not supported (`COHDBL001`) | `abs(a.age)`, `STDDEV_SAMP`, `STDDEV_POP`, `PERCENTILE_CONT`, `PERCENTILE_DISC`; six [aggregates](functions/aggregate-functions.md) execute |
| HAVING | Recognized, not supported (`COHDBL001`) | GROUP BY executes; post-group HAVING is excluded |
| Map projection | Recognized, not supported (`COHDBL001`) | `RETURN a {.name}` |
| Questioned paths, nested quantifiers and subpath assignments/WHERE | Recognized, not supported (`COHDBL001`) | [Quantified edges/subpaths](patterns.md#quantified-paths) execute |
| Stored composite property values | Recognized, not supported (`COHDBL001`) for syntax; `COHDBG003` for parameters | MATCH/INSERT/CREATE stored properties stay scalar-only |
| List/pattern comprehensions and slicing | Recognized, not supported (`COHDBL001`) | `[x IN xs | x]`, pattern comprehensions, `xs[1..3]` |
| Projection after deletion | Recognized, not supported (`COHDBL001`) | `DELETE a RETURN a` |
| Procedure calls | Recognized, not supported (`COHDBL001`) | `CALL` |
| Server, database, graph, or schema selection | Recognized, not supported (`COHDBL001`) | `USE`, `CREATE DATABASE`, `CREATE GRAPH`, `DROP GRAPH`, `ALTER SCHEMA`, `SHOW DATABASES`, `SESSION SET GRAPH` |
| Transaction statements | Recognized, not supported (`COHDBL001`) | `BEGIN TRANSACTION`, `COMMIT` |
| Cypher doubled-dash arrows | Rejected (`GQL0008` or `GQL0002`) | ISO `->`, `<-`, `-`, `<->` execute; see [Patterns](patterns.md) |
| Tilde edge forms | Recognized, not supported (`COHDBL001`) | Only directed stored relationships, matched in either orientation where requested |
| Direct stored-property chain | Recognized, not supported (`COHDBL001`) | `a.b.c`; nested map member access executes |
| Multiple statements in a request | Not in the language | Two statements separated by semicolons |
| Catalog filtering or projection | Not in the language | `SHOW LABELS RETURN label` |
| Catalog mutation composition | Not in the language | `SHOW LABELS DELETE n` produces `GQL0007` |

`GqlLanguageProfile` declares count, sum, avg, min, max, collect_list and collect. Its larger reserved keyword table is a lexical
inventory, not a list of implemented features. Malformed or invalid composition generally produces
`GQL0002`, while catalog mutation composition has its dedicated read-only error.

## Engine and delivery limits

GQL cannot select another graph or database. Database lifecycle uses the engine API, explicit
transactions use the session API, and label/type/index management uses the session-bound schema API.

GraphDatabaseServer serves scalar projections, mutations, SHOW, EXPLAIN and path queries.
ExecutePaths accepts one bare node, relationship or path binding from a read-only composed query, including
DISTINCT or star over one named binding. Computed, multiple and multi-binding star projections
report COHDBG001. A path binding supplied only by optional clauses also reports COHDBG001 during
planning; a mandatory clause must bind or constrain it. Execute accepts scalar and negotiated composite columns; entity results report COHDBG001 with
the ExecutePaths direction before reads, writer locks or mutations, including empty matches.
EXPLAIN descriptions remain scalar even for entity returns. GraphClient provides pooled connections,
materialized scalar/list/map results and single-use path streams. RETURN arithmetic/parameters, DISTINCT,
star and standalone evaluation execute. Other scalar calls, LIKE, BETWEEN, EXISTS and quantified
predicates remain excluded; [value expressions](expressions/operators.md) run in WHERE and RETURN.

Graph-specific security policies, replication, compiled-schema provisioning, configurable graph
collation, and large property chunking are deferred or out of scope in the engine design.
Serializable isolation is rejected.

## See also

Final lists of nodes/relationships remain outside wire Execute, even with graph.composite-values
(COHDBG001; #1662). Intermediate collect and UNWIND preserve entity kinds and can produce scalar
properties or a single mandatory final entity through ExecutePaths. A write anywhere in a composed
statement excludes ExecutePaths. [Linear composition](clauses/next.md) documents the eager stage order.

- **[Language (GQL)](index.md)** — supported reference.
- **[Graph](../index.md)** — engine status.
- **[Diagnostics](diagnostics.md)** — diagnostic meanings.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlQueryParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlLanguageProfile.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/src/GqlQueryParser.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlCapabilityDiagnosticTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Language/tests/GqlCatalogParserTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.
