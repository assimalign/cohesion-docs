# Assimalign.Cohesion.IdentityHub.Models

This project contains IdentityHub's legacy tenant-directory persistence DTOs and identifier value types.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

This project contains IdentityHub's legacy tenant-directory persistence DTOs and identifier value
types. Their existing public members remain source compatible. Authenticated principals and
application credentials now point to the canonical `Assimalign.Cohesion.IdentityModel` contracts
rather than introducing IdentityHub-local claim, session, token, credential, or JWK families.

`User.Identity` and `ServicePrincipal.Identity` carry canonical `IIdentitySubject` values.
`ApplicationCredential.Credential` carries the canonical immutable `IdentityCredential`.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Core` | `CohesionProjectReference` |
| `Assimalign.Cohesion.IdentityModel` | `CohesionProjectReference` |

[Parent: IdentityHub](../index.md)

## Sources

- **Primary source** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Models/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Models/src/Assimalign.Cohesion.IdentityHub.Models.csproj`.
