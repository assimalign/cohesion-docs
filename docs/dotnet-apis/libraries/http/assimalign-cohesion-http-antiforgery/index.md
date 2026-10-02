# Assimalign.Cohesion.Http.Antiforgery

Protects HTTP requests with paired antiforgery cookie and request tokens.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

The signed double-submit model protects both halves so an injected cookie alone cannot authenticate
a request token. Antiforgery composes protocol, cookie, form, and forwarded-identity contracts
without adding these concerns to the HTTP root. Validation is an explicit server-side operation; in
a Web application, `Assimalign.Cohesion.Web.Antiforgery` registers the service with
`AddAntiforgery` and validates protected endpoints with `UseAntiforgery`.

The cookie token is `Secure` whenever the request's effective scheme is HTTPS: `EffectiveScheme`
from `Http.Forwarded`, so TLS terminated at a trusted proxy counts once the forwarded-headers
middleware runs before the token is stored. `CookieSecure = true` forces the flag on plain HTTP
too. The cookie token is essential by default (`CookieIsEssential`), so a cookie-consent policy
emits it before the user consents. The default protector signs with a per-process random key, which
is for development only; deployed applications set a ring-backed `Protector`. Every
`IHttpAntiforgeryFeature` reports `nameof(IHttpAntiforgeryFeature)` as its name, so an exchange
carries one antiforgery service and assigning `context.Antiforgery` replaces a registered one.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Http.Cookies`](../../http/assimalign-cohesion-http-cookies/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Http.Forms`](../../http/assimalign-cohesion-http-forms/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Http.Forwarded`](../../http/assimalign-cohesion-http-forwarded/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `AntiforgeryValidationException` | `src/Exceptions/AntiforgeryValidationException.cs` |
| `HttpAntiforgery` | `src/HttpAntiforgery.cs` |
| `HttpAntiforgeryOptions` | `src/HttpAntiforgeryOptions.cs` |
| `HttpAntiforgeryTokenSet` | `src/HttpAntiforgeryTokenSet.cs` |
| `HttpContextAntiforgeryExtensions` | `src/Extensions/HttpContextAntiforgeryExtensions.cs` |
| `IHttpAntiforgery` | `src/Abstractions/IHttpAntiforgery.cs` |
| `IHttpAntiforgeryFeature` | `src/Abstractions/IHttpAntiforgeryFeature.cs` |
| `IHttpAntiforgeryProtector` | `src/Abstractions/IHttpAntiforgeryProtector.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src/Assimalign.Cohesion.Http.Antiforgery.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/README.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src/Exceptions/AntiforgeryValidationException.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src/HttpAntiforgery.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src/HttpAntiforgeryOptions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src/HttpAntiforgeryTokenSet.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src/Extensions/HttpContextAntiforgeryExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src/Abstractions/IHttpAntiforgery.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src/Abstractions/IHttpAntiforgeryFeature.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src/Abstractions/IHttpAntiforgeryProtector.cs`.
