# Assimalign.Cohesion.IdentityModel.Protocols.Saml design

Design decisions and ownership boundaries for `Assimalign.Cohesion.IdentityModel.Protocols.Saml`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Assertion structure and role-scoped metadata remain explicit instead of flattening away protocol
meaning. Encrypted-element markers are preserved for a token implementation to handle. Validation
checks data rules rather than claiming transport or cryptographic execution.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.IdentityModel.Protocols`,
`Assimalign.Cohesion.IdentityModel`. The [overview](index.md#dependencies) distinguishes project
references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/src/Assimalign.Cohesion.IdentityModel.Protocols.Saml.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/README.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/src`.
