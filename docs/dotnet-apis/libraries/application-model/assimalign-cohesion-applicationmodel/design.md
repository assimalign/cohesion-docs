# Assimalign.Cohesion.ApplicationModel design

Design decisions and ownership boundaries for `Assimalign.Cohesion.ApplicationModel`.

> **Status:** Implemented.

[Overview](index.md) · [Examples](examples/index.md)

## Why a separate identity kernel

Merging the identity into Definition would make every running instance load the logical model;
merging it into Protocol would put wire types into the logical model. The small package is the cost
of keeping the logical side (Definition) and the physical side (Hosting) apart. Every package of the
model reaches it; it references Core only.

## Decisions

- **C1 — Placement rule** — A type lives in the lowest package every one of its consumers already
  references: names every plane uses live here; anything inside a gateway-to-instance message, or a
  seam both sides use, lives in Protocol; a seam one side uses lives in that side's package.
- **C2 — One copy of each type** — Packages reference the owner of a type instead of compiling shared
  source, so the manifest an in-process instance publishes is the type the gateway registers. The
  one shared file is the internal name grammar.
- **C3 — One namespace** — Identity, Protocol, and Definition types declare
  `Assimalign.Cohesion.ApplicationModel`, so a gateway program composes with one `using`.
- **C4 — Naming** — Every model type starts with `Application`, then its side (`Gateway`, `Instance`,
  `Model`, `Resource`), then the concept. A client is `IApplication<Holder><Target>Client`: the
  gateway's client to an instance is `IApplicationGatewayInstanceClient`; the instance's client to
  its gateway is `IApplicationInstanceGatewayClient`.
- **C5 — Platform pairs** — A platform owns the channel between an instance and its gateway, both
  ways. Its instance half is the client a resource opts into; its gateway half deploys workloads,
  receives publications, and calls instances. No core package carries a transport.

## Dependency boundary

The only build input is `Assimalign.Cohesion.Core`. No resource-area package may resolve a gateway
assembly (`COHRES003`), so the identity is all a resource and a gateway have in common besides the
protocol.

## Sources

- **Primary source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel/docs/DESIGN.md`.
- **Area design** — `cohesion/docs/libraries/ApplicationModel/DESIGN.md`.
