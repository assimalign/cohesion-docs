# Database

Database supplies shared kernels, model engines, hosting, clients, and orchestration contracts.

> **Status:** Partial. Package-level pages distinguish implemented behavior from deferred surfaces.

The multi-model OLTP database engine family for Cohesion: five independent database engines —
**SQL**, **Documents**, **Graph**, **Blob**, and **KeyValuePair** — sharing one durable kernel
(storage, write-ahead logging, transactions, indexing). A hosted database is an ordinary
customer-owned `Sdk.Database` executable whose `Program.cs` composes its engines, code-first schema,
servers, and provisioning.

## Packages

| Package | Role | Shared-framework delivery |
|---|---|---|
| [`Assimalign.Cohesion.Database`](assimalign-cohesion-database/index.md) | `Root` | Public reference and runtime |
| [`Assimalign.Cohesion.Database.ApplicationModel`](assimalign-cohesion-database-applicationmodel/index.md) | `Application` model | Separate package; not in this framework |
| [`Assimalign.Cohesion.Database.Blob`](assimalign-cohesion-database-blob/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Blob.Catalog`](assimalign-cohesion-database-blob-catalog/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Blob.Client`](assimalign-cohesion-database-blob-client/index.md) | Client | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Blob.Replication`](assimalign-cohesion-database-blob-replication/index.md) | Feature library | Not listed in this framework |
| [`Assimalign.Cohesion.Database.Blob.Security`](assimalign-cohesion-database-blob-security/index.md) | Feature library | Not listed in this framework |
| [`Assimalign.Cohesion.Database.Blob.Storage`](assimalign-cohesion-database-blob-storage/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Cache`](assimalign-cohesion-database-cache/index.md) | Feature library | Not listed in this framework |
| [`Assimalign.Cohesion.Database.Cache.Catalog`](assimalign-cohesion-database-cache-catalog/index.md) | Feature library | Not listed in this framework |
| [`Assimalign.Cohesion.Database.Cache.Client`](assimalign-cohesion-database-cache-client/index.md) | Client | Separate package; not in this framework |
| [`Assimalign.Cohesion.Database.Cache.Language`](assimalign-cohesion-database-cache-language/index.md) | Feature library | Not listed in this framework |
| [`Assimalign.Cohesion.Database.Cache.Storage`](assimalign-cohesion-database-cache-storage/index.md) | Feature library | Not listed in this framework |
| [`Assimalign.Cohesion.Database.Client`](assimalign-cohesion-database-client/index.md) | Client | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Documents`](assimalign-cohesion-database-documents/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Documents.Catalog`](assimalign-cohesion-database-documents-catalog/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Documents.Client`](assimalign-cohesion-database-documents-client/index.md) | Client | Separate package; not in this framework |
| [`Assimalign.Cohesion.Database.Documents.Language`](assimalign-cohesion-database-documents-language/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Documents.Replication`](assimalign-cohesion-database-documents-replication/index.md) | Feature library | Not listed in this framework |
| [`Assimalign.Cohesion.Database.Documents.Security`](assimalign-cohesion-database-documents-security/index.md) | Feature library | Not listed in this framework |
| [`Assimalign.Cohesion.Database.Documents.Storage`](assimalign-cohesion-database-documents-storage/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Embedded`](assimalign-cohesion-database-embedded/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Execution`](assimalign-cohesion-database-execution/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Governance`](assimalign-cohesion-database-governance/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Graph`](assimalign-cohesion-database-graph/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Graph.Catalog`](assimalign-cohesion-database-graph-catalog/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Graph.Client`](assimalign-cohesion-database-graph-client/index.md) | Client | Separate package; not in this framework |
| [`Assimalign.Cohesion.Database.Graph.Language`](assimalign-cohesion-database-graph-language/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Graph.Replication`](assimalign-cohesion-database-graph-replication/index.md) | Feature library | Not listed in this framework |
| [`Assimalign.Cohesion.Database.Graph.Security`](assimalign-cohesion-database-graph-security/index.md) | Feature library | Not listed in this framework |
| [`Assimalign.Cohesion.Database.Graph.Storage`](assimalign-cohesion-database-graph-storage/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Hosting`](assimalign-cohesion-database-hosting/index.md) | Hosting | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Indexing`](assimalign-cohesion-database-indexing/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.KeyValuePair`](assimalign-cohesion-database-keyvaluepair/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.KeyValuePair.Catalog`](assimalign-cohesion-database-keyvaluepair-catalog/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.KeyValuePair.Client`](assimalign-cohesion-database-keyvaluepair-client/index.md) | Client | Public reference and runtime |
| [`Assimalign.Cohesion.Database.KeyValuePair.Replication`](assimalign-cohesion-database-keyvaluepair-replication/index.md) | Feature library | Not listed in this framework |
| [`Assimalign.Cohesion.Database.KeyValuePair.Security`](assimalign-cohesion-database-keyvaluepair-security/index.md) | Feature library | Not listed in this framework |
| [`Assimalign.Cohesion.Database.KeyValuePair.Storage`](assimalign-cohesion-database-keyvaluepair-storage/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Language`](assimalign-cohesion-database-language/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Memory`](assimalign-cohesion-database-memory/index.md) | Feature library | Not listed in this framework |
| [`Assimalign.Cohesion.Database.Protocol`](assimalign-cohesion-database-protocol/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Replication`](assimalign-cohesion-database-replication/index.md) | Feature library | Not listed in this framework |
| [`Assimalign.Cohesion.Database.Security`](assimalign-cohesion-database-security/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Sql`](assimalign-cohesion-database-sql/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Sql.Catalog`](assimalign-cohesion-database-sql-catalog/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Sql.Client`](assimalign-cohesion-database-sql-client/index.md) | Client | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Sql.Language`](assimalign-cohesion-database-sql-language/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Sql.Replication`](assimalign-cohesion-database-sql-replication/index.md) | Feature library | Not listed in this framework |
| [`Assimalign.Cohesion.Database.Sql.Schema`](assimalign-cohesion-database-sql-schema/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Sql.Security`](assimalign-cohesion-database-sql-security/index.md) | Feature library | Not listed in this framework |
| [`Assimalign.Cohesion.Database.Sql.Storage`](assimalign-cohesion-database-sql-storage/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Sql.Tcp`](assimalign-cohesion-database-sql-tcp/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Storage`](assimalign-cohesion-database-storage/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Testing`](assimalign-cohesion-database-testing/index.md) | Testing | Not listed in this framework |
| [`Assimalign.Cohesion.Database.Transactions`](assimalign-cohesion-database-transactions/index.md) | Feature library | Public reference and runtime |
| [`Assimalign.Cohesion.Database.Types`](assimalign-cohesion-database-types/index.md) | Feature library | Public reference and runtime |

## Architecture

The area root rolls up model-neutral child roots for types, language, storage, transactions,
execution, indexing, protocol, security, and governance. Model-specific concepts belong in
the SQL, Documents, Graph, Blob, and KeyValuePair families. The same engines support direct
in-process use and hosted operation; `Database.Embedded` composes engines without a
wire server. This architectural capability does not imply that every other resource already
uses an embedded engine.

## Dependency boundary

The single runtime module is `Assimalign.Cohesion.Database.Hosting`. Roots and feature libraries do
not depend on the hosting family or on `Assimalign.Cohesion.Hosting` and its child libraries.
Feature registration composes against the root contracts. The exact runtime may reference its area
root and its hosting-family integrations; integrations may not reference the exact runtime. The
declarative application model remains separate from the runtime. See the
[resource dependency rules](../index.md#dependency-rules) .

The hosting family in this area contains `Assimalign.Cohesion.Database.Hosting`.

## Framework and SDK

`Assimalign.Cohesion.Sdk.Database` delivers the `Assimalign.Cohesion.App.Database` family. Its
reference-pack project declares `CohesionFrameworkName` and imports
`frameworks/Assimalign.Cohesion.App.props`, the public and private assembly inventory.
`CohesionFrameworkAssembly` entries appear in the reference and runtime packs;
`CohesionFrameworkPrivateAssembly` entries appear only at runtime.

| Public reference-pack assembly |
|---|
| `Assimalign.Cohesion.App.Database` |
| `Assimalign.Cohesion.Database` |
| `Assimalign.Cohesion.Database.Types` |
| `Assimalign.Cohesion.Database.Language` |
| `Assimalign.Cohesion.Database.Storage` |
| `Assimalign.Cohesion.Database.Transactions` |
| `Assimalign.Cohesion.Database.Indexing` |
| `Assimalign.Cohesion.Database.Execution` |
| `Assimalign.Cohesion.Database.Governance` |
| `Assimalign.Cohesion.Database.Protocol` |
| `Assimalign.Cohesion.Database.Security` |
| `Assimalign.Cohesion.Database.Client` |
| `Assimalign.Cohesion.Database.Embedded` |
| `Assimalign.Cohesion.Database.Hosting` |
| `Assimalign.Cohesion.Database.Sql` |
| `Assimalign.Cohesion.Database.Sql.Tcp` |
| `Assimalign.Cohesion.Database.Sql.Language` |
| `Assimalign.Cohesion.Database.Sql.Catalog` |
| `Assimalign.Cohesion.Database.Sql.Schema` |
| `Assimalign.Cohesion.Database.Sql.Storage` |
| `Assimalign.Cohesion.Database.Sql.Client` |
| `Assimalign.Cohesion.Database.KeyValuePair` |
| `Assimalign.Cohesion.Database.KeyValuePair.Storage` |
| `Assimalign.Cohesion.Database.KeyValuePair.Catalog` |
| `Assimalign.Cohesion.Database.KeyValuePair.Client` |
| `Assimalign.Cohesion.Database.Blob` |
| `Assimalign.Cohesion.Database.Blob.Storage` |
| `Assimalign.Cohesion.Database.Blob.Catalog` |
| `Assimalign.Cohesion.Database.Blob.Client` |
| `Assimalign.Cohesion.Database.Documents` |
| `Assimalign.Cohesion.Database.Documents.Language` |
| `Assimalign.Cohesion.Database.Documents.Catalog` |
| `Assimalign.Cohesion.Database.Documents.Storage` |
| `Assimalign.Cohesion.Database.Graph` |
| `Assimalign.Cohesion.Database.Graph.Language` |
| `Assimalign.Cohesion.Database.Graph.Catalog` |
| `Assimalign.Cohesion.Database.Graph.Storage` |

| Private runtime assembly |
|---|
| `Assimalign.Cohesion.Web` |
| `Assimalign.Cohesion.Web.Hosting` |
| `Assimalign.Cohesion.Web.Hosting.Resources` |
| `Assimalign.Cohesion.Web.Hosting.Health` |
| `Assimalign.Cohesion.Web.Health` |
| `Assimalign.Cohesion.Web.Routing` |
| `Assimalign.Cohesion.Http` |
| `Assimalign.Cohesion.Http.Connections` |
| `Assimalign.Cohesion.Http.RequestLimits` |
| `Assimalign.Cohesion.Connections.Tcp` |
| `Assimalign.Cohesion.Connections.Quic` |
| `Assimalign.Cohesion.Connections.Security` |
| `Assimalign.Cohesion.IdentityModel` |
| `Assimalign.Cohesion.IdentityModel.Token` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` |

`Application`-model and client packages are NuGet-only and are excluded from the area shared
framework. The package table distinguishes assemblies present in the source tree from those included
by the current framework inventory.

## Related documentation

- **Product** — [Database](../../../database/index.md).
- **SDK** — [`Assimalign.Cohesion.Sdk.Database`](../../sdks/sdk-database/index.md).
- **Parent** — [Resources](../index.md).

## Sources

- **Primary source** — `cohesion/resources/Database/README.md`.
- **Source** — `cohesion/.claude/rules/resource-areas.md`.
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.props`.
- **Source** — `cohesion/frameworks/Assimalign.Cohesion.App.Database.Refs/src/Assimalign.Cohesion.App.Database.Refs.csproj`.
- **Architecture source** — `cohesion/docs/resources/Database/DESIGN.md`.
