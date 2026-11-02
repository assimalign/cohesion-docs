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
| MERGE variable-length/quantified paths | Rejected (`CG00002`) | [MERGE](statements/merge.md) executes one complete fixed path with ON CREATE/ON MATCH SET |
| MERGE bound relationship or decorated repeated/bound node | Rejected (`CG03007` / `CG03006`) | Reuse bound nodes as bare variables; declare new relationships |
| Whole-map parameter/expression assignment | Recognized, not supported (`CK00001`) | SET n = $p, SET n += properties(other); literal whole maps execute |
| EXCEPT, INTERSECT, OTHERWISE | Recognized, not supported (`CK00001`) until GQ-Q2 | [UNION/UNION ALL](clauses/union.md) execute |
| Unsupported scalar/statistical calls | Recognized, not supported (`CK00001`) | `ELEMENTS()`, temporal constructors, `LOG10`, `STDDEV_SAMP`, `STDDEV_POP`, `PERCENTILE_CONT`, `PERCENTILE_DISC`; six [aggregates](functions/aggregate-functions.md) execute |
| HAVING | Recognized, not supported (`CK00001`) | GROUP BY executes; post-group HAVING is excluded |
| Map projection | Recognized, not supported (`CK00001`) | `RETURN a {.name}` |
| Nested modes/selectors; unselected unbounded WALK | Recognized, not supported (`CK00001`) | Outer modes execute; selected unbounded WALK executes under budgets; LIMIT cannot bound an unselected WALK |
| Counted ANY | Recognized, not supported (`CK00001`) | [ANY/ALL/SHORTEST selectors and inline WHERE](path-selectors.md) execute |
| Questioned paths, nested quantifiers and subpath assignments | Recognized, not supported (`CK00001`) | [Quantified edges/subpaths](patterns.md#quantified-paths) execute; inline WHERE executes |
| Stored composite property values | Recognized, not supported (`CK00001`) for syntax; `CG03003` for parameters | MATCH/INSERT/CREATE/SET stored properties stay scalar-only |
| Full-query EXISTS bodies, parenthesized EXISTS operands, COUNT/COLLECT blocks and bare pattern predicates | Recognized, not supported (`CK00001`) | [Braced pattern EXISTS](expressions/exists.md) and NOT EXISTS execute |
| Pattern-comprehension extensions | Outside the form | Explicit modes/selectors and parenthesized quantified groups; [pattern comprehensions](expressions/pattern-comprehensions.md) execute |
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

GQL [selectors](path-selectors.md), including ANY SHORTEST, execute per ordered endpoint pair
under statement budgets and the 64-hop backstop. Minimum-hop searches are also available
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
star and standalone evaluation execute. [Core scalar calls](functions/scalar-functions.md) and [CAST](expressions/cast.md) execute. LIKE, BETWEEN and quantified comparison
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
