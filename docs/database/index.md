# Database

Cohesion Database provides independent model engines over a shared durable storage and transaction kernel.

> **Status:** Partial. SQL, Document, Graph, Key-Value Pair, and Blob execute; Cache is deferred.

## Start here

- **[Overview](overview.md)** — Engine families, shared components, host composition, and getting started.
- **[SQL](sql/index.md)** — Relational engine and the measured SQL language reference.
- **[Document](document/index.md)** — Document collections and the OQL language subset.
- **[Graph](graph/index.md)** — Property graphs and the GQL language subset.
- **[Key-Value Pair](key-value-pair/index.md)** — Ordered byte keys, commands, and conditional writes.
- **[Cache](cache/index.md)** — Deferred cache model.
- **[Blob](blob/index.md)** — Containers and streamed object storage.

## SQL

The SQL engine plans and executes statements against typed tables, a durable catalog, and shared
indexes. Its declared language includes a measured subset of joins, grouping, aggregates,
subqueries, collation, row mutations, schema changes, and session transactions. The SQL server and
typed client carry the same bounded surface over a model-owned wire family.

## Document

The Document model stores versioned JSON documents in collections. Its Object Query Language (OQL)
subset supports queries and index definition over nested document shapes. Collection operations
use database-bound sessions and the shared transaction and storage components.

## Graph

The Graph model stores nodes, relationships, and properties with durable adjacency and property
indexes. Its Graph Query Language (GQL) subset provides bounded traversal, projection, mutation,
and catalog operations; it does not claim complete standard GQL coverage.

## Key-Value Pair

The Key-Value Pair engine keeps an ordered space of opaque byte keys and values. It provides point
operations, ordered scans, and entity-tag conditional writes, plus its model server and client.
Keys compare by unsigned lexicographic byte order.

## Cache

> **Status:** Not yet implemented. The Cache package is a placeholder outside the MVP, deferred behind Key-Value Pair.

Cache is listed separately in the resource navigation, but the current source does not supply a
working cache engine. Use its section for the explicit status rather than inferring cache behavior
from the key-value model.

## Blob

The Blob engine stores named streamed objects within containers. Chunked storage, atomic upload
publication, version-retaining readers, metadata, and prefix listing use the shared kernel.
The model also supplies a transport-neutral server and typed streaming client; it has no statement
query language.

## API reference

[Database .NET APIs](../dotnet-apis/resources/database/index.md) describes the resource packages.
[Documentation home](../index.md) returns to the site navigation.

## Sources

- **Area map and engine status** — `cohesion/resources/Database/README.md`
- **Architecture** — `cohesion/docs/resources/Database/DESIGN.md`
- **SQL contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Language/docs/DIALECT.md`
- **Document engine** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/OVERVIEW.md`
- **Graph engine** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/OVERVIEW.md`
- **Key-value engine** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/OVERVIEW.md`
- **Blob engine and wire surface** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/docs/OVERVIEW.md`
- **Cache deferral** — `cohesion/docs/programs/DATABASE_MVP_FEATURES.md`

