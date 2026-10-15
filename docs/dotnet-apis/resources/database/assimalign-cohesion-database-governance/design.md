# Assimalign.Cohesion.Database.Governance design

This page describes the boundaries and implementation shape of `Assimalign.Cohesion.Database.Governance`.

> **Status:** Partial.

`IResourceGovernor.TryAdmitAsync` accepts a workload class, an estimated cost, and an optional
cancellation token, and returns `ValueTask<bool>` indicating admission. The project contains this
contract but no resource-governor implementation. There is no project overview or design document;
this reference is derived from `src/Abstractions/IResourceGovernor.cs`.

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Governance/src/Assimalign.Cohesion.Database.Governance.csproj`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Governance/src/Abstractions/IResourceGovernor.cs`.
