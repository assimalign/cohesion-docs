# Assimalign.Cohesion.Web.CookiePolicy design

This page describes the boundaries and implementation shape of `Assimalign.Cohesion.Web.CookiePolicy`.

> **Status:** Partial.

`CookiePolicyOptions` is an empty public class. `UseCookiePolicy(Action<CookiePolicyOptions>)`
rejects a null callback and registers middleware that invokes the next component. It does not invoke
the callback or apply cookie-policy enforcement. The source explicitly leaves enforcement to later
application-model work.

The design document describes the intended split between the wire-level
`Assimalign.Cohesion.Http.Cookies` model and application policy. Cookie rejection, rewriting, and
policy options described there are not implemented by the current source.

## Declared dependencies

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.Cookies` | `CohesionProjectReference` |

[Assembly overview](index.md) · [Examples](examples/index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.CookiePolicy/docs/DESIGN.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.CookiePolicy/src/CookiePolicyOptions.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.CookiePolicy/src/Extensions/CookiePolicyExtensions.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.CookiePolicy/src/Assimalign.Cohesion.Web.CookiePolicy.csproj`.
