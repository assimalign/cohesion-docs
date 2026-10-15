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
transports serialize that header like any other field, one field line per value. Neither
`Http.Connections` nor the HTTP/1.1 upgrade writer in `Http.ProtocolUpgrade` takes a dependency on
this package, and no serializer reads `IHttpResponseCookieFeature` (#1154).

To replace the response feature, remove the existing feature's slot first: the feature collection
is keyed by `IHttpFeature.Name`, and `Get<T>()` returns the first match, so a second feature with a
different name leaves the existing one in charge. A replacement must also keep `Set-Cookie` in sync,
typically by queuing into the replaced feature's header-synchronized collection, as
`Web.CookiePolicy` does; whatever the header holds is what every response, the `101` included,
sends.

The `HttpCookieOptions.Path` and `Domain` setters throw `ArgumentException` for a value outside
RFC 6265 §4.1.1 (#1153), so a path or domain built from untrusted input cannot add an attribute
(`Path=/x; SameSite=None`) or a control character to the line. `Path` is US-ASCII without `;` or
controls. `Domain` is dot-separated letter-digit-hyphen labels, optionally preceded by one dot.
`null` (and an empty `Domain`) omits the attribute. The parser drops a received cookie whose `Path`
or `Domain` is outside the grammar. See the [design](design.md#attribute-grammar).

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
