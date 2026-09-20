# Assimalign.Cohesion.Web.Authentication design

This page describes the boundaries and implementation shape of `Assimalign.Cohesion.Web.Authentication`.

> **Status:** Partial.

The package places authenticated `ClaimsPrincipal` state in an `IAuthenticationFeature` on
`IHttpContext.Features`. The `User` extension property returns an empty principal when the feature
is absent and reuses an existing feature when assigning a principal.

`AuthenticationBuilder`, `AuthenticationOptions`, and `AuthenticationScheme` compose handlers.
`IAuthenticationHandler` defines authentication, challenge, and forbidden handling;
`IAuthenticationSignInHandler` adds sign-in and sign-out. `AddAuthentication` composes against
`IWebApplicationBuilder`; `UseAuthentication` composes the request pipeline.

The README uses older feature names. This reference uses the current source names
`IAuthenticationFeature` and the internal `AuthenticationFeature` implementation.

## Declared dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Http` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Security.DataProtection` | `CohesionProjectReference` |

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/src/Abstractions/IAuthenticationFeature.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/src/Abstractions/IAuthenticationHandler.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/src/Abstractions/IAuthenticationResultFeature.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/src/Abstractions/IAuthenticationService.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/src/Abstractions/IAuthenticationSignInHandler.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/src/AuthenticateResult.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/src/AuthenticationBuilder.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/src/AuthenticationOptions.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/src/AuthenticationProperties.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/src/AuthenticationScheme.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/src/AuthenticationService.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/src/AuthenticationTicket.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/src/Exceptions/AuthenticationFailureException.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/src/Extensions/AuthenticationWebApplicationExtensions.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/src/Extensions/HttpContextAuthenticationExtensions.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/src/Extensions/HttpContextAuthenticationVerbExtensions.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/src/Internal/AuthenticationFeature.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/src/Internal/AuthenticationResultFeature.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/src/Internal/DefaultAuthenticationService.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/src/Metadata/ApiEndpointMetadata.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/src/Metadata/IApiEndpointMetadata.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/src/Assimalign.Cohesion.Web.Authentication.csproj`.
