# Assimalign.Cohesion.ApplicationModel.Gateway design

Design decisions and ownership boundaries for `Assimalign.Cohesion.ApplicationModel.Gateway`.

> **Status:** Partial. Each platform owns authentication of publications and calls; it is open on every platform.

[Overview](index.md) · [Examples](examples/index.md)

## Run

`StartAsync` takes the gateway's record over with a new epoch, deploys the resources in dependency
order (independent ones at once), waits for an instance of each deployment to publish, reads its
outputs, and tears down what the model no longer declares. The run then continues until
`StopAsync`, a shutdown signal, or a failure that `Completion` reports.

## Decisions

- **G1 — The platform is a provider, not a subclass** — `IApplicationGatewayInfrastructureProvider`
  deploys and tears down workloads and supplies infrastructure actions.
- **G9 and G39 — No transport here** — A platform's gateway half brings it. The provider is also the
  gateway's inbound channel: the gateway starts it with its context, the channel hands each
  publication to `context.Publisher`, and the gateway calls instances through the platform's
  `IApplicationGatewayInstanceClient`.
- **G10 and G12 — Reconcile, don't redeploy** — The provider plans each resource against what runs;
  a resource's revision is a fingerprint of its manifest, so only changed resources update.
- **G11 and G35 — The record lives behind a store** — `IApplicationGatewayStore` keeps deployments
  and snapshots (`JsonFileApplicationGatewayStore` by default). Each run takes the record over with a
  new epoch, and the newest run wins.
- **G15 — Instances hold leases** — A publication's answer carries a lease the instance renews; an
  expired instance is unregistered.
- **G16, G17, G18 — Dependency order and readiness** — A resource deploys once what it depends on is
  ready: an instance of that deployment published (G34 deployment ids) and its outputs were read.
- **G20 — A failed step stops only its dependents** — Independent resources continue.
- **G22 — Settings are filled in at the step** — From this gateway's outputs and its parent's.
- **G25 — A provider sees only its own platform's extensions**.
- **G26 and G27 — Children join their parent; a gateway is built with its model** — Each gateway
  runs its own model, deployed by its own pipeline; `ApplicationGatewayBuilder` declares the model
  and builds the gateway.
- **G36 — A workload that stops before it publishes fails its step at once**.
- **G38 — An operator's action runs on one instance, and a write reaches every instance** — The
  write's snapshot is recorded and restored on the other instances and on every later publication.
- **G40 — Start, stop, and completion** — Instead of one run call, with `RunAsync` as an extension.

## Boundaries

The gateway references Definition and the protocol, never `ApplicationModel.Hosting`. No resource
area may resolve a gateway assembly (`COHRES003`). Authentication of publications and calls belongs
to each platform.

## Sources

- **Primary source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/docs/DESIGN.md`.
- **Area design** — `cohesion/docs/libraries/ApplicationModel/DESIGN.md`.
