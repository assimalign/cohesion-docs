# Assimalign.Cohesion.DependencyInjection design

Design decisions and ownership boundaries for `Assimalign.Cohesion.DependencyInjection`.

> **Status:** Partial.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Registration descriptors are separate from call-site execution and scope ownership.
`ServiceProviderOptions.EnableDynamicCode=false` selects the interpreted resolver before a compiled
engine is created. That option does not remove constructor reflection; explicit factories or
instances are needed for reflection-free construction.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/src/Assimalign.Cohesion.DependencyInjection.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/DependencyInjection/Assimalign.Cohesion.DependencyInjection/src`.
