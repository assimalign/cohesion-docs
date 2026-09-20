# Assimalign.Cohesion.Database.Replication design

This page describes the boundaries and implementation shape of `Assimalign.Cohesion.Database.Replication`.

> **Status:** Partial.

`IReplicationCoordinator` exposes `StartAsync` and `StopAsync`, each with an optional
`CancellationToken` and a `Task` result. The project references `Database.Storage`; it contains no
replication coordinator implementation. This reference is derived from the interface source and
project file because the project has no overview or design document.

## Declared dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Database.Storage` | `CohesionProjectReference` |

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Replication/src/Assimalign.Cohesion.Database.Replication.csproj`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Replication/src/Abstractions/IReplicationCoordinator.cs`.
