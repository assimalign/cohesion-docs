# Assimalign.Cohesion.Database.Indexing design

This page describes the boundaries and implementation shape of `Assimalign.Cohesion.Database.Indexing`.

> **Status:** Partial.

## Intent

Every model needs ordered lookups: SQL secondary indexes, document indexes, graph adjacency, the KV
primary structure. Building four bespoke trees would quadruple the hardest code in the platform, so
index structures live in the kernel and models bring only their key extraction. The SQL engine is
the first real consumer (#912): CREATE/DROP INDEX, write-path maintenance, and planner seeks all
compose this package's manager, trees, and cursors.

## Incremental range cursors

[BTreeIndex.OpenCursor](b-tree-index.md#range-cursors) captures its snapshot and owns copies of
the range bounds without visiting a page. [BTreeCursor](b-tree-cursor.md) reads one visible entry
in the first batch, then at most 64 per batch, with at most 256 retained entries examined per
latch hold. Invisible runs and undo-empty leaves yield too. Forward/reverse scans descend to
their bound and resume by logical (key, reference, writer) identity after each batch; no physical
page/slot survives a move. Full scans cost O(b h + visited leaf pages) for b batches and height h;
empty-leaf probes can add descents.

A registry keyed by storage instance and fixed root page shares the latch across replacement
managers, reattachment and rollback. The order remains transaction/key locks, tree latch, then
page pin/pool monitor, with no await or transaction-lock wait under the tree latch.
[StoragePageLatch.EnterRead(CancellationToken)](../assimalign-cohesion-database-storage/storage-page-latch.md)
lets cancellation interrupt latch acquisition. Opening and move tokens reach every page/entry
read; errors dispose the cursor. Callers retain their snapshot pin and finish enumeration before
mutating through its owner. SQL DML still collects all targets before writes.

SQL seeks, Key-Value scans, Documents index reads/unique probes and Graph property, label and
adjacency scans pass their operation token through cursor open and moves. This changes neither
the index page format nor the caller's snapshot lifetime. Tests cover million-key short reads,
both directions, boundary mutations, empty leaves, cancellation and corruption.

## Byte-comparable keys

`IndexKey` compares by unsigned lexicographic byte order — the single comparison rule every
structure (and every future on-disk page compare) uses. Type order is preserved at *encoding* time:
integers are big-endian with the sign bit flipped (`FromInt64`), so numeric order and byte order
coincide; composite keys concatenate component encodings. String collation encoding is deliberately
**not** defined here — it belongs to the shared type system (`Database.Types`), which owns collation
identity; `IndexKey.From(DatabaseKeyWriter)` is the bridge for typed and composite keys.

This is the same design center as FoundationDB tuples and MySQL/InnoDB memcmp-able keys: one dumb,
fast comparator at the bottom, all type intelligence pushed to encoding.

`IndexKey.FromString(value, collation)` delegates to the shared writer. `Binary`, `CaseInsensitive`
, and `CaseAccentInsensitive` produce deterministic transformed UTF-8 keys. Equivalent spellings
have identical bytes, equality, content hashes, and unique-lock identities; original spelling never
breaks a collation tie. `Invariant` is explicitly not index-backed and key construction rejects it.
The owning model must resolve index collation and seek eligibility; the B+Tree itself continues to
compare raw bytes.

## The B+Tree implementation

`BTreeIndexManager.Create(options)` composes B+Trees over `PageType.Index` pages — one node per
page, a sorted offset directory with entry data growing from the body end, key capacity capped
([BTreeIndex.MaxKeyLength](b-tree-index.md#encoded-key-limit) = 1,024 bytes) so a node always
holds several entries and splits stay correct. Both insertion paths accept exactly this limit.
Models share this public constant for preflight validation. Encoded components, NULL markers and
collation transforms count; entry-reference and writer-stamp tiebreakers are separate.

- **Every page mutation rides the owning storage transaction** through the storage
  layer's `OpenPageForWrite`/`AllocatePageForWrite` — before-images at first touch,
  after-images at commit. That is the whole crash story: a crash mid-split reverts
  to the consistent pre-transaction tree; committed splits replay from the journal
  (the crash suites prove both). `IStorageTransactionSource` is how the engine
  pairs logical transaction contexts with their storage transactions.
- **MVCC entries, tombstone deletes.** Leaf entries carry writer and deleter
  sequence stamps; reads filter through the caller's snapshot (writer visible,
  deleter absent-or-invisible). Deletes stamp tombstones so old snapshots
  keep seeing the entry; an aborted deleter's stamp reverts physically with its
  page image. Safe index-history pruning removes obsolete entries during the coordinator purge
  pass; empty-leaf reclamation and node merges remain separate work.
- **Uniqueness checks the LATEST state, not the snapshot.** Two transactions that
  began before each other's commits would both pass a snapshot-visibility check
  (write skew). Instead: unique inserts *and deletes* first take an exclusive
  key lock (hashed key) in the shared lock manager; once held, a live entry
  (deleter stamp zero) can only be committed or our own — uncommitted others are
  excluded by the lock, and aborted writers' entries were physically reverted.
- **Concurrency (MVP): a tree-level reader/writer latch.** Writers exclusive;
  cursors copy bounded batches of visible entries under the read latch, so no
  latch is held across awaits and readers never see a torn structure. Lock
  coupling / latch-per-node is a measured-need follow-up.
- **Directory persistence belongs to the catalog.** The manager keeps an in-memory
  directory and exports `BTreeIndexRegistration`s (`IIndexRegistry`); the model
  catalog persists them and re-attaches on open (`ExistingIndexes`). `Root` splits
  change the root page id — catalogs re-export at their persistence points.
  Dropping an index is a directory operation; its pages await vacuum.

## Leaf split policy

An append to the rightmost leaf keeps all but one existing entry on the left. The incoming
entry is inserted after splitting and descending again. The same policy applies at the end of a
duplicate run only when the whole leaf, including the incoming key, has one value and its upper
ancestor separator proves the run ends there. An equal-key separator cannot prove this, because
the run may continue in the next leaf. Mixed-key leaves and other insertions use the midpoint.
Internal-node splits retain their existing byte-based midpoint policy.

This adapts PostgreSQL's rightmost and single-value strategies. In the deterministic
5,000-entry tests with 1,024-byte keys, ascending keys and duplicates inserted in reference order
use 834 leaves at about 78% fill, compared with 1,666 leaves at about 39% under midpoint splits.
These are page/byte measurements without pruning, rather than a throughput guarantee.

Denser ascending loads leave less room for updates. Adding one new reference for every key after
that load ends with 1,944 leaves for random updates or 2,500 for ascending updates, compared with
1,667 under the former policy. This additional splitting is the accepted space/headroom trade-off.
The policy changes neither B-tree page format 2 nor cursor continuation or transactional rollback.

## Transactional binding

`IIndex` mutations take an `ITransactionContext` — index entries are stamped and become visible
under the same MVCC rules as the data they reference. There is no "non-transactional index write"
surface; recovery replays index changes from the same WAL as data changes. Unique enforcement
happens at insert against the *visible* state (a unique violation with an in-flight competing writer
resolves through the lock manager, not the index).

### The maintenance surfaces (model-engine consumers)

The SQL engine's index adoption (#912) added a small family of operations that deliberately take the
**physical bracket** (`IStorageTransaction`) instead of a transaction context — they run where no
statement bracket exists:

- **`InsertVersionAsync(bracket, key, reference, writer, deleter)`** — the
  offline (DDL-blocking) build path: an index built over existing rows inserts
  each stored version with its original stamps, so pre-existing snapshots read
  through the new index exactly what the row scan shows them. No uniqueness
  check — the builder detects live duplicates itself under the object's
  exclusive lock (online rebuild remains a non-goal).
- **`EraseAsync` / `ClearDeleterAsync`** — the logical-undo pair: physically
  remove an aborted writer's insert; clear an aborted writer's tombstone. Both
  verify the recorded stamp before acting, so replays and stale ledgers no-op.
  Physical removal drops only the directory slot; the entry bytes stay orphaned
  in the node until a split rebuilds it (bounded space for a rare path).
- **`IIndexManager.PurgeWritersAsync(bracket, writers)`** — the open-time
  recovery obligation: one walk per tree removes every unproven writer's
  entries and clears their tombstones (the in-memory undo ledger died with the
  process; snapshots have no commit-log awareness, so unproven stamps must not
  serve reads). Idempotent across the crash window.
- **`OpenCursor(TransactionSnapshot, range)`** — reads through an explicit
  snapshot, so a statement-scoped reader (per-statement snapshots under
  ReadCommitted) sees exactly the same visibility through the index as through
  its row scan.
- **`IndexKey.Hash()`** — publishes the FNV-1a key-lock identity so a writer
  that must never wait inside a serialized apply scope can pre-acquire the
  unique-key lock in its own lock phase and rely on the lock manager's
  same-owner re-grant when the tree acquires it again internally.

### Shared record-version undo binding (#918)

`RecordVersionIndex` implements the new `Database.Transactions.IRecordVersionIndex` contract by
forwarding encoded key bytes to the existing `IIndex.EraseAsync` and `ClearDeleterAsync` operations.
Engines supply this bridge to the shared `RecordSpaceVersionStore` ledger. `Index` key construction
and stamp verification remain in Indexing; Transactions needs no Indexing or area-root reference,
and the existing `IIndex` and `IStorageTransactionSource` interfaces are unchanged. `Open`-time index
scrubbing still uses `IIndexManager.PurgeWritersAsync` between the coordinator's record scrub and
its final checkpoint.

## `Entry` references are opaque `ulong`s

The index maps keys to entry references the owning storage layer understands (page address, row id,
node id). Making the reference generic (`IIndex<TReference>`) would infect every cursor and page
layout with a type parameter for zero runtime benefit — models already own both sides of the
mapping.

## Latest live-entry lookup

[BTreeIndex.FindLiveEntryAsync](b-tree-index.md) returns the latest live reference for a complete
unique key, without snapshot filtering. With the configured LockManager, it acquires the exclusive
hashed entry lock before the tree's read latch, retaining it until transaction completion;
pre-held locks re-grant. SQL calls it outside the physical apply gate after pre-acquiring proposal
keys. Tombstones/rolled-back inserts return null, and rolled-back deletes restore the original
reference. A nonunique index raises InvalidOperationException. There is no page-layout change.

## Error model

`IndexException` is the package's own exception root and inherits `Exception` directly, not the
area's `DatabaseException` — this package is a child root the area root rolls up (2026-07-13
inversion), so it must stay independently consumable.
`IndexUniqueViolationException : IndexException` is the typed unique-violation surface; model
engines that expose index failures on the area's error surface translate at their own boundary.

Attached-index integrity failures cross the engine boundary as
[DatabaseCorruptionException](../assimalign-cohesion-database/database-corruption-exception.md),
leading with CK06003 and retaining CK06102 plus the model diagnostic. The original
IndexCorruptionException and page remain available. Ordinary SQL, Key-Value Pair, Document and
Graph index failures use the model code;
specific format refusals retain CK06101. Inner and aggregate causes receive the same classification.
See [model mappings](../../../../database/overview.md#engine-failure-diagnostics).

## Relationship to `Database.Storage`

`Database.Storage` provides the *physical* substrate through `IStoragePageManager` — index pages
(`PageType.Index`) are allocated, pinned, and flushed like any other page and live in the same
storage files. This project is the *logical* layer: structures, keys, cursors, uniqueness. The
B+Tree implementation binds the two. (An earlier string-based `IStorageIndexManager` stub in
`Database.Storage` was removed during the #157 alignment — it duplicated this project's
`IIndexManager` at the wrong layer with no design behind it.)

## Non-goals

- **No full-text or spatial** — indexes in the MVP surface (future `IndexKind` members; the enum is the extension point).
- **No online index rebuild in the contract yet** — DDL-blocking builds first.
- **No persisted statistics** — planners use bounded CountRange probes under their statement snapshot.

## AOT posture

Pure contracts and span-based value objects. No reflection.

## Declared dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Storage` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Transactions` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |

[Assembly overview](index.md) · [Examples](examples/index.md)

## Safe index-history pruning

SQL, Documents, Graph and Key-Value Pair connect their B-tree manager to the coordinator's version-purge pass. The coordinator supplies the safe exclusive horizon and serializes index pruning against statement apply and physical rollback. Only nonzero deleters strictly below that horizon are removed. Active snapshots, including statement pins and minima captured while an older writer was active, retain the versions they can see.

A manager call visits at most 64 leaf pages across its directory and resumes on later calls. Each leaf is its own cancellable physical bracket; earlier completed leaves stay reclaimed if a later leaf fails. Empty leaves and separator identities remain valid; this does not merge or reclaim tree pages.

Unique inserts take their key lock and tree write latch, prune eligible entries in that key's run, check uniqueness and reuse the insertion position during a single descent. Read cursors remain read-only. Standalone managers need a proven PruneBound provider for opportunistic pruning; explicit PruneAsync accepts a caller-proven horizon. See [manager options and pruning API](b-tree-index-manager.md).

The #1393 decision records the existing physical index-page rollback as status quo. It adds no statement stamps, index format change or new visibility model. BTreePruneTests, BTreeUniqueDescentTests and DatabaseIndexPruneTests cover horizon equality, held snapshots, continuation, cancellation and unique-descent work.

## Sources

- **Split policy and measured shapes** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Indexing/src/BTreeIndex.cs` and `tests/BTreeShapeTests.cs` in the same project.
- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Indexing/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Indexing/src/Assimalign.Cohesion.Database.Indexing.csproj`.
