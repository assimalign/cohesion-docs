# Assimalign.Cohesion.Http.RequestLimits design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.RequestLimits`.

> **Status:** Partial.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The feature is attached on all three protocol parse paths and writes through to the transport's
limit state. It becomes read-only when body reading starts. The documented wire-level enforcement
remains HTTP/1.1-only; feature presence on HTTP/2 or HTTP/3 does not imply enforcement.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Http`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/src/Assimalign.Cohesion.Http.RequestLimits.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/src`.
