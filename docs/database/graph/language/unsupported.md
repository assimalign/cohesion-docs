# Unsupported GQL features

The executable GQL subset rejects recognized features outside its bounded profile.

> **Status:** Partial. This matrix records the current executable profile and delivery limits.

## Capability matrix

These constructs are recognized but produce the capability diagnostic rather than being executed.
The examples below are rejected forms, not executable syntax diagrams.

| Construct | Support | Rejected form |
|---|---|---|
| Optional or mandatory matching | Recognized, not supported (`COHDBL001`) | `OPTIONAL MATCH`, `MANDATORY MATCH` |
| Sorting and pagination | Recognized, not supported (`COHDBL001`) | `ORDER BY`, `LIMIT`, `OFFSET`, `SKIP` |
| Intermediate projection | Recognized, not supported (`COHDBL001`) | `WITH` |
| Property updates and merge | Recognized, not supported (`COHDBL001`) | `SET`, `REMOVE`, `MERGE` |
| Set operations and deduplication | Recognized, not supported (`COHDBL001`) | `UNION`, `DISTINCT` |
| Other predicates | Recognized, not supported (`COHDBL001`) | `OR`, `NOT`, `XOR`, `IN`, `IS NULL` |
| Functions and aggregation | Recognized, not supported (`COHDBL001`) | `count(a)`, `abs(a.age)` |
| Wildcard projection | Recognized, not supported (`COHDBL001`) | `RETURN *` |
| Quantified paths | Recognized, not supported (`COHDBL001`) | `[r*1..3]`, `->{1,3}` |
| Parameters | Recognized, not supported (`COHDBL001`) | `$age` |
| Collection literals | Recognized, not supported (`COHDBL001`) | `[1, 2]`, nested property maps |
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

The current `GraphDatabaseServer` accepts only catalog `SHOW` statements. Other graph queries
receive `ExecutionFailure` with `The graph wire server supports catalog SHOW statements only.`
Path codecs are present, but sending `ExecutePaths` to this server produces `ProtocolViolation`
and closes the session. A general graph query server/client remains outside this delivery.

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

