# Configure database TCP listeners

Reference Assimalign.Cohesion.Database.Tcp in a standalone composition root. App.Database delivers it for Database SDK consumers. Import the shared extension namespace alongside the model options.

```csharp
using System;
using Assimalign.Cohesion.Database;
using Assimalign.Cohesion.Database.Sql;
using Assimalign.Cohesion.Database.Documents;
using Assimalign.Cohesion.Database.Graph;
using Assimalign.Cohesion.Database.KeyValuePair;
using Assimalign.Cohesion.Database.Blob;

var sql = new SqlDatabaseServerOptions().Listen(new Uri("cohesion-db://localhost:5740"));
var documents = new DocumentDatabaseServerOptions().Listen(new Uri("cohesion-db://localhost:5741"));
var graph = new GraphDatabaseServerOptions().Listen(new Uri("cohesion-db://localhost:5742"));
var keyValue = new KeyValueDatabaseServerOptions().Listen(new Uri("cohesion-db://localhost:5743"));
var blob = new BlobDatabaseServerOptions().Listen(new Uri("cohesion-db://localhost:5744"));
```

Pass each options instance to its model server or configure it in the engine builder's AddServer callback. The server owns the listener through its lifecycle. Listen preserves the concrete options type and requires a positive endpoint port.

[All examples](index.md) · [Assembly](../index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Tcp/tests/DatabaseTcpTests.cs`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Tcp/samples/Assimalign.Cohesion.Database.Tcp.AotSample/TcpServerSmoke.cs`.
