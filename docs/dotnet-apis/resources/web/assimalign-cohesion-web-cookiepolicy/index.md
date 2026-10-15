# Assimalign.Cohesion.Web.CookiePolicy

Cookie-policy enforcement for the Cohesion Web pipeline.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

Cookie-policy enforcement for the Cohesion Web pipeline. `UseCookiePolicy` makes every cookie the
application appends through `context.Response.Cookies` pass the site's policy before it reaches a
`Set-Cookie` field. The policy covers consent for non-essential cookies, the `Secure`, `HttpOnly`,
and minimum `SameSite` floors, the RFC 6265bis requirements a browser enforces by ignoring a cookie,
and the 400-day lifetime cap. The cookie model it enforces against lives in
`Assimalign.Cohesion.Http.Cookies`; this package owns the policy.

## What it provides

- **`UseCookiePolicy(Action<CookiePolicyOptions>?)`** — a pipeline verb on
  `IWebApplicationPipelineBuilder`. It validates and captures the options at registration and
  replaces each exchange's response cookie feature, so `Web.Sessions`, `Web.Authentication.Cookie`,
  `Http.Antiforgery`, and application code are all covered without changes of their own.
- **`CookiePolicyOptions`**:
  - `Secure`: `SameAsRequest` by default, which follows the effective scheme, so a trusted
    TLS-terminating proxy counts; or `Always`, or `None`.
  - `HttpOnly`: `None` or `Always`.
  - `MinimumSameSitePolicy`.
  - `SameSiteNoneWithoutSecure` and `PrefixViolation`: `Upgrade` or `Reject`.
  - `MaxLifetime` and `TimeProvider`: the lifetime cap and the clock it is applied against.
  - `CheckConsentNeeded`, plus the consent cookie's name, value, and attributes.
  - `OnRejected`: a hook that observes every dropped cookie.
- **`ICookieConsentFeature`** — per-exchange consent state (`IsConsentNeeded`, `HasConsent`,
  `CanTrack`) and the `GrantConsent` / `WithdrawConsent` verbs. Resolve it with
  `context.Features.Get<ICookieConsentFeature>()`.
- **`CookiePolicyRejectionContext`** / **`CookiePolicyRejectionReason`** — what `OnRejected`
  receives: the exchange, the cookie as appended, and why it was dropped.

The defaults are secure without breaking ordinary sites: `Secure` whenever the client used HTTPS,
RFC 6265bis violations repaired rather than dropped, the 400-day cap, no `HttpOnly` or `SameSite`
floor, and no consent requirement. A rule only ever adds protection; nothing the application set is
removed.

## Usage

See the [source-backed usage examples](examples/index.md).

The Cookie authentication ticket is essential by default, so a consent requirement never prevents
sign-in.

`UseCookiePolicy` and its types are in the `Assimalign.Cohesion.Web.CookiePolicy` namespace. The
pass-through verb this package shipped before enforcement landed was in `Assimalign.Cohesion.Web`,
so an application that registered it imports the new namespace.

## Dependencies

- **`Assimalign.Cohesion.Web`** — the pipeline abstractions the verb extends.
- **`Assimalign.Cohesion.Http.Cookies`** — the cookie model: `HttpCookie`, the response cookie
  feature the policy replaces, and `HttpCookie.ClampLifetime`.
- **`Assimalign.Cohesion.Http.Forwarded`** — the `EffectiveScheme` read behind `SameAsRequest`,
  which falls back to the transport's scheme when no forwarded-headers middleware ran.
- **`Assimalign.Cohesion.Http`** — the HTTP context and feature collection.

No DI, configuration, logging, or hosting dependency. The package is delivered to applications
through the `App.Web` shared framework (via `Sdk.Web`). See [`DESIGN.md`](design.md) for the
interception mechanism, rule order, defaults, consent model, pipeline placement, and non-goals.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.Cookies` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.Forwarded` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.CookiePolicy/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.CookiePolicy/src/Assimalign.Cohesion.Web.CookiePolicy.csproj`.
- **Namespace change** — `cohesion/resources/Web/Assimalign.Cohesion.Web.CookiePolicy/src/Extensions/CookiePolicyExtensions.cs` and `cohesion/docs/programs/HTTP_WEB_PROGRAM_PLAN.md`.
