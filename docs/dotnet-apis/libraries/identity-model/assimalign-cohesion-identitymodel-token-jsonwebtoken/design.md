# Assimalign.Cohesion.IdentityModel.Token.JsonWebToken design

Design decisions and ownership boundaries for `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Compact parsing preserves authoritative header and claim data and rejects duplicate-member
ambiguity. Writing supports ES256, while reusable verifiers handle supported RSA and ECDSA
algorithms. Callers own keys and trust policy; structural validation alone is not signature
verification.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.IdentityModel.Token`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/README.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src`.
