# Assimalign.Cohesion.Http.Cookies

Adds typed request and response cookies to the HTTP feature model.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

Cookies are opt-in features over the wire-level `Cookie` and `Set-Cookie` fields. Extension members
retain property-style access while keeping cookie types out of the protocol root. The request side
tokenizes the `Cookie` header the first time `request.Cookies` is read. The response collection
writes every change through to `response.Headers[Set-Cookie]`, one value per cookie, and the
transports serialize that header like any other field, so `Http.Connections` takes no dependency on
this package; only the HTTP/1.1 upgrade writer in `Http.ProtocolUpgrade` reads
`IHttpResponseCookieFeature` directly.

To replace the response feature, remove the existing feature's slot first: the feature collection
is keyed by `IHttpFeature.Name`, and `Get<T>()` returns the first match, so a second feature with a
different name leaves the existing one in charge. A replacement must also keep `Set-Cookie` in sync,
typically by queuing into the replaced feature's header-synchronized collection, as
`Web.CookiePolicy` does.

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

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/README.md`.

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
