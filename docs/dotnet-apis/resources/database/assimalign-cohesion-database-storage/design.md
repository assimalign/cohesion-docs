# Assimalign.Cohesion.Database.Storage design

This page describes the boundaries and implementation shape of `Assimalign.Cohesion.Database.Storage`.

> **Status:** Partial.

The physical layer of the Data Platform kernel (area architecture: resources/`Database`/DESIGN.md
(`cohesion/docs/resources/Database/DESIGN.md`) §3.2). This document records the design decisions
that shape the storage model; the program-level requirements it satisfies are R1 (ACID), R3 (shared
kernel), and R6 (NativeAOT) in the area design.

## Design intent

One model-agnostic physical substrate: fixed-size pages, a pin-counted buffer pool, a slotted-page
record layout, and a write-ahead journal. Model engines bring *layouts* (what bytes mean inside a
page body) but never their own paging, caching, or logging. The guardrails that keep this layer
trustworthy are structural: every page load is checksum-verified, every write-back is
checksum-stamped, and durability flows through the journal only — there are no side files.

## The page model

[StoragePageLatch](storage-page-latch.md) protects structure reads from writers and physical
rollback. Its EnterRead(CancellationToken) overload checks cancellation between approximately
10 ms waits and leaves no shared hold on cancellation, including a race with acquisition.
Non-cancelable acquisition retains the indefinite-wait behavior. B+Tree batches use this
overload and release every page/latch before returning a move.

- **8 KiB pages, 96-byte header.** The header layout (`Page.Header`) is an explicit
  struct persisted on disk, so field offsets are a wire contract: id (0), LSN (8),
  checksum (16), flags (20), type (21), slot count (22), free-data end (24), overflow
  size (28), reserved nonce/MAC space for encryption at rest (32–63), then the
  owner tag (64) driving per-owner record chains (72–95 reserved).
- **Free and reclaimed pages.** `PageType.Free` (0) requires an entirely zero-filled page.
  Freed pages use `PageType.Reclaimed` (10), with checksummed metadata. Open reads and
  validates the complete bytes of free/reclaimed pages before making them reusable.
  A zeroed first sector or a false Free header, even with a matching checksum, cannot
  silently remove a live page from its owner or expose it to allocation.
- **Page LSN.** Every page carries the LSN of the journal record that last modified it.
  This is the hook for the write-ahead rule (a page may not be written to the data
  stream until the journal is durable up to its LSN) and for idempotent recovery replay
  (apply a record only if it is newer than the page). The storage layer stores the
  field; the journal build-out (#160) enforces the rule.
- **Checksums on every read path.** CRC-32C over the full page with the checksum field
  zeroed. Stamped centrally in the buffer pool's write-back (the only path to the data
  stream), verified centrally in the buffer pool's load (the only path from it).
  Every nonzero pooled page requires a verified checksum. A stored zero is accepted only
  for an entirely zero page. A computed CRC of zero is stored and verified as `0xFFFFFFFF`;
  this escape aliases raw CRC values 0 and `0xFFFFFFFF`.

[Page](page.md) span accessors validate one snapshot of the reserved overflow body length before
constructing a span, including over caller memory. Valid lengths are 0..8,096 bytes; invalid
geometry throws StorageCorruptionException with page context rather than clamping. OverflowSize
means a body within one 8 KiB buffer; multi-page overflow storage is unimplemented. The new
length-taking pointer constructor refuses undersized memory before reading a header, and ignores
extra bytes. Default-page span access throws InvalidOperationException.

### Page 0 and alternating headers

Page 0 is outside the buffer pool. It has an immutable identity block and two alternating,
independently checksummed header slots. Open chooses the newest valid generation and reads its
checkpoint anchor chain. Its checksums are separate from pooled-page checksums.

Open reads page 0 in independent 4 KiB halves, completing short reads within each half. IOException,
StorageIOException or premature zero-byte read invalidates the entire affected half, including
partially read bytes. A valid other slot supplies identity/checkpoint state only if the verified
journal proves coverage before replay writes: its first retained LSN is positive and at or below
the selected slot's LsnFloor, or its first record is a checkpoint at LsnFloor + 1 with the same
complete anchor. A newer idle checkpoint with the same floor/anchor also qualifies; it can lose
only higher sequence reservations that stamped no records. Empty/torn prefixes or checkpoints with
a different floor/anchor prove nothing. Refusal rethrows the original header I/O error without
writing either file. A recovered leading half schedules identity repair on the next slot-0 write.

When no slot verifies and a completely read half establishes damage, page-0
[StorageCorruptionException](storage-corruption-exception.md) uses COHDBS003 and retains the other
half's read error as its inner cause. If both halves are unreadable, there is no checksum evidence
and the original I/O classification survives. Every accepted I/O fallback emits Warning
HeaderReadFallback (18), including consecutive opens. Sequence assignment resumes above the
verified header floor and retained journal maximum; page headers cannot reconstruct model sequence
stamps. The disk layout and storage format remain unchanged.

### Storage format 4

`StorageFileHeader.CurrentFormatVersion` is 4. Older and newer storage formats are refused with
`StorageFormatException` / COHDBS001 before reading the journal; there is no upgrade or legacy
reader. This kernel version is separate from SQL data/catalog format 25 and Graph record format 4.

The changed public members are:

```csharp
// Assimalign.Cohesion.Database.Storage
public unsafe struct StorageFileHeader
{
    public const int CurrentFormatVersion = 4;
    // Other members omitted.
}
public enum PageType : byte
{
    Free = 0,
    // Other values retain their identities.
    Reclaimed = 10
}

// Assimalign.Cohesion.Database.Storage.Units
public readonly unsafe struct Page
{
    public uint Checksum { get; set; }
    // Other members omitted.
}
```

Checksum reads/writes expose the stored field; central write-back stamps it and page load verifies
it. Integrity failures throw [StorageCorruptionException](storage-corruption-exception.md), with
Code COHDBS003 and a PageId when known. Engine callers receive
[DatabaseCorruptionException](../assimalign-cohesion-database/database-corruption-exception.md).

## File handles, positional I/O, and durability

`Storage` file opening accepts an `IFileSystem`; omitting it selects the physical file system. Data,
backup, and journal files are opened with `OpenHandle`, which returns the explicit
`IFileSystemFileHandle` contract. The buffer pool and recovery pass page offsets to its positional
reads and writes through `StorageStream`. Their I/O does not depend on a shared stream cursor; the
pool's existing locking, page layout, and write-ahead ordering remain unchanged.

Durability follows the handle through composition: the storage journal retains the `StorageStream`
durability contract and forwards a durable request to `IFileSystemFileHandle.Flush(durable: true)`.
Capability comes from `SupportsDurableFlush`, never a runtime stream type. This fixes **#1018**:
wrapping a physical handle cannot silently turn the journal's durable flush into an ordinary
buffered flush. The physical handle implements that request with `RandomAccess.FlushToDisk`.

The dependency direction keeps the physical implementation as the default behind the file-system
contract:

```mermaid
flowchart LR
    Storage["Database.Storage"] --> Contract["FileSystem — IFileSystemFileHandle"]
    Storage --> Physical["FileSystem.Physical — default"]
    Physical --> Contract
```

**Durability follows the storage.** `Storage.SupportsDurableFlush` requires both data and journal
handles to support durable flushing: checkpointing cannot safely discard a durable journal before
its data pages are durable. Backups are separate from the commit/checkpoint path.
`ConfigureCommitDurability` derives an unset choice as `Synchronous` for capable storage and `None`
otherwise. An explicit `Synchronous` or `Grouped` choice on unsupported storage fails at engine
open, naming the storage and the setting. Model factories resolve the default before initialization
and recovery; engines validate their explicit options before serving the database.

Low-level durable journal operations still throw `NotSupportedException` on unsupported handles,
rather than silently downgrading the request. Reading existing journal bytes does not advance
`DurableLsn`: only a completed explicit durable flush does. Reopening live memory or an
operating-system cache is not durability evidence.

Constructors accepting an ordinary `Stream` explicitly provide **no durability**, even if that
stream happens to wrap a physical file. Such callers must use the handle overload to carry the
contract. `StorageStream.FromInMemory()` is likewise non-durable. Tests of recovery ordering use
explicit simulated durable handles; that fixture contract models persistence across their simulated
crash rather than claiming that production memory is durable.

The regression gate observes the durable flag on a recording handle around the real physical handle
used by the composed storage journal. A reopen or simulated crash assertion alone cannot prove this
request: operating-system buffering can preserve bytes even when no durable flush was issued.

## The buffer pool

`Pin`-counting with RAII handles (`IStoragePageHandle`): a page cannot be evicted while pinned, dirty
pages are written back (checksum-stamped) before eviction, and handles release their pin on dispose.
Contrast with a `Memory<byte>` -pooling design: pages are *pinned* GC handles exposing raw pointers
because the slotted-page and header structs are `unsafe` overlays — the pool guarantees pointer
stability for the handle's lifetime.

- **Eviction is least-recently-used** over unpinned entries: pins and cache hits move a
  page to the MRU end; capacity overflow evicts from the LRU end, skipping pinned
  pages, and fails loudly (`StorageIOException`) when every resident page is pinned —
  the pool never silently exceeds its memory budget. LRU (not clock/2Q) because the
  pool is fully lock-serialized anyway, so the precise policy costs nothing extra and
  is trivially testable.
- **Buffers are reused**: evicted entries return their pinned 8 KiB buffers (GC handle
  and all) to a recycle stack, so steady-state page churn performs zero allocations —
  fresh pages are zeroed on reuse so recycled buffers never leak prior content.
- **Failed loads never poison the cache**: a page that fails checksum verification is
  not cached; its buffer goes straight back to the recycle stack and the exception
  propagates.
- **One lock.** All pool state is guarded by a single monitor. Page *content* access is
  the caller's concern (a handle hands out a raw pointer); the transaction layer above
  provides content-level isolation. Sharding the lock is a measured-need optimization,
  not a default.

Flush I/O releases the pool monitor. A flush snapshots dirty frame identities, copies one
page image under the monitor, then writes outside it in 16-page bursts with a yield between
bursts. The final stream flush also runs outside the monitor. Cached pins and dirty marks
can progress while write-ahead, page-write or stream-flush I/O is blocked. An I/O lease keeps
the copied frame from being recycled; eviction skips it or waits with the monitor released.
Dirty-version checks preserve changes made while an older image is in flight. A separate
gate serializes flush, resize and disposal; disposal drains I/O before clearing frames.
Foreground cache misses and evictions still perform their own I/O under the pool monitor.

StoragePageManager allocation releases its reserved page ID if obtaining an overwrite pin fails.
If extension or initialization fails after pinning, it releases both the pin and reservation.
The unjournaled-write audit runs before buffer initialization, so a refusing audit preserves a
resident free page's bytes and dirty version. Repeated pool-exhaustion refusals do not leak pages.

## The record layer

`SlottedPage` implements the classic slotted layout: records grow forward from the header, the slot
directory grows backward from the page end, deletion marks a slot (length 0) and `Compact`
defragments. All four models share this because their unit of storage — row, document, KV entry,
node/edge record — is "a variable-length byte sequence addressed by (page, slot)". Records above
`SlottedPage.MaxRecordSize` are rejected at the API boundary; multi-page records ride overflow pages
(a later feature — the flags and page type are reserved).

### Per-owner record chains

Data pages carry an 8-byte **owner tag** in the page header (offset 64, taken from the reserved
area; zero = the shared, untagged space, which is also what pre-tag files read — no format flag
needed). The tag is model-agnostic: storage never interprets it beyond grouping.
`InsertRecord(transaction, ownerId, data)` lands the record on the owner's current write page
(allocating and tagging a new page when needed), `GetUnitIterator(ownerId)` iterates only the
owner's pages, and `GetOwnerPages(ownerId)` exposes the chain. This is what turns a model's "scan
one object" from O(storage) into O(object) — the SQL engine passes table object ids, so a table scan
stops decoding the whole database.

- **The directory is in-memory only, page headers are the truth.** The per-owner
  page directory is rebuilt on open by the same header scan that rebuilds the
  free-space map (no extra I/O) and maintained at allocation/free time. A persisted
  directory could drift from reality; headers cannot (the FSM precedent).
- **Owner tags are WAL-covered like all page bytes.** A fresh chain page is tagged
  *before* its first-touch before-image is captured, so a rolled-back allocation
  restores an empty page still belonging to the chain — a safe leak the owner's
  next insert reuses.
- **Chain release (`FreeOwnerPages`) is transactional with commit-deferred
  reuse.** Each page is retyped `Reclaimed` under the transaction (before-image
  covered — rollback and crash recovery restore the chain bytes), but the pages
  re-enter the free-space map and leave the directory only when the transaction
  **commits**. Freeing eagerly would let the allocator hand a page to a new owner
  while the release could still roll back — the rollback's before-image would then
  resurrect old content over live data. Deferral makes that impossible.
- **A stale write touch cannot claim a replacement allocation.** Current-write-page lookup and
  bounded owner-release snapshots capture the page id and its free generation together under
  the owner-directory lock, then the free-map lock. Page-manager lookup likewise checks allocation
  and captures generation in one map hold before pinning. First-touch registration rechecks that
  generation under the transaction lock before granting a page write lock. A committed free
  between lookup and registration invalidates the touch even after reuse by the same owner or
  while the replacement's bracket is active. A page already locked by this bracket needs no recheck.
  A stale insert allocates afresh; owner release skips the stale page and checks the owner tag
  before retyping. Explicit write-open throws StorageIOException: `Page <id> is not allocated.`
  Refusal releases the pin without a write lock, pre-image or page record.
- **Failed first-touch allocation releases its reservation.** When registration fails before
  recording a pre-image, cleanup removes the owner/index directory entry, stamps a Reclaimed page
  and returns the id while holding the transaction lock that releases its write lock. Cleanup
  performs no device I/O, preserving the registration error. An allocation already held by another
  bracket is retained rather than recycled.
- **Generations are transient.** They change no disk format and start at zero on open. Entries
  survive reuse while brackets remain active and clear when the last bracket finishes under the
  same transaction lock used by Begin. Retained entries cover distinct pages freed since the last
  quiescent point; dictionary capacity follows the largest such interval, at most the file's page count.
- **Why release is O(pages), not O(1).** Full-page-image logging prices a
  transactional free at one page touch per page (before-image + after-image in the
  journal). A directory-level O(1) release needs a persisted allocation structure
  (the reserved bitmap-FSM page type) so freeing can be a metadata write; until
  that lands, chains keep releases proportional to the object, which is already
  incomparably better than the previous permanent leak.

### Transactional owner release

The relevant public members are:

```csharp
public StoragePageHandle OpenPageForWrite(StorageTransaction transaction, PageId pageId);
public int FreeOwnerPages(StorageTransaction transaction, ulong ownerId, int maximumPages = int.MaxValue);
```

FreeOwnerPages snapshots a bounded batch of owner data/index pages and returns the number actually
released, including pages already pending free for that owner in this bracket. Pages freed or
reused since the snapshot are skipped; a short return alone does not prove reclamation complete.
Reuse begins at commit. A nonpositive maximum throws ArgumentOutOfRangeException. Foreign/inactive
brackets and rival page locks throw StorageTransactionException. OpenPageForWrite also throws
StorageIOException for an unallocated or stale allocation or page 0. These changes add no diagnostic code.

With a newly created storage facade, this adapts the allocation/release setup in StorageStaleTouchTests:

```csharp
using (var allocation = storage.BeginTransaction())
{
    using (var page = storage.AllocatePageForWrite(allocation, PageType.Index, ownerId: 17)) { }
    allocation.Commit();
}
using (var release = storage.BeginTransaction())
{
    int released = storage.FreeOwnerPages(release, 17, maximumPages: 32);
    release.Commit();
    Console.WriteLine(released); // 1
}
```

First touch follows structure latch, pin with pool monitor released, transaction lock, then
free-map lock. Never hold the map lock across a pin or a wait for the transaction lock.
See `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/tests/StorageStaleTouchTests.cs`
for the deterministic lookup/pin/free/reuse races, including rollback and recovery.

## The journal (write-ahead log)

[StorageJournal](storage-journal.md) is the durability mechanism — the *only* one. Frames are length-prefixed,
magic-tagged, and CRC-protected; a torn or corrupted tail terminates the read scan and is ignored —
it belongs to work that was never acknowledged. Records are typed and binary (begin / commit /
rollback / checkpoint / full page image / page delta / committed page image / opaque logical operation).
Transaction identity at this level is a compact monotonic `long` sequence; GUID identity belongs
to the transaction layer above.

### Write ordering rules (steal / no-force)

1. **Full image at first touch since checkpoint.** The page's first change since the checkpoint
   journals a recoverable full image before mutation. Page changes then apply in the buffer pool.
2. **The write-ahead gate.** The buffer pool may steal (evict) a dirty page at any
   time, but its write-back first forces the journal durable up to the page's LSN —
   so any uncommitted content that reaches the data file is always undoable from a
   durable before-image.
3. **Commit records changes before acknowledgment.** Commit appends changed byte runs as PageDelta,
   or a CommittedPageImage when the delta is large, followed by COMMIT. Durable modes acknowledge
   only after EnsureDurable(commitLsn). Data pages are not forced; recovery replays them (no-force).
4. **Rollback restores prior bytes.** Before-images retained for the bracket, or spilled to the
   journal, restore the pooled pages. A rollback record marks the outcome.
5. **Page-level single-writer.** A page touched by an active transaction is
   write-locked to it (conflicts throw rather than wait). Record-level concurrency is
   `Database.Transactions`' job above this layer; full-image logging is only correct
   because two transactions can never interleave on one page. This division is
   permanent in the MVCC integration design (area DESIGN.md §3.8): storage
   transactions remain the **physical WAL bracket** — the MVCC manager layers
   row-grain snapshots/locks *above* them (paired per transaction via
   `IStorageTransactionSource`), and page locks stop being the user-visible
   conflict surface without ever weakening the invariant that makes page-image
   logging correct.

### Storage transaction ownership

Every record/page mutation validates its bracket's owning Storage by reference before latch
enlistment, pins, locks, pre-images or journal records. A foreign bracket throws
[StorageTransactionException](storage-transaction-exception.md) with
[StorageTransactionErrorCode.ForeignStorage](storage-transaction-error-code.md), naming both
storages. Equal sequence numbers in different storages do not share a page-lock owner: locks hold
the actual bracket object. Commit/rollback route through its immutable owner.

First touch takes an already-held structure latch and pin before the transaction monitor; the pool
monitor is released first. Completion takes owner-directory/free-map locks inside that monitor,
releases page locks before publishing pending frees, and takes no structure latch there.
StoragePageManager.AllocatePage and FreePage are internal. External page writes use
Storage.AllocatePageForWrite/OpenPageForWrite; transactional deletion and FreeOwnerPages defer
reuse until commit. Rollback restores the original bytes and owner membership.

### The physical/logical bracket interplay (MVCC layering rules)

Logical readers reserve storage-counter sequences but open no physical bracket and append no
lifecycle records. The coordinator appends a logical BEGIN only at the first admitted apply, before
that bracket can stamp versions. Only enrolled writers append COMMIT/abort records or wait for
commit durability. A reader promoted after a checkpoint enrolls before its first stamp, preserving
recovery classification. See [coordinator lifecycle](../assimalign-cohesion-database-transactions/transaction-coordinator.md).

The MVCC session binding (area DESIGN.md §3.8, first delivered by the SQL engine) uses these
storage-side rules that keep the logical layer sound:

- **One sequence namespace.** Storage.ReserveTransactionSequence supplies logical and physical
  identities from one counter. The coordinator uses separately sequenced physical statement
  brackets; records carry the logical writer stamp, and only the logical COMMIT proves it at
  recovery. The lower-level BeginTransaction(long sequence) also supports caller-adopted sequences;
  that bracket appends no BEGIN because the caller owns its lifecycle record.
- **The sequence floor.** The file header persists the storage's high-water
  transaction sequence (`LastTransactionSequence`, updated on every header
  write). On open, sequence assignment resumes above `max(journal-max, floor)`:
  a checkpoint truncates the journal — the only other sequence witness — while
  MVCC row stamps persist in data pages, so a recycled sequence would corrupt
  snapshot visibility. Files written before the field read zero, a safe floor
  (they predate row stamps).
- **Checkpoints carry logical writers; the open-time checkpoint is deferrable.**
  `Checkpoint(ReadOnlySpan<long>)` embeds in-flight logical writer sequences in the
  truncating checkpoint record (their begin records are being destroyed;
  `TransactionRecovery.Analyze` reads them back so an unproven sequence still
  classifies as aborted). `Storage`-level brackets must still be quiescent — the
  active-count interlock is unchanged, and the writer list is the caller's to
  supply because storage cannot see above its own layer. Symmetrically,
  `OpenExisting(checkpointOnOpen: false)` lets an engine analyze the recovered
  journal *before* the truncation destroys the records classification reads.
- **Inner brackets may commit non-durably.** `Commit(awaitDurability: false)`
  appends the same change records and COMMIT without the durable
  flush — for per-statement physical brackets whose durability is owned by the
  outer logical transaction's commit record: the journal is ordered, so
  flushing the later record makes the earlier ones durable first, and a crash
  before logical COMMIT leaves its stamps unproven and subject to logical scrub. The write-ahead
  gate protects stolen pages regardless of the flag; the flag never weakens
  the rule that an *acknowledged* commit is durable, because acknowledgment
  belongs to the outer commit.

Full page images supply recovery's base; deltas and committed images carry subsequent changes.
The encoded byte runs omit free gaps while retaining the page's base-LSN chain.

### Recovery replay rules

Recovery restores FullPageImage records regardless of transaction outcome, then applies PageDelta
and CommittedPageImage records only for sequences with a COMMIT. Changes name their base LSN;
a missing image, a chain gap or malformed payload refuses open with StorageCorruptionException.
Replay rebuilds from the journal rather than trusting an on-disk page's LSN, so stolen writes and
torn pages are overwritten. Protected header/anchor pages are excluded. Physical recovery precedes
logical scrub; the engine checkpoints only after consuming transaction classification.

Initialization verifies frame CRC-32C checksums, classifies commits and plans the last applicable
record per page in one streaming pass. Recovery consumes that cached analysis, then performs one
ordered replay through a bounded page cache. The immediate replay can reuse the verified prefix;
ordinary subsequent reads do not. See [streaming recovery](#empty-pages-and-streaming-journal-recovery).

### Checkpoints

Recovery protects both complete, verified checkpoint anchor chains from allocation and replay.
The newest slot's chain must verify. The older chain is protected only if it still verifies
completely; an interrupted header update may already have rewritten it.

Retired anchor pages return to the in-memory allocator only after the replacement header slot's
write and data flush succeed. A failed slot write leaves them allocated until reopen. Retirement
keeps their CheckpointAnchor bytes intact. At open, only pages typed CheckpointAnchor outside
both live chains are reclaimed without trusting their checksum, including a missing checksum.
Ordinary Free and Reclaimed pages still require complete verification.

A new anchor receives a journal full-page initialization image while pinned, before write-back
can tear its transition from Free or Reclaimed. Recovery restores that image before the open scan,
including when the second half of its first write reached disk first. Its image cannot be
truncated until the chain and replacement slot are durable. These rules tolerate retired or
partially initialized anchors after a power interruption while preserving acknowledged records.

`Checkpoint()` durably flushes all page state and **truncates** the journal, writing a fresh
checkpoint record whose LSN continues the sequence (LSNs never restart — page LSN comparisons depend
on monotonicity across truncation). Checkpointing requires no active transactions — truncating live
before-images would orphan stolen writes; fuzzy checkpoints are a later feature (the record already
carries the active-transaction set). Clean shutdown checkpoints, so a clean reopen recovers
instantly.

With a background checkpointer (#902) checkpoints race live transactions, so the emptiness check
hardened from "no page write locks" to an **active-transaction count** taken in `BeginTransaction`
under the transaction lock and released exactly once per commit/rollback: a begun-but-untouched
transaction holds no page lock yet has already appended its begin record, and the whole checkpoint
now runs *under* the transaction lock, so no transaction can slip between the emptiness check and
the truncation. Lock order is transaction lock → header → flush gate → buffer pool monitor →
journal medium → journal append. Flush I/O releases the pool monitor; eviction never takes the
flush gate and waits for a frame lease with the monitor released. Never wait for the medium while holding the append lock. Medium owners briefly
take the append lock to detach/publish prefixes and latch failures before yielding the medium.
A checkpoint attempted while transactions are active still throws
`StorageTransactionException` — background checkpointers treat that as "busy, retry next pass".

Read-only logical transactions can begin, pin cached pages and commit or roll back while a
checkpoint flushes. Sequence reservations use an atomic counter rather than the physical
transaction lock, and reader ends do not take the lifecycle-log gate. Snapshot capture and
active-table insertion remain together under the manager's begin lock. The checkpoint still
holds the statement-apply gate, so writers wait through it. A reader promoted later enrolls in
the retained journal before stamping pages, including when its reservation exceeds the captured
header floor. This change provides reader progress, with no checkpoint latency guarantee or
completion-time target.

The physical-file latency probe is opt-in with COHESION_DB_PROBES=1; default test runs report it
as skipped. From a writable cohesion checkout, PowerShell users can run:

```powershell
$env:COHESION_DB_PROBES = '1'
dotnet test resources/Database/Assimalign.Cohesion.Database.Transactions/tests/Assimalign.Cohesion.Database.Transactions.Tests.csproj --filter FullyQualifiedName~CheckpointLatencyProbeTests
```

The probe reports nearest-rank p99 and maximum latency for reads overlapping checkpoint work
over physical files with a 32 MiB pool. It runs at least five checkpoints and continues until
overlap is observed. Measured times have no pass/fail threshold.

### Checkpoint triggering

Journal size and interval are soft triggers, not a hard journal bound. When the shared checkpoint
worker collects a lane that ended deferred, it rechecks backoff and due state in that same pass
and attempts at most one new checkpoint. It no longer leaves a consumed size wake until the next
poll. Failed outcomes keep their retry delay; busy storage waits for the next pass. Cancellation
and shutdown retain lane ownership, and unfinished work is reported once per database per pass.
This resolves the deferred-lane wake gap only; #1283 remains open and journal growth is not bounded.

See the root and storage DESIGN sources and
`cohesion/resources/Database/Assimalign.Cohesion.Database/shared/DatabaseCheckpointWorker.cs`.

The journal-size checkpoint setting remains a soft trigger: one atomic statement can overshoot it
before a checkpoint runs. The hard journal-size bound is deferred; wave 7 does not impose bracket
caps or split statements. See [checkpoint triggering](#checkpoint-triggering).

### Appends during journal I/O

The medium owner detaches the append buffer and captures its target LSN under the append lock,
then writes and fsyncs with that lock released. Appenders fill a replacement buffer during I/O.
A full replacement waits for the medium outside the append lock. After warm-up two retained
buffers are reused, each at most 1 MiB; oversized frames use a pooled direct buffer.

Only the written prefix is published as durable. Queued force callers recheck that prefix, so
one fsync can cover their requests without confirming later buffered appends. Checkpoint truncation
waits for the medium and pauses appends; disposal also waits, closes appends and drains once.
A failed write or fsync latches offline before releasing the medium, refusing later writes and
confirmations beyond the established prefix. Reopen recovery decides the surviving outcome.
See [StorageJournal](storage-journal.md) for signatures, ownership and exceptions.

### `Commit` durability modes (group commit)

`Storage.CommitDurability` controls how journal records reach stable storage:

- **`Synchronous` (default):** commit calls `EnsureDurable(commitLsn)` inline — one
  fsync per commit, simplest latency profile.
- **`Grouped`:** commit registers its LSN on the internal group-commit gate, wakes
  the engine's flush worker through the `OnCommitPending` hook, and waits. The worker
  calls `IStorage.FlushPendingCommits()` — one durable flush covering the highest
  pending LSN — and wakes every covered committer, so concurrent commits share one
  fsync. **Self-help invariant:** a committer not woken within `GroupCommitWindow`
  flushes inline itself; a missing, stalled, or misconfigured worker costs bounded
  latency, never durability. A commit is acknowledged only after its records are
  durable in either mode.
- **`None`:** commits do not flush to durable storage because the backing store
  cannot provide it. The same before images, after images, and commit records are
  appended. Page write-back keeps ordinary journal-before-page flush ordering;
  recovery, checkpoints, explicit flushes, and shutdown perform ordinary flushes
  without advancing the durable LSN or publishing durable group-commit progress.

`Synchronous = 0` and `Grouped = 1` retain their shipped enum values; `None = 2` is additive. The
low-level `Storage` property retains its synchronous default for callers explicitly managing
composition. Engine defaults are derived from the backing handles, and the resolved value is visible
through `CommitDurability`. The policy is applied after the commit record exists;
`EnsureCommitDurable` also lets an outer logical transaction apply the same policy to its later
commit record. MVCC visibility, joins, and constraint enforcement do not read this setting.

The gate lives in storage (not the engine) because commit blocks inside `CommitTransaction`; the
engine contributes only the worker loop and the wake signal. Page write locks release after the
durability wait, exactly as in synchronous mode.

### Paced page write-back

`IStorage.WriteBackDirtyPages(maxPages)` writes back a bounded batch of dirty buffered pages without
evicting them — the page-writer worker's pass between checkpoints, so a checkpoint's `FlushAll` does
not spike. Every write-back path (this one, eviction, `FlushAll`) funnels through the buffer pool's
single write-back routine, so the write-ahead gate (journal durable ≥ page LSN) holds for stolen
pages here exactly as everywhere else.

### What is deliberately unlogged

Page 0 (the file header) carries only recomputable bookkeeping and is rebuilt or revalidated on
open; it is flushed but never journaled. Page allocation is likewise not undone on rollback — a page
allocated by an aborted transaction is restored to its empty initialized image and leaks safely
until reused.

## Empty pages and streaming journal recovery

Deleting the last live record in a data page retypes it as `Reclaimed` inside the physical bracket and
registers a pending free, exactly as an owner-chain release does. The allocator and owner directory
change only after commit. `Rollback` restores the original page image and keeps its owner membership.
Inserts check page type and owner before reusing a current-write-page hint, including a hint to a
page pending release in the same bracket. Iterators release a pin before skipping a page that was
freed during a scan.

[StorageJournal.ReadSequential](storage-journal.md#sequential-reads-and-payload-ownership) holds the
synchronous medium lock and pauses appends throughout enumeration. Each public payload is an
owned copy valid after MoveNext and disposal; ReadAll payloads have the same guarantee. Consume
on one thread without awaiting or checkpointing. Appending on that thread while enumeration is
active throws InvalidOperationException. Completion, early disposal and consumer exception or
cancellation release the lock and restore the stream position. Both paths drain before reading
when online; offline readers see only the medium.

Storage recovery and consistency checks use an internal borrowed reader whose pooled payload is
valid only until the next MoveNext or disposal. Transaction recovery uses owned public records
across the assembly boundary. No borrowed API is exposed to callers.

Initialization combines CRC-32C verification, commit classification and page planning. Unproven
brackets retain their last record identity per page until a commit promotes it; rollback releases
those pending page entries. File ordinals prevent a late commit from promoting an older record
over a later full image. Reuse of a rolled-back adopted sequence followed by a commit can require
a fresh planning pass, preserving the existing rule that any COMMIT admits that sequence's changes.

Recovery consumes and releases the cached analysis. Only one immediately following redo enumeration
on the same journal instance can reuse its verified prefix. An intervening read or append revokes
that trust; every public read, other borrowed read and later analysis verifies CRCs afresh.
Truncation clears cached analysis and verified-prefix trust before SetLength, even if truncation
then fails. Replay order, transaction classification and torn-tail handling remain unchanged.

Replay retains transaction/page identities and a bounded page cache, with temporary identities per
unproven transaction/page during initialization. It does not retain journal payloads. A cached page
is written at its last applicable record; cache pressure can write it earlier and read it back with
checksum/LSN verification. Memory follows identities and cache capacity rather than journal bytes.

## Error model

`StorageException` is the child root for this library. `StorageIOException` (stream and allocation
failures), `SlottedPageException` (record layout violations), `StorageCorruptionException`
(COHDBS003, checksum/header integrity failures — carries the `PageId`), and `JournalException` (journal
framing/state violations) all derive from it, so consumers can catch the family or the specific
failure.

At engine boundaries, storage corruption becomes DatabaseCorruptionException with the database
name, optional page and original storage cause. Reads fail rather than silently omitting damaged
data; legitimate record reclamation retains absence semantics. Refused opens can carry additional
model diagnostics. See the [public exception API](../assimalign-cohesion-database/database-corruption-exception.md).

Foreign-storage mutations retain StorageTransactionException with Code ForeignStorage under the
storage child root; ordinary transaction violations use Unspecified. Invalid overflow geometry and
known page-0 integrity damage use COHDBS003. Header read failures without journal coverage, or two
unreadable halves without checksum evidence, remain I/O failures.

## Storage EventSource

Provider `Assimalign.Cohesion.Database.Storage` emits the following recovery warning:

| Id | Event | Level | Payload | Trigger |
| --- | --- | --- | --- | --- |
| 18 | HeaderReadFallback | Warning | database, slot, generation, lsnFloor, exceptionType, exceptionMessage | A half-page read failed and journal coverage proves the selected fallback safe, before replay |

It reports every accepted fallback without throttling, including consecutive opens. Missing coverage
emits no success warning and refuses open before replay writes. The existing PageChecksumFailed
(Error 16) still reports pooled-page checksum failures; page 0 is outside that pool.

## AOT posture

No reflection, no runtime codegen. `Header` structs are explicit-layout overlays read through
pointers; encodings are hand-written span code. `AllowUnsafeBlocks` is enabled for the pointer
overlays — the unsafe surface is confined to `Units/` and the buffer pool's pinned buffers.

## Non-goals

- **No model semantics.** Nothing here knows what a row or document is.
- **No distributed I/O.** One storage instance = one file set on one machine.
  Replication is outside the implemented surface; the former Database.Replication project is removed.
- **No encryption yet.** The header reserves nonce/MAC space and `PageFlags.Encrypted`;
  the encryption-at-rest feature (#861) implements it beneath the buffer pool.

## Declared dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.FileSystem` | `CohesionProjectReference` |
| `Assimalign.Cohesion.FileSystem.Physical` | `CohesionProjectReference` |

[Assembly overview](index.md) · [Examples](examples/index.md)

- **Wave-7 recovery and geometry** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/tests/StorageHeaderReadTests.cs`, `StorageTransactionOwnershipTests.cs` and `PageTests.cs` in the same directory.

## Sources

- **Reader progress and frame cleanup** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/tests/StorageCheckpointFlushTests.cs`.
- **Checkpoint-independent readers and late promotion** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/tests/TransactionCoordinatorRecoveryTests.cs`.
- **Opt-in measurements** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Transactions/tests/CheckpointLatencyProbeTests.cs`.

- **Format-4 integrity and interrupted anchors** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/tests/StoragePageIntegrityTests.cs`, `StorageFormatTests.cs`, `StorageAnchorReclamationTests.cs` and `StorageAnchorDurabilityTests.cs` in the same directory.

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/docs/DESIGN.md`.
- **Journal concurrency and ownership** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/Journal/StorageJournal.cs` and `tests/JournalTests.cs` / `tests/JournalConcurrencyTests.cs`.
- **Recovery equivalence and checksum scope** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/tests/RecoveryAnalysisTests.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/src/Assimalign.Cohesion.Database.Storage.csproj`.
