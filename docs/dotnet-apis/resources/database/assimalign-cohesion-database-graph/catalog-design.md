# Graph catalog design

This page describes the catalog component inside `Assimalign.Cohesion.Database.Graph`.

> **Status:** Implemented.

Namespace: `Assimalign.Cohesion.Database.Graph.Catalog`. Assembly: `Assimalign.Cohesion.Database.Graph.dll`. NuGet package: `Assimalign.Cohesion.Database.Graph`.

This catalog component ships inside the model package. Keep its namespace
imports and replace the former satellite package reference with `Assimalign.Cohesion.Database.Graph`; recompile consumers.
The model enforces engine → Catalog → Storage → shared kernel through architecture tests.

## Intent and dependencies

The catalog gives each logical graph database a durable vocabulary. A label or relationship type has
a stable GUID identity and an ordinal, case-sensitive name. Property and index metadata references
the stable identity, so dropping and recreating the same name does not reuse old child metadata.
Identities cannot be reused across definitions or renamed. Label names and relationship-type names
occupy separate namespaces. Index and constraint names share one database-wide ordinal,
case-sensitive namespace. The writer lock serializes name reservations through commit/rollback.
A duplicate current name uses CG03008; a stale Snapshot writer retains CG04007.

A dropped identity cannot name another definition until version purge reclaims its old records.
Older databases can contain dropped/live names sharing an identity; candidate lookup checks every
visible version and pruning one name preserves the other.

The Graph engine depends on this catalog. The catalog depends on `Graph.Storage`'s raw metadata record
operations and on the shared transaction kernel; it does not use engine types. `Graph.Storage`
owns physical node-property B+Trees, while catalog index records supply names and object discovery.
The engine coordinates both through the same logical transaction.

The dependency diagram shows component and shared-package directions.

```mermaid
flowchart LR
    Engine["Graph engine"] --> Catalog["Graph Catalog component"]
    Catalog --> Store["Graph Storage component"]
    Catalog --> Tx["Database.Transactions"]
    Catalog --> Root["Database root ownership"]
    Store --> Kernel["Database.Storage"]
```

## Public seam and ownership

`GraphCatalog.Open` returns the sealed `GraphCatalog`; codecs are internal. Every
mutation takes the caller's `TransactionContext`, and none commits it. The caller holds the
database definition lock to serialize DDL and detect conflicting writes before publication. Reads
use the caller's snapshot.

After acquiring that lock, each catalog mutation compares the snapshot's physical metadata reference
with the latest committed reference. A changed definition, property, or index raises a graph catalog
write conflict; changing a child also checks its parent. Dropping a definition compares its entire
child set, preventing a stale snapshot from leaving newly committed child metadata behind. Schema
ownership is checked against the latest definition, so an older snapshot cannot bypass a newly
applied ownership marker. Concurrent first writes of the same label therefore serialize and the
stale writer must retry its transaction.

An initial label or relationship-type definition may explicitly carry `Schema` ownership and a
nonblank schema name. Once visible, every save or drop of that definition refuses with
`DatabaseObjectLockedException`. Its operation is `ALTER LABEL`, `DROP LABEL`,
`ALTER RELATIONSHIP TYPE`, or `DROP RELATIONSHIP TYPE`. Property and index mutations also consult
the parent and use its ALTER operation. This is an enforcement seam, not compiled schema
provisioning. Data writes that use a label do not alter its definition.

Property metadata supports an optional shared `DatabaseType` and a required flag. The engine uses
these declarations for schema checks; the catalog does not inspect node content. Indexes carry a
name and property key, and are nonunique. The physical tree identity and registration stay entirely
in `Graph.Storage`. A catalog index save cannot silently retarget an existing name to another
property.

Named [GraphConstraintMetadata](graph-constraint-metadata.md) rules identify a parent GUID and
property, with an optional type. A null Type denotes existence; type rules permit absent/null
values. Creating through GraphSchema streams existing owner data and refuses a violation with
CG03010. Duplicate rule kinds or incompatible type rules use CG03009. Named and identical
unnamed rules may coexist independently. GetDeclaredPropertyKeys excludes named effects;
GetPropertyKeys and SHOW report effective rules. Independently dropping a constraint preserves
the other rules and the discovered property key.

## On-disk format

Catalog records occupy `Graph.Storage` owner 0 pages. Graph data, adjacency, and physical index
registrations occupy other owners. Each record begins with the shared 16-byte stamp prefix: an
unsigned 64-bit little-endian creator sequence at offset 0, and an unsigned 64-bit little-endian
deleter sequence at offset 8. Zero means no deleter. The shared record-space adapter scrubs these
records with all graph data during recovery.

The payload begins at byte 16:

| Offset / order | Encoding | Meaning |
| --- | --- | --- |
| 16 | byte | Kind: 1 label, 2 relationship type, 3 property key, 4 named index, 5 named constraint |
| 17 | byte | Format version, currently 2 for every kind |
| 18 | 16 bytes | CLR `Guid.ToByteArray()` representation: definition ID for kinds 1/2, parent ID for kinds 3/4/5 |
| 34 | string | Definition name, property key, index name, or constraint name |
| Following name, kinds 1/2 | byte, string | `DatabaseObjectOwner` value and owning schema |
| Following name, kind 3 | byte, byte | Shared `DatabaseType` value (255 means unconstrained), required flag (0 or 1) |
| Following name, kind 4 | string | Indexed scalar property key |
| Following name, kind 5 | string, byte | Property key and DatabaseType (255 denotes existence) |

Only catalog v2 is admitted. Earlier/future record versions refuse with CG06019 before logical
recovery scrub, with no legacy reader or upgrade path; recreate the database. Physical journal
redo runs before this gate. Empty catalogs have no versioned records to refuse. The graph
entity/tree format remains 5.

All fixed-width numeric values are little-endian. Strings use a signed 32-bit little-endian UTF-8
byte length followed by exactly that many strict UTF-8 bytes. Length -1 represents null, accepted
only for owning schema. Definition IDs must be nonempty; names and index keys must be nonblank. Only
schema-owned definitions may carry a schema name, and they must carry one. Unknown kinds, versions,
enum values, invalid lengths, malformed UTF-8, and trailing bytes are rejected with
`GraphCatalogException`. Decoding has no reflection or dynamic serialization.

Encoding validates the complete UTF-8 record against the 8,092-byte SlottedPage.MaxRecordSize
before physical apply. Names, owning schema and index property text all count, including headers
and lengths; this is not a character-count limit. Exact fit is accepted. Overflow throws
GraphElementTooLargeException at the catalog and becomes structured CG02009 through the engine.

## MVCC, mutation, and recovery

Opening scans only owner 0 and builds an in-memory directory keyed by kind, parent, and name. The
directory holds physical references and creator stamps, not mutable metadata copies. Every read
reloads the record and checks identity, creator, and deleter visibility against the snapshot.
Reclaimed slots and reused identities invalidate cached references; corrupt live records propagate
an error rather than appearing absent.

Saving a version tombstones the old version and inserts the new one inside one shared physical
statement bracket. Both changes register with the coordinator's version store. Deleting a definition
tombstones its property and index records in that same bracket. A failed bracket publishes no
directory references; logical rollback removes created versions and restores deleters. Snapshot
readers continue to see their original definitions until they leave.

Recovery order is storage open, coordinator analyze-and-scrub, catalog open, then coordinator
complete-recovery. The graph store separately scrubs physical index entries before the recovery
checkpoint. Catalog crash tests copy live data and WAL before disposal, proving that committed
definitions survive and partial definition/child changes disappear.

Identity discovery uses an in-memory GUID-to-definition-key directory. Property/index discovery
uses ordinal sorted names selected by kind and parent identity. Both are built at open and
maintained with the version directory under its lock. They select candidates only: reads still
reload/decode records and apply the same MVCC checks. Stale references prune lazily, and old
snapshots keep the original definitions after a dropped name is recreated with another identity.
[FindLabel(Guid, snapshot) and FindRelationshipType(Guid, snapshot)](graph-catalog.md) use these
directories. Saving N definitions takes O(N log N) or better catalog work for bounded version
chains, including property discovery. Whole-kind SHOW still enumerates that kind. No disk-format
change is involved. This is a metadata directory; traversal uses graph adjacency and physical indexes. Catalog
disposal and flushing remain owned by the engine and its shared storage/coordinator.

Through engine operations, snapshot-changed definitions become DatabaseTransactionAbortedException
with CG04007 and the catalog cause. Deterministic identity reuse/rename, name/identity mismatch,
index retargeting or missing owner becomes DatabaseException with CG03003. Malformed records
use CG06006. Direct catalog calls retain GraphCatalogException; see
[exception boundaries](graph-catalog-exception.md).

## Scope

The catalog supplies no server-scope names, security checks, wire client, replication, or compiled
schema provisioning. Graph index create/drop must coordinate catalog metadata with the corresponding
physical store change; publishing only one side is not an engine operation.


## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Assimalign.Cohesion.Database.Graph.csproj`.
