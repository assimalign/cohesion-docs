# Assimalign.Cohesion.Http.Streaming design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.Streaming`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The host registers an exchange interceptor to attach the streaming API. Starting a response,
writing, flushing, and completing are explicit lifecycle steps. The feature depends on core response
seams rather than the concrete HTTP transport assembly.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Http`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Streaming/src/Assimalign.Cohesion.Http.Streaming.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Streaming/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Streaming/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Streaming/src`.
