# Assimalign.Cohesion.IdentityModel.Token design

Design decisions and ownership boundaries for `Assimalign.Cohesion.IdentityModel.Token`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Concrete token packages populate mutable descriptors that the token base snapshots. Document format
is distinct from the authentication protocol that produced it. Neutral validation stays in this
layer; format-specific claims and cryptography stay in descendants.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.IdentityModel`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token/src/Assimalign.Cohesion.IdentityModel.Token.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/README.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token/src`.
