# Assimalign.Cohesion.IdentityModel.Token.Saml

Normalizes SAML assertion tokens and validates their assertion-specific conditions.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[IdentityModel](../index.md)

## Scope

Name identifiers, attributes, and authentication statements project onto the canonical root model.
Audience restrictions require agreement across restriction groups rather than a flat union.
Subject-confirmation validation retains freshness, recipient, and response-correlation rules.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.IdentityModel.Token`](../../identity-model/assimalign-cohesion-identitymodel-token/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `SamlConditions` | `src/SamlConditions.cs` |
| `SamlConfirmationMethods` | `src/SamlConfirmationMethods.cs` |
| `SamlEncryptedElement` | `src/SamlEncryptedElement.cs` |
| `SamlNameId` | `src/SamlNameId.cs` |
| `SamlSubjectConfirmation` | `src/SamlSubjectConfirmation.cs` |
| `SamlSubjectConfirmationData` | `src/SamlSubjectConfirmationData.cs` |
| `SamlSubjectExtensions` | `src/Extensions/SamlSubjectExtensions.cs` |
| `SamlToken` | `src/SamlToken.cs` |
| `SamlTokenValidationCodes` | `src/SamlTokenValidationCodes.cs` |
| `SamlTokenValidationOptions` | `src/SamlTokenValidationOptions.cs` |
| `ISamlToken` | `src/Abstractions/ISamlToken.cs` |
| `SamlTokenDescriptor` | `src/SamlTokenDescriptor.cs` |

## Sources

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/src/Assimalign.Cohesion.IdentityModel.Token.Saml.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/README.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/src`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/src/SamlConditions.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/src/SamlConfirmationMethods.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/src/SamlEncryptedElement.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/src/SamlNameId.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/src/SamlSubjectConfirmation.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/src/SamlSubjectConfirmationData.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/src/Extensions/SamlSubjectExtensions.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/src/SamlToken.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/src/SamlTokenValidationCodes.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/src/SamlTokenValidationOptions.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/src/Abstractions/ISamlToken.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.Saml/src/SamlTokenDescriptor.cs`.
