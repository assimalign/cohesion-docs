# Assimalign.Cohesion.Database.Sql.Client

The typed SQL client: a relational surface — commands, typed result sets, a SQL-scoped error taxonomy, and a telemetry hook — layered over the shared `Assimalign.Cohesion.Database.Client` core.

> **Status:** Partial.

## Contract dependencies and client errors

This client references Database.Client, Sql.Protocol, shared Protocol and Types, and Connections. Wire contracts and transferred values use the
`Assimalign.Cohesion.Database.Sql.Protocol` namespace. The client references no engine.
COHDBCLIENT001 rejects engine project references, resolved engine assemblies and engine source imports.

`SqlClientException` derives directly from `System.Exception` and declares its own
non-null, read-only Diagnostics. Catch this client exception for remote failures; catching
`DatabaseException` no longer handles them. See the [migration guide](../../../../database/model-project-migration.md#remote-failures).

## Reference pages

- **[SqlConnection](sql-connection.md)** — Rental lifetime, operations and exceptions.
- **[SqlParameterCollection](sql-parameter-collection.md)** — Public contract and usage.
- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.
- **[SqlResultSet](sql-result-set.md)** — Public signature and diagnostics.
- **[SqlClientException](sql-client-exception.md)** — Values and usage.

The typed SQL client: a relational surface — commands, typed result sets, a SQL-scoped error
taxonomy, and a telemetry hook — layered over the shared `Assimalign.Cohesion.Database.Client` core.

## Purpose

`Database.Client` is the model-agnostic client core: it dials the server, runs the wire handshake,
and pools authenticated connections. This package owns SQL result materialization and the SQL-shaped
ergonomics an application expects — an ADO.NET-familiar command/parameter/result-set surface with
typed column access — without re-implementing any of the transport, framing, or pooling below it.

The client **does not parse SQL**. It sends statement text and parameters over the wire; the
server's SQL session parses, plans, and executes them. That keeps the client contract stable and
independent of the engine's internal plan structures. Database.Sql.Protocol supplies wire codecs;
the client has no dependency on the SQL engine, catalog or storage implementation.

## Scope

- **`SqlClient.Create(SqlClientOptions)` → `SqlClient`** — a pooling client bound to one
  database on one server.
- **`SqlClient.ConnectAsync()` → `SqlConnection`** — rents a typed connection (a pooled,
  authenticated session under the hood).
- **`SqlCommand` + `SqlParameterCollection`** — statement text with named parameters
  (the `@`/`$` sigil is normalized away on bind).
- **`SqlConnection.QueryAsync` / `ExecuteAsync` / `ExecuteScalarAsync<T>`** — run commands
  and get a `SqlResultSet`, an affected count, or a scalar.
- **`SqlResultSet` / `SqlRow` / `SqlColumn`** — typed, ordinal- and name-addressable rows
  with widening numeric getters.
- **`SqlClientException` + `SqlClientErrorKind`** — a SQL-scoped failure taxonomy mapped
  from the core's wire codes, with a `ConnectionUsable` flag.
- **`SqlClientObserver`** — an allocation-free telemetry hook fired around every command.

## Dependencies

- **`Assimalign.Cohesion.Database`** — the `DatabaseException` area root.
- **`Assimalign.Cohesion.Database.Client`** — the pooling client core this layers over.
- **`Assimalign.Cohesion.Database.Sql`** — the SQL family identifiers and payload codecs.
- **`Assimalign.Cohesion.Database.Types`** — `DatabaseType` column identities.
- **`Assimalign.Cohesion.Connections`** — the transport `IDuplexConnectionFactory` handed to
  the client core.

## Usage

See the [source-backed usage examples](examples/index.md).

See `docs/DESIGN.md` for the design decisions behind the surface.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Protocol` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Client` | `CohesionSharedSource` |
| `Assimalign.Cohesion.Database.Client` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Sql.Protocol` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Types` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Connections` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Current project boundaries** — `cohesion/resources/Database/README.md`.
- **Build guard** — `cohesion/resources/Database/Directory.Build.targets`.

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Client/src/Assimalign.Cohesion.Database.Sql.Client.csproj`.
