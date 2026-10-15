# Assimalign.Cohesion.IdentityModel.Protocols.Saml

Models Security Assertion Markup Language (SAML) protocol contracts and metadata.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[IdentityModel](../index.md)

## Scope

Assertion structure and role-scoped metadata remain explicit instead of flattening away protocol
meaning. Encrypted-element markers are preserved for a token implementation to handle. Validation
checks data rules rather than claiming transport or cryptographic execution.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.IdentityModel.Protocols`](../../identity-model/assimalign-cohesion-identitymodel-protocols/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.IdentityModel`](../../identity-model/assimalign-cohesion-identitymodel/index.md) | `CohesionSharedSource` |

## Principal public types

| Type | Source file |
|---|---|
| `SamlAssertion` | `src/SamlAssertion.cs` |
| `SamlAttributeStatement` | `src/SamlAttributeStatement.cs` |
| `SamlAuthnContext` | `src/SamlAuthnContext.cs` |
| `SamlAuthnRequest` | `src/SamlAuthnRequest.cs` |
| `SamlAuthnStatement` | `src/SamlAuthnStatement.cs` |
| `SamlConditions` | `src/SamlConditions.cs` |
| `SamlEncryptedElement` | `src/SamlEncryptedElement.cs` |
| `SamlEntityMetadata` | `src/SamlEntityMetadata.cs` |
| `SamlLogoutRequest` | `src/SamlLogoutRequest.cs` |
| `SamlLogoutResponse` | `src/SamlLogoutResponse.cs` |
| `SamlNameId` | `src/SamlNameId.cs` |
| `SamlResponse` | `src/SamlResponse.cs` |
| `SamlSubject` | `src/SamlSubject.cs` |
| `SamlSubjectConfirmation` | `src/SamlSubjectConfirmation.cs` |
| `SamlAssertionDescriptor` | `src/SamlAssertionDescriptor.cs` |
| `SamlAssertionValidationOptions` | `src/SamlAssertionValidationOptions.cs` |

## Sources

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/src/Assimalign.Cohesion.IdentityModel.Protocols.Saml.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/README.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/src`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/src/SamlAssertion.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/src/SamlAttributeStatement.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/src/SamlAuthnContext.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/src/SamlAuthnRequest.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/src/SamlAuthnStatement.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/src/SamlConditions.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/src/SamlEncryptedElement.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/src/SamlEntityMetadata.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/src/SamlLogoutRequest.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/src/SamlLogoutResponse.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/src/SamlNameId.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/src/SamlResponse.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/src/SamlSubject.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/src/SamlSubjectConfirmation.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/src/SamlAssertionDescriptor.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.Saml/src/SamlAssertionValidationOptions.cs`.
