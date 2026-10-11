# Assimalign.Cohesion.Http.InterimResponses design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.InterimResponses`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

A host registers the interceptor to attach the feature to exchanges. Interim responses precede the
final response and use the core response seam, keeping the transport independent of this package.
Convenience members cover Continue and Early Hints.

The automatic `Expect: 100-continue` handshake is not in scope here. That is a wire-level interop
concern the transport handles unconditionally on HTTP/1.1, HTTP/2 and HTTP/3 (it solicits the body
with `100 Continue` at its first read, so a withholding client does not deadlock; HTTP/2 and HTTP/3
since #1385), independent of this package. See the
[`Assimalign.Cohesion.Http.Connections` design](../assimalign-cohesion-http-connections/design.md).

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
