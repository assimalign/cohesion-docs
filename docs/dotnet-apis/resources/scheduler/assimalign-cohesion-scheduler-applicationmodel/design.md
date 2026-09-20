# Assimalign.Cohesion.Scheduler.ApplicationModel design

This page describes the boundaries and implementation shape of `Assimalign.Cohesion.Scheduler.ApplicationModel`.

> **Status:** Partial.

The package is an opt-in orchestration adapter. `SchedulerResource` owns the typed manifest and
delegates to an internal planner. The planner accepts Scheduler manifests with a singleton stateless
Deployment, requires an http TCP endpoint for the control plane, and rejects persistent Volume
mounts and storage overrides. Extra endpoints and non-persistent Configuration or Secret mounts
retain the generic planner behavior.

`SchedulerResourceControlPlane.Create` returns a fresh default control plane for every enabled
resource. Runtime hosting attaches the built Scheduler host and serves that plane over the
manifest's http endpoint.

Horizontal scaling is deferred until the Scheduler area defines leader election or distributed
occurrence coordination. Both the SDK defaults and planner therefore require one replica and a
maximum of one.

## Declared dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.ApplicationModel` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Hosting.Resources` | `CohesionProjectReference` |

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler.ApplicationModel/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Scheduler/Assimalign.Cohesion.Scheduler.ApplicationModel/src/Assimalign.Cohesion.Scheduler.ApplicationModel.csproj`.
