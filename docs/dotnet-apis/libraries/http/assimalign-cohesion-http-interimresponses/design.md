# Assimalign.Cohesion.Http.InterimResponses design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.InterimResponses`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

A host registers the interceptor to attach the feature to exchanges. Interim responses precede the
final response and use the core response seam, keeping the transport independent of this package.
Convenience members cover Continue and Early Hints.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Http`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/src/Assimalign.Cohesion.Http.InterimResponses.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/src`.
