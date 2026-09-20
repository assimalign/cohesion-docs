# Assimalign.Cohesion.Http.Cookies design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.Cookies`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Cookies are opt-in features over the wire-level `Cookie` and `Set-Cookie` fields. Extension members
retain property-style access while keeping cookie types out of the protocol root. Parsing and
response flushing cooperate with the exchange lifecycle.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Http`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/src/Assimalign.Cohesion.Http.Cookies.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/src`.
