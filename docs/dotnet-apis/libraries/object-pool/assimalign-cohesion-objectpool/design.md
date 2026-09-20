# Assimalign.Cohesion.ObjectPool design

Design decisions and ownership boundaries for `Assimalign.Cohesion.ObjectPool`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Object creation, return acceptance, and retention are separate decisions. Generic pool contracts
allow reusable callers while a default implementation supplies standard retention. Returning rented
objects is the caller's responsibility, including exceptional paths.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/ObjectPool/Assimalign.Cohesion.ObjectPool/src/Assimalign.Cohesion.ObjectPool.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/ObjectPool/Assimalign.Cohesion.ObjectPool/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ObjectPool/Assimalign.Cohesion.ObjectPool/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/ObjectPool/Assimalign.Cohesion.ObjectPool/src`.
