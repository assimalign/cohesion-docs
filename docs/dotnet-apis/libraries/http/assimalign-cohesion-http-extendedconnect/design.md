# Assimalign.Cohesion.Http.ExtendedConnect design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.ExtendedConnect`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The feature identifies a CONNECT exchange carrying the `:protocol` pseudo-header and exposes the
requested protocol. It describes the transition request; application code chooses the response and
inner protocol. It does not itself implement WebSocket behavior.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Http`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/src/Assimalign.Cohesion.Http.ExtendedConnect.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ExtendedConnect/src`.
