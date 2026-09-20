# Assimalign.Cohesion.Hosting design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Hosting`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Plain Hosting depends only on Core. Host execution and run observers do not own resource mounts,
process signals, control planes, or health reporting. Those opt-in concerns extend the run seam from
sibling packages rather than changing every host.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src/Assimalign.Cohesion.Hosting.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Hosting/Assimalign.Cohesion.Hosting/src`.
