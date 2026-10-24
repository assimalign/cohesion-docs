# Graph

The Graph engine stores property-graph nodes and relationships in database-bound sessions.

> **Status:** Partial. Embedded and wire scalar queries, mutations, catalog queries, EXPLAIN and path streams execute.

`Assimalign.Cohesion.Database.Graph` implements an embedded property-graph engine with durable
catalog metadata, adjacency indexes, node-property indexes, and a bounded Graph Query Language
(GQL) profile. The language targets a subset of ISO/IEC 39075:2024; it does not claim full conformance.

Model Catalog and Storage components ship inside their model assembly. Wire messages, codecs and
transferred values ship in the model Protocol package, shared by engine and client. Remote clients
reference contracts without engines. See [package/import/error migration](../model-project-migration.md).

## In this section

- **[Language (GQL)](language/index.md)** — matching, mutations, catalog queries, and language limits.

## Database and session model

`GraphDatabaseEngine` creates, opens, enumerates, and drops logical databases.
`GraphDatabase` exposes typed node, relationship, and traversal operations; each operation receives
an explicit `GraphDatabaseSession` belonging to that exact database. Session identity is checked even
when databases share an engine or name. Queries cannot select a different database.

`GraphSchema.Open(database, session)` provides label/type discovery, property metadata, ownership
enforcement, and node-property index creation. Labels, relationship types, property names, and
variable names use ordinal, case-sensitive identity. String predicates use ordinal comparison;
SQL collation settings do not apply.

## Transactions and traversal

Snapshot and ReadCommitted isolation are supported. Serializable is rejected. Explicit transactions
use the session API; GQL has no transaction-control syntax. Mutations join the session transaction,
and a mutation failure rolls back the owning logical transaction.

Cancellation observed before explicit commit starts leaves the transaction usable and commits
nothing. A started commit ignores caller cancellation and returns its real outcome. Automatic
commit takes over without the caller token after execution completes; execution and lock waits
remain cancellable. A lost wire connection can still lose a committed result. See
[the shared transaction boundary](../../dotnet-apis/resources/database/assimalign-cohesion-database-transactions/transaction-coordinator.md#commit-and-rollback).

An aborted explicit transaction refuses statements, BEGIN and COMMIT until rollback with
structured CG04007 and the original cause. Its in-process message is unprefixed; the wire
still starts with CG04007. Catalog snapshot conflicts are retryable aborts with that code;
deterministic catalog identity/name errors use CG03003. See [error handling](language/diagnostics.md).

GQL matches bounded relationship-unique trails, including [quantified edges/subpaths](language/patterns.md#quantified-paths)
and list-valued group variables. [Core scalar functions and CAST](language/functions/scalar-functions.md)
execute with plan-time signatures and a shared transaction-start clock. Typed `TraverseAsync` is a separate breadth-first
operation with an explicit maximum depth and a visited-node set; it excludes the starting node.
Both surfaces seek only the selected directions and catalog type IDs; self-loops appear once in
either-direction reads. TraverseAsync yields deterministic breadth-first results and has no
candidate-expansion cap. GQL retains its [physical/viable counting rules](language/patterns.md#remarks).

[MATCH and OPTIONAL MATCH](language/statements/match.md) compose in source order, joining shared
variables and multiplying unrelated patterns. An unmatched optional preserves its input with NULL
for new variables; its WHERE filters candidates before that extension. Later patterns anchor on
bound variables before scans or seeks. Path streams require a projected binding constrained by a
mandatory clause; bindings supplied only by optionals are refused during planning.

[ORDER BY](language/clauses/order-by.md) supports explicit NULL placement and stable ties.
[OFFSET/SKIP and LIMIT](language/clauses/limit-offset.md) page rows or supported path returns;
offset precedes limit. The output tail follows all mutations and DISTINCT, with full materialization
and existing match/candidate caps. Default NULL order is last ascending, first descending.

[Aggregates](language/functions/aggregate-functions.md) and [GROUP BY](language/clauses/group-by.md)
execute with implicit keys, RETURN aliases and GROUP BY (). COUNT is Int64 locally and on the wire;
SUM preserves exact integral/Decimal work and Decimal scale. Collection results negotiate
graph.composite-values. Grouping and paging do not reduce input match limits.

## Property and label updates

[SET](language/clauses/set.md) and [REMOVE](language/clauses/remove.md) update scalar properties,
replace/merge literal maps and add/remove node labels. Mutating RETURN carries affected counts
in process and on the wire. OPTIONAL NULL targets and pure no-ops count zero; repeated rows
update the current working copy and count each changed entity once.

Snapshot uses first-updater-wins. ReadCommitted mutations refresh after acquiring the writer
lock and replan before evaluation. Format-4 records keep prior versions visible to old snapshots;
older graph records/registrations are refused before logical recovery scrub with
[CG06017](language/diagnostics.md), requiring recreation with no upgrade path. The GQL profile executes 26 of
33 declared clauses.

Detected storage damage uses [CK06003](language/diagnostics.md), with database/page context
in DatabaseCorruptionException. An unsupported graph-record open also retains CG06017 and
CG06018. Reads fail instead of silently omitting damaged data.

## Storage and catalog

Each persistent database owns `graph.dat`, `graph.log`, and `graph.bak`. Shared kernel components
provide pages, journaling, transactions, and indexes. Catalog definitions remain discoverable after
the last corresponding graph element is deleted. Schema-owned definitions reject ad-hoc alterations
with `DatabaseObjectLockedException`.

Catalog lookups use in-memory identity and parent directories while preserving MVCC visibility;
dropping and recreating a name does not return its replacement for the old identity. Complete
UTF-8 metadata records, including names and owning schema, must fit 8,092 bytes. Overflow reports
CG02009 before publication. See [catalog design](../../dotnet-apis/resources/database/assimalign-cohesion-database-graph/catalog-design.md).

## Delivery boundaries

The embedded engine and GraphDatabaseServer execute the documented GQL subset. GraphClient rents
database-bound connections for materialized scalar/list/map rows and single-use path streams. Parameters
bind runtime scalar, list and map values in expressions; stored pattern properties stay scalar-only. [RETURN](language/clauses/return.md) supports
computed/aliased expressions, DISTINCT, star and standalone evaluation. Numeric comparisons,
IN and DISTINCT agree on exact represented values; arithmetic keeps its own promotion.
[CASE](language/expressions/case.md), [COALESCE](language/expressions/coalesce.md),
[NULLIF](language/expressions/nullif.md), [list/map values](language/expressions/list-values.md) and
[subscripts/member access](language/expressions/property-access.md) execute. Wire Execute validates
result kinds before reads or mutation. Protocol 2.1 negotiates diagnostics and graph.composite-values;
[GraphValueCodec](../../dotnet-apis/resources/database/assimalign-cohesion-database-graph-protocol/graph-value-codec.md)
provides the bounded value format. Constructed limits fail inside the statement transaction before
commit. Path payloads stay unchanged; path completion exposes
warnings through [GraphPathResult](../../dotnet-apis/resources/database/assimalign-cohesion-database-graph-client/graph-path-result.md).
Explicit wire transactions remain unsupported. Graph security-policy integration, replication,
large property chunking and compiled-schema provisioning remain outside this delivery.

Path frames are sent in bounded pooled batches, with unchanged per-path bytes and a 16 MiB
payload limit. The client decodes each path once and hands it off in wire order through a
one-slot queue; the server still materializes matched paths. See
[wire delivery and phase counters](../../dotnet-apis/resources/database/assimalign-cohesion-database-graph/design.md#batched-path-delivery)
and [client lifetime/performance](../../dotnet-apis/resources/database/assimalign-cohesion-database-graph-client/design.md#path-lifetime).

Catalog/storage failures return coded ExecutionFailure and leave the authenticated session ready.
Execution-resource disposal uses CG09019; a disposed protocol transport faults the session
without another error write. Cleanup disposal after a successful autocommit preserves success
(except OutOfMemoryException) and emits Graph Warning event 13, StatementCleanupFailed.

## See also

[Linear composition](language/clauses/next.md) executes LET, FILTER, FOR/UNWIND, RETURN … NEXT
and Cypher WITH as eager stages. WITH WHERE filters after paging. Intermediate entity lists may
be unwound, but final entity lists remain unavailable over the wire (CG05001; #1662).

- **[Database](../index.md)** — engine navigation.
- **[Database overview](../overview.md)** — shared database model.
- **[Database APIs](../../dotnet-apis/resources/database/index.md)** — resource reference.
- **[SQL language](../sql/language/index.md)** — the separate relational dialect.

## Database lifecycle

Create/open require the original graph.dat, graph.log and graph.bak file set. Missing sidecars
are refused with CG06018 and never recreated, including for an empty closed database. Preserve
ambiguous files and restore the complete backup. A directory without data or a data file no larger
than one page is an incomplete create; its refusal directs DropDatabaseAsync then CreateDatabaseAsync.
Drop deletes the data file first. Recovery tolerates a torn uncommitted extension at the tail;
transient I/O failures ask for an open retry after resolution, without restore advice.

See [lifecycle APIs and examples](../../dotnet-apis/resources/database/assimalign-cohesion-database-graph/design.md#database-creation-open-and-drop)
and [file diagnostics](language/diagnostics.md#database-files).

## Engine health
Background write-back, group-commit flush, version purge and checkpoint work run per database
on capped lanes: 64 non-stuck operations and at most 128 lane threads per engine/worker kind.
Admission is round-robin; stuck I/O releases its admission slot after four worker intervals,
clamped to 1–4 seconds, while keeping storage ownership. Idle checks dispatch no lane; parked
runners retire after 30 seconds. Saturation/stuck observations leave databases serving and emit
root warning 35; disposal joins in-flight I/O and emits warning 36 periodically. See
[background worker limits and operations](../../dotnet-apis/resources/database/assimalign-cohesion-database/design.md#maintenance-lane-isolation).

Persistent version-purge refusals are reported once after three actual attempts spanning at least
one second on the engine clock. They set engine Faulted while the database stays in service and
HasFailingWorker remains false for the observation. ForeignStorage refusals report immediately as
operational failures. See [purge behavior](../../dotnet-apis/resources/database/assimalign-cohesion-database-graph/design.md#version-purge-refusals)
and [shared engine health](../overview.md#engine-health-and-version-purge).

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/src/Assimalign.Cohesion.Database.Graph.Client.csproj`.
