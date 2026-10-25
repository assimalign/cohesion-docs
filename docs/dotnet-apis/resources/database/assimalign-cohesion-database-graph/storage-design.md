# Graph storage design

This page describes the storage component inside `Assimalign.Cohesion.Database.Graph`.

> **Status:** Partial.

Namespace: `Assimalign.Cohesion.Database.Graph.Storage`. Assembly: `Assimalign.Cohesion.Database.Graph.dll`. NuGet package: `Assimalign.Cohesion.Database.Graph`.

This storage component ships inside the model package. Keep its namespace
imports and replace the former satellite package reference with `Assimalign.Cohesion.Database.Graph`; recompile consumers.
The model enforces engine → Catalog → Storage → shared kernel through architecture tests.

## Ownership and composition

The sealed public GraphStore is created by GraphStore.Open. `GraphStorage` is the thin shared-kernel storage subclass. The engine owns its
storage streams and coordinator; the store owns neither and has no independent disposal or
background workers. All identities are reserved from the kernel's durable transaction-sequence
namespace, so rollback and restart do not reuse an identity.

The Graph root owns query planning, label/type validation and session scoping. Graph.Catalog owns
schema metadata. This component owns physical graph records and their indexes. The dependency arrows
point to the shared `Storage`, Transactions and Indexing packages; no model-specific paging, journal,
lock manager or B+Tree is implemented here.

## Physical layout and adjacency

The shared file header persists `StorageModel.Graph`. Slotted data pages carry an owner identifier:
owner 0 is Graph.Catalog metadata, owner 1 is physical B+Tree registrations, and owner 2 contains
nodes, relationships and logical property-index definitions. Shared B+Tree pages use the kernel's
`PageType.Index` format.

One relationship record contains both directed endpoints and a catalog type GUID supplied to
CreateRelationshipAsync. The store does not verify the GUID/name pairing; the engine resolves it
from Graph.Catalog. The shared adjacency B+Tree, registered under object ID UInt64.MaxValue and
name graph, maps (node, direction, type ID, relationship ID) to a packed physical location.
Each 33-byte key contains an unsigned big-endian UInt64 node, direction byte (1 outgoing, 2 incoming),
a 16-byte big-endian GUID and unsigned big-endian UInt64 relationship. Every relationship has two
entries, including a self-loop. Both-direction reads skip the incoming self-loop before decoding
it again, without retaining a set proportional to outgoing degree.

The diagram shows the same layout: each node's adjacency prefix selects references to relationship
records, and each relationship record contains both endpoints.

```mermaid
flowchart LR
    X["B+Tree A, outgoing, type ID"] --> R["Relationship record: A, B, type ID, type, properties"]
    Y["B+Tree B, incoming, type ID"] --> R
    R --> A["Node A record"]
    R --> B["Node B record"]
```

The filtered [GetIncidentAsync](graph-store.md#incident-reads) streams closed ranges from
(node, direction, type ID, 0) through (node, direction, type ID, UInt64.MaxValue).
Null types select all types in the requested directions; empty selects none; duplicate IDs add no
visits. For b bounded batches and r retained entries, work is O(b log E + r).
Only snapshot-visible references are decoded; unrelated directions/types add no relationship decodes.
The streaming API specifies no cross-range ordering. The unfiltered list overload and engine sort
visible relationships by identity.

The shared B+Tree opens lazily and copies one entry in its first batch, then at most 64 visible
entries per batch, examining at most 256 retained entries per latch hold. Cancellation reaches
latch acquisition and page/entry reads. The executor merges type/direction streams by identity,
retaining one decoded head per range. Type discovery and initial heads count toward its expansion
budget; refusal, cancellation and errors dispose every active stream. List-returning APIs still
materialize their required result. Creating, replacing or deleting
relationships maintains both direction-specific entries in one physical statement bracket.

Compound keys make every current relationship entry distinct. This is also deliberate protection
against a duplicate-key lower-bound seek starting in a later leaf after a shared B+Tree split. A
regression creates 240 incident edges, forces splits, reopens the file set, and checks all edges.
There is no kernel change.

## Version 4 record format

All multibyte fixed-width values below are little-endian unless a key explicitly says otherwise.
Every slotted record starts with the shared 16-byte stamp prefix: writer sequence at offsets 0-7,
deleter sequence at offsets 8-15. Zero deleter means live. Graph records have kind at offset 16,
format version 4 at offset 17, a nonzero 64-bit logical identity at offsets 18-25, and an
UInt64 revision at offsets 26-33. Revision starts at zero and increases on replacement. The
remaining payload follows that header:

| Kind | Meaning | Payload after identity |
| --- | --- | --- |
| 1 | Node | Int32 label count, (label string, 16-byte catalog GUID in Guid.ToByteArray order) pairs, property map |
| 2 | Relationship | UInt64 source, UInt64 target, 16-byte catalog type GUID (Guid.ToByteArray order), type string, property map |
| 3 | Property index definition | Label string, property-key string |

A property map is an Int32 count followed by ordinally sorted `(key string, scalar)` pairs. Counts
must be nonnegative and bounded by the maximum record size. Strings use the .NET BinaryWriter length
format: a 7-bit encoded nonnegative UTF-8 byte length and those bytes, with strict UTF-8 validation.
Labels are ordinally deduplicated and sorted. The explicit scalar encoding is:

| `Tag` | Payload |
| --- | --- |
| 0 | Null, no payload |
| 1 | Boolean byte |
| 2 | String |
| 3, 4 | SByte, Byte |
| 5, 6 | Int16, UInt16 |
| 7, 8 | Int32, UInt32 |
| 9, 10 | Int64, UInt64 |
| 11 | `Decimal`, BinaryWriter's four Int32 words |
| 12, 13 | Finite IEEE-754 Float64, Float32 |

Record length is limited to the shared `SlottedPage.MaxRecordSize`. Unsupported CLR objects,
non-finite floating values and oversized records fail before insertion. This version does not define
arrays, nested objects or overflow chains. Nodes and relationships are immutable versions.
UpdateNodeAsync replaces labels/properties; UpdateRelationshipAsync preserves endpoints/type and
replaces properties. Both publish even equal values; GQL filters pure no-ops. Creation accepts
complete records with caller-reserved identities and caller-resolved label GUIDs. Formats 1, 2 and 3 are refused before logical
recovery scrub with CG06017 and the recreation remedy; there is no upgrade reader. Graph records
and tree registrations use version 4; catalog and B-tree page formats are unchanged. The low-level
StorageCorruptionException carries the code in its message; the engine retains it alongside
CK06003 and CG06018 in
[DatabaseCorruptionException](../assimalign-cohesion-database/database-corruption-exception.md).
No SQLSTATE is assigned.

Format 4 adds the catalog GUID after each label and the label-postings tree. A cleanly closed
older file set is refused without changing any file. Physical WAL recovery precedes this fence
on a crashed file set; the fence runs before logical scrub or the recovery checkpoint. There is
no backfill or format upgrade.

Owner-one physical registration records are exactly 34 bytes: a zero stamp prefix, kind `4`,
version `4`, UInt64 object ID, and Int64 root page ID. Every tree is named `graph`; its object ID
distinguishes it. A tree root stays fixed through splits; registrations are saved when trees are
created and reloaded after
physical statement rollback. They are physical infrastructure, so logical rollback leaves an unused
tree available for recovery; logical property-index definitions remain stamped and determine
visibility.

Physical references use the high 48 bits for the page ID and low 16 bits for the slot. Readers check
page allocation, data-page owner, record identity and writer stamp before trusting cached
references. Checksum errors and real I/O failures propagate; reclaimed pages/slots and reused
locations are treated as stale directory entries.

## Secondary indexes

The MVCC label-postings B+Tree uses object ID UInt64.MaxValue - 1 and name `graph`. Each
24-byte key holds a big-endian catalog label GUID followed by a big-endian UInt64 node identity;
the value is the packed record location. Label text is stored in the node, outside the key.
CreateNodeAsync and UpdateNodeAsync require the caller-resolved label-name/GUID map; storage
takes no Catalog dependency and trusts that pairing. Missing/empty IDs are refused.

Create, replacement (including property-only updates) and delete maintain every label posting
in the same physical bracket as records and property indexes. Old snapshots retain old membership;
rollback, purge and crash recovery handle postings with their records. Labeled records without
the required tree registration refuse open as corruption. Empty or wholly unlabeled stores need
no label tree. Open reconstructs name/token mappings and advisory retained-posting estimates.

Label reads seek selected ranges in O(b log P + r) index work for b batches and r retained postings. They decode
only visible matching references in identity order. Reused label names with historical catalog
IDs and pure pattern unions merge ranges with duplicate suppression. Conjunctions scan the
cheapest required label and retain the full residual. Unfiltered scans still enumerate identities.
Readers release the directory monitor before taking tree/page latches and take no graph writer
lock. Shared cursors copy bounded cancellable batches and release pages/latches before decoding
or merging label ranges. See [BTreeCursor](../assimalign-cohesion-database-indexing/b-tree-cursor.md).

Each label/property definition owns a B+Tree whose object ID is its durable definition identity.
Keys consist of the shared `DatabaseKeyWriter` scalar encoding followed by the node identity as
eight unsigned big-endian bytes. The scalar encoding is null, Boolean, ordinal UTF-16 big-endian
string bytes, or Float64 for every numeric CLR type. The scalar prefix may occupy at most 1016
bytes, leaving eight bytes within the shared 1024-byte key limit. Missing properties have no entry;
a present null has a null key.

Numeric keys are a candidate projection. Nearby Int64 or `Decimal` values can map to the same Float64
value. Every result is compared to its original scalar: integral/`Decimal` pairs compare exactly as
`Decimal`, and a pair involving Float32 or Float64 compares as Double. This matches the root evaluator
and supports very large and very small finite floating values. Positive and negative zero share one
key. Prefix seeks bound the candidate range by node identities zero and `UInt64.MaxValue`; the
identity suffix also makes a heavily repeated property value safe across B+Tree leaf splits.

`Index` creation backfills visible nodes under the database writer lock and preserves source record
stamps. Every subsequent node create/update/delete maintains all current definitions in its transaction.
Dropping an index tombstones its logical definition; older snapshots retain the tree and their view.
This MVP retains dropped physical trees for later reclamation rather than recycling them while
snapshots may still reference them. An exact seek costs `O(log N + k)` index work for `k`
candidates, including numeric projection collisions and retained versions; ordering its `m` visible
matches adds `O(m log m)` CPU work.

## Atomicity, conflicts and recovery

All graph writers acquire the shared database-level exclusive lock and retain it until logical
commit/rollback; readers use MVCC snapshots. The lock is deliberately coarse for the MVP. After a
wait, the store checks cancellation and active transaction state and releases a grant received after
rollback. Update and deletion checks compare the caller's snapshot with the latest committed view
while holding the lock; a changed record produces `TransactionAbortedException`. A stale detach
cannot silently cascade a relationship that appeared after its snapshot.

Creating a relationship applies the record and endpoint indexes in one coordinator statement.
Deleting a connected node without detach is refused before mutation. Detach tombstones the node,
every incident relationship, both adjacency entries and all node-property entries in one physical
bracket. Version-ledger entries allow logical rollback to restore those records and index stamps
together. Committed tombstones remain available to older snapshots until the shared version-purge
worker's safe bound passes. Label postings follow the same lifecycle.

Opening follows this order:

1. `GraphStorage.Open(..., checkpointOnOpen: false)` runs shared physical recovery.
2. Run GraphStore.EnsureIndexFormat before scrub, refusing older graph records/registrations with CG06017 and incompatible B-trees.
3. With the coordinator constructed, call `AnalyzeAndScrub()` to undo unproven record writers.
4. `GraphStore.Open` loads owner-two record directories and owner-one registrations.
5. `RecoverIndexesAsync(plan.Aborted)` scrubs aborted index writers and deleters.
6. `CompleteRecovery()` checkpoints after lifecycle classification has been consumed.

Crash tests ordinarily flush the journal and clone data and journal memory streams without disposing
the live database. MemoryStream has no durable-flush contract; these serialized images test recovery
replay and scrub, not physical persistence. They prove committed nodes, relationships and property
indexes survive while a partially applied logical transaction disappears. The graph store keeps an
identity directory of every retained version rebuilt in O(R log R) for R retained records at open.
Persisted revisions order each chain newest first; reads select the first visible version. Label
scans seek selected retained postings and decode visible matches in identity order. Traversal path enumeration and cycle limits are
owned and documented by the Graph root.

## Version publication and isolation

Each replacement encodes the record and new index keys before mutation, moves label postings and property-index
or adjacency entries, and tombstones the old version in one physical bracket. Rollback restores
stamps/removes new versions. Snapshot pins protect historical versions from purge; reclaimed/reused
references are removed from the directory. A per-transaction async gate serializes full store
mutations, including statement views, through directory publication.

Snapshot keeps its horizon and uses first-updater-wins. Original ReadCommitted contexts take a
fresh snapshot after the writer lock; caller-pinned statement views keep their horizon and may
conflict. Relationship insertion checks latest endpoint existence, so a concurrent endpoint
property update alone does not cause a conflict. See [GraphStore](graph-store.md).


## Confirmed decoding and page reuse

A record shorter than the 18-byte graph format header reports located truncation corruption
(`CK06003`), preserving its page, slot and `EndOfStreamException` cause. It does not receive the
unsupported-format `CG06017` code or recreation advice. A readable unsupported version still
uses that format refusal.

Record reads confirm their private bytes against a page write sequence before accepting a result or reporting a decode failure. Overlapping forward writes and rollback cause a retry. Stable malformed records retain page/slot context and the decoder cause through storage corruption translation. The shared record layer compacts fragmented pages before extension and refills partly empty older pages through in-memory free-byte hints, rechecking allocation generation and capacity. These changes preserve the stored format. See [shared record-layer behavior](../assimalign-cohesion-database-storage/design.md#confirmed-reads-and-record-space-reuse).

## Sources

- **Header truncation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Internal/Storage/GraphRecordCodec.cs` and `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/StorageCorruptionTests.cs`.

- **Label lifecycle and recovery** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/tests/Storage/GraphStoreLabelTests.cs`.

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/src/Assimalign.Cohesion.Database.Graph.csproj`.
