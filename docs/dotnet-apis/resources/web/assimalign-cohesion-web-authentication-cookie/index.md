# Assimalign.Cohesion.Web.Authentication.Cookie

The cookie authentication scheme handler for the Cohesion web stack.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

The cookie authentication scheme handler for the Cohesion web stack. Signs users in with a
data-protected ticket cookie, validates that cookie on later requests, applies sliding expiration,
and drives the login / logout / access-denied flow.

## What it provides

- **`CookieAuthenticationOptions`** — cookie name and attributes, login /
  logout / access-denied paths, ticket lifetime, sliding-expiration toggle,
  and the `IDataProtector` that seals the ticket.
- **`CookieAuthenticationDefaults`** — the default scheme name
  (`"Cookies"`) and paths.
- **`CookieAuthentication.CreateHandler(options)`** — the factory the
  composition root calls; the concrete handler stays internal.

## How it fits

- **Implements `IAuthenticationSignInHandler`** — from
  `Assimalign.Cohesion.Web.Authentication` (the scheme model).
- **Seals tickets through** — `Assimalign.Cohesion.Security.DataProtection`
  — keys are never hand-rolled.
- **Keys its redirect-vs-`401`/`403` decision** — on the `IApiEndpointMetadata`
  endpoint marker resolved through `Assimalign.Cohesion.Web.Routing`.

`Register` it at the composition root, not here:

See the [source-backed usage examples](examples/index.md).

See [docs/DESIGN.md](design.md) for the ticket format, sliding-renewal rule, and the
redirect-vs-status decision.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Http` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.Cookies` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Routing` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Authentication` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Security.DataProtection` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication.Cookie/README.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication.Cookie/src/Assimalign.Cohesion.Web.Authentication.Cookie.csproj`.
