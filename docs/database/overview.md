# Overview

Cohesion Database composes independent model engines over shared storage, transaction, indexing, and protocol components.

> **Status:** Partial. Five model engines execute; Cache is deferred and language/operational support remains bounded.

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
| `Assimalign.Cohesion.Database.Replication` | Replication coordinator contract; implementation remains post-MVP |
| `Assimalign.Cohesion.Database.Governance` | Resource-governor admission contract; operational build-out remains post-MVP |
| `Assimalign.Cohesion.Database.Hosting` | Application composition, configuration, services, provisioning, and server lifecycle |
| `Assimalign.Cohesion.Database.Embedded` | In-process engine composition, lookup, and reverse-order disposal |

The area root composes child roots; child roots do not depend on it. Each model composes the same
kernel rather than introducing separate paging, journaling, or locking. Model-specific layout,
catalog, language, client, security, and replication concerns stay with the model.

> **Status:** Not yet implemented. Replication and governance have contracts, but the operational implementations described by the program are deferred.

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

## Embedded use

Omit a server factory when the application only needs local engine access. A model's
`Create(options)` also creates an operational engine directly, as the demo does.
`EmbeddedDatabase` composes supplied engines, exposes name/model lookup, and owns their
reverse-order disposal. That facade's ownership differs from hosting's borrowed-instance path.
Engine-owned maintenance runs in embedded and hosted modes alike.

## Wire model

Each endpoint fixes a model family for the entire connection. The shared protocol owns framing,
handshake, version 1.0 negotiation, errors, and lifecycle; each model owns message identifiers and
payload codecs. Model clients use the shared pooled client to run their exchanges.

SQL sends statement text plus named values. A row result returns a header, rows, then completion;
a write returns completion with an affected count. Statements execute one at a time, without
pipelining or multiplexing. Transactions travel as SQL commands. Database selection occurs during
startup and remains fixed for the session.

## MVP boundaries

The current SQL contract measures 33 of 49 declared clauses. Document and Graph have their own
declared language subsets; SQL feature support does not transfer to them. Cache remains outside the
MVP. Security contracts do not imply complete model authorization, and replication/governance
contracts do not imply operational implementations.

The Phase 29 hosting implementation supersedes older eager-builder examples in the program plan.
It adds no ApplicationModel functionality; that work remains deferred by the hosting design.
Use current package documentation and measured dialects rather than historical roadmap entries
to determine available behavior.

## See also

[Database](index.md) · [SQL language](sql/language/index.md) · [Database .NET APIs](../dotnet-apis/resources/database/index.md) · [Database SDK](../dotnet-apis/sdks/sdk-database/index.md)

## Sources

- **Area architecture** — `cohesion/docs/resources/Database/DESIGN.md`
- **Package map** — `cohesion/resources/Database/README.md`
- **MVP boundaries** — `cohesion/docs/programs/DATABASE_MVP_FEATURES.md`
- **Historical sequencing** — `cohesion/docs/programs/DATABASE_PROGRAM_PLAN.md`
- **Current hosting contract** — `cohesion/docs/programs/DATABASE_HOSTING_DESIGN.md`
- **Hosting implementation** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Hosting/docs/DESIGN.md`
- **Template composition** — `cohesion/tooling/templates/Assimalign.Cohesion.Templates/src/content/cohesion-app/Acme.Database/Program.cs`
- **In-process examples** — `cohesion/resources/Database/samples/Assimalign.Cohesion.Database.Demo/Program.cs`
- **Embedded ownership** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Embedded/docs/DESIGN.md`
- **Client API** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/docs/OVERVIEW.md`
- **Client settings** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/src/DatabaseConnectionSettings.cs`
- **TCP client transport** — `cohesion/libraries/Connections/Assimalign.Cohesion.Connections.Tcp/src/TcpConnectionFactory.cs`
- **Shared wire model** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Protocol/docs/OVERVIEW.md`
- **SQL wire exchange** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql/docs/WIRE-PROTOCOL.md`
- **Security contracts** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Security/docs/OVERVIEW.md`
- **Replication contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Replication/src/Abstractions/IReplicationCoordinator.cs`
- **Governance contract** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Governance/src/Abstractions/IResourceGovernor.cs`
- **Document model** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Documents/docs/OVERVIEW.md`
- **Graph model** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Graph/docs/OVERVIEW.md`
- **Key-value model** — `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair/docs/OVERVIEW.md`
- **Current Blob surface** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob/docs/OVERVIEW.md`
