# Overview

Cohesion Database composes independent model engines over shared storage, transaction, indexing, and protocol components.

> **Status:** Partial. Five model engines execute; Cache is deferred and language/operational support remains bounded.

Model Catalog and Storage components ship inside their model assembly. Wire messages, codecs and
transferred values ship in the model Protocol package, shared by engine and client. Remote clients
reference contracts without engines. See [package/import/error migration](model-project-migration.md).

The area design records ISO/IEC 39075:2024 as the Graph standard selected on 2026-09-17.
The executable [GQL reference](graph/language/index.md) describes its bounded subset and extensions.

## Resource and engines

The Database resource is Cohesion's multi-model online transaction processing (OLTP) data platform.
A customer-owned executable using `Assimalign.Cohesion.Sdk.Database` composes engines, schemas,
servers, and provisioning. Engines can also run directly inside another application's process.

| Engine family | Data model and language | Current boundary |
| --- | --- | --- |
| [SQL](sql/index.md) | Relational tables; Structured Query Language (SQL) | Measured declared subset, SQL server and typed client |
| [Document](document/index.md) | Versioned JavaScript Object Notation (JSON) documents; Object Query Language (OQL) | Bounded query/index-definition subset |
| [Graph](graph/index.md) | Property graph; ISO/IEC 39075:2024 Graph Query Language (GQL) | Bounded traversal and mutation subset |
| [Key-Value Pair](key-value-pair/index.md) | Ordered opaque byte keys and values; model commands | Point operations, scans, conditional writes, server and client |
| [Cache](cache/index.md) | Deferred model package | Placeholder, outside the minimum viable product (MVP) |
| [Blob](blob/index.md) | Named containers and streamed objects; typed stream API | Durable streaming engine, model server, and typed streaming client |

Each engine implements the shared lifecycle/session contracts but keeps its own vocabulary.
Relational tables live in the SQL family, collections in Documents, and nodes/relationships in
Graph. Sessions bind to one database; query languages cannot address another database or administer
the server. Cross-engine transactions are not provided.

## Shared pieces

| Component | Responsibility and status |
| --- | --- |
| `Assimalign.Cohesion.Database` | Area root: engine/database/session/transaction contracts, object ownership, compiled-schema identity, provisioning seam |
| `Assimalign.Cohesion.Database.Storage` | Durable pages, buffer management, write-ahead log (WAL), recovery, record storage and disposable query-file substrate |
| `Assimalign.Cohesion.Database.Indexing` | Shared key/index structures, including B+Tree indexes |
| `Assimalign.Cohesion.Database.Transactions` | Multi-version concurrency control (MVCC), snapshots, locks, transaction state, and durability binding |
| `Assimalign.Cohesion.Database.Types` | Shared scalar identities, collation, binary encoding, and boxed-value codec |
| `Assimalign.Cohesion.Database.Language` | Shared lexer/parser/diagnostic infrastructure |
| `Assimalign.Cohesion.Database.Execution` | Query request/result and execution-pipeline contracts |
| `Assimalign.Cohesion.Database.Protocol` | Bounded framing, handshake, lifecycle, errors, version negotiation, fixed model-family binding |
| `Assimalign.Cohesion.Database.Client` | Shared connection settings, pooling, authentication, framing, and exchange lifetime |
| `Assimalign.Cohesion.Database.Security` | Authentication and authorization contracts; model-specific enforcement is a separate concern |
| `Assimalign.Cohesion.Database.Hosting` | Application composition, configuration, services, and server lifecycle |
| `Assimalign.Cohesion.Database.Hosting.ApplicationModel` | Managed-instance registration, publication, snapshot restore, and application-added actions |
| `Assimalign.Cohesion.Database.Embedded` | In-process engine composition, lookup, and reverse-order disposal |

The area root composes child roots; child roots do not depend on it. Each model composes the same
kernel rather than introducing separate paging, journaling, or locking. Model-specific layout,
catalog, language, client, security, and replication concerns stay with the model.

The former replication and governance placeholder projects and contracts are absent. These
features have no current runtime API.

The [temporary-space substrate](../dotnet-apis/resources/database/assimalign-cohesion-database-storage/design.md#disposable-query-storage)
provides quotas, checked runs, sorting and hash partitions. SQL owns its configuration; no query
operator uses it yet, so large queries still refuse on their memory grant.

## Host composition

`DatabaseApplication.CreateBuilder(args)` collects intent through `AddSql`,
`AddDocuments`, `AddGraph`, `AddKeyValue`, and `AddBlob`.
Each verb takes an engine name and a callback that runs during the one-shot `Build`. The callback
configures the model's engine builder, declares databases with `AddDatabase`, and may register
deferred worker/server factories through `AddWorker` and `AddServer`.
There is no application-builder `AddServer` or composition `Use` method.

`Build` creates usable engines and freezes registration, even if construction fails.
`application.Context.GetEngine(name)` returns a borrowed engine reference. Engines own their
workers and nested servers. Engine Build opens, recovers, and provisions declared databases before
the application can start. Deferred migrations may continue on engine workers under their admission
gates. The application supports one start lifecycle. Factory-created engines belong to the
application; directly registered instances stay caller-owned. Finish caller-owned sessions before
disposing the application.

Lifecycle services come from both `builder.AddService(...)` and `IHostService` registrations on
`builder.Services`. After engines and servers are registered and every `AddService` product exists,
Build invokes the container's lifecycle factories once, directly. Every `StartingAsync` callback
finishes before any service starts. Start order is `AddService` registrations, container services, the
optional reopen service, then servers in engine/attachment order. Each service group preserves
registration order; stop reverses the complete order, draining servers first. `AddService` factory
products and container lifecycle factory products belong to the application and are disposed in
reverse order before engines. Supplied instances remain caller-owned. Container lifecycle
registrations must be singletons; scoped or transient registrations, null products, and duplicate
lifecycle objects fail Build. A forwarding factory returning a provider-owned service gives both
owners disposal responsibility, so its `Dispose` must be idempotent. Use `AddService(instance)` or
an instance registration to retain caller ownership. See
[hosting ownership](../dotnet-apis/resources/database/assimalign-cohesion-database-hosting/design.md#nested-servers-and-ownership).

Concrete hosting supplies Cohesion configuration and dependency injection. It loads optional base
and environment JSON, `COHESION_CONFIG__` variables, captured arguments, and explicit providers,
with later providers winning. Its resolver runs interpreted with `EnableDynamicCode = false`.
The root and model builder interfaces expose no configuration or dependency-injection types.

## Getting started

This program declares a SQL engine, a schema, and a Transmission Control Protocol (TCP) server.
Paths and endpoints are explicit application settings. The model's engine build provisions its
declared database during the application's one-shot Build, before the server starts.

In a Database SDK executable, this host provisions an `audit` database containing a schema-owned
`t` table. The TCP integration package supplies `Listen`; import Assimalign.Cohesion.Database to use it with any model options.

```csharp
using System;

using Assimalign.Cohesion.Database;
using Assimalign.Cohesion.Database.Hosting;
using Assimalign.Cohesion.Database.Sql;
using Assimalign.Cohesion.Hosting;

DatabaseApplicationBuilder builder = DatabaseApplication.CreateBuilder(args);

builder.AddSql("audit-sql", sql =>
{
    sql.Options.RootPath = "./data/sql";
    sql.AddDatabase("audit", database => database.Schema(schema =>
        schema.Table<Person>("t", table => table.Key(person => person.Id))));
    sql.AddServer(server => server.Listen(new Uri("cohesion-db://127.0.0.1:5439")));
});

await using DatabaseApplication application = builder.Build();
await application.RunAsync();

internal sealed record Person(int Id, string Name, int Age);
```

A separate client can run a statement against this database:

```csharp
using System;

using Assimalign.Cohesion.Connections.Tcp;
using Assimalign.Cohesion.Database.Client;
using Assimalign.Cohesion.Database.Sql.Client;

await using var client = SqlClient.Create(new SqlClientOptions
{
    Settings = DatabaseConnectionSettings.Parse(
        "Database=audit;Endpoint=127.0.0.1:5439"),
    ConnectionFactory = new TcpConnectionFactory()
});
await using var connection = await client.ConnectAsync();
SqlResultSet rows = await connection.QueryAsync(
    "SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS " +
    "WHERE TABLE_NAME = 't' ORDER BY ORDINAL_POSITION;");
foreach (SqlRow row in rows)
{
    Console.WriteLine(row.GetString(0) + ": " + row.GetString(1));
}
```

The client references `Assimalign.Cohesion.Database.Sql.Client` and
`Assimalign.Cohesion.Connections.Tcp`. Connection-string settings name the database, principal,
endpoint, and pool size; transport selection is a typed factory, not a connection-string plugin.
The default authenticator is `DatabaseAuthenticator.AllowAll`; the example uses that existing
development behavior rather than configuring model authorization.

For SQL row examples, use the [SELECT fixture](sql/language/statements/select.md#examples).
The repository's Database demo also creates all five engines directly, with no listeners.

## Managed resources

A Database program becomes a managed resource by adding this registration before Build:

```csharp
using Assimalign.Cohesion.ApplicationModel.Hosting.InProcess;
using Assimalign.Cohesion.Database.Hosting.ApplicationModel;

builder.Services.AddResource("orders-db", resource => resource.UseInProcess());
```

`UseInProcess()` opts into the platform's client. When the environment names no gateway platform,
`AddResource` registers nothing and the program runs as a plain application. A named platform the
program did not opt into fails registration. Managed registration adds one singleton lifecycle
service. After Build has opened, recovered, and migrated the engines, that service publishes the
instance's manifest and restores gateway snapshots in `StartingAsync`, before services or servers
start. It opens the control plane in `StartAsync`; servers drain before it closes on stop.

Database supplies no actions or instance health contributors of its own yet. The application adds
actions with `AddAction`, using its source-generated JSON contexts. A gateway write runs on one
instance and its snapshot is restored on the others. Catalog data and principals remain local to
each instance, so the retired catalog-mutation commands have no replacement yet. See
[the integration API](../dotnet-apis/resources/database/assimalign-cohesion-database-hosting-applicationmodel/index.md).

The gateway is a separate `Sdk.Gateway` executable. It declares Database generically:

```csharp
using Assimalign.Cohesion.ApplicationModel.Gateway;
using Assimalign.Cohesion.ApplicationModel.Gateway.InProcess;

ApplicationGatewayBuilder builder = ApplicationGateway.CreateBuilder("local").UseInProcess();
builder.AddResource("Database", "orders-db");

await using ApplicationGateway gateway = builder.Build();
await gateway.RunAsync();
```

Its project references the Database program and names the resource on that reference. The build
exposes the program's entry point and registers it with the in-process platform:

```xml
<ItemGroup>
  <ProjectReference Include="..\Orders.Database\Orders.Database.csproj" CohesionResource="orders-db" />
</ItemGroup>
```

The Database program references `ApplicationModel.Hosting.InProcess` for its platform opt-in;
`Database.Hosting.ApplicationModel` is already public in `App.Database`.

Three limits apply today:

- **Endpoints** — When COHESION_GATEWAY is nonempty, a model server without an explicit listener
  consumes its named runtime assignment: `sql`, `documents`, `graph`, `kv` or `blob`. Explicit
  listeners win. The in-process platform currently allocates only `http`, now used for Database
  health; dependents still pass wire addresses as literal settings. Two servers of one model need
  distinct listeners. See [server options](../dotnet-apis/resources/database/assimalign-cohesion-database/database-server-options.md).
- **Readiness** — The gateway treats first publication as ready, before Database servers listen
  and before its control plane opens. A dependent can deploy before Database accepts connections.
  The Database admin readiness gate is separate and does not change that gateway behavior.
- **Startup window** — The gateway's startup window must cover engine open, recovery, and inline
  schema migration inside Build, because publication follows that work. The Database host's
  `StartupTimeout` bounds service start only. Deferred migrations can continue on engine workers.

The integration design records [seven open questions](../dotnet-apis/resources/database/assimalign-cohesion-database-hosting-applicationmodel/design.md#current-limits),
including Database-owned actions, instance health, and the publication-to-open window. Managed HTTP
health binds the platform's plain-HTTP `http` assignment at a literal IP or localhost. Unsupported
schemes/hosts are skipped with Hosting event 18; a usable assignment replaces explicit health with
event 19. Unmanaged applications opt in with
[DatabaseApplicationBuilder.AddHealthEndpoint(Uri)](../dotnet-apis/resources/database/assimalign-cohesion-database-hosting/database-application-builder.md#addhealthendpoint).
Health starts before model servers, stops after they drain, and retains its existing readiness
gate. Socket bind failures report CK07102 and roll back startup.

Gateway `StopAsync` leaves the Database application running. Disposing the gateway disposes its
owned platform; the in-process platform then shuts down its Database program. See the
[in-process example](../dotnet-apis/resources/database/assimalign-cohesion-database-hosting-applicationmodel/examples/database-resource-in-process-tests.md).

## Pooled connection leases

Each rent creates a new DatabaseConnection over an internal pooled connector. Disposal permanently
ends the lease; metadata and execution then throw ObjectDisposedException, while IsOpen returns
false. Repeated disposal or abort joins its original return, so a stale reference cannot affect a
later renter. Pool shutdown closes returned connectors and fails waiting renters as disposed.

SQL, Graph and Documents negotiate server-reported transaction status and retained session state. Pool return rolls back active/failed work, then runs the family reset only if Ready still reports retained state (SQL uses DISCARD ALL). Reuse requires Idle and no retained state. Missing/invalid status, failed reset or a legacy transaction-capable peer causes connector discard.

See [connection lifetime](../dotnet-apis/resources/database/assimalign-cohesion-database-client/database-connection.md)
and [pooling](../dotnet-apis/resources/database/assimalign-cohesion-database-client/design.md#lifecycle-and-errors).

## Database file sets and lifecycle

Create/open/drop are engine operations. SQL completes only provably empty interrupted creation;
ambiguous catalogs and missing sidecars are refused without replacement files. Graph, Document and
Blob require their original journal and backup even for an empty closed database. Drop deletes the
primary data file before sidecars. Preserve ambiguous file sets and follow the coded refusal.

| Model | File-set refusal code | Reference |
| --- | --- | --- |
| SQL | CS03401 | [Creation and open](sql/language/diagnostics.md#database-creation-and-open) |
| Graph | CG06018 | [Database files](graph/language/diagnostics.md#database-files) |
| Document | CD06103 | [Database files](document/language/diagnostics.md#database-files) |
| Blob | CB06004 | [Database files](blob/operations.md#database-file-diagnostics) |
| Key-Value Pair | CV06104; CV06106 for incomplete creation or missing catalog | [File sets](key-value-pair/diagnostics.md#missing-file-sets-and-interrupted-drop) |

These codes have no assigned SQLSTATE. Recovery tolerates uncommitted torn tails; that does not
permit replacing missing journals. Transient open/inspection I/O failures can be retried after
resolution and do not advise restore or drop. SQL remains at format 36.

Key-Value Pair refuses an entirely missing catalog before opening either storage. File-backed
data of at most one page also receives CV06106 and a drop/recreate remedy; longer data with no
catalog requires restoring the complete backup. These refusals leave all surviving files unchanged.

The shared storage format is **4**, independent of SQL's data/catalog format 36 and Graph's record
format 5. Storage formats 3 and older have no upgrade reader. A Free page must be entirely zero;
reclaimed pages have a separate type and checksum. Every nonzero pooled page requires checksum
verification, with a raw CRC of zero escaped as `0xFFFFFFFF`. Page 0 has its own identity and
header-slot checksums. See [page integrity](../dotnet-apis/resources/database/assimalign-cohesion-database-storage/design.md#storage-format-4).

A format refusal leaves every file byte-for-byte unchanged, whether the database was cleanly
closed or crashed on an older format. This includes data, journals, backups, catalog sets and
sidecars. When replay is needed to inspect a gate, the engine uses a recovered read-only view,
disposes it, then opens the accepted database for normal physical and logical recovery. Probing
performs no checkpoint, truncation or file creation. Accepted-database recovery is unchanged.
Recovered probes emit no Storage lifecycle events, page-read counts or storage-gauge changes;
a corrupt page they read still emits PageChecksumFailed (Error). See
[read-only format preflight](../dotnet-apis/resources/database/assimalign-cohesion-database-storage/design.md#read-only-recovered-format-preflight).

Detected storage damage raises
[DatabaseCorruptionException](../dotnet-apis/resources/database/assimalign-cohesion-database/database-corruption-exception.md)
with CK06003, the database, optional page and original cause. Refused opens retain existing
model diagnostics; Graph, Document and Blob also carry CG06018, CD06103 and CB06004.
Reads fail instead of silently omitting damaged data. Legitimate record reclamation still reads
as absence. SQL and Key-Value Pair map CK06003 to SQLSTATE XX001; other model references
assign none. Follow its verified-backup recovery advice. Attached-index corruption also retains
CK06102 and the model diagnostic with the index cause/page.

Recovery preserves both complete, verified checkpoint-anchor chains. Retired anchors outside those
chains are reclaimable even after a torn write or missing checksum; ordinary Free/Reclaimed pages
still require full verification. Newly initialized anchors have journal repair images. See
[checkpoint recovery](../dotnet-apis/resources/database/assimalign-cohesion-database-storage/design.md#checkpoints).

### Header read fallback

Storage reads page 0 in independent 4 KiB halves. After a read error it accepts the other valid
slot only when the retained journal proves coverage, before replay writes. Every accepted writable-open fallback
emits HeaderReadFallback (Warning 18); missing coverage refuses open as I/O. A readable damaged
survivor with no valid slot is CK06003 corruption; two wholly unreadable halves retain I/O.
See [storage recovery](../dotnet-apis/resources/database/assimalign-cohesion-database-storage/design.md#page-0-and-alternating-headers).
The journal-size checkpoint setting is a soft trigger; holder-release progress is described under
[checkpoint triggering](#checkpoint-triggering).

## Engine failure diagnostics

Engine boundaries translate storage and index failures into area exceptions with structured
Error diagnostics and the original cause. Transaction failures retain their area exception
types, and aborted-session refusals carry model diagnostics. Classification searches inner exceptions
and aggregate branches; corruption takes precedence over ordinary I/O. Specific format and
offline/unconfirmed-commit classifications remain in force. An I/O open failure releases its
resources and publishes no database, permitting retry once the condition is resolved.

| Model | Storage/I/O | Catalog/index | Additional failure codes |
| --- | --- | --- | --- |
| SQL | CS06041 (58030 for I/O) | CS06041 | CS04005: aborted transaction; CS05042 (54000): encoded row/index-key size limit; CS05043 (57014): statement timeout; CS04044 (55P03): lock timeout; CS05045 (54000 for size refusals): result encoding or header/row limit; CS04006: changed-target retry |
| Key-Value Pair | CV06105 (58030 for I/O) | CV06105 | CV04101: aborted transaction |
| Document | CD06104 | CD06105 | CD03024: index exists; CD01025: index absent; CD02026: encoded key exceeds 1,024 bytes; CD04101: aborted transaction |
| Graph | CG06006 | CG06006 | CG04007: catalog snapshot conflict/aborted transaction; CG03003: deterministic catalog error; CG02009: oversized metadata; CG09019: disposed execution resource |
| Blob | CB06005 | CB06006 | CB02007: failed/inactive upload; CB04001: aborted transaction |

No SQLSTATE is assigned to these codes except the mappings shown.
Attached-index corruption leads with CK06003 and retains CK06102 plus the model's catalog/index
code. Storage/index failures abort explicit transactions; roll back before continuing. SQL and
Key-Value Pair ordinary statement errors keep their existing statement-atomic behavior. SQL,
Document, Graph and Key-Value Pair wire ExecutionFailure preserves the session; Blob closes it.
Codes belong in Diagnostics. Storage-failure/corruption and SQL/Key-Value Pair/Graph aborted-transaction
refusal messages are ordinary unprefixed text. Refusals retain CS04005, CV04101 or CG04007
and their original cause. Wire writers format the prefix once. SQL event 25 and Key-Value Pair event 11, TransactionCleanupFailed,
record secondary cleanup failures while preserving the original coded cause.

Graph disposal during execution or a wire lifetime guard reports CG09019 and keeps the
authenticated session ready; disposal of the protocol transport faults it without another write.
Graph Warning event 13, StatementCleanupFailed, records cleanup failure after successful
autocommit without changing that success, except OutOfMemoryException still propagates.
See [Graph wire behavior](../dotnet-apis/resources/database/assimalign-cohesion-database-graph/design.md#graph-wire-family).

Documents Warning event 13 and Blob Warning event 14 are also named `StatementCleanupFailed`.
After successful automatic commit, they report snapshot-cleanup failure without changing success;
their payload is `database`, `exceptionType`. `OutOfMemoryException` still propagates. Cleanup
failure before commit success or in an explicit transaction still fails the operation. See
[Documents](document/language/diagnostics.md#operation-cleanup) and [Blob](blob/operations.md#operation-cleanup).

## Engine health and version purge

SQL, Key-Value Pair, Document, Graph and Blob keep retrying background work after failure.
A purge blocked by the same page/owning bracket for at least three actual attempts over at least
FailureBackoff (one second on the engine clock) emits one observational WorkerFailed report for
that episode. Calls made before a retry is due do not count. A changed blocker or completed work
resets the episode; unrelated writes and journal activity do not. Close, drop and replacement end it.

The worker Fault and engine Faulted state expose the refusal while the database stays in service.
HasFailingWorker(database) remains false for an observation alone; existing operations and new
handshakes remain served. Observation-only reports never contribute to the offline window or its
minimum failed-pass count. Successful work clears the observation. A busy full pass retries after
FailureBackoff, rather than waiting for the next maintenance interval.

ForeignStorage refusals are reported immediately as operational failures, including checkpoints;
they follow the existing offline policy. A deferred checkpoint failure reaches the next worker
checkpoint call. Document and Blob catalog cleanup preserves an earlier purge exception's type
when non-OutOfMemoryException history maintenance also fails, attaching the cleanup failure in
`Exception.Data["HistoryMaintenanceFailure"]`. No diagnostic code or SQLSTATE is added.
See [worker APIs and events](../dotnet-apis/resources/database/assimalign-cohesion-database/design.md#worker-failure-reporting).
Cache has no implemented engine or purge worker.

## Maintenance operations

SQL, Key-Value Pair, Document, Graph and Blob isolate page write-back, WAL group flush, version
purge and checkpoints on dedicated database lanes. SQL/Key-Value index-maintenance dispatchers
have the same isolation, with empty production bodies. Each engine/worker kind admits 64 non-stuck
operations and retains at most 128 lane threads. Due databases are served round-robin. Stuck work
releases its admission slot after four worker intervals, clamped to 1–4 seconds, while retaining
the I/O and storage ownership. At the hard thread ceiling, more work waits for a runner to return.
Idle checks dispatch nothing; parked runners retire after 30 seconds.

Capacity/stuck observations set worker Fault and engine Faulted while databases stay served;
they do not advance offline policy or replace unresolved operational faults. Root provider
`Assimalign.Cohesion.Database` emits warning 35, WorkerLaneDelayed, naming database, worker kind,
reason (`capacity` or `stuck`) and elapsed operation time, at most once per second per database/worker.
Investigate the named storage/device and worker before treating the state as database unavailability.

Disposal waits for all in-flight lane I/O before storage closes. Warning 36,
WorkerLaneDisposalWaiting, repeats once per second per running database/worker with elapsed disposal
time. No disposal timeout abandons I/O; a device that never answers can keep shutdown waiting.
The lane caps are internal defaults rather than public tuning options. See
[worker limits, APIs and events](../dotnet-apis/resources/database/assimalign-cohesion-database/design.md#maintenance-lane-isolation).
Cache has no implemented engine or workers.

## Commit and cancellation

SQL, Document, Graph, Key-Value Pair and Blob use one explicit commit/rollback cancellation rule.
Cancellation observed before the end starts leaves the transaction usable and commits nothing.
Once the end is claimed, commit ignores caller cancellation and reports its real append/durability
outcome. Automatic commit takes over without the caller token after statement execution completes.
Self-committing SQL DDL finishes mandatory cleanup and delivers the committed result despite late
cancellation. Execution and lock waits remain cancellable before that boundary. A wire connection
that dies after commit can still lose the outcome frame. See [the coordinator API and example](../dotnet-apis/resources/database/assimalign-cohesion-database-transactions/transaction-coordinator.md#commit-and-rollback).

If a full journal buffer fails to drain before COMMIT is appended, no commit LSN is assigned:
the transaction ends Faulted, storage goes offline with JournalFlush, and the model's existing
offline diagnostic reaches the caller. Reopen scrubs the uncommitted writer. A failure after COMMIT
has been buffered instead has a committed-unconfirmed outcome. The model transaction-failure tests
cover this boundary for all five engines.

## Savepoints and transfer transactions

Named [SQL](sql/language/statements/savepoint.md), [OQL](document/language/statements/transactions.md#named-savepoints)
and [GQL](graph/language/statements/transactions.md) controls retain their model failure rules.
The completed [conformance matrix](../dotnet-apis/resources/database/assimalign-cohesion-database-transactions/design.md#savepoint-execution-conformance)
covers all five engines, incomplete-undo recovery, crash recovery and teardown during admitted
undo. [Blob wire transfers](blob/operations.md#host-transactions) can borrow an open host transaction;
completed uploads then await host commit and downloads pin one metadata/content version.

## Checkpoint triggering

Catalog writes and table reclamation share the checkpoint interlock with statement apply, undo
and pruning. A holder releasing admission checks size-due work and runs the checkpoint before
another writer enters, bounding journal growth under sustained writes even if the worker has not
run. Lanes recheck due state after dispatch. Refusals name the gate holder and physical blockers
through checkpoint events. One atomic bracket can still overshoot the size trigger; there is no
fixed byte cap. See
[storage design](../dotnet-apis/resources/database/assimalign-cohesion-database-storage/design.md#checkpoint-triggering).

## Read-only transactions and durability

SQL and Key-Value Pair's group-commit workers also flush provisional data/catalog storages while
create/open is still constructing the database. The flusher takes the storage snapshot before the
published-instance snapshot and flushes uncovered storages without the engine lock, so construction
commits can finish. Published databases retain per-database failure/backoff handling; a provisional
flush failure reaches the constructing commit. Documents, Blob and Graph retain self-help during open.

SQL, Document, Graph, Key-Value Pair and Blob share lazy journal enrollment. Beginning a logical
transaction reserves its sequence and captures its snapshot without appending a journal record.
Its first admitted write apply appends a logical BEGIN before opening the physical statement bracket;
later applies need no second BEGIN. Even an admitted apply that changes nothing remains a writer.

Autocommit reads, explicit read-only commit/rollback, disposal and ReadCommitted snapshot pins
append no lifecycle records and wait for no flush. They still leave the active table and release
their locks at completion. Writer COMMIT/abort records and commit durability keep their existing
guarantees. A reader promoted after a checkpoint enrolls before its first stamp, preserving recovery
classification. Readers can begin, pin cached pages and end while a checkpoint flushes;
writers still wait through its statement-apply gate. Flush I/O runs outside the pool monitor.
The physical-file latency probe is opt-in with COHESION_DB_PROBES=1 and sets no timing threshold;
see [checkpoint behavior](../dotnet-apis/resources/database/assimalign-cohesion-database-storage/design.md#checkpoints).
See [TransactionCoordinator](../dotnet-apis/resources/database/assimalign-cohesion-database-transactions/transaction-coordinator.md).

The shared [storage journal](../dotnet-apis/resources/database/assimalign-cohesion-database-storage/storage-journal.md)
accepts buffered appends while a durable flush is in progress. Only the force's written prefix is
confirmed durable; later appends need a later force. Failed writes or fsync still take the database
offline. Startup combines frame-checksum verification, commit classification and page planning in
one pass, then replays in journal order using that verified prefix. Later reads verify afresh.
See [recovery](../dotnet-apis/resources/database/assimalign-cohesion-database-storage/design.md#empty-pages-and-streaming-journal-recovery)
for ownership, replay and bounded-cache behavior.

## Embedded use

Omit a server factory when the application only needs local engine access. A model's
`Create(options)` also creates an operational engine directly, as the demo does.
`EmbeddedDatabase` composes supplied engines, exposes name/model lookup, and owns their
reverse-order disposal. That facade's ownership differs from hosting's borrowed-instance path.
Engine-owned maintenance runs in embedded and hosted modes alike.

## Wire model

Each model server owns its own `IDuplexConnectionListener`; the endpoint selects the model.
Transport selection belongs to the composition root. `Database.Tcp` supplies the shared
`Listen(Uri)` convenience for all five model server options for consumers that choose TCP. Engine packages reference no concrete
transport.

Each endpoint fixes a model family for the entire connection. The shared protocol owns framing,
handshake, version 2.1 capability negotiation, errors, and lifecycle. Each model Protocol package owns
message identifiers and payload codecs. Model clients use the shared pooled client to run their exchanges.

SQL sends statement text plus named values. A row result returns a header, rows, then completion;
a write returns completion with an affected count. Statements execute one at a time, without
pipelining or multiplexing. Transactions travel as SQL commands. Database selection occurs during
startup and remains fixed for the session.

SQL and GQL negotiate Warning/Error diagnostics; Documents uses its own Failure frame.
[EXPLAIN](sql/language/statements/explain.md) across all three languages returns one eight-column
plan shape without executing data statements. DocumentClient serves OQL and document/collection
operations; GraphPathResult exposes warnings at path completion. Graph additionally negotiates
[graph.composite-values](graph/language/expressions/list-values.md#limits-and-wire-capability)
for bounded list/map parameters and results, without changing Path payloads.

### Terminal delivery

All five servers drain complete terminal Errors/capacity refusals under ShutdownDrainTimeout.
TerminalDrainLinger defaults to two seconds after output completion so an unread pooled peer does
not occupy a session slot for the whole outer budget. Drain/linger must be positive, finite and
within 4,294,967,294 milliseconds. Clients consume queued terminal errors before new requests.
[Protocol](../dotnet-apis/resources/database/assimalign-cohesion-database-protocol/design.md#terminal-delivery-and-frame-writes)
describes warning counters, final cleanup joining and the remaining TLS delivery gap (#1086).

## MVP boundaries

SQL [schema DDL](sql/language/statements/create-schema.md) persists namespaces in current storage format 36;
dbo remains protected and code-first provisioning stays there. [Set operations](sql/language/clauses/set-operations.md)
execute in every query position. Document adds [34 SQL++ scalars](document/language/functions/scalar-functions.md),
and Graph supports [streaming stage composition](graph/language/clauses/next.md), including Cypher WITH/UNWIND.

The current SQL contract measures 91 of 93 declared clauses, including executable MERGE with
ordered actions and RETURNING, and executable [WITH RECURSIVE](sql/language/clauses/with.md).
Aggregate [FILTER](sql/language/clauses/filter.md) and
[Boolean/statistical aggregates](sql/language/functions/aggregate-functions.md#boolean-and-statistical-aggregates) execute.
[Grouping sets and masks](sql/language/clauses/group-by.md) and
[standalone sequence SQL](sql/language/statements/create-sequence.md) also execute.
Recursive fixpoints execute on demand, sharing readers and reclaiming consumed rows. Document and Graph have their own
declared language subsets; SQL feature support does not transfer to them. Cache remains outside the
MVP. Security contracts do not imply complete model authorization. The former replication and
governance projects are removed and supply no current API.

SQL [isolation options and early SET TRANSACTION](sql/language/statements/set-transaction.md)
select ReadCommitted or the original BEGIN snapshot. OQL [key JOIN and NEST](document/language/clauses/join.md)
complete its T2 lane with explicit source aliases, per-source metadata and bounded lookup caching.
GQL [EXCEPT/INTERSECT and OTHERWISE](graph/language/clauses/union.md) execute multiset operations
and lazy whole-result fallback under shared work/memory budgets.

SQL supports [ON CONFLICT](sql/language/clauses/on-conflict.md) with EXCLUDED and RETURNING,
[distinct/Boolean tests and LIKE ESCAPE](sql/language/expressions/operators.md), and final-row
foreign-key validation for multi-row UPDATE. OQL supports [text transactions](document/language/statements/transactions.md) in process and over
the wire; CD07002 only refuses peers without negotiated transaction-status. GQL supports outer
[WALK/TRAIL/SIMPLE/ACYCLIC modes](graph/language/patterns.md#path-modes), with TRAIL as its default.
SQL [UPDATE ... FROM](sql/language/statements/update.md#from-sources) and
[DELETE ... USING](sql/language/statements/delete.md#using-sources) execute; UPDATE rejects
multiple source matches per target with CS05068 before writes, while DELETE deduplicates targets.

Use the managed-resource integration and current engine-builder APIs alongside the measured
dialects to determine available behavior. Historical roadmap entries do not define the runtime.

## See also

[Database](index.md) · [SQL language](sql/language/index.md) · [Database .NET APIs](../dotnet-apis/resources/database/index.md) · [Database SDK](../dotnet-apis/sdks/sdk-database/index.md)

## Index-history maintenance

SQL, Documents, Graph and Key-Value Pair prune obsolete index history during the coordinator version-purge pass, under the same safe snapshot horizon as record versions. Unique inserts also prune their key run during their single descent. Old snapshots retain required versions. This does not merge empty tree pages or change disk formats. See [index maintenance](../dotnet-apis/resources/database/assimalign-cohesion-database-indexing/design.md#safe-index-history-pruning).

## Record reads and space reuse

Unlatched reads confirm against the page write sequence and retry overlapping mutations; decode failures are reported only from a confirmed copy, with record location and cause. Inserts and growing updates compact fragmented pages when packed capacity suffices. Inserts also revisit partly empty older pages using in-memory free-byte hints before extending. See [storage behavior](../dotnet-apis/resources/database/assimalign-cohesion-database-storage/design.md#confirmed-reads-and-record-space-reuse).

## Buffer-pool concurrency

Cached page pins use concurrent lookup and frame-local pin counts. Clock replacement chooses unpinned victims, and a single pending load coalesces readers of the same cold page. Reads, writes and growth run outside the global mapping lock; capacity includes loading reservations. Flush, resize and disposal retain explicit I/O ownership and WAL ordering. See [buffer-pool design](../dotnet-apis/resources/database/assimalign-cohesion-database-storage/design.md#the-buffer-pool).

Replacement uses clock reference bits rather than LRU ordering. Buffer-pool growth and shrink
both wait for in-flight cold I/O. Open seeds free-byte hints from page headers; unknown candidate
geometry is measured under a read pin before write touch, avoiding pre-images for full pages.
A sole mutable page handle carries read permission across serialized thread handoffs and awaits;
nested writers for that page on another thread are refused before exposing mutable bytes. See
[confirmed reads and reuse](../dotnet-apis/resources/database/assimalign-cohesion-database-storage/design.md#confirmed-reads-and-record-space-reuse).

## Sources

- **Graph standard** — `cohesion/docs/resources/Database/DESIGN.md` records ISO/IEC 39075:2024 in the model-root table.

- **Provisional durability and storage integrity** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlWorkerResilienceTests.cs`, `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueWorkerResilienceTests.cs`, and `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/docs/DESIGN.md`.

- **Area architecture** — `cohesion/docs/resources/Database/DESIGN.md`
- **Package map** — `cohesion/resources/Database/README.md`
- **MVP boundaries** — `cohesion/docs/programs/DATABASE_MVP_FEATURES.md`
- **Historical sequencing** — `cohesion/docs/programs/DATABASE_PROGRAM_PLAN.md`
- **Current hosting contract** — `cohesion/docs/programs/DATABASE_HOSTING_DESIGN.md`
- **Hosting implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting/docs/DESIGN.md`
- **Current composition and ordering** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting/src/DatabaseApplicationBuilder.cs`
- **Managed resources** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting.ApplicationModel/docs/DESIGN.md` and `tests/TestObjects/RunningResource.cs`
- **Gateway programs and project references** — `cohesion/docs/libraries/ApplicationModel/DESIGN.md`
- **Embedded ownership** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Embedded/docs/DESIGN.md`
- **Client API** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/docs/OVERVIEW.md`
- **Client settings** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/src/DatabaseConnectionSettings.cs`
- **TCP client transport** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/src/TcpConnectionFactory.cs`
- **Shared wire model** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/docs/OVERVIEW.md`
- **SQL wire exchange** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/WIRE-PROTOCOL.md`
- **Security contracts** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Security/docs/OVERVIEW.md`
- **Document model** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/OVERVIEW.md`
- **Graph model** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/OVERVIEW.md`
- **Key-value model** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/OVERVIEW.md`
- **Current Blob surface** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/docs/OVERVIEW.md`
