# Sql storage design

This page describes the storage component inside `Assimalign.Cohesion.Database.Sql`.

> **Status:** Partial.

The SQL model's storage layout component (area architecture: resources/`Database`/DESIGN.md
(`cohesion/docs/resources/Database/DESIGN.md`) §3.3 — every model brings a layout, never its own
paging/WAL). This component is deliberately thin: `SqlStorage` derives from the kernel's `Storage`
base and re-exposes the protected record operations as a public row-oriented surface. All hard
invariants (write-ahead ordering, recovery replay, buffer-pool behavior, per-owner record chains)
live in — and are documented by — `Assimalign.Cohesion.Database.Storage`.

Namespace: `Assimalign.Cohesion.Database.Sql.Storage`. Assembly: `Assimalign.Cohesion.Database.Sql.dll`. NuGet package: `Assimalign.Cohesion.Database.Sql`.

This storage component ships inside the model package. Keep its namespace
imports and replace the former satellite package reference with `Assimalign.Cohesion.Database.Sql`; recompile consumers.
The model enforces engine → Catalog → Storage → shared kernel through architecture tests.

## Why-this-not-that

- **A facade, not a layout engine.** SQL rows are "variable-length byte sequences
  addressed by (page, slot)" — exactly the substrate's slotted-page unit — so the
  SQL layout adds no page format of its own. The facade remains separate from engine logic (rather than
  the engine using `Storage` directly) because the base class's record operations
  are `protected` by design: models opt into the record surface they actually
  expose, and `SqlStorage` is where the SQL model does that, plus where a
  SQL-specific layout would grow if one ever became necessary.
- **Row placement is owner-chained.** `InsertRow(transaction, ownerId, row)` passes
  the owning table's object id through to the substrate's per-owner record chains,
  giving each table page locality: table scans (`GetUnitIterator(objectId)`) touch
  only that table's pages, and `DROP TABLE` releases the chain. The ownerless
  overloads write the shared (owner-zero) space — the catalog file set's layout,
  where kind-discriminated metadata records are few and scanned as a whole.
- **The journal is a public composition seam.** `WriteAheadJournal` returns the
  existing `IStorageJournal` contract, matching Documents, Graph, and Blob storage.
  It replaces the shipped-to-shipped friend grant and hands the
  engine's transaction coordinator the same journal the storage brackets write page
  images to, for the manager's journal-bound transaction log and open-time recovery
  analysis. `Open(..., checkpointOnOpen: false)` exists for the same reason — the
  engine analyzes the recovered journal before the truncating checkpoint destroys
  the records classification reads (see `ISqlStorageStrategy`). `Storage` retains
  ownership of the journal; composers coordinate journal writes and checkpoints
  through their transaction coordinator and must not dispose it independently.

## Table reclamation and page reuse

The SQL engine reclaims unreachable table owner chains in durable batches of at most 32 pages,
after its snapshot and statement-reader fences allow release. Shared storage rechecks the free
generation captured when its kernel lookup learned the page id. A free/reuse before first touch
refuses or skips that allocation, including same-owner reuse; stale inserts allocate afresh.
See [kernel reuse rules](../assimalign-cohesion-database-storage/design.md#transactional-owner-release).

FreeOwnerPages can skip stale snapshot entries, so reclaiming fewer than 32 pages is insufficient
to finish a table. After committing the batch, SqlTableReclamation also requires
`!DataStorage.HasOwnerPages(objectId)` before removing the durable catalog reclamation intent.
A crash after the data commit but before intent removal safely retries discovery. This changes
neither the page format nor the public API.

## Identity and sequence reservations

SQL format 29 persists standalone and identity-owned definitions as catalog kind 18 and
reservations as kind 19. Kind 18 includes owning table id, column name and identity ownership;
kind 16 is retired and never decoded. Compiled schemas remain v8. Format 28 and earlier SQL
formats are refused with no upgrade reader. See [catalog sequence objects](catalog-design.md#standalone-sequences).

Identity and sequences share a catalog-serialized allocator using Int128 intermediate arithmetic.
Before returning any value from a new block, it commits the durable endpoint through the configured
storage commit policy. Blocks contain at most 32 values and stop at the bound. CACHE is persisted
metadata and does not change this block size or create per-session caches.

Recovery skips the complete durable reservation, including values never handed out. Without
intentional CYCLE or restart, a crash does not reissue previously returned values. Synchronous and
Grouped policies confirm durability before delivery; None retains its process-crash-only guarantee.
For a default sequence starting at 1, returning 1 reserves through 32; reopening returns 33.
Allocation is independent of row transactions, so rollback leaves gaps. It does not advance the
catalog DDL generation. Exhaustion without CYCLE uses CS02002 (22003) without reserving or
advancing state; CYCLE wraps to the directional bound in a new durable reservation.

Create/replace/drop and allocation share the catalog monitor; a retired incarnation cannot allocate
(CS04067, 40001). The order is existing caller object grants, catalog monitor, storage transaction/
apply gate, then page latches. Allocation acquires no engine/coordinator lock and does not wait for
DML. Unconfirmed reservation commits fence storage until reopen. Disposal closes allocation under
the monitor before storage closes, including cached values. Malformed sequence records retain
CS06061 (XX001) through corruption translation.

## Index leaf utilization

SQL indexes use the shared [B-tree leaf split policy](../assimalign-cohesion-database-indexing/design.md#leaf-split-policy).
Rightmost appends and eligible single-value run ends keep leaves denser; other leaf splits use
the midpoint. The 5,000-entry, 1,024-byte-key ascending and duplicate tests measure about 78%
fill instead of 39%. Updates after an ascending load can require more splits and finish with
more leaves. This is an internal allocation policy and adds no storage format or tuning option.

## Error model

No exception types of its own: everything is the substrate's `StorageException` family. The SQL
engine translates at its model boundary per the area error policy.

## AOT posture

Inherits the substrate's posture (explicit-layout header overlays, span-based codecs); this component
adds no reflection and no serialization.

## Non-goals

- **No SQL semantics (types, visibility, catalogs)** — those live in `Database.Sql`,
  `Database.Sql.Catalog`, and the row codec inside the engine.
- **No bespoke page formats** — until a measured need exists; the slotted layout is the
  layout.


## Confirmed decoding and page reuse

Record reads confirm their private bytes against a page write sequence before accepting a result or reporting a decode failure. Overlapping forward writes and rollback cause a retry. Stable malformed records retain page/slot context and the decoder cause through storage corruption translation. The shared record layer compacts fragmented pages before extension and refills partly empty older pages through in-memory free-byte hints, rechecking allocation generation and capacity. These changes preserve the stored format. See [shared record-layer behavior](../assimalign-cohesion-database-storage/design.md#confirmed-reads-and-record-space-reuse).

## Sources

- **Shared allocator and crash recovery** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Catalog/SqlCatalog.Allocator.cs`, `tests/SqlSequenceTests.cs` and `tests/SqlSequenceReviewTests.cs`.

- **Completion check** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Engine/Storage/SqlTableReclamation.cs`.
- **Reuse races** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/tests/StorageStaleTouchTests.cs`.
- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Assimalign.Cohesion.Database.Sql.csproj`.
