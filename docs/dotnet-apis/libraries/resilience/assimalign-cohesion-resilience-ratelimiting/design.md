# Assimalign.Cohesion.Resilience.RateLimiting design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Resilience.RateLimiting`.

> **Status:** Implemented.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

`UseRateLimiter` adapts an explicitly supplied `RateLimiter`, permit count, and rejection callback.
Tests verify that unavailable permits reject execution and available permits allow results. The
current strategy supersedes the exploratory description in the older design.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Resilience`, `System.Threading.RateLimiting`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.RateLimiting/src/Assimalign.Cohesion.Resilience.RateLimiting.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.RateLimiting/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.RateLimiting/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.RateLimiting/src`.

- **Source** — `cohesion/libraries/Resilience/Assimalign.Cohesion.Resilience.RateLimiting/tests`.
