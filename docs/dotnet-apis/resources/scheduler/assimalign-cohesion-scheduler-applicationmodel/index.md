# Assimalign.Cohesion.Scheduler.ApplicationModel

Use IApplicationBuilder.AddScheduler to add a build-produced Scheduler manifest to an application graph.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

`Use` `IApplicationBuilder.AddScheduler` to add a build-produced Scheduler manifest to an application
graph. Platform gateways consume the resulting platform-neutral ResourcePlan; this assembly is not
part of the Scheduler runtime shared framework.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.ApplicationModel` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |

[Parent: Scheduler](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler.ApplicationModel/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler.ApplicationModel/src/Assimalign.Cohesion.Scheduler.ApplicationModel.csproj`.
