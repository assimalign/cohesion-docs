# Assimalign.Cohesion.ApplicationModel.Gateway

Realizes application graphs in dependency order and tracks resource and command observations.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[ApplicationModel](../index.md)

## Scope

Readiness and required command application gate dependent startup. Stop preserves declarations,
whereas teardown removes owned commands and resources in reverse order. Command clients receive the
target application's certificate validator; replacing a client must preserve that trust boundary.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.ApplicationModel`](../../application-model/assimalign-cohesion-applicationmodel/index.md) | `CohesionProjectReference` |
| `Assimalign.Cohesion.ConfigurationStore.Client` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Database.Client` | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Hosting.Resources`](../../hosting/assimalign-cohesion-hosting-resources/index.md) | `CohesionProjectReference` |
| `Assimalign.Cohesion.IdentityHub.Client` | `CohesionProjectReference` |
| [`Assimalign.Cohesion.IdentityModel.Token.JsonWebToken`](../../identity-model/assimalign-cohesion-identitymodel-token-jsonwebtoken/index.md) | `CohesionProjectReference` |
| `Assimalign.Cohesion.Rezolvr.Client` | `CohesionProjectReference` |
| `Assimalign.Cohesion.SecretStore.Client` | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Security.DataProtection`](../../security/assimalign-cohesion-security-dataprotection/index.md) | `CohesionProjectReference` |
| `System.Security.Cryptography.ProtectedData` | `CohesionPackageReference` |

## Principal public types

| Type | Source file |
|---|---|
| `ApplicationGateway` | `src/ApplicationGateway.Rendering.cs` |
| `ApplicationGatewayOptions` | `src/ApplicationGatewayOptions.cs` |
| `IGatewayResourceCommandClient` | `src/Abstractions/IGatewayResourceCommandClient.cs` |
| `IResourceTransportTrustProvider` | `src/Abstractions/IResourceTransportTrustProvider.cs` |
| `TrustedIssuer` | `src/TrustedIssuer.cs` |
| `ApplicationGatewayCommandLine` | `src/ApplicationGatewayCommandLine.cs` |
| `ApplicationGatewayResourceExtensions` | `src/Extensions/ApplicationGatewayResourceExtensions.cs` |
| `IApplicationGatewayControlPlane` | `src/Abstractions/IApplicationGatewayControlPlane.cs` |
| `IApplicationGatewayControlPlaneFactory` | `src/Abstractions/IApplicationGatewayControlPlaneFactory.cs` |
| `IApplicationTrustGateway` | `src/Abstractions/IApplicationTrustGateway.cs` |
| `IAuthenticatedControlPlaneClient` | `src/Abstractions/IAuthenticatedControlPlaneClient.cs` |
| `IContainerResourceOptionsBuilder` | `src/Abstractions/IContainerResourceOptionsBuilder.cs` |
| `IExecutableResourceOptionsBuilder` | `src/Abstractions/IExecutableResourceOptionsBuilder.cs` |
| `IGatewayStoreClient` | `src/Abstractions/IGatewayStoreClient.cs` |
| `IGatewayTrustKeyRepository` | `src/Abstractions/IGatewayTrustKeyRepository.cs` |
| `IImageRealizer` | `src/Abstractions/IImageRealizer.cs` |

## Sources

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/Assimalign.Cohesion.ApplicationModel.Gateway.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/ApplicationModel/README.md`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/ApplicationGateway.Rendering.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/ApplicationGatewayOptions.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/Abstractions/IGatewayResourceCommandClient.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/Abstractions/IResourceTransportTrustProvider.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/TrustedIssuer.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/ApplicationGatewayCommandLine.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/Extensions/ApplicationGatewayResourceExtensions.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/Abstractions/IApplicationGatewayControlPlane.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/Abstractions/IApplicationGatewayControlPlaneFactory.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/Abstractions/IApplicationTrustGateway.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/Abstractions/IAuthenticatedControlPlaneClient.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/Abstractions/IContainerResourceOptionsBuilder.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/Abstractions/IExecutableResourceOptionsBuilder.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/Abstractions/IGatewayStoreClient.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/Abstractions/IGatewayTrustKeyRepository.cs`.

- **Source** — `cohesion/libraries/ApplicationModel/Assimalign.Cohesion.ApplicationModel.Gateway/src/Abstractions/IImageRealizer.cs`.
