# Unsupported GQL features

The executable GQL subset rejects recognized features outside its bounded profile.

> **Status:** Partial. This matrix records the current executable profile and delivery limits.

## Capability matrix

These constructs are recognized but produce the capability diagnostic rather than being executed.
The examples below are rejected forms, not executable syntax diagrams.

| Construct | Support | Rejected form |
|---|---|---|
| Mandatory keyword | Recognized, not supported (`COHDBL001`) | `MANDATORY MATCH` |
| Sorting and pagination | Recognized, not supported (`COHDBL001`) | `ORDER BY`, `LIMIT`, `OFFSET`, `SKIP` |
| Intermediate projection and chaining | Recognized, not supported (`COHDBL001`) | `WITH`, `NEXT` |
| Property updates and merge | Recognized, not supported (`COHDBL001`) | `SET`, `REMOVE`, `MERGE` |
| Set operations | Recognized, not supported (`COHDBL001`) | `UNION` |
| Functions and aggregation | Recognized, not supported (`COHDBL001`) | `count(a)`, `abs(a.age)` |
| Map projection | Recognized, not supported (`COHDBL001`) | `RETURN a {.name}` |
| Quantified paths | Recognized, not supported (`COHDBL001`) | `[r*1..3]`, `->{1,3}` |
| Collection values outside IN | Recognized, not supported (`COHDBL001`) | List/map parameters and nested maps |
| Projection after deletion | Recognized, not supported (`COHDBL001`) | `DELETE a RETURN a` |
| Procedure calls and unwinding | Recognized, not supported (`COHDBL001`) | `CALL`, `UNWIND` |
| Server, database, graph, or schema selection | Recognized, not supported (`COHDBL001`) | `USE`, `CREATE DATABASE`, `CREATE GRAPH`, `DROP GRAPH`, `ALTER SCHEMA`, `SHOW DATABASES`, `SESSION SET GRAPH` |
| Transaction statements | Recognized, not supported (`COHDBL001`) | `BEGIN TRANSACTION`, `COMMIT` |
| Bare relationship arrows | Not in the language | Bracketed relationships are required |
| Nested property references | Not in the language | More than one property-access step |
| Multiple statements in a request | Not in the language | Two statements separated by semicolons |
| Catalog filtering or projection | Not in the language | `SHOW LABELS RETURN label` |
| Catalog mutation composition | Not in the language | `SHOW LABELS DELETE n` produces `GQL0007` |

`GqlLanguageProfile` has an empty function table. Its larger reserved keyword table is a lexical
inventory, not a list of implemented features. Malformed or invalid composition generally produces
`GQL0002`, while catalog mutation composition has its dedicated read-only error.

## Engine and delivery limits

GQL cannot select another graph or database. Database lifecycle uses the engine API, explicit
transactions use the session API, and label/type/index management uses the session-bound schema API.

GraphDatabaseServer serves scalar projections, mutations, SHOW, EXPLAIN and path queries.
ExecutePaths accepts one bare node, relationship or path binding from read-only MATCH, including
DISTINCT or star over one named binding. Computed, multiple and multi-binding star projections
report COHDBG001. A path binding supplied only by optional clauses also reports COHDBG001 during
planning; a mandatory clause must bind or constrain it. Execute accepts statically scalar columns; entity results report COHDBG001 with
the ExecutePaths direction before reads, writer locks or mutations, including empty matches.
EXPLAIN descriptions remain scalar even for entity returns. GraphClient provides pooled connections,
materialized scalar results and single-use path streams. RETURN arithmetic/parameters, DISTINCT,
star and standalone evaluation execute. Function calls, LIKE, BETWEEN, EXISTS and quantified
predicates remain excluded; [value expressions](expressions/operators.md) run in WHERE and RETURN.

Graph-specific security policies, replication, compiled-schema provisioning, configurable graph
collation, and large property chunking are deferred or out of scope in the engine design.
Serializable isolation is rejected.

## See also

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
