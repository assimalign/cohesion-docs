# Assimalign.Cohesion.IdentityModel design

Design decisions and ownership boundaries for `Assimalign.Cohesion.IdentityModel`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The root is the family's dependency anchor and references only the base class library. Protocol
contracts and token documents are separate branches. Claim canonicalization preserves provenance
while presenting one vocabulary to resource platforms.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.IdentityModel`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src/Assimalign.Cohesion.IdentityModel.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/README.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src`.
