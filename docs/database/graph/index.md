# Graph

The Graph engine stores property-graph nodes and relationships in database-bound sessions.

> **Status:** Partial. Embedded queries and catalog wire queries are implemented; general graph wire queries are not.

`Assimalign.Cohesion.Database.Graph` implements an embedded property-graph engine with durable
catalog metadata, adjacency indexes, node-property indexes, and a bounded Graph Query Language
(GQL) profile. The language targets a subset of ISO/IEC 39075:2024; it does not claim full conformance.

## In this section

- **[Language (GQL)](language/index.md)** — matching, mutations, catalog queries, and language limits.

## Database and session model

`GraphDatabaseEngine` creates, opens, enumerates, and drops logical databases.
`IGraphDatabase` exposes typed node, relationship, and traversal operations; each operation receives
an explicit `IDatabaseSession` belonging to that exact database. Session identity is checked even
when databases share an engine or name. Queries cannot select a different database.

`GraphSchema.Open(database, session)` provides label/type discovery, property metadata, ownership
enforcement, and node-property index creation. Labels, relationship types, property names, and
variable names use ordinal, case-sensitive identity. String predicates use ordinal comparison;
SQL collation settings do not apply.

## Transactions and traversal

Snapshot and ReadCommitted isolation are supported. Serializable is rejected. Explicit transactions
use the session API; GQL has no transaction-control syntax. Mutations join the session transaction,
and a mutation failure rolls back the owning logical transaction.

GQL matches finite relationship-unique trails. Typed `TraverseAsync` is a separate breadth-first
operation with an explicit maximum depth and a visited-node set; it excludes the starting node.
The two traversal surfaces have different result and repetition rules.

## Storage and catalog

Each persistent database owns `graph.dat`, `graph.log`, and `graph.bak`. Shared kernel components
provide pages, journaling, transactions, and indexes. Catalog definitions remain discoverable after
the last corresponding graph element is deleted. Schema-owned definitions reject ad-hoc alterations
with `DatabaseObjectLockedException`.

## Delivery boundaries

The embedded engine executes the documented GQL subset. `GraphDatabaseServer` exposes only the
catalog `SHOW` statements over the wire. Path codecs exist, but the current server rejects
`ExecutePaths`. General graph wire queries, a Graph wire client, graph security-policy integration,
replication, large property chunking, and compiled-schema provisioning are outside this delivery.

## See also

- **[Database](../index.md)** — engine navigation.
- **[Database overview](../overview.md)** — shared database model.
- **[Database APIs](../../dotnet-apis/resources/database/index.md)** — resource reference.
- **[SQL language](../sql/language/index.md)** — the separate relational dialect.

## Sources

- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Catalog/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Storage/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph.Client/src/Assimalign.Cohesion.Database.Graph.Client.csproj`.

