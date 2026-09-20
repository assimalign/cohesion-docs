# Assimalign.Cohesion.IdentityModel.Token.JsonWebToken

Parses, writes, and verifies JSON Web Token documents.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[IdentityModel](../index.md)

## Scope

Compact parsing preserves authoritative header and claim data and rejects duplicate-member
ambiguity. Writing supports ES256, while reusable verifiers handle supported RSA and ECDSA
algorithms. Callers own keys and trust policy; structural validation alone is not signature
verification.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.IdentityModel.Token`](../../identity-model/assimalign-cohesion-identitymodel-token/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `JoseAlgorithms` | `src/JoseAlgorithms.cs` |
| `JoseHeader` | `src/JoseHeader.cs` |
| `JoseHeaderParameterNames` | `src/JoseHeaderParameterNames.cs` |
| `JsonWebToken` | `src/JsonWebToken.cs` |
| `JsonWebTokenClaimTypes` | `src/JsonWebTokenClaimTypes.cs` |
| `JsonWebTokenDescriptor` | `src/JsonWebTokenDescriptor.cs` |
| `JsonWebTokenSignatureVerifier` | `src/JsonWebTokenSignatureVerifier.cs` |
| `JsonWebTokenValidationCodes` | `src/JsonWebTokenValidationCodes.cs` |
| `JsonWebTokenValidationOptions` | `src/JsonWebTokenValidationOptions.cs` |
| `JsonWebTokenWriter` | `src/JsonWebTokenWriter.cs` |
| `IJsonWebToken` | `src/Abstractions/IJsonWebToken.cs` |
| `IJsonWebTokenSignatureVerifier` | `src/Abstractions/IJsonWebTokenSignatureVerifier.cs` |
| `IJsonWebTokenWriter` | `src/Abstractions/IJsonWebTokenWriter.cs` |
| `JoseHeaderDescriptor` | `src/JoseHeaderDescriptor.cs` |
| `JsonWebTokenParts` | `src/JsonWebTokenParts.cs` |

## Sources

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/README.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src/JoseAlgorithms.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src/JoseHeader.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src/JoseHeaderParameterNames.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src/JsonWebToken.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src/JsonWebTokenClaimTypes.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src/JsonWebTokenDescriptor.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src/JsonWebTokenSignatureVerifier.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src/JsonWebTokenValidationCodes.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src/JsonWebTokenValidationOptions.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src/JsonWebTokenWriter.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src/Abstractions/IJsonWebToken.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src/Abstractions/IJsonWebTokenSignatureVerifier.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src/Abstractions/IJsonWebTokenWriter.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src/JoseHeaderDescriptor.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src/JsonWebTokenParts.cs`.
