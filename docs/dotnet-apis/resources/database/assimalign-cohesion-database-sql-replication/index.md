# Assimalign.Cohesion.Database.Sql.Replication

The SQL replication project currently contains the empty `ISqlReplicationSource` marker interface.

> **Status:** Not yet implemented.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

`ISqlReplicationSource` is a public marker in `Assimalign.Cohesion.Database.Sql.Replication`. It
declares no members. The project references `Assimalign.Cohesion.Database.Sql` and
`Assimalign.Cohesion.Database.Replication`; schema-change ordering and row-change streams are
described in the source comment as work to be built out. No replication implementation is present.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Sql` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Replication` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Replication/src/Assimalign.Cohesion.Database.Sql.Replication.csproj`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Sql.Replication/src/Abstractions/ISqlReplicationSource.cs`.
