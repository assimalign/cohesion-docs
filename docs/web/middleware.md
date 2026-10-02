# Middleware

Web feature packages compose typed request behavior through the root application pipeline.

> **Status:** Partial. Every listed family has a runtime implementation; distributed cache and session backends remain adapter work.

## Composition model

Feature registration extends `IWebApplicationBuilder` or `IWebApplicationPipelineBuilder`.
Runtime dependencies are captured during composition. Request middleware exchanges state through
typed features on the Hypertext Transfer Protocol (HTTP) context, avoiding per-request service
location. Each feature package owns its builder verbs rather than adding them to `Web.Hosting`.

Pipeline order matters because middleware can wrap downstream execution or answer the request
itself. [Routing](routing.md) is not terminal: `UseRouting` selects the endpoint, publishes it on
the request, and calls the next middleware, and the end of the pipeline runs the endpoint. So
middleware registered after `UseRouting` runs before the endpoint and can read the endpoint's
metadata, which is where endpoint policies such as CORS, authorization, timeouts, rate limits and
antiforgery are applied.

Endpoint policies fail closed. When an endpoint declares CORS, authorization, a timeout, a rate
limit or antiforgery and no middleware applied it, because `UseCors`, `UseAuthorization`,
`UseRequestTimeouts`, `UseRateLimiting` or `UseAntiforgery` is missing or registered before
`UseRouting`, the request fails with an `InvalidOperationException` instead of running the endpoint
unprotected.

The pipeline can also branch. `Map(path, ...)` runs a separate middleware chain for requests under
a path prefix and publishes the prefix as the path base, `MapWhen` branches on a predicate,
`UseWhen` runs extra middleware for matching requests and then rejoins the main pipeline, and
`Run` ends a pipeline with a terminal middleware. Branches hold middleware only; routes are mapped
on the application.

## Feature families

| Package family | Implemented behavior |
|---|---|
| `Web.ForwardedHeaders` | Trusted-proxy handling and an effective client/host/scheme feature. |
| `Web.HostFiltering` | Allowed-host enforcement against the effective host (forwarded by a trusted proxy, else transport-resolved); rejects a host outside the allowlist. |
| `Web.HttpsPolicy` | HTTPS redirects and HTTP Strict Transport Security (HSTS) response policy. |
| `Web.SecurityHeaders` | `nosniff`, clickjacking protection and a referrer policy by default; opt-in Content Security Policy with per-request nonces, Permissions-Policy and cross-origin isolation fields; endpoint overrides. |
| `Web.Authentication` | Named schemes, default selection, principal feature, and request dispatch. |
| `Web.Authentication.Cookie` | Protected tickets, sign-in/out, and sliding expiration. |
| `Web.Authentication.Bearer` | Bearer token validation and principal construction using IdentityModel. |
| `Web.Authorization` | Policies over `ClaimsPrincipal`, `RequireAuthorization`/`AllowAnonymous` metadata, default, fallback and named policies, per-endpoint schemes, and challenge or forbid through the schemes. |
| `Web.Cors` | Validated CORS policies, preflight answers, actual-response headers with `Vary: Origin`, and per-endpoint policy selection. |
| `Web.CookiePolicy` | Consent gating for non-essential cookies, `Secure`/`HttpOnly`/`SameSite` floors, RFC 6265bis prefix rules, and the 400-day cap, applied as each cookie is appended. |
| `Web.Sessions` | Lazy store-backed sessions, cookie identity, commit/slide, and identifier regeneration. |
| `Web.Forms` | Form parsing through `Http.Forms` and `IHttpFormFeature`. |
| `Web.Antiforgery` | Cross-site request forgery token validation for endpoints that require it, including form-bound typed endpoints, with a `400` problem response on failure. |
| `Web.StaticFiles` | Web-root serving, conditional GET, single byte ranges, default documents, precompressed assets, and the single-page-application fallback (`MapFallbackToFile`). |
| `Web.Compression` | Negotiated response compression and bounded request decompression. |
| `Web.Caching` | Server-owned GET/HEAD output caching with policy metadata, variation, tags, and size accounting. |
| `Web.RequestTimeouts` | Global/endpoint timeout policies, cancellation, and configurable 504 responses. |
| `Web.RateLimiting` | Global/named policies, partitioned limiters, queues, and 429 rejection handling. |
| `Web.Diagnostics` | Field-selected request logging, allowlist redaction, bounded body capture, and access-log files. |
| `Web.Health` | Independent check model, readiness/liveness selection, and pipeline endpoints. |
| `Web.Query` | QUERY request negotiation, conditional requests, and method-preserving redirects. |

## Ordering and behavior to preserve

[Middleware order](middleware-order.md) gives the one registration order for every Web
middleware, the reason for each position, and the whole order in code. From the front of the
pipeline to the endpoint: `UseHttpLogging` and `UseSecurityHeaders` wrap everything; forwarded
headers, host filtering, HTTPS redirection and HSTS settle the client's identity and transport; the
exception boundary, status-code pages, cookie policy, compression, static files, authentication and
sessions follow; then `UseRouting` and the endpoint policies: CORS, authorization, timeouts, rate
limits, forms, antiforgery and the output cache.

- **Proxy and host policy** — Forwarded-header processing goes ahead of every middleware that reads
  the client's identity on the way in; only `UseHttpLogging` and `UseSecurityHeaders`, which read
  none, go ahead of it, and host filtering follows it directly. Configure proxy trust explicitly
  rather than treating caller-supplied forwarded headers as transport facts. HTTPS redirection,
  HSTS, host filtering, the cookie policy, sessions, cookie authentication, the antiforgery cookie
  token, compression and logging all read the forwarded (effective) scheme, host and client
  address.
- **Security headers and the exception boundary** — `UseSecurityHeaders` and `UseHsts` go ahead of
  `UseErrorHandling`, so the boundary's error page carries their fields.
- **Endpoint policies** — `UseCors`, `UseAuthorization`, `UseRequestTimeouts`, `UseRateLimiting`,
  `UseAntiforgery` and `UseOutputCache` read the endpoint `UseRouting` published, so they go after
  it, in that order. `UseCors` comes first because a preflight carries no credentials, and anything
  ahead of it could reject one. Registered before `UseRouting`, CORS, authorization, a timeout, a
  rate limit or antiforgery fails its endpoint's requests, and output caching keeps only its base
  policy: it never stores a response from an endpoint that carries cache metadata. The global rate
  limiter and the default timeout still work in either position.
- **CORS and error responses** — A fault that reaches an exception boundary registered ahead of
  `UseCors` becomes an error page without CORS headers, which a browser hides from a cross-origin
  caller. See [the CORS trade-off](middleware-order.md#the-cors-trade-off).
- **Output cache and compression** — Register `UseOutputCache` ahead of `UseResponseCompression`
  so cached variants are not confused across `Accept-Encoding` values. In an application that
  caches, compression therefore also follows `UseRouting`, and a middleware that answers before
  routing, such as static files, relies on its own encoding (precompressed assets).
- **Static files** — `UseStaticFiles()` serves the application's web root, `wwwroot` under the
  content root, and never the content root or the working directory. A single-page application
  answers client-side routes with `MapFallbackToFile("index.html")`; see [Routing](routing.md).
- **Cookies and sessions** — `UseCookiePolicy` goes ahead of every middleware that writes a
  cookie and judges each cookie as it is appended. The cookie authentication ticket and the
  antiforgery cookie token are essential by default, so a consent requirement does not drop them;
  the session cookie is not, unless `HttpSessionOptions.CookieIsEssential` is set. Cookie
  authentication protects tickets through `IDataProtector`. Sessions lazily acquire state and
  persist changes or slide expiration after downstream execution.
- **Health integration** — `Web.Health` owns the Web model. `Web.Hosting.Health` is the optional
  adapter for shared hosting contributors; the feature itself has no hosting-library dependency.

Response compression supports gzip and Brotli and is off for HTTPS by default under its security
policy. Request decompression supports gzip, Brotli, and deflate, returns 413 when the decompressed
size limit is exceeded, and 415 for unsupported codings.

Output caching bypasses responses with prohibitive cache directives, `Set-Cookie`, non-200 status,
or authenticated requests under its default rules. Distributed cache and session backends remain
adapter work through `IOutputCacheStore` and `IHttpSessionStore`.

For fault handling and content writers, see [Endpoints and responses](endpoints.md).
Return to [Web](index.md).

## Sources

- **Feature map** — `cohesion/resources/Web/README.md`.
- **Ordering** — `cohesion/docs/resources/Web/MIDDLEWARE_ORDER.md`.
- **Endpoint selection and ordering** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Routing/docs/DESIGN.md`.
- **Pipeline branching** — `cohesion/resources/Web/Assimalign.Cohesion.Web/docs/DESIGN.md`.
- **Cache and compression order** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Caching/docs/DESIGN.md`.
- **Authentication** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/docs/DESIGN.md`, `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication.Cookie/docs/DESIGN.md`, and `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication.Bearer/docs/DESIGN.md`.
- **Forms and health** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Forms/docs/DESIGN.md` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Health/docs/DESIGN.md`.
- **Security families** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authorization/docs/OVERVIEW.md`, `cohesion/resources/Web/Assimalign.Cohesion.Web.Cors/docs/OVERVIEW.md`, `cohesion/resources/Web/Assimalign.Cohesion.Web.CookiePolicy/docs/OVERVIEW.md`, `cohesion/resources/Web/Assimalign.Cohesion.Web.Antiforgery/docs/OVERVIEW.md`, and `cohesion/resources/Web/Assimalign.Cohesion.Web.SecurityHeaders/docs/OVERVIEW.md`.
- **Cookie defaults** — `cohesion/resources/Web/Assimalign.Cohesion.Web.CookiePolicy/docs/DESIGN.md` and `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/src/HttpSessionOptions.cs`.
