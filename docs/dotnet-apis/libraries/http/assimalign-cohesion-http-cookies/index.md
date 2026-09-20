# Assimalign.Cohesion.Http.Cookies

Adds typed request and response cookies to the HTTP feature model.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

Cookies are opt-in features over the wire-level `Cookie` and `Set-Cookie` fields. Extension members
retain property-style access while keeping cookie types out of the protocol root. Parsing and
response flushing cooperate with the exchange lifecycle.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `HttpCookie` | `src/HttpCookie.cs` |
| `HttpCookieCollection` | `src/HttpCookieCollection.cs` |
| `HttpCookieExtensions` | `src/Extensions/HttpCookieExtensions.cs` |
| `HttpCookieLimits` | `src/HttpCookieLimits.cs` |
| `HttpCookieOptions` | `src/HttpCookieOptions.cs` |
| `HttpCookieSameSiteMode` | `src/HttpCookieOptions.cs` |
| `IHttpCookieCollection` | `src/Abstractions/IHttpCookieCollection.cs` |
| `IHttpRequestCookieFeature` | `src/Abstractions/IHttpCookieFeature.Request.cs` |
| `IHttpResponseCookieFeature` | `src/Abstractions/IHttpCookieFeature.Response.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/src/Assimalign.Cohesion.Http.Cookies.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/src/HttpCookie.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/src/HttpCookieCollection.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/src/Extensions/HttpCookieExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/src/HttpCookieLimits.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/src/HttpCookieOptions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/src/Abstractions/IHttpCookieCollection.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/src/Abstractions/IHttpCookieFeature.Request.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/src/Abstractions/IHttpCookieFeature.Response.cs`.
