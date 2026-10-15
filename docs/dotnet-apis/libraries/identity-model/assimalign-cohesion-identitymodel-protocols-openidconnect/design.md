# Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect design

Design decisions and ownership boundaries for `Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

These are descriptive contracts with data-rule validation. Claim surfaces reuse canonical identity
types, and protocol constants retain their wire meanings. Cryptographic token operations belong to
the JSON Web Token package rather than the protocol contract branch.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.IdentityModel.Protocols`,
`Assimalign.Cohesion.IdentityModel`, `Assimalign.Cohesion.IdentityModel.Protocols`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/src/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/README.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/src`.
