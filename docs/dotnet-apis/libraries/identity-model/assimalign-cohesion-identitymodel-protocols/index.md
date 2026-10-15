# Assimalign.Cohesion.IdentityModel.Protocols

Defines transport-neutral authentication protocol envelopes and metadata.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[IdentityModel](../index.md)

## Scope

Shared roles, bindings, endpoint kinds, parties, and validation results prevent each protocol branch
from inventing incompatible foundations. Protocol response status is fail-closed. Concrete branches
supply protocol-specific messages without taking transport ownership.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.IdentityModel`](../../identity-model/assimalign-cohesion-identitymodel/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.IdentityModel`](../../identity-model/assimalign-cohesion-identitymodel/index.md) | `CohesionSharedSource` |
| [`Assimalign.Cohesion.IdentityModel.Protocols`](../../identity-model/assimalign-cohesion-identitymodel-protocols/index.md) | `CohesionSharedSource` |

## Principal public types

| Type | Source file |
|---|---|
| `ProtocolBinding` | `src/ProtocolBinding.cs` |
| `ProtocolEndpoint` | `src/ProtocolEndpoint.cs` |
| `ProtocolEndpointKind` | `src/ProtocolEndpointKind.cs` |
| `ProtocolExchange` | `src/ProtocolExchange.cs` |
| `ProtocolKey` | `src/ProtocolKey.cs` |
| `ProtocolMessage` | `src/ProtocolMessage.cs` |
| `ProtocolMetadata` | `src/ProtocolMetadata.cs` |
| `ProtocolParty` | `src/ProtocolParty.cs` |
| `ProtocolRequest` | `src/ProtocolRequest.cs` |
| `ProtocolResponse` | `src/ProtocolResponse.cs` |
| `ProtocolResponseStatus` | `src/ProtocolResponseStatus.cs` |
| `ProtocolRole` | `src/ProtocolRole.cs` |
| `ProtocolValidationCodes` | `src/ProtocolValidationCodes.cs` |
| `ProtocolValidationDiagnostic` | `src/ProtocolValidationDiagnostic.cs` |
| `ProtocolValidationResult` | `src/ProtocolValidationResult.cs` |
| `ProtocolValidationSeverity` | `src/ProtocolValidationSeverity.cs` |

## Sources

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/src/Assimalign.Cohesion.IdentityModel.Protocols.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/README.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/src`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/src/ProtocolBinding.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/src/ProtocolEndpoint.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/src/ProtocolEndpointKind.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/src/ProtocolExchange.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/src/ProtocolKey.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/src/ProtocolMessage.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/src/ProtocolMetadata.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/src/ProtocolParty.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/src/ProtocolRequest.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/src/ProtocolResponse.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/src/ProtocolResponseStatus.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/src/ProtocolRole.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/src/ProtocolValidationCodes.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/src/ProtocolValidationDiagnostic.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/src/ProtocolValidationResult.cs`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Protocols/src/ProtocolValidationSeverity.cs`.
