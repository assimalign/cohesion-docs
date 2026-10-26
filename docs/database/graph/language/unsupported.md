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
| Mandatory keyword | Recognized, not supported (`CK00001`) | `MANDATORY MATCH` |
| Standalone ordering and paging | Recognized, not supported (`CK00001`) | `MATCH (a) ORDER BY a.age RETURN a`; ordering/paging after RETURN executes |
| FOR ordinality/offset suffixes | Recognized, not supported (`CK00001`) | `FOR x IN xs WITH ORDINALITY i`, `WITH OFFSET i`; [FOR/UNWIND](clauses/for.md) executes |
| NEXT YIELD and FOREACH | Recognized, not supported (`CK00001`) | [RETURN … NEXT](clauses/next.md) and [WITH](clauses/with.md) execute |
| Merge statement | Recognized, not supported (`CK00001`) | MERGE, including ON CREATE SET and ON MATCH SET; [SET](clauses/set.md)/[REMOVE](clauses/remove.md) stages execute |
| Whole-map parameter/expression assignment | Recognized, not supported (`CK00001`) | SET n = $p, SET n += properties(other); literal whole maps execute |
| Set operations | Recognized, not supported (`CK00001`) | `UNION` |
| Unsupported scalar/statistical calls | Recognized, not supported (`CK00001`) | `ELEMENTS()`, temporal constructors, `LOG10`, `STDDEV_SAMP`, `STDDEV_POP`, `PERCENTILE_CONT`, `PERCENTILE_DISC`; six [aggregates](functions/aggregate-functions.md) execute |
| HAVING | Recognized, not supported (`CK00001`) | GROUP BY executes; post-group HAVING is excluded |
| Map projection | Recognized, not supported (`CK00001`) | `RETURN a {.name}` |
| Nested path modes, unbounded WALK and path selectors | Recognized, not supported (`CK00001`) | Outer [WALK/TRAIL/SIMPLE/ACYCLIC](patterns.md#path-modes) execute; LIMIT cannot bound WALK |
| Questioned paths, nested quantifiers and subpath assignments/WHERE | Recognized, not supported (`CK00001`) | [Quantified edges/subpaths](patterns.md#quantified-paths) execute |
| Stored composite property values | Recognized, not supported (`CK00001`) for syntax; `CG03003` for parameters | MATCH/INSERT/CREATE/SET stored properties stay scalar-only |
| List/pattern comprehensions and slicing | Recognized, not supported (`CK00001`) | `[x IN xs | x]`, pattern comprehensions, `xs[1..3]` |
| Projection after deletion | Recognized, not supported (`CK00001`) | `DELETE a RETURN a` |
| Procedure calls | Recognized, not supported (`CK00001`) | `CALL` |
| Server, database, graph, or schema selection | Recognized, not supported (`CK00001`) | `USE`, `CREATE DATABASE`, `CREATE GRAPH`, `DROP GRAPH`, `ALTER SCHEMA`, `SHOW DATABASES`, `SESSION SET GRAPH` |
| Transaction options | Rejected (`CG00002`) | WORK, TRAN, READ ONLY/WRITE, isolation and DEFERRABLE suffixes on BEGIN/START |
| Transaction extensions | Unsupported | Savepoints and composed controls; standalone BEGIN [TRANSACTION], START TRANSACTION, COMMIT and ROLLBACK execute in process and over the wire |
| Cypher doubled-dash arrows | Rejected (`CG00008` or `CG00002`) | ISO `->`, `<-`, `-`, `<->` execute; see [Patterns](patterns.md) |
| Tilde edge forms | Recognized, not supported (`CK00001`) | Only directed stored relationships, matched in either orientation where requested |
| Direct stored-property chain | Recognized, not supported (`CK00001`) | `a.b.c`; nested map member access executes |
| Multiple statements in a request | Not in the language | Two statements separated by semicolons |
| Catalog filtering or projection | Not in the language | `SHOW LABELS RETURN label` |
| Catalog mutation composition | Not in the language | `SHOW LABELS DELETE n` produces `CG00007` |

`GqlLanguageProfile` declares core scalars, CAST, count, sum, avg, min, max, collect_list and collect. Its larger reserved keyword table is a lexical
inventory, not a list of implemented features. Malformed or invalid composition generally produces
`CG00002`, while catalog mutation composition has its dedicated read-only error.

## Engine and delivery limits

GQL path selectors, including ANY SHORTEST, remain CK00001. Minimum-hop searches are available
through the typed [GraphDatabase API](../../../dotnet-apis/resources/database/assimalign-cohesion-database-graph/graph-database.md#shortest-distance-and-path).

GQL cannot select another graph or database. Database lifecycle uses the engine API, explicit
transactions use standalone ISO text controls or the typed embedded session API, and label/type/index management uses the session-bound schema API.

GraphDatabaseServer serves scalar projections, mutations, SHOW, EXPLAIN and path queries.
ExecutePaths accepts one bare node, relationship or path binding from a read-only composed query, including
DISTINCT or star over one named binding. Computed, multiple and multi-binding star projections
report CG05001. A path binding supplied only by optional clauses also reports CG05001 during
planning; a mandatory clause must bind or constrain it. Execute accepts scalar and negotiated composite columns; entity results report CG05001 with
the ExecutePaths direction before reads, writer locks or mutations, including empty matches.
EXPLAIN descriptions remain scalar even for entity returns. GraphClient provides pooled connections,
materialized scalar/list/map results and single-use path streams. RETURN arithmetic/parameters, DISTINCT,
star and standalone evaluation execute. [Core scalar calls](functions/scalar-functions.md) and [CAST](expressions/cast.md) execute. LIKE, BETWEEN, EXISTS and quantified
predicates remain excluded; [value expressions](expressions/operators.md) run in WHERE and RETURN.

Graph-specific security policies, replication, compiled-schema provisioning, configurable graph
collation, and large property chunking are deferred or out of scope in the engine design.
Serializable isolation is rejected.

## See also

Final lists of nodes/relationships remain outside wire Execute, even with graph.composite-values
(CG05001; #1662). Intermediate collect and UNWIND preserve entity kinds and can produce scalar
properties or a single mandatory final entity through ExecutePaths. A write anywhere in a composed
statement excludes ExecutePaths. [Linear composition](clauses/next.md) documents the source-ordered execution.

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
