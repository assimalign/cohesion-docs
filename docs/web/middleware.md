# Middleware

Web feature packages compose typed request behavior through the root application pipeline.

> **Status:** Partial. Most listed families have runtime implementations; authorization and cross-origin middleware remain scaffolds.

## Composition model

Feature registration extends `IWebApplicationBuilder` or `IWebApplicationPipelineBuilder`.
Runtime dependencies are captured during composition. Request middleware exchanges state through
typed features on the Hypertext Transfer Protocol (HTTP) context, avoiding per-request service
location. Each feature package owns its builder verbs rather than adding them to `Web.Hosting`.

Pipeline order matters because middleware can wrap downstream execution or answer the request
itself. [Routing](routing.md) is not terminal: `UseRouting` selects the endpoint, publishes it on
the request, and calls the next middleware, and the end of the pipeline runs the endpoint. So
middleware registered after `UseRouting` runs before the endpoint and can read the endpoint's
metadata, which is where endpoint policies such as rate limits and timeouts are applied.

Endpoint policies fail closed. When an endpoint declares a rate limit or a timeout and no
middleware applied it, because `UseRateLimiting` or `UseRequestTimeouts` is missing or registered
before `UseRouting`, the request fails with an `InvalidOperationException` instead of running the
endpoint unprotected.

The pipeline can also branch. `Map(path, ...)` runs a separate middleware chain for requests under
a path prefix and publishes the prefix as the path base, `MapWhen` branches on a predicate,
`UseWhen` runs extra middleware for matching requests and then rejoins the main pipeline, and
`Run` ends a pipeline with a terminal middleware. Branches hold middleware only; routes are mapped
on the application.

## Feature families

| Package family | Implemented behavior |
|---|---|
| `Web.ForwardedHeaders` | Trusted-proxy handling and an effective client/host/scheme feature. |
| `Web.HostFiltering` | Allowed-host enforcement; rejects a transport-resolved host outside the allowlist. |
| `Web.HttpsPolicy` | HTTPS redirects and HTTP Strict Transport Security (HSTS) response policy. |
| `Web.Authentication` | Named schemes, default selection, principal feature, and request dispatch. |
| `Web.Authentication.Cookie` | Protected tickets, sign-in/out, and sliding expiration. |
| `Web.Authentication.Bearer` | Bearer token validation and principal construction using IdentityModel. |
| `Web.CookiePolicy` | Site policy for request/response cookie collections. |
| `Web.Sessions` | Lazy store-backed sessions, cookie identity, commit/slide, and identifier regeneration. |
| `Web.Forms` | Form parsing through `Http.Forms` and `IHttpFormFeature`. |
| `Web.StaticFiles` | Web-root serving, conditional GET, single byte ranges, default documents, precompressed assets, and the single-page-application fallback (`MapFallbackToFile`). |
| `Web.Compression` | Negotiated response compression and bounded request decompression. |
| `Web.Caching` | Server-owned GET/HEAD output caching with policy metadata, variation, tags, and size accounting. |
| `Web.RequestTimeouts` | Global/endpoint timeout policies, cancellation, and configurable 504 responses. |
| `Web.RateLimiting` | Global/named policies, partitioned limiters, queues, and 429 rejection handling. |
| `Web.Diagnostics` | Field-selected request logging, allowlist redaction, bounded body capture, and access-log files. |
| `Web.Health` | Independent check model, readiness/liveness selection, and pipeline endpoints. |
| `Web.Query` | QUERY request negotiation, conditional requests, and method-preserving redirects. |

`Web.Authorization` and `Web.Cors` have project files but no implementation sources in this
checkout. The existence of packages or a project-map row does not provide authorization or
cross-origin resource sharing (CORS) behavior.

## Ordering and behavior to preserve

A typical order, from the front of the pipeline to the endpoint:

```csharp
// Behind a proxy, first: everything after it reads the effective scheme, host and client address.
app.UseForwardedHeaders(options =>
{
    options.Headers = ForwardedHeaderNames.XForwarded;
    options.KnownNetworks.Add(IPNetwork.Parse("10.0.0.0/8"));
});
app.UseHostFiltering(options => options.AllowedHosts.Add("example.com"));
app.UseErrorHandling();        // wraps everything downstream
app.UseHttpsRedirection();
app.UseStaticFiles();          // answers file requests before routing
app.UseAuthentication();
app.UseRouting();              // selects the endpoint and continues
app.UseRequestTimeouts();      // endpoint policies: after UseRouting
app.UseRateLimiting();
app.UseOutputCache();
app.UseResponseCompression();  // inside the output cache
```

- **Proxy and host policy** — Forwarded-header processing belongs at the front of the pipeline;
  host filtering is also an early boundary. Configure proxy trust explicitly rather than treating
  caller-supplied forwarded headers as transport facts. HTTPS redirection, HSTS, host filtering,
  sessions, cookie authentication, compression and logging all read the forwarded (effective)
  scheme, host and client address.
- **Endpoint policies** — `UseRateLimiting`, `UseRequestTimeouts` and `UseOutputCache` read the
  endpoint `UseRouting` published, so they go after it. Registered before it, a rate limit or
  timeout fails its endpoint's requests, and output caching keeps only its base policy: it never
  stores a response from an endpoint that carries cache metadata. The global rate limiter and the
  default timeout still work in either position.
- **Output cache and compression** — Register `UseOutputCache` ahead of `UseResponseCompression`
  so cached variants are not confused across `Accept-Encoding` values. In an application that
  caches, compression therefore also follows `UseRouting`, and a middleware that answers before
  routing, such as static files, relies on its own encoding (precompressed assets).
- **Static files** — `UseStaticFiles()` serves the application's web root, `wwwroot` under the
  content root, and never the content root or the working directory. A single-page application
  answers client-side routes with `MapFallbackToFile("index.html")`; see [Routing](routing.md).
- **Cookies and sessions** — Cookie authentication protects tickets through `IDataProtector`.
  Sessions lazily acquire state and persist changes or slide expiration after downstream execution.
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

- **Feature map and ordering** — `cohesion/resources/Web/README.md`.
- **Endpoint selection and ordering** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Routing/docs/DESIGN.md`.
- **Pipeline branching** — `cohesion/resources/Web/Assimalign.Cohesion.Web/docs/DESIGN.md`.
- **Cache and compression order** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Caching/docs/DESIGN.md`.
- **Authentication** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/docs/DESIGN.md`, `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication.Cookie/docs/DESIGN.md`, and `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication.Bearer/docs/DESIGN.md`.
- **Forms and health** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Forms/docs/DESIGN.md` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Health/docs/DESIGN.md`.
- **Unimplemented surfaces** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authorization/src/` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Cors/src/`.
