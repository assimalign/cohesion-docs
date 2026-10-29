# Document

The Documents engine stores versioned JSON documents in named collections within a logical database.

> **Status:** Partial. Local and wire document operations, OQL SQL++ queries, collection/index DDL and durable storage execute.

Model Catalog and Storage components ship inside their model assembly. Wire messages, codecs and
transferred values ship in the model Protocol package, shared by engine and client. Remote clients
reference contracts without engines. See [package/import/error migration](../model-project-migration.md).

## Collections and sessions

`DocumentDatabaseEngine.Create` creates an engine. `DocumentDatabase` provides collection creation
and lookup, and each session binds to one database. Collection names, document identifiers, and JSON
property names are case-sensitive. Collections obtained through `session.Database` participate in
that session's transaction and remain bound to its lifetime.

Documents contain UTF-8 encoded JavaScript Object Notation (JSON). Object, array, and scalar roots
are accepted. Collections may contain documents with different shapes; the engine does not infer
a fixed schema. Absent fields and incompatible traversal evaluate to [MISSING](language/expressions/missing.md),
distinct from NULL, under SQL++ semantics.

[SELECT](language/statements/select.md) supports ALL/DISTINCT and VALUE/RAW with object and array
constructors. A bare top-level MISSING becomes NULL before deduplication and counting on every
transport; named members preserve absence. SELECT constructors allow 256 result levels, while
stored/INSERT JSON retains its 128-level limit.

[Expression subqueries](language/expressions/subqueries.md) return arrays on the
statement's snapshot and work/memory budgets, including correlated and DML forms. [EXISTS](language/expressions/exists.md) tests array presence.
The [operator precedence](language/expressions/operators.md#precedence) now places IS above
comparisons and EXISTS between them; this changes unparenthesized mixed predicates.

## Reading and writing

[34 SQL++ scalar functions](language/functions/scalar-functions.md) provide string, exact numeric,
date, type, array and conditional operations. NULL and MISSING retain their separate rules.
NOW_STR/NOW_MILLIS share the transaction-start UTC clock; COUNT/LENGTH/POSITION and other
integer-result functions preserve Int64 metadata and long values locally and over the wire.

- **Query and index language** — [Object Query Language (OQL)](language/index.md) provides `SELECT`,
  `INSERT`, `UPSERT`, `UPDATE`, `DELETE`, `EXPLAIN`, collection DDL, `CREATE INDEX` and `DROP INDEX` through the session's parse, plan, and execute pipeline.
- **OQL replacement** — [UPSERT](language/statements/upsert.md) inserts absent keys and replaces existing documents whole, maintaining indexes atomically. Repeated keys within one statement fail.
- **Document replacement** — `DocumentCollection.PutAsync` replaces the complete JSON value by
  identifier and returns a new version. An optional expected version must match a visible document.
- **Document deletion** — `DocumentCollection.DeleteAsync` removes the identified document using
  the supplied session. OQL provides insert-only [INSERT](language/statements/insert.md) and predicate
  [DELETE](language/statements/delete.md). [UPDATE](language/statements/update.md) provides nested
  SET/UNSET changes from the statement snapshot.
- **Collection changes** — direct database operations use automatic transactions; operations through
  `session.Database` use its active transaction.

Automatic operations commit on success and roll back on failure. A failing operation in an explicit
transaction rolls back that transaction. Snapshot and read-committed isolation are supported;
serializable isolation is rejected. Writers are serialized per logical database, and conflicting
changes raise `DatabaseTransactionAbortedException`.

Cancellation observed before explicit commit starts leaves the transaction usable with nothing
committed. A started commit ignores caller cancellation and returns its real outcome. Automatic
commit owns an uncancellable end after execution completes; execution and lock waits remain
cancellable. A dead wire connection can still lose the result. See
[commit and rollback](../../dotnet-apis/resources/database/assimalign-cohesion-database-transactions/transaction-coordinator.md#commit-and-rollback).

A content chunk failure returns to the engine, which aborts once through its transaction end
gate. Storage never independently aborts the borrowed transaction. The explicit session retains
`CD04101` refusals until rollback. After a successful automatic commit, snapshot-cleanup failure
preserves success and emits Warning event 13, `StatementCleanupFailed`; `OutOfMemoryException`
still propagates. See [error handling](language/diagnostics.md#operation-cleanup).

## Storage and indexes

The storage package validates JSON and persists stamped chunk chains. The catalog maintains
versioned identities and [single/composite, optionally UNIQUE indexes](language/statements/create-index.md)
in shared B+Trees. The typed catalog also supports [DISTINCT array-element postings](../../dotnet-apis/resources/database/assimalign-cohesion-database-documents/document-index-key.md);
OQL [array DDL and access planning](language/statements/create-index.md#array-keys-and-when) execute. Index definition and registration records
are v3; v1/v2 indexes are refused before recovery writes with CD06105 and require database
recreation with this engine and have no upgrade path. Index creation populates
existing documents immediately, while replacement and deletion maintain indexes in the same
transaction as document content.

Current catalog lookups stay independent of pre-snapshot overwrite count: tests read three
metadata records after 256 overwrites, also after reopen. Older snapshots retain required
history. The purge worker retires obsolete references for idle names without further lookups;
safe content/metadata reclamation lets sustained overwrites reuse pages. See
[catalog history](../../dotnet-apis/resources/database/assimalign-cohesion-database-documents/catalog-design.md#bounded-version-history).

File-backed databases use `document.dat`, `document.log`, and `document.bak`. Memory-backed databases
use the same storage and transaction implementation over memory streams. Queries and JSON values
are materialized in managed memory; chunked persistence does not imply bounded query memory.
Document content is limited to `Int32.MaxValue` bytes and JSON nesting to 128 levels.

String predicates, ordering, and grouping use ordinal, case-sensitive comparison. SQL collations
do not change document behavior. Schema-owned collections reject collection and index definition
changes, while their document contents remain mutable.

## Server and pooled client

[RETURNING](language/clauses/returning.md) projects INSERT/UPSERT/UPDATE post-images or DELETE
pre-images with the affected count. Projection and 16 MiB frame/256-level serialization preflight
run before commit. A positive StatementMemoryBudget (64 MiB default) estimates retained SELECT
source/group staging, native aggregates, projection/order storage and RETURNING together;
GROUP AS wrappers/arrays and DML pending buffers share the counter. Exhaustion gives CD05018
before publishing results; pending-buffer exhaustion precedes any write. Application callback-owned
state is outside the estimate. See [aggregate memory](language/functions/aggregate-functions.md#statement-memory).
No capability flag is negotiated.
Older clients reject RETURNING completion after an autocommit write has committed; retry reapplies
it. Closing an explicit session rolls its uncommitted work back. See
[client compatibility](../../dotnet-apis/resources/database/assimalign-cohesion-database-documents-client/design.md#returning-and-client-compatibility).

DocumentDatabaseServer serves OQL and document/collection operations through messages 64–72.
Protocol 2.0 changed projected rows to JSON Document frames; current shared protocol 2.1 negotiates
startup capabilities. Documents uses its Failure frame for structured diagnostics. Model payload
readers reject malformed UTF-8 rather than replacing characters.

The server formatter retains one encoded frame. TCP backpressure bounds queued output; InMemory
can queue more bytes if its reader stops. Engine queries and client results remain materialized. DocumentClient rents authenticated, database-bound connections and exposes QueryAsync,
ExecuteAsync, Get/Put/Delete and collection operations. One operation runs at a time. Completed
statement failures preserve the connection; malformed protocol traffic or unfinished cancellation
discards it. [Text transactions](language/statements/transactions.md) execute on the same rental;
return rolls back active or failed work before reuse, or discards an uncertified session.

See [DocumentClient](../../dotnet-apis/resources/database/assimalign-cohesion-database-documents-client/document-client.md)
and [DocumentProtocol](../../dotnet-apis/resources/database/assimalign-cohesion-database-documents-protocol/document-protocol.md).

## Reference

- **Language** — [Language (OQL)](language/index.md).
- **Database context** — [Database](../index.md) and [Overview](../overview.md).
- **Related language** — [Language (SQL)](../sql/language/index.md).
- **API reference** — [Database resource packages](../../dotnet-apis/resources/database/index.md).

## Key joins

[JOIN and NEST](language/clauses/join.md) look up stored documents by META(alias).id equality,
with INNER/LEFT preservation, arrays for NEST and per-source metadata. Joined statements require
explicit aliases in every SELECT scope. Lookups share the operation snapshot and a bounded
statement cache. This completes the OQL T2 lane.

## Array queries

[UNNEST and LEFT UNNEST](language/clauses/unnest.md)
expand arrays before filtering; [ANY/SOME/EVERY](language/expressions/quantified-predicates.md)
and [array IN](language/expressions/operators.md#in-and-between) test elements with SQL++
NULL/MISSING rules. SELECT * with UNNEST returns synthetic binding objects.

## Database lifecycle

Detected storage damage uses [CK06003](language/diagnostics.md#database-files) through
DatabaseCorruptionException with database and optional page context. Reads fail instead of
omitting damaged documents; refused corrupt opens also retain CD06103. Restore from a verified
backup as directed by the error. No SQLSTATE is assigned.

Create/open require the original document.dat, document.log and document.bak file set. Missing sidecars
are refused with CD06103 and never recreated, including for an empty closed database. Preserve
ambiguous files and restore the complete backup. A directory without data or a data file no larger
than one page is an incomplete create; its refusal directs DropDatabaseAsync then CreateDatabaseAsync.
Drop deletes the data file first. Recovery tolerates a torn uncommitted extension at the tail;
transient I/O failures ask for an open retry after resolution, without restore advice.

See [lifecycle APIs and examples](../../dotnet-apis/resources/database/assimalign-cohesion-database-documents/design.md#database-creation-open-and-drop)
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
operational failures. Catalog history-maintenance failure preserves the original purge exception.
See [purge behavior](../../dotnet-apis/resources/database/assimalign-cohesion-database-documents/design.md#version-purge-refusals)
and [shared engine health](../overview.md#engine-health-and-version-purge).

## Text transaction control

In-process and wire sessions accept [BEGIN/START, COMMIT and ROLLBACK](language/statements/transactions.md),
with Snapshot by default or explicit READ COMMITTED. CD07002 refuses controls only when the peer
has not negotiated transaction-status; a current client negotiates it. OQL collection/index DDL inside an explicit
transaction reports CD04105 and faults it; ROLLBACK recovers the session.

## Sources

- **Engine** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/OVERVIEW.md`.
- **Semantics and limits** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/DESIGN.md`.
- **Catalog** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/OVERVIEW.md`.
- **Storage** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/OVERVIEW.md`.
