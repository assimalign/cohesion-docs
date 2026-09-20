# Assimalign.Cohesion.Database.Sql.Tcp

Reference this package when the composition root chooses TCP for a SQL endpoint.

> **Status:** Implemented.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[`SqlDatabaseServerOptionsExtensions`](sql-database-server-options-extensions.md)** — Documented public type.

Reference this package when the composition root chooses TCP for a SQL endpoint. It provides the
existing `SqlDatabaseServerOptions.Listen(Uri)` extension in the `Assimalign.Cohesion.Database.Sql`
namespace. The engine accepts only the generic `IConnectionListener`; this integration constructs a
`TcpConnectionListener`.

See [design](design.md) .

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Sql` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections.Tcp` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Tcp/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Tcp/src/Assimalign.Cohesion.Database.Sql.Tcp.csproj`.
