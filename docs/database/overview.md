# Overview

Cohesion Database composes independent model engines over shared storage, transaction, indexing, and protocol components.

> **Status:** Partial. Five model engines execute; Cache is deferred and language/operational support remains bounded.

Model Catalog and Storage components ship inside their model assembly. Wire messages, codecs and
transferred values ship in the model Protocol package, shared by engine and client. Remote clients
reference contracts without engines. See [package/import/error migration](model-project-migration.md).

## Resource and engines

The Database resource is Cohesion's multi-model online transaction processing (OLTP) data platform.
A customer-owned executable using `Assimalign.Cohesion.Sdk.Database` composes engines, schemas,
servers, and provisioning. Engines can also run directly inside another application's process.

| Engine family | Data model and language | Current boundary |
| --- | --- | --- |
| [SQL](sql/index.md) | Relational tables; Structured Query Language (SQL) | Measured declared subset, SQL server and typed client |
| [Document](document/index.md) | Versioned JavaScript Object Notation (JSON) documents; Object Query Language (OQL) | Bounded query/index-definition subset |
| [Graph](graph/index.md) | Property graph; Graph Query Language (GQL) | Bounded traversal and mutation subset |
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
| `Assimalign.Cohesion.Database.Storage` | Durable pages, buffer management, write-ahead log (WAL), recovery, and record storage |
| `Assimalign.Cohesion.Database.Indexing` | Shared key/index structures, including B+Tree indexes |
| `Assimalign.Cohesion.Database.Transactions` | Multi-version concurrency control (MVCC), snapshots, locks, transaction state, and durability binding |
| `Assimalign.Cohesion.Database.Types` | Shared scalar identities, collation, binary encoding, and boxed-value codec |
| `Assimalign.Cohesion.Database.Language` | Shared lexer/parser/diagnostic infrastructure |
| `Assimalign.Cohesion.Database.Execution` | Query request/result and execution-pipeline contracts |
| `Assimalign.Cohesion.Database.Protocol` | Bounded framing, handshake, lifecycle, errors, version negotiation, fixed model-family binding |
| `Assimalign.Cohesion.Database.Client` | Shared connection settings, pooling, authentication, framing, and exchange lifetime |
| `Assimalign.Cohesion.Database.Security` | Authentication and authorization contracts; model-specific enforcement is a separate concern |
| `Assimalign.Cohesion.Database.Hosting` | Application composition, configuration, services, provisioning, and server lifecycle |
| `Assimalign.Cohesion.Database.Embedded` | In-process engine composition, lookup, and reverse-order disposal |

The area root composes child roots; child roots do not depend on it. Each model composes the same
kernel rather than introducing separate paging, journaling, or locking. Model-specific layout,
catalog, language, client, security, and replication concerns stay with the model.

The former replication and governance placeholder projects and contracts are absent. These
features have no current runtime API.

## Host composition

`DatabaseApplication.CreateBuilder(args)` collects intent through `AddSql`,
`AddDocuments`, `AddGraph`, `AddKeyValue`, and `AddBlob`.
Callbacks run during the one-shot `Build`. Each callback configures an engine builder and may
register deferred worker/server factories through `AddWorker` and `AddServer`.
There is no application-builder `AddServer` or composition `Use` method.

`Build` creates usable engines and freezes registration, even if construction fails.
`application.Context.GetEngine(name)` returns a borrowed engine reference. Engines own their
workers and nested servers. Additional services, including compiled-schema provisioning, start
before servers; shutdown drains servers first. The application supports one start lifecycle.
Factory-created engines belong to the application; directly registered instances stay caller-owned.
Finish caller-owned sessions before disposing the application.

Concrete hosting supplies Cohesion configuration and dependency injection. It loads optional base
and environment JSON, `COHESION_CONFIG__` variables, captured arguments, and explicit providers,
with later providers winning. Its resolver runs interpreted with `EnableDynamicCode = false`.
The root and model builder interfaces expose no configuration or dependency-injection types.

## Getting started

This is a local-path adaptation of the template's `Program.cs`: deferred SQL registration,
nested Transmission Control Protocol (TCP) server creation, `SqlSchema.Compile`, named provisioning,
one-shot build, then run.
The original template receives its path and endpoint through generated
`Resource.Mounts.Data.Path` and `Resource.Endpoints.Db`.

In a Database SDK executable, this host provisions an `audit` database containing a schema-owned
`t` table. The TCP integration package supplies `Listen`; it uses the SQL namespace.

```csharp
using System;

using Assimalign.Cohesion.Database.Hosting;
using Assimalign.Cohesion.Database.Sql;
using Assimalign.Cohesion.Database.Sql.Schema;
using Assimalign.Cohesion.Hosting;

DatabaseApplicationBuilder builder = DatabaseApplication.CreateBuilder(args);

builder.AddSql((_, engine) =>
{
    engine.EngineName = "audit-sql";
    engine.RootPath = "./data/sql";
    engine.AddServer(value => SqlDatabaseServer.Create(
        (SqlDatabaseEngine)value,
        new SqlDatabaseServerOptions().Listen(
            new Uri("tcp://127.0.0.1:5439"))));
});

SqlCompiledSchema schema = SqlSchema.Compile("audit", database =>
{
    database.Table<Person>("t", table => table.Key(person => person.Id));
});
builder.AddDatabase("audit-sql", "audit", schema);

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

## Pooled connection leases

Each rent creates a new DatabaseConnection over an internal pooled connector. Disposal permanently
ends the lease; metadata and execution then throw ObjectDisposedException, while IsOpen returns
false. Repeated disposal or abort joins its original return, so a stale reference cannot affect a
later renter. Pool shutdown closes returned connectors and fails waiting renters as disposed.

> **Known gap (#1540):** End or roll back an explicit transaction before disposing its connection.
> A healthy pooled session retains unfinished transaction state, which a later renter can inherit.

See [connection lifetime](../dotnet-apis/resources/database/assimalign-cohesion-database-client/database-connection.md)
and [pooling](../dotnet-apis/resources/database/assimalign-cohesion-database-client/design.md#lifecycle-and-errors).

## Database file sets and lifecycle

Create/open/drop are engine operations. SQL completes only provably empty interrupted creation;
ambiguous catalogs and missing sidecars are refused without replacement files. Graph, Document and
Blob require their original journal and backup even for an empty closed database. Drop deletes the
primary data file before sidecars. Preserve ambiguous file sets and follow the coded refusal.

| Model | File-set refusal code | Reference |
| --- | --- | --- |
| SQL | COHSQLC001 | [Creation and open](sql/language/diagnostics.md#database-creation-and-open) |
| Graph | COHDBG018 | [Database files](graph/language/diagnostics.md#database-files) |
| Document | COHDBD003 | [Database files](document/language/diagnostics.md#database-files) |
| Blob | COHDBB004 | [Database files](blob/operations.md#database-file-diagnostics) |
| Key-Value Pair | COHDBK004 | [File sets](key-value-pair/diagnostics.md#missing-file-sets-and-interrupted-drop) |

These codes have no assigned SQLSTATE. Recovery tolerates uncommitted torn tails; that does not
permit replacing missing journals. Transient open/inspection I/O failures can be retried after
resolution and do not advise restore or drop. SQL remains at format 25.

The shared storage format is **4**, independent of SQL's data/catalog format 25 and Graph's record
format 4. Storage formats 3 and older have no upgrade reader. A Free page must be entirely zero;
reclaimed pages have a separate type and checksum. Every nonzero pooled page requires checksum
verification, with a raw CRC of zero escaped as `0xFFFFFFFF`. Page 0 has its own identity and
header-slot checksums. See [page integrity](../dotnet-apis/resources/database/assimalign-cohesion-database-storage/design.md#storage-format-4).

Detected storage damage raises
[DatabaseCorruptionException](../dotnet-apis/resources/database/assimalign-cohesion-database/database-corruption-exception.md)
with COHDBS003, the database, optional page and original cause. Refused opens retain existing
model diagnostics; Graph, Document and Blob also carry COHDBG018, COHDBD003 and COHDBB004.
Reads fail instead of silently omitting damaged data. Legitimate record reclamation still reads
as absence. SQL and Key-Value Pair map COHDBS003 to SQLSTATE XX001; other model references
assign none. Follow its verified-backup recovery advice. Attached-index corruption also retains
COHDBI002 and the model diagnostic with the index cause/page.

Recovery preserves both complete, verified checkpoint-anchor chains. Retired anchors outside those
chains are reclaimable even after a torn write or missing checksum; ordinary Free/Reclaimed pages
still require full verification. Newly initialized anchors have journal repair images. See
[checkpoint recovery](../dotnet-apis/resources/database/assimalign-cohesion-database-storage/design.md#checkpoints).

## Engine failure diagnostics

Engine boundaries translate storage and index failures into area exceptions with structured
Error diagnostics and the original cause. Transaction failures retain their area exception
types, and aborted-session refusals carry model diagnostics. Classification searches inner exceptions
and aggregate branches; corruption takes precedence over ordinary I/O. Specific format and
offline/unconfirmed-commit classifications remain in force. An I/O open failure releases its
resources and publishes no database, permitting retry once the condition is resolved.

| Model | Storage/I/O | Catalog/index | Additional failure codes |
| --- | --- | --- | --- |
| SQL | COHSQLE041 (58030 for I/O) | COHSQLE041 | COHSQLT005: aborted transaction |
| Key-Value Pair | COHDBK005 (58030 for I/O) | COHDBK005 | COHDBK001: aborted transaction |
| Document | COHDBD004 | COHDBD005 | COHDOC024: index exists; COHDOC025: index absent; COHDOC026: encoded key exceeds 1,024 bytes; COHDBD001: aborted transaction |
| Graph | COHDBG006 | COHDBG006 | COHDBG007: catalog snapshot conflict/aborted transaction; COHDBG003: deterministic catalog error; COHDBG009: oversized metadata; COHDBG019: disposed execution resource |
| Blob | COHDBB005 | COHDBB006 | COHDBB007: failed/inactive upload; COHDBB001: aborted transaction |

No SQLSTATE is assigned to these codes except the SQL/Key-Value Pair I/O mappings shown.
Attached-index corruption leads with COHDBS003 and retains COHDBI002 plus the model's catalog/index
code. Storage/index failures abort explicit transactions; roll back before continuing. SQL and
Key-Value Pair ordinary statement errors keep their existing statement-atomic behavior. SQL,
Document, Graph and Key-Value Pair wire ExecutionFailure preserves the session; Blob closes it.
Codes belong in Diagnostics. Storage-failure/corruption and SQL/Key-Value Pair/Graph aborted-transaction
refusal messages are ordinary unprefixed text. Refusals retain COHSQLT005, COHDBK001 or COHDBG007
and their original cause. Wire writers format the prefix once. SQL event 25 and Key-Value Pair event 11, TransactionCleanupFailed,
record secondary cleanup failures while preserving the original coded cause.

Graph disposal during execution or a wire lifetime guard reports COHDBG019 and keeps the
authenticated session ready; disposal of the protocol transport faults it without another write.
Graph Warning event 13, StatementCleanupFailed, records cleanup failure after successful
autocommit without changing that success, except OutOfMemoryException still propagates.
See [Graph wire behavior](../dotnet-apis/resources/database/assimalign-cohesion-database-graph/design.md#graph-wire-family).

## Checkpoint triggering

Checkpoint size and interval are soft triggers. After collecting a deferred lane, the shared worker
rechecks due work in the same pass and attempts at most one new checkpoint. Failures keep their
retry delay; busy storage waits for the next pass. This closes a consumed-wake gap without bounding
journal size; #1283 remains open. See [storage design](../dotnet-apis/resources/database/assimalign-cohesion-database-storage/design.md#checkpoint-triggering).

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
Transport selection belongs to the composition root. `Database.Sql.Tcp` supplies the existing
`Listen(Uri)` convenience for consumers that choose TCP. Engine packages reference no concrete
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

## MVP boundaries

SQL [schema DDL](sql/language/statements/create-schema.md) persists namespaces in current storage format 25;
dbo remains protected and code-first provisioning stays there. [Set operations](sql/language/clauses/set-operations.md)
execute in every query position. Document adds [34 SQL++ scalars](document/language/functions/scalar-functions.md),
and Graph supports [eager stage composition](graph/language/clauses/next.md), including Cypher WITH/UNWIND.

The current SQL contract measures 51 of 57 declared clauses. Document and Graph have their own
declared language subsets; SQL feature support does not transfer to them. Cache remains outside the
MVP. Security contracts do not imply complete model authorization. The former replication and
governance projects are removed and supply no current API.

SQL supports [ON CONFLICT](sql/language/clauses/on-conflict.md) with EXCLUDED and RETURNING,
[distinct/Boolean tests and LIKE ESCAPE](sql/language/expressions/operators.md), and final-row
foreign-key validation for multi-row UPDATE. OQL executes 31 of 32 declared clauses and GQL 26 of 33.

The Phase 29 hosting implementation supersedes older eager-builder examples in the program plan.
It adds no ApplicationModel functionality; that work remains deferred by the hosting design.
Use current package documentation and measured dialects rather than historical roadmap entries
to determine available behavior.

## See also

[Database](index.md) · [SQL language](sql/language/index.md) · [Database .NET APIs](../dotnet-apis/resources/database/index.md) · [Database SDK](../dotnet-apis/sdks/sdk-database/index.md)

## Sources

- **Provisional durability and storage integrity** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/tests/SqlWorkerResilienceTests.cs`, `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/tests/KeyValueWorkerResilienceTests.cs`, and `cohesion/resources/Database/Assimalign.Cohesion.Database.Storage/docs/DESIGN.md`.

- **Area architecture** — `cohesion/docs/resources/Database/DESIGN.md`
- **Package map** — `cohesion/resources/Database/README.md`
- **MVP boundaries** — `cohesion/docs/programs/DATABASE_MVP_FEATURES.md`
- **Historical sequencing** — `cohesion/docs/programs/DATABASE_PROGRAM_PLAN.md`
- **Current hosting contract** — `cohesion/docs/programs/DATABASE_HOSTING_DESIGN.md`
- **Hosting implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting/docs/DESIGN.md`
- **Template composition** — `cohesion/tooling/templates/Assimalign.Cohesion.Templates/src/content/cohesion-app/Acme.Database/Program.cs`
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
