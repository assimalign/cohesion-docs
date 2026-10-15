# Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect

Models OpenID Connect metadata, messages, token contracts, and logout data.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[IdentityModel](../index.md)

## Scope

These are descriptive contracts with data-rule validation. Claim surfaces reuse canonical identity
types, and protocol constants retain their wire meanings. Cryptographic token operations belong to
the JSON Web Token package rather than the protocol contract branch.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.IdentityModel.Protocols`](../../identity-model/assimalign-cohesion-identitymodel-protocols/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.IdentityModel`](../../identity-model/assimalign-cohesion-identitymodel/index.md) | `CohesionSharedSource` |
| [`Assimalign.Cohesion.IdentityModel.Protocols`](../../identity-model/assimalign-cohesion-identitymodel-protocols/index.md) | `CohesionSharedSource` |

## Principal public types

| Type | Source file |
|---|---|
| `OpenIdConnectClientMetadata` | `src/OpenIdConnectClientMetadata.cs` |
| `OpenIdConnectClientRegistrationRequest` | `src/OpenIdConnectClientRegistrationRequest.cs` |
| `OpenIdConnectIdToken` | `src/OpenIdConnectIdToken.cs` |
| `OpenIdConnectLogoutToken` | `src/OpenIdConnectLogoutToken.cs` |
| `OpenIdConnectProviderMetadata` | `src/OpenIdConnectProviderMetadata.cs` |
| `OpenIdConnectUserInfo` | `src/OpenIdConnectUserInfo.cs` |
| `OpenIdConnectAuthorizationRequest` | `src/OpenIdConnectAuthorizationRequest.cs` |
| `OpenIdConnectAuthorizationRequestDescriptor` | `src/OpenIdConnectAuthorizationRequestDescriptor.cs` |
| `OpenIdConnectAuthorizationResponse` | `src/OpenIdConnectAuthorizationResponse.cs` |
| `OpenIdConnectAuthorizationResponseDescriptor` | `src/OpenIdConnectAuthorizationResponseDescriptor.cs` |
| `OpenIdConnectAuthorizationResponseValidationOptions` | `src/OpenIdConnectAuthorizationResponseValidationOptions.cs` |
| `OpenIdConnectBackChannelLogoutRequest` | `src/OpenIdConnectBackChannelLogoutRequest.cs` |
| `OpenIdConnectBackChannelLogoutRequestDescriptor` | `src/OpenIdConnectBackChannelLogoutRequestDescriptor.cs` |
| `OpenIdConnectClaimsSource` | `src/OpenIdConnectClaimsSource.cs` |
| `OpenIdConnectClaimTypes` | `src/OpenIdConnectClaimTypes.cs` |
| `OpenIdConnectClientMetadataDescriptor` | `src/OpenIdConnectClientMetadataDescriptor.cs` |

## Sources

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/src/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/README.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/src`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/src/OpenIdConnectClientMetadata.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/src/OpenIdConnectClientRegistrationRequest.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/src/OpenIdConnectIdToken.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/src/OpenIdConnectLogoutToken.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/src/OpenIdConnectProviderMetadata.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/src/OpenIdConnectUserInfo.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/src/OpenIdConnectAuthorizationRequest.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/src/OpenIdConnectAuthorizationRequestDescriptor.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/src/OpenIdConnectAuthorizationResponse.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/src/OpenIdConnectAuthorizationResponseDescriptor.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/src/OpenIdConnectAuthorizationResponseValidationOptions.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/src/OpenIdConnectBackChannelLogoutRequest.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/src/OpenIdConnectBackChannelLogoutRequestDescriptor.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/src/OpenIdConnectClaimsSource.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/src/OpenIdConnectClaimTypes.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols.OpenIdConnect/src/OpenIdConnectClientMetadataDescriptor.cs`.
