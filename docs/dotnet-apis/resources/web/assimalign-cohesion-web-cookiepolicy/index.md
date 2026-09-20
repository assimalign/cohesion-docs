# Assimalign.Cohesion.Web.CookiePolicy

The cookie-policy package currently registers pass-through middleware without enforcing a policy.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

`CookiePolicyOptions` is an empty public class. `UseCookiePolicy(Action<CookiePolicyOptions>)`
rejects a null callback and registers middleware that invokes the next component. It does not invoke
the callback or apply cookie-policy enforcement. The source explicitly leaves enforcement to later
application-model work.

The design document describes the intended split between the wire-level
`Assimalign.Cohesion.Http.Cookies` model and application policy. Cookie rejection, rewriting, and
policy options described there are not implemented by the current source.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.Cookies` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.CookiePolicy/src/Assimalign.Cohesion.Web.CookiePolicy.csproj`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.CookiePolicy/src/CookiePolicyOptions.cs`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.CookiePolicy/src/Extensions/CookiePolicyExtensions.cs`.
