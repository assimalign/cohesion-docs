# Model project migration

Update package references, wire-type imports and remote error handling after model consolidation.

> **Status:** Implemented.

## Model packages and namespaces

SQL, Graph, Documents, KeyValuePair and Blob each include their former Catalog, Storage and empty
Security projects. Cache includes its former empty Catalog and Storage projects. Seventeen satellite
projects are removed. Catalog and Storage public types retain their namespaces and members, but
their assembly and package are now `Assimalign.Cohesion.Database.{Model}`.

Reference the model package for embedded or server work. Remove satellite package references,
recompile consumers and update assembly-qualified type names. No compatibility assemblies or type
forwarders are provided. Inside each implemented model, architecture tests enforce
engine → Catalog → Storage → shared kernel. Cache remains deferred.

## Wire packages

| New package and namespace | Public types moved from the engine |
| --- | --- |
| [Assimalign.Cohesion.Database.Sql.Protocol](../dotnet-apis/resources/database/assimalign-cohesion-database-sql-protocol/index.md) | 5: family, message identifiers and request/result codecs |
| [Assimalign.Cohesion.Database.Graph.Protocol](../dotnet-apis/resources/database/assimalign-cohesion-database-graph-protocol/index.md) | 13: messages, GraphValueCodec, GraphPath, GraphNode/GraphRelationship and their IDs |
| [Assimalign.Cohesion.Database.Documents.Protocol](../dotnet-apis/resources/database/assimalign-cohesion-database-documents-protocol/index.md) | 14: messages, identifiers, operations, Document, DocumentId and DocumentVersion |
| [Assimalign.Cohesion.Database.KeyValuePair.Protocol](../dotnet-apis/resources/database/assimalign-cohesion-database-keyvaluepair-protocol/index.md) | 5: family, message identifiers and request/result codecs |
| [Assimalign.Cohesion.Database.Blob.Protocol](../dotnet-apis/resources/database/assimalign-cohesion-database-blob-protocol/index.md) | 15: messages, transfer helper and BlobProperties |

These 52 types change namespace, assembly and package identity. Update imports to the matching
Protocol namespace. Engine requests, planners, catalogs, storage and servers remain in their model
assemblies. The move preserves wire bytes, disk formats, diagnostic codes and codec behavior.

A remote Graph application can reference these packages without referencing the Graph engine:

```xml
<ItemGroup>
  <PackageReference Include="Assimalign.Cohesion.Database.Graph.Client" />
  <PackageReference Include="Assimalign.Cohesion.Database.Graph.Protocol" />
</ItemGroup>
```

Use the repository's package-version management. Import transferred values from their new owner:

```csharp
using System;
using System.Collections.Generic;

using Assimalign.Cohesion.Database.Graph.Protocol;

var node = new GraphNode(new GraphNodeId(1), new[] { "Person" },
    new Dictionary<string, object?> { ["name"] = "Ada" });
Console.WriteLine(node.Id.Value); // 1
```

SQL and KeyValuePair each define `ProtocolExecuteMessage`, `ProtocolResultHeaderMessage` and
`ProtocolResultCompleteMessage`. Use namespace aliases when importing both families.

## Client dependencies

Each implemented model client references Database.Client, its model Protocol, shared Protocol and
Types, and Connections. Documents.Client also references model-neutral Execution. Database.Client
references Protocol, Types and Connections. Cache.Client is still an empty placeholder.

`COHDBCLIENT001` rejects client dependencies on engines and engine implementation families. It
checks evaluated project references, resolved assembly identities and engine source imports,
including references that only establish build order. Renamed DLLs and package references receive
the same checks. The client-only sample validates its reference closure before trim/AOT publishing.

## Remote failures

All six client exceptions derive directly from `System.Exception`. None derives from
`DatabaseException`, and model exceptions do not derive from `DatabaseClientException`.

| API used | Exception to catch | Diagnostics |
| --- | --- | --- |
| Shared Database.Client | DatabaseClientException | Wire diagnostics converted to shared Diagnostic values |
| SQL client | SqlClientException | Copied from a DatabaseClientException or nested SqlClientException |
| Graph client | GraphClientException | Copied from a DatabaseClientException or nested GraphClientException |
| Documents client | DocumentClientException | Explicit constructor diagnostics; null means empty |
| KeyValuePair client | KeyValueClientException | Empty, including when an inner exception has diagnostics |
| Blob client | BlobClientException | Empty, including when an inner exception has diagnostics |

Every Diagnostics property is declared by its client exception and returns a non-null read-only
snapshot. Supplied entries are defensively copied and stably ordered by source offset, with
unlocated entries last. Constructors, wire codes, inner exceptions and connection-reuse rules are
preserved. Catch the exception for the client API you call; an existing `catch (DatabaseException)`
will no longer handle remote failures. Continue using engine exceptions for embedded operations.

For example, with an existing SQL connection:

```csharp
using System;

using Assimalign.Cohesion.Database.Sql.Client;

try
{
    await connection.ExecuteAsync("SELECT absent FROM t");
}
catch (SqlClientException failure)
{
    Console.WriteLine(failure.Code);
    foreach (var diagnostic in failure.Diagnostics)
        Console.WriteLine(diagnostic.Code);
}
```

See [SqlClientException](../dotnet-apis/resources/database/assimalign-cohesion-database-sql-client/sql-client-exception.md)
for connection usability and transaction retry guidance.

## Sources

- `cohesion/resources/Database/README.md`.
- `cohesion/docs/resources/Database/DESIGN.md`.
- `cohesion/resources/Database/Directory.Build.targets`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/shared/ClientDiagnostics.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Client/samples/Assimalign.Cohesion.Database.Client.AotSample/Program.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.KeyValuePair.Client/src/Exceptions/KeyValueClientException.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Blob.Client/src/Exceptions/BlobClientException.cs`.
