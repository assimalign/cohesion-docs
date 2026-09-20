# Assimalign.Cohesion.Http.DigestFields design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.DigestFields`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Digest values use structured fields and support SHA-256 and SHA-512 computation. The verifier
rejects malformed fields before dispatch; HTTP/2 body verification is lazy and reports mismatches on
terminal reads. Recognized deprecated algorithms are not enabled for computation.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Http`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/Assimalign.Cohesion.Http.DigestFields.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src`.
