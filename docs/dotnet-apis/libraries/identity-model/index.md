# IdentityModel

Canonical identity contracts with independent protocol and token-document branches.

[Libraries](../index.md)

## Packages

| Package | Responsibility | Reference |
|---|---|---|
| `Assimalign.Cohesion.IdentityModel` | Defines canonical subjects, credentials, claims, sessions, and authentication results. | [Overview](assimalign-cohesion-identitymodel/index.md) |
| `Assimalign.Cohesion.IdentityModel.Protocols` | Defines transport-neutral authentication protocol envelopes and metadata. | [Overview](assimalign-cohesion-identitymodel-protocols/index.md) |
| `Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect` | Models OpenID Connect metadata, messages, token contracts, and logout data. | [Overview](assimalign-cohesion-identitymodel-protocols-openidconnect/index.md) |
| `Assimalign.Cohesion.IdentityModel.Protocols.Saml` | Models Security Assertion Markup Language (SAML) protocol contracts and metadata. | [Overview](assimalign-cohesion-identitymodel-protocols-saml/index.md) |
| `Assimalign.Cohesion.IdentityModel.Token` | Normalizes token documents and shared issuer, audience, and temporal validation. | [Overview](assimalign-cohesion-identitymodel-token/index.md) |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` | Parses, writes, and verifies JSON Web Token documents. | [Overview](assimalign-cohesion-identitymodel-token-jsonwebtoken/index.md) |
| `Assimalign.Cohesion.IdentityModel.Token.Saml` | Normalizes SAML assertion tokens and validates their assertion-specific conditions. | [Overview](assimalign-cohesion-identitymodel-token-saml/index.md) |

## Dependencies and delivery

The repository places this area in delivery wave 3. The wave is a dependency-ordering guide, not a
release or completeness guarantee.

| Assembly | Declared dependencies |
|---|---|
| `Assimalign.Cohesion.IdentityModel` | `Assimalign.Cohesion.IdentityModel` (CohesionSharedSource) |
| `Assimalign.Cohesion.IdentityModel.Protocols` | `Assimalign.Cohesion.IdentityModel` (CohesionProjectReference), `Assimalign.Cohesion.IdentityModel` (CohesionSharedSource), `Assimalign.Cohesion.IdentityModel.Protocols` (CohesionSharedSource) |
| `Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect` | `Assimalign.Cohesion.IdentityModel.Protocols` (CohesionProjectReference), `Assimalign.Cohesion.IdentityModel` (CohesionSharedSource), `Assimalign.Cohesion.IdentityModel.Protocols` (CohesionSharedSource) |
| `Assimalign.Cohesion.IdentityModel.Protocols.Saml` | `Assimalign.Cohesion.IdentityModel.Protocols` (CohesionProjectReference), `Assimalign.Cohesion.IdentityModel` (CohesionSharedSource) |
| `Assimalign.Cohesion.IdentityModel.Token` | `Assimalign.Cohesion.IdentityModel` (CohesionProjectReference) |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` | `Assimalign.Cohesion.IdentityModel.Token` (CohesionProjectReference) |
| `Assimalign.Cohesion.IdentityModel.Token.Saml` | `Assimalign.Cohesion.IdentityModel.Token` (CohesionProjectReference) |

## Sources

- **Source** — `cohesion/libraries/README.md`.

- **Source** — `cohesion/README.md`.

- **Source** — `cohesion/docs/programs/DELIVERY_ROADMAP.md`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/IdentityModel/README.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src/Assimalign.Cohesion.IdentityModel.csproj`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/src/Assimalign.Cohesion.IdentityModel.Protocols.csproj`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/src`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/src/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect.csproj`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/src`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/src/Assimalign.Cohesion.IdentityModel.Protocols.Saml.csproj`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/src`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token/src/Assimalign.Cohesion.IdentityModel.Token.csproj`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token/src`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken.csproj`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/src/Assimalign.Cohesion.IdentityModel.Token.Saml.csproj`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/src`.
