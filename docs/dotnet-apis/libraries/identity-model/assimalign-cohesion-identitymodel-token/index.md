# Assimalign.Cohesion.IdentityModel.Token

Normalizes token documents and shared issuer, audience, and temporal validation.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[IdentityModel](../index.md)

## Scope

Concrete token packages populate mutable descriptors that the token base snapshots. Document format
is distinct from the authentication protocol that produced it. Neutral validation stays in this
layer; format-specific claims and cryptography stay in descendants.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.IdentityModel`](../../identity-model/assimalign-cohesion-identitymodel/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `IdentityToken` | `src/IdentityToken.cs` |
| `IdentityTokenDescriptor` | `src/IdentityTokenDescriptor.cs` |
| `IdentityTokenKind` | `src/IdentityTokenKind.cs` |
| `IdentityTokenValidationOptions` | `src/IdentityTokenValidationOptions.cs` |
| `IIdentityToken` | `src/Abstractions/IIdentityToken.cs` |
| `TokenValidationDiagnostic` | `src/TokenValidationDiagnostic.cs` |
| `TokenValidationResult` | `src/TokenValidationResult.cs` |
| `IdentityTokenExtensions` | `src/Extensions/IdentityTokenExtensions.cs` |
| `TokenValidationCodes` | `src/TokenValidationCodes.cs` |
| `TokenValidationSeverity` | `src/TokenValidationSeverity.cs` |

## Sources

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token/src/Assimalign.Cohesion.IdentityModel.Token.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/README.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token/src`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token/src/IdentityToken.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token/src/IdentityTokenDescriptor.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token/src/IdentityTokenKind.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token/src/IdentityTokenValidationOptions.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token/src/Abstractions/IIdentityToken.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token/src/TokenValidationDiagnostic.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token/src/TokenValidationResult.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token/src/Extensions/IdentityTokenExtensions.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token/src/TokenValidationCodes.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token/src/TokenValidationSeverity.cs`.
