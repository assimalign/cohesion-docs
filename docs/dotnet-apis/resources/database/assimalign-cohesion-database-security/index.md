# Assimalign.Cohesion.Database.Security

The model-agnostic security contracts of the Data Platform: who a connection is (`IDatabaseAuthenticator`) and what a principal may do (`IAuthorizationService`).

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

The model-agnostic security contracts of the Data Platform: who a connection is
(`IDatabaseAuthenticator`) and what a principal may do (`IAuthorizationService`). Model-specific
security features (SQL grants, document-collection ACLs, …) remain future concerns within each
engine assembly and build on these seams. The empty model Security satellites have been removed.

## Scope

- **Authentication** — `IDatabaseAuthenticator` verifies the principal a client
  claims during the wire-protocol handshake, given whatever evidence bytes the
  client's authentication response carries. `DatabaseAuthenticator.AllowAll` is
  the built-in trust-everything implementation (MVP/development posture).
- **Authorization** — `IAuthorizationService` evaluates principal/resource/action
  decisions.

## Dependencies

None — a leaf contract project.

## Consumers

The server runtime in `Database.Hosting` drives `IDatabaseAuthenticator` from its session handshake;
engines consume `IAuthorizationService` as the authorization surface
matures.

See [DESIGN.md](design.md) for the seam decisions and the MVP posture.

[Parent: Database](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Security/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Database/Assimalign.Cohesion.Database.Security/src/Assimalign.Cohesion.Database.Security.csproj`.
