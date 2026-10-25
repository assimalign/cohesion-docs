# Assimalign.Cohesion.Database.Tcp

The shared TCP composition package supplies Listen(Uri) for SQL, Documents, Graph, Key-Value Pair and Blob server options. It replaces the retired Assimalign.Cohesion.Database.Sql.Tcp package. Consumers reference Database.Tcp and add `using Assimalign.Cohesion.Database;`. The server keeps the generic IDuplexConnectionListener boundary.

> **Status:** Implemented.

- [DatabaseServerOptionsExtensions](database-server-options-extensions.md) — Generic Listen extension and endpoint validation.
- [Shared options](../assimalign-cohesion-database/database-server-options.md) — Transport/authentication/session defaults.
- [Design](design.md) — Dependency and ownership rules.
- [Examples](examples/index.md) — Five-model composition.

## Delivery

App.Database publicly carries Database.Tcp for SDK consumers. Connections.Tcp remains runtime-private in the framework closure; engines reference generic Connections. Standalone composition roots can reference Database.Tcp directly. The package references Database and Connections.Tcp.

[Parent: Database](../index.md)

## Sources

- `cohesion/resources/Database/Assimalign.Cohesion.Database.Tcp/docs/OVERVIEW.md`.
- `cohesion/resources/Database/Assimalign.Cohesion.Database.Tcp/src/Assimalign.Cohesion.Database.Tcp.csproj`.
