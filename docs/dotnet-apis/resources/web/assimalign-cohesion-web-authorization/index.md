# Assimalign.Cohesion.Web.Authorization

Endpoint authorization for the Cohesion Web pipeline.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

Endpoint authorization for the Cohesion Web pipeline. The package decides whether the principal a
request authenticated as may reach the endpoint it matched, and answers the requests that may not
with a challenge or a forbid through `Web.Authentication`. It evaluates over the BCL
`ClaimsPrincipal` that `Web.Authentication` establishes (`context.User`).

## Scope

- **Policies** (`AuthorizationPolicy`, built with `AuthorizationPolicyBuilder`): immutable lists of
  requirements a request must all satisfy (an authenticated user, roles, claims with allowed values,
  sync or async delegate assertions, or a custom `IAuthorizationRequirement`), plus the
  authentication schemes that establish the principal they evaluate.
- **Options** (`AuthorizationOptions`, registered with `AddAuthorization`): the default policy (an
  authenticated user), an optional fallback policy for requests without authorization metadata, and
  named policies. They become read-only once registered.
- **Endpoint metadata** (`AuthorizationMetadata`, attached with `RequireAuthorization(...)` and
  `AllowAnonymous()` on routes and groups). Every authorization item on an endpoint applies, except
  those declared before the most specific `AllowAnonymous`, which clears them. A requirement
  declared after it still applies, so a protected route inside an anonymous group stays protected;
  in ASP.NET Core, `AllowAnonymous` anywhere on the endpoint wins.
- **The middleware** (`UseAuthorization`), after `UseRouting` and `UseAuthentication`: it computes
  the endpoint's effective policy, authenticates the policy's own schemes when it names any
  (per-endpoint scheme selection), evaluates the policy, and challenges an anonymous caller or
  forbids an authenticated one. A CORS preflight is never authorized.
- **Fail closed.** An endpoint that requires authorization fails at dispatch with
  `InvalidOperationException` when `UseAuthorization` is missing or registered ahead of
  `UseRouting`, instead of running unauthorized.

## Dependencies

- **`Assimalign.Cohesion.Web`** — the builder, pipeline, and middleware abstractions the verbs build
  on.
- **`Assimalign.Cohesion.Web.Routing`** — the published endpoint and its metadata, the
  convention-builder seam the endpoint verbs extend, and the acknowledgement routing checks before
  it dispatches.
- **`Assimalign.Cohesion.Web.Authentication`** — `context.User`, and the `AuthenticateAsync`,
  `ChallengeAsync` and `ForbidAsync` verbs the middleware answers through.
- **`Assimalign.Cohesion.Http`** — the HTTP context.

It never references `Assimalign.Cohesion.Web.Hosting` or any `Assimalign.Cohesion.Hosting*` library
(the resource hosting-isolation rules `COHRES001` and `COHRES004`).

## Usage

See the [source-backed usage examples](examples/index.md).

Register `UseAuthorization` after `UseRouting` (which publishes the endpoint) and after
`UseAuthentication` (which establishes `context.User`), and ahead of middleware that serves or
caches responses, such as `UseOutputCache`. A CORS middleware goes ahead of it.

See `docs/DESIGN.md` for the evaluation flow, the combination rules, the fail-closed carrier design,
scheme selection, the fallback policy's reach, and the IdentityModel adapter planned for #828.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Http` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Authentication` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Routing` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authorization/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authorization/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authorization/src/Assimalign.Cohesion.Web.Authorization.csproj`.
