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

Before **each** reclamation bracket, including between owners in one pass, cleanup yields to a
due or pending checkpoint. Only an already-running bracket can add WAL beyond the configured
checkpoint threshold. Once that bracket commits, it wakes the checkpointer if a checkpoint is
due; yielding polls do not send repeated wakes. The size trigger counts appended bytes since the
last completed checkpoint, excluding the checkpoint's own record, so even a one-byte threshold
can progress. Verbose SQL event 27, TableReclamationYielded, reports a yielding drain poll.

This applies to cleanup after every DROP, TRUNCATE and rewrite, including abandoned prepared
owners and retired owners after successful joint publication. Snapshot/reader fences and page
reuse rules still apply. Write-back, truncation and allocation policies are unchanged.

## Joint migration space admission

Build checks only a small, declaration-sized space floor before saving a joint TYPE intent.
It takes no owner-directory snapshot, visits no data/index page and decodes zero rows. The floor
is `F = 1 MiB + (ceil(I / 3072) + 4) * (5B + 168)`, where I is the serialized intent's UTF-8 byte
length and B is 8,192 bytes. A floor refusal reports `CS03402` online and preserves a kept crash
intent without writing it.

Before any prepared write, the worker measures actual heap slots, encoded record/component bytes
and index pages. It projects conversion growth, rather than charging declared VARCHAR/TEXT maxima.
For example, INT to TEXT/VARCHAR allows 11 rendered ASCII characters plus four encoding bytes per
non-NULL value; NULL remains one byte. CHAR adds possible padding. Application casts without a
known growth bound start at measured size, then remain subject to the copy watchdog. Unchanged
tables do not increase the estimated need. Resumed intents repeat worker admission.

Admission requires `N = R + J + ceil((R + J) / 4) + V + E`: replacement allocation R after reusable
free pages, bounded copy WAL J, a 25% estimate margin, metadata-derived publication allowance V
and engine reserve E. Copy-owned WAL forces a checkpoint at 16 MiB plus the next bracket's bound;
busy brackets retry the checkpoint with cancellation. These checkpoints briefly stall all database
writers under the apply gate, including writers on untouched tables.

The volume watchdog checks each index-root/copy bracket and checks again immediately before
catalog publication, after validation. It requires enough free space for the next bracket plus
E, or V plus E at publication. Bounds include measured index heights, possible splits, row bytes
and reusable free pages. Sparse growth also charges copy-owned unflushed allocations, capped at
the data pool capacity and reset by a successful forced checkpoint. A refusal stops with `CS03402`
and needed/available bytes, discards preparation and leaves the old schema online, subject to the
storage exceptions below. It retains headroom for cleanup after either failure or publication.

### Engine reserve and a worked calculation

The reserve is `E0 = F + W + K`, `E = E0 + max(1 MiB, ceil(E0 / 4))`. W covers reclamation WAL:
with B = 8,192, one 32-page bracket has bound `U = 32 * 2 * (B + 46) + 152 = 527384` bytes.
For checkpoint threshold C, `W = min(P * (B + 46) + 152 * (ceil(P / 32) + O), C + U)`;
if C is zero, use the whole first term. P is the larger old/prepared page bound, updated during
copying, and O is the larger retiring/prepared owner count.

K is `BufferPoolCapacity + 1 MiB` when data-file growth can be sparse, otherwise zero. Unix file
growth is conservatively sparse. Windows checks the actual file and containing directory:
compressed, sparse or integrity-stream attributes, UNC/network paths, and unknown/inaccessible
allocation modes count as sparse. Ordinary local Windows files use eager allocation. Memory
strategies have no volume limit. The margin covers cleared-page deltas, headers and metadata.

For a 3,072-byte serialized intent, DESIGN's floor formula gives
`F = 1048576 + (1 + 4) * (40960 + 168) = 1254216` bytes. This is only the Build floor.
As an illustrative worker calculation, if R = 8 MiB, J = 16 MiB, V = 2 MiB and E = 10 MiB,
admission needs `8 + 16 + 6 + 2 + 10 = 42 MiB`. E must be derived from the actual engine and
owners above; these sample terms are not a fixed migration allowance.

### Residual risk

Admission is an estimate, not a reservation. The final residual risk is other writers' WAL and
data allocations exceeding E's margin, `max(1 MiB,ceil(E0/4))`, during copying and during the
whole reclamation drain after a stop or publication. Other writers' WAL can accrue from C until
truncation while an in-flight, possibly stalled, reclamation bracket delays the checkpoint;
W bounds reclamation's own WAL, and E's margin absorbs the other writers' growth.
Cheap file/volume probes cannot establish every backing allocation policy: undetected
copy-on-write, deduplication, thin provisioning, remote-server backing policies, reparse-target
allocation modes and changes after probing remain risks, along with the engine-wide disk-full
behaviour that applies to every workload.

### Storage failures that still take the database offline

The joint path does not clear or weaken storage latches. Reads, conversion/constraint failures,
cancellation and prepared data-page write failures stay online when no independent latch fires.
The explicit offline exceptions remain:

| Failure | StorageOfflineCause |
| --- | --- |
| Unsafe journal append/frame/drain/flush, including ENOSPC, zero-byte failure and any possibly partial write; unconfirmed commit/durability wait | JournalFlush |
| Failed durable data-file flush | DataFlush |
| Failure after issuing a checkpoint header-slot write | HeaderWrite |
| Repeated page write-back, checkpoint, write-ahead-flush or version-purge failures reaching the engine failure window | PageWriteBackFailures, CheckpointFailures, WriteAheadFlushFailures, VersionPurgeFailures |
| Second consecutive failed checkpoint at the journal size cap | JournalSizeLimit |
| Non-TYPE deferred provisioning step failure | ProvisioningFailure |
| Interrupted open-time recovery | RecoveryInterrupted |

The joint path never requests ProvisioningFailure. Unknown WAL/commit outcomes cannot safely
retry online. An unconfirmed joint publication uses `CS03403`, retains new trees/directory records
and lets reopen/recovery decide old versus new; it does not claim the old schema is unchanged.
Other corruption/recovery fences keep their existing policies. See
[joint publication and recovery](design.md#joint-type-migrations) and
[operator health](../../../../database/sql/index.md#typed-migration-health-and-recovery).

## Identity and sequence reservations

SQL format 33 persists standalone and identity-owned definitions as catalog kind 18 and
reservations as kind 19. Kind 18 includes owning table id, column name and identity ownership;
kind 16 is retired and never decoded. Compiled schemas use v11. Format 32 and earlier SQL
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

- **Admission and reserve** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Provisioning/SqlSchemaProvisioner.JointSpace.cs`, `tests/SqlJointTypeMigrationTests.FourthPass.cs`, `.FifthPass.cs` and `.SixthPass.cs` at `0b00f181e`.
- **Offline causes** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/StorageOfflineCause.cs` and `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlJointTypeMigrationTests.EdgeFixes.cs`.
- **Shared allocator and crash recovery** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Catalog/SqlCatalog.Allocator.cs`, `tests/SqlSequenceTests.cs` and `tests/SqlSequenceReviewTests.cs`.

- **Completion check** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Internal/Engine/Storage/SqlTableReclamation.cs`.
- **Reuse races** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/tests/StorageStaleTouchTests.cs`.
- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/src/Assimalign.Cohesion.Database.Sql.csproj`.
