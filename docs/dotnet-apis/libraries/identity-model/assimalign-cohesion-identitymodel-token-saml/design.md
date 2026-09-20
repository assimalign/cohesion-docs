# Assimalign.Cohesion.IdentityModel.Token.Saml design

Design decisions and ownership boundaries for `Assimalign.Cohesion.IdentityModel.Token.Saml`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Name identifiers, attributes, and authentication statements project onto the canonical root model.
Audience restrictions require agreement across restriction groups rather than a flat union.
Subject-confirmation validation retains freshness, recipient, and response-correlation rules.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.IdentityModel.Token`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/src/Assimalign.Cohesion.IdentityModel.Token.Saml.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/README.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/src`.
