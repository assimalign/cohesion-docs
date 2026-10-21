# Assimalign.Cohesion.Database.Storage

The physical storage kernel of the Cohesion Data Platform: fixed-size pages, a buffer pool with pin-counted caching, slotted-page record layout, a free-space map, and the journal (write-ahead log) that provides durability.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[StorageJournal](storage-journal.md)** — Concurrent appends, durability prefixes and owned sequential payloads.
- **[StoragePageLatch](storage-page-latch.md)** — Shared/exclusive structure holds and cancellable read acquisition.
- **[StorageCorruptionException](storage-corruption-exception.md)** — COHDBS003, page context and wrapped-cause discovery.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

The physical storage kernel of the Cohesion Data Platform: fixed-size pages, a buffer pool with
pin-counted caching, slotted-page record layout, a free-space map, and the journal (write-ahead log)
that provides durability. Every database model (SQL, Documents, Graph, Blob, KeyValuePair) composes
this project for its on-disk representation. Model-specific layouts live in the `src/Storage/`
components of their engine assemblies, with their existing namespaces.

## Scope

- **Pages** — 8 KiB `Page` unit with a 96-byte header (id, LSN, CRC-32C checksum, type,
  flags, slot bookkeeping), `SlottedPage` variable-length record layout, `PageSlot`
  directory entries.
- **Buffer pool** — Internal pin/unpin caching over a `StorageStream`;
  checksum stamped on write-back, verified on load.
- **Page management** — `StoragePageManager` allocation/free/retrieval/flush;
  `StorageFreeSpaceMap` allocation tracking, rebuilt from verified page bytes on open.
- **Records** — `Storage` abstract base with insert/read/update/delete over slotted
  pages and `StorageUnitIterator` full scans.
- **Journal** — [StorageJournal](storage-journal.md) write-ahead logging with begin/commit/rollback
  markers, CRC-protected frames, and recovery replay of committed operations.
- **File set** — each storage instance owns three streams: data (`.dat`), journal
  (`.log`), and backup (`.bak`), wrapped by `StorageStream`.

Storage format 4 requires entirely zero Free pages and checksummed Reclaimed pages. Nonzero
pooled pages never bypass checksum verification; raw CRC zero is escaped as `0xFFFFFFFF`.
There is no upgrade path from older formats. See [format and recovery](design.md#storage-format-4)
and [checkpoint anchors](design.md#checkpoints) for power-interruption handling. Readers can
begin, pin cached pages and end during checkpoint flush I/O; writers still wait through the
checkpoint. The latency probe is opt-in with COHESION_DB_PROBES=1 and sets no timing threshold.

## Dependencies

This kernel references FileSystem and FileSystem.Physical. Consumers include Database.Transactions
(WAL binding), Database.Indexing (index pages), and the Storage components inside model engines.

## Usage

Models derive a thin facade from `Storage` (see the `Sql.Storage` namespace in `Assimalign.Cohesion.Database.Sql` for the
canonical example):

See the [source-backed usage examples](examples/index.md).

See [DESIGN.md](design.md) for the architecture and the decisions behind it.

## Large streamed records

Model packages can chain records larger than a page while retaining the shared kernel. Deleting a
record releases its page transactionally once its last live slot disappears.
[StorageJournal.ReadSequential](storage-journal.md#sequential-reads-and-payload-ownership) streams
records with owned payloads. Storage recovery uses an internal borrowed reader and combines
checksum verification, commit classification and page planning during initialization, followed by
ordered replay. ReadAll returns a materialized snapshot. Appends can proceed during a durable
flush; failed writes/fsync still take the database offline. See [design](design.md) for replay and reclamation.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.FileSystem` | `CohesionProjectReference` |
| `Assimalign.Cohesion.FileSystem.Physical` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/Assimalign.Cohesion.Database.Storage.csproj`.
