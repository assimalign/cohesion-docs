# Assimalign.Cohesion.Http.ClientFactory design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.ClientFactory`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Client disposal does not immediately dispose a shared handler. Expired handlers are retired after
references drain, allowing rotation to refresh network state. Named options configure headers,
timeouts, handlers, and the factory-owned redirect policy.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Http`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/src/Assimalign.Cohesion.Http.ClientFactory.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.ClientFactory/src`.
