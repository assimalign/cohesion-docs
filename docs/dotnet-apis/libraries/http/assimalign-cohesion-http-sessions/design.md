# Assimalign.Cohesion.Http.Sessions design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.Sessions`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

`IHttpSession` exposes byte-array values, keys, availability, and explicit load/commit operations.
Typed string and integer helpers are extension members over that binary contract. An attached
feature supplies the session without adding application state to the protocol core.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Http`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/src/Assimalign.Cohesion.Http.Sessions.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/src`.
