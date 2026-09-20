# Configure a TCP listener

Configure SQL server options with the TCP listener extension.

> **Status:** Implemented.

The database template composes this same listener inside its nested server factory. The extension
returns the options for chaining; the composition root owns the listener.

```csharp
using System;

using Assimalign.Cohesion.Database.Sql;

var options = new SqlDatabaseServerOptions()
    .Listen(new Uri("cohesion-db://localhost:5740"));
```

[All examples](index.md) · [Assembly overview](../index.md)

## Sources

- **Primary source** — `cohesion/tooling/templates/Assimalign.Cohesion.Templates/src/content/cohesion-database/Program.cs`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Tcp/docs/Assembly/Assimalign.Cohesion.Database.Sql/SqlDatabaseServerOptionsExtensions/OVERVIEW.md`.
