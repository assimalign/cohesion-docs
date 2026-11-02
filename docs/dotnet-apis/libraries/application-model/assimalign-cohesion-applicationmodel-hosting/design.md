# Assimalign.Cohesion.ApplicationModel.Hosting design

Design decisions and ownership boundaries for `Assimalign.Cohesion.ApplicationModel.Hosting`.

> **Status:** Implemented.

[Overview](index.md) · [Examples](examples/index.md)

## Decisions

- **D1 — No ambient state** — A control plane exists only where an area creates one; there is no
  process-wide registry. A nested builder therefore cannot discover an outer resource, and one
  process can host several instances.
- **D3 — State is a snapshot per action** — Replication restores whole values instead of replaying
  writes. A write runs once, on one instance, and the gateway restores its snapshot on the others.
- **D4 — Startup state arrives in the publication's answer** — The resource service restores the
  answered snapshots before the host starts serving, so a new instance never serves its code
  defaults after an operator changed them.
- **D5 — Publication fails fast** — A publisher that throws fails the host's start; the gateway's
  startup window and the platform's restart policy decide what happens next.
- **D6 — Dispatch returns statuses** — `NotFound`, `NotSupported`, and `Rejected` are results, which
  a platform maps to its own codes without catching exceptions.
- **D14 — Registrations are leases** — The publication answer carries a lease that the resource
  service renews; a gateway that no longer knows the instance gets a fresh publication.
- **D15 — The control plane has no lifecycle** — `ApplicationInstanceService` is the
  `IHostLifecycleService` that joins the host: it publishes and restores in `StartingAsync`, opens the
  control plane in `StartAsync`, and closes it and stops renewing in `StopAsync`.
- **D19 — The platform owns the transport** — This library keeps only what hosting needs.
  `IApplicationInstanceGatewayClient` is a platform's instance half; `ApplicationInstanceOptions`
  holds the clients a resource opted into and selects the one the environment's `COHESION_GATEWAY`
  names. A named platform the resource did not opt into fails the application's build.
- **D21 — Added actions are shared by every area** — `AddAction` (as an instance or a factory over
  the application's services), `JsonContexts`, and `CreateActions` live on the base options. A
  duplicate action name fails the application's start.

## Lifecycle

| Phase | What the resource service does |
|---|---|
| `StartingAsync` | Publishes the manifest through the selected client and restores the answered snapshots, before any application service or server starts. |
| `StartAsync` | Starts the client carrying the gateway's calls to the control plane, and renews the lease. |
| `StopAsync` | Closes the control plane to the gateway and stops renewing. |

Without a platform named in the environment, the area's `AddResource` registers nothing and the
application runs unmanaged.

## Sources

- **Primary source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Hosting/docs/DESIGN.md`.
- **Area design** — `cohesion/docs/libraries/ApplicationModel/DESIGN.md`.
- **Runtime contract** — `cohesion/docs/RUNTIME_CONTRACT.md`.
