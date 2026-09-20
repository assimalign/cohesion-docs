# Assimalign.Cohesion.Database.Sql.Replication design

This page describes the boundaries and implementation shape of `Assimalign.Cohesion.Database.Sql.Replication`.

> **Status:** Not yet implemented.

`ISqlReplicationSource` is a public marker in `Assimalign.Cohesion.Database.Sql.Replication`. It
declares no members. The project references `Assimalign.Cohesion.Database.Sql` and
`Assimalign.Cohesion.Database.Replication`; schema-change ordering and row-change streams are
described in the source comment as work to be built out. No replication implementation is present.

## Declared dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Sql` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Replication` | `CohesionProjectReference` |

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Replication/src/Assimalign.Cohesion.Database.Sql.Replication.csproj`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Replication/src/Abstractions/ISqlReplicationSource.cs`.
