# Assimalign.Cohesion.Database.Replication

Database replication lifecycle is defined by the `IReplicationCoordinator` contract.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

`IReplicationCoordinator` exposes `StartAsync` and `StopAsync`, each with an optional
`CancellationToken` and a `Task` result. The project references `Database.Storage`; it contains no
replication coordinator implementation. This reference is derived from the interface source and
project file because the project has no overview or design document.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Storage` | `CohesionProjectReference` |

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Replication/src/Assimalign.Cohesion.Database.Replication.csproj`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Replication/src/Abstractions/IReplicationCoordinator.cs`.
