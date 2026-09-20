# Middleware

Web feature packages compose typed request behavior through the root application pipeline.

> **Status:** Partial. Most listed families have runtime implementations; authorization and cross-origin middleware remain scaffolds.

## Composition model

Feature registration extends `IWebApplicationBuilder` or `IWebApplicationPipelineBuilder`.
Runtime dependencies are captured during composition. Request middleware exchanges state through
typed features on the Hypertext Transfer Protocol (HTTP) context, avoiding per-request service
location. Each feature package owns its builder verbs rather than adding them to `Web.Hosting`.

Pipeline order matters because middleware can wrap downstream execution or terminate it.
In particular, [routing](routing.md) is terminal on a match. Put features that must observe or wrap
an endpoint before its routing terminal.

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
| `Web.StaticFiles` | Conditional GET, single byte ranges, default documents, and precompressed assets. |
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

- **Proxy and host policy** — Forwarded-header processing belongs at the front of the pipeline;
  host filtering is also an early boundary. Configure proxy trust explicitly rather than treating
  caller-supplied forwarded headers as transport facts.
- **Output cache and compression** — Register `UseOutputCache` ahead of `UseResponseCompression`
  so cached variants are not confused across `Accept-Encoding` values.
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
- **Pipeline terminal** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Routing/docs/DESIGN.md`.
- **Authentication** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication/docs/DESIGN.md`, `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication.Cookie/docs/DESIGN.md`, and `cohesion/resources/Web/Assimalign.Cohesion.Web.Authentication.Bearer/docs/DESIGN.md`.
- **Forms and health** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Forms/docs/DESIGN.md` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Health/docs/DESIGN.md`.
- **Unimplemented surfaces** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Authorization/src/` and `cohesion/resources/Web/Assimalign.Cohesion.Web.Cors/src/`.
