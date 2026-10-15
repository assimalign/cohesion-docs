# Assimalign.Cohesion.IdentityHub.Models design

This page describes the boundaries and implementation shape of `Assimalign.Cohesion.IdentityHub.Models`.

> **Status:** Partial.

## Compatibility boundary

The existing mutable tenant, user, group, application, and role DTOs predate the canonical
IdentityModel family. They remain in place so current consumers keep their public shapes and
generated ULID value types. `New` identity semantics are additive links to canonical contracts: users
and service principals expose `IIdentitySubject`, while application credential records expose
`IdentityCredential`.

## Canonical identity ownership

IdentityModel owns subjects, identifiers, claims, credentials, authentication sessions and results.
Its token and protocol branches own JWT, JOSE/JWK, OIDC, and SAML contracts. This project does not
mirror those types. IdentityHub persistence and workflow code composes the canonical immutable
objects alongside its service-specific tenant identifiers.

## Dependencies and AOT posture

Core remains required by the generated ULID-backed legacy identifiers. IdentityModel supplies the
canonical identity contracts. Both are AOT-compatible; this project adds no reflection, runtime
discovery, or reflection-based serialization.

## Non-goals

- **Replacing or renaming legacy** — DTO members in this compatibility change.
- **Defining token, claim, authentication-session,** — validation, or JWK models.
- **Implementing IdentityHub storage or** — protocol behavior.

## Declared dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Core` | `CohesionProjectReference` |
| `Assimalign.Cohesion.IdentityModel` | `CohesionProjectReference` |

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Models/docs/DESIGN.md`.
- **Source** — `cohesion/resources/IdentityHub/Assimalign.Cohesion.IdentityHub.Models/src/Assimalign.Cohesion.IdentityHub.Models.csproj`.
