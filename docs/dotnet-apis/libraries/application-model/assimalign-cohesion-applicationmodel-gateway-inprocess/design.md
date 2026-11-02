# Assimalign.Cohesion.ApplicationModel.Gateway.InProcess design

Design decisions and ownership boundaries for `Assimalign.Cohesion.ApplicationModel.Gateway.InProcess`.

> **Status:** Implemented.

[Overview](index.md) · [Examples](examples/index.md)

## Decisions

- **I1 — In process is a platform, not a gateway** — The package supplies the provider, channel, and
  instance client of an ordinary `ApplicationGateway`.
- **I2 — A program is bound by its entry point** — `AddProgram(provider, name, main)` binds a
  resource to the `MainAsync` of a program the gateway references.
- **I3 and I4 — Each program runs on its own thread in its own environment** — The gateway enters an
  `AppEnvironmentScope` on each program's thread holding what a container's environment would: the
  application and resource names, `Gateway` (`inprocess`), the `ManifestEndpoint`
  (`inprocess://<id>/`), the `Deployment` id, the gateway's environment name, and each setting under
  its configuration key. A resource's loopback port is chosen when it first deploys and kept for its
  lifetime, so an update listens where its predecessor did.
- **I5 and I6 — The transport is a channel** — Programs publish over an in-process channel that
  receives only while a gateway runs.
- **I7 — Programs outlive their gateway** — A stopped gateway leaves its programs running, as a
  container platform would; disposing the infrastructure stops them.
- **I8 — Plans compare what runs** — A resource whose program does not run plans `Create`; one that
  runs with the same settings plans `None`; one with other settings plans `Update`, which restarts it
  on the same ports.
- **I9 — Stopping is the scope's shutdown** — Tearing down, replacing, or disposing cancels the
  program's shutdown token; `StopTimeout` (thirty seconds) bounds the wait.
- **I10 — The endpoint output is the loopback address** the program listens on.
- **I11 — A program that stops before it publishes is reported at once**.
- **I12 — The gateway calls a program's control plane directly**, through the instance half's
  attached control plane.

## Sources

- **Primary source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway.InProcess/docs/DESIGN.md`.
- **Area design** — `cohesion/docs/libraries/ApplicationModel/DESIGN.md`.
