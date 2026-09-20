# Assimalign.Cohesion.Web.Authentication.Bearer

The JWT bearer authentication scheme handler for the Cohesion web stack.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

The JWT bearer authentication scheme handler for the Cohesion web stack. Reads an
`Authorization: Bearer` token, verifies its signature and validates its claims, and materializes a
`ClaimsPrincipal`. Stateless: it re-validates the caller-supplied token on every request.

## What it provides

- **`JwtBearerOptions`** — valid issuers / audiences, signing keys,
  allowed algorithms, clock skew, and the name/role claim types.
- **`JwtBearerDefaults`** — the default scheme name (`"Bearer"`).
- **`IJwtSignatureVerifier`** — + `JwtSignatureVerifier.CreateHmac/CreateRsa/CreateEcdsa`
  — the compatibility seam: HMAC remains Web-local, while the RSA/ECDSA factories
  adapt the reusable IdentityModel JWT verifiers.
- **`JwtBearerAuthentication.CreateHandler(options)`** — the factory the
  composition root calls; the concrete handler stays internal.

## How it fits

- **Implements `IAuthenticationHandler`** — from
  `Assimalign.Cohesion.Web.Authentication` (the scheme model).
- **Consumes `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken`** — for
  document validation (issuer / audience / lifetime / algorithm) and asymmetric signature
  verification. The public Bearer seam remains stable, and headless callers can use the lower
  JWT verifier without referencing the Web area.
- **Emits RFC 6750 `WWW-Authenticate** — Bearer` challenges.

`Register` it at the composition root, not here:

See the [source-backed usage examples](examples/index.md).

See [docs/DESIGN.md](design.md) for the validation order, the algorithm-confusion defense, and the
JWT→`ClaimsPrincipal` mapping.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Http` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Authentication` | `CohesionProjectReference` |
| `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication.Bearer/README.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication.Bearer/src/Assimalign.Cohesion.Web.Authentication.Bearer.csproj`.
