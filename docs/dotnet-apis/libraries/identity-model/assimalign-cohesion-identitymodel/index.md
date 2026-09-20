# Assimalign.Cohesion.IdentityModel

Defines canonical subjects, credentials, claims, sessions, and authentication results.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[IdentityModel](../index.md)

## Scope

The root is the family's dependency anchor and references only the base class library. Protocol
contracts and token documents are separate branches. Claim canonicalization preserves provenance
while presenting one vocabulary to resource platforms.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.IdentityModel`](../../identity-model/assimalign-cohesion-identitymodel/index.md) | `CohesionSharedSource` |

## Principal public types

| Type | Source file |
|---|---|
| `AuthenticationContext` | `src/Authentication/AuthenticationContext.cs` |
| `AuthenticationFailure` | `src/Authentication/AuthenticationFailure.cs` |
| `AuthenticationProtocol` | `src/Authentication/AuthenticationProtocol.cs` |
| `AuthenticationResult` | `src/Authentication/AuthenticationResult.cs` |
| `AuthenticationSession` | `src/Authentication/AuthenticationSession.cs` |
| `IdentityAttribute` | `src/Claims/IdentityAttribute.cs` |
| `IdentityClaim` | `src/Claims/IdentityClaim.cs` |
| `IdentityClaimCollection` | `src/Claims/IdentityClaimCollection.cs` |
| `IdentityClaimMapper` | `src/Claims/IdentityClaimMapper.cs` |
| `IdentityClaimMappings` | `src/Claims/IdentityClaimMappings.cs` |
| `IdentityClaimProvenance` | `src/Claims/IdentityClaimProvenance.cs` |
| `IdentityClaimTypes` | `src/Claims/IdentityClaimTypes.cs` |
| `IdentityClaimValue` | `src/Claims/IdentityClaimValue.cs` |
| `IdentityCredential` | `src/Credentials/IdentityCredential.cs` |
| `IdentityKind` | `src/Subjects/IdentityKind.cs` |
| `IdentityModelException` | `src/Exceptions/IdentityModelException.cs` |

## Sources

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src/Assimalign.Cohesion.IdentityModel.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/README.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src/Authentication/AuthenticationContext.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src/Authentication/AuthenticationFailure.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src/Authentication/AuthenticationProtocol.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src/Authentication/AuthenticationResult.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src/Authentication/AuthenticationSession.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src/Claims/IdentityAttribute.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src/Claims/IdentityClaim.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src/Claims/IdentityClaimCollection.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src/Claims/IdentityClaimMapper.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src/Claims/IdentityClaimMappings.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src/Claims/IdentityClaimProvenance.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src/Claims/IdentityClaimTypes.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src/Claims/IdentityClaimValue.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src/Credentials/IdentityCredential.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src/Subjects/IdentityKind.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel/src/Exceptions/IdentityModelException.cs`.
