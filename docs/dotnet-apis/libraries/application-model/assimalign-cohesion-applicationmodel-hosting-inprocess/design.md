# Assimalign.Cohesion.ApplicationModel.Hosting.InProcess design

Design decisions and ownership boundaries for `Assimalign.Cohesion.ApplicationModel.Hosting.InProcess`.

> **Status:** Implemented.

[Overview](index.md) · [Examples](examples/index.md)

## Decisions

- **H1 — The endpoint is resolved on every call** — The client reads its gateway's endpoint from the
  instance's environment (`AppEnvironment.Variables.ManifestEndpoint`) and finds the gateway
  registered there each time it publishes, renews, or attaches, so a gateway that runs again over the
  same infrastructure is found again.
- **H2 — The control plane opens only once state is restored** — The client attaches the control
  plane in `StartAsync`, after every host `StartingAsync` has restored the publication's snapshots,
  and detaches it in `StopAsync`.
- **H3 — The manifest names the endpoint as the control plane's address** — In process, the gateway
  reaches the control plane through the endpoint the instance publishes to.
- **H4 — Calls run in the instance's environment** — The client captures the instance's execution
  context when it attaches, so an action reads the instance's variables and ambient state, never the
  gateway's.
- **H5 — Content crosses as bytes** — The gateway receives the manifest object itself; an
  invocation's content is bytes the control plane's serializer reads.

## Boundary

The package references Hosting and the protocol, never `ApplicationModel.Gateway`. The resource's
program therefore runs unchanged on any other platform it opts into.

## Sources

- **Primary source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Hosting.InProcess/docs/DESIGN.md`.
- **Area design** — `cohesion/docs/libraries/ApplicationModel/DESIGN.md`.
