# Assimalign.Cohesion.Web.Antiforgery

Cross-site request forgery (CSRF) protection for the Cohesion Web pipeline.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

Cross-site request forgery (CSRF) protection for the Cohesion Web pipeline. The package wires the
`Assimalign.Cohesion.Http.Antiforgery` token engine (a signed double-submit cookie and request
token) into a Web application: it registers the application's antiforgery service, validates
protected endpoints in a middleware, and declares protection per endpoint with routing metadata. It
never reimplements token cryptography; it chooses the protector the engine seals tokens with.

## Scope

- **`builder.Services.AddAntiforgery(...)`** (builder time) creates the application's
  `IHttpAntiforgery` service and registers it as an application feature, an `IHttpFeature`
  singleton, so every exchange carries it and a handler mints tokens with
  `context.RequireAntiforgery.GetAndStoreTokens(context)`. The verb is a component integration the
  application's compilation receives (owner decision 34, #1380); this package takes no
  dependency-injection reference.
- **Protector selection.** `AddAntiforgery(dataProtectionProvider)` seals tokens with a protector
  the application's `Security.DataProtection` key ring derives for the antiforgery purpose, so
  tokens survive restarts and validate on every instance that shares the key repository.
  `AddAntiforgery()` without a provider falls back to the engine's per-process random key, which is
  for **development only**: a restart invalidates every token, and instances reject each other's
  tokens.
- **`UseAntiforgery`** (pipeline time, after `UseRouting`) validates unsafe-method requests to
  endpoints that carry `AntiforgeryMetadata.Required`. Safe methods (`GET`, `HEAD`, `OPTIONS`,
  `TRACE`) and CORS preflights pass through; `QUERY` carries a body and is validated. A failed
  validation is answered with `400 Bad Request` as RFC 9457 `application/problem+json`, and the
  endpoint does not run.
- **Endpoint metadata and verbs.** `RequireAntiforgery()` and `DisableAntiforgery()` attach the
  sealed `AntiforgeryMetadata` to a route or a route group, most specific declaration winning. Every
  typed endpoint with a `[FromForm]` parameter or an uploaded-file parameter requires antiforgery
  automatically: the Web endpoint-binding generator attaches the requirement when the application
  references this package. A form read for its token that exceeds a configured Http.Forms limit is
  answered `413 Content Too Large`, as the endpoint's own binding would answer it.
- **Fail closed.** An endpoint that requires validation fails at dispatch with
  `InvalidOperationException` when `UseAntiforgery` is missing or registered ahead of `UseRouting`,
  instead of running unprotected.

## Dependencies

- **`Assimalign.Cohesion.Web`** — the builder, pipeline and middleware abstractions the verbs build
  on.
- **`Assimalign.Cohesion.Web.Routing`** — the published route match and metadata the middleware
  reads, the convention-builder seam the verbs extend, and the acknowledgement routing checks before
  dispatch.
- **`Assimalign.Cohesion.Web.ProblemDetails`** — the `application/problem+json` rejection.
- **`Assimalign.Cohesion.Http.Antiforgery`** — the token engine, `IHttpAntiforgery`, and its
  options.
- **`Assimalign.Cohesion.Http.Forms`** — the form read for the form-token flow.
- **`Assimalign.Cohesion.Http.Streaming`** — the committed-response check before a rejection.
- **`Assimalign.Cohesion.Security.DataProtection`** — the purpose-bound protector behind production
  tokens.
- **`Assimalign.Cohesion.Http`** — the HTTP context, methods, media types, and header keys.

It never references `Assimalign.Cohesion.Web.Hosting` or any `Assimalign.Cohesion.Hosting*` library
(the resource hosting-isolation rule, `COHRES001`/`COHRES004`). The package and `Http.Antiforgery`
are members of the `App.Web` shared framework.

## Usage

See the [source-backed usage examples](examples/index.md).

To share the key ring cookie authentication uses, pass the same provider to both
(`builder.Services.AddAuthentication(auth => auth.UseDataProtection(dataProtection).AddCookie())`
and `builder.Services.AddAntiforgery(dataProtection)`); each derives its own purpose, so neither
accepts the other's payloads.

The cookie token is `Secure` whenever the request's effective scheme is HTTPS: a direct TLS
connection, or TLS terminated at a trusted proxy that `UseForwardedHeaders`, registered ahead of the
handler that stores the token, vouched for. `HttpAntiforgeryOptions.CookieSecure = true` forces the
flag on plaintext requests too. The cookie token is also essential by default
(`HttpAntiforgeryOptions.CookieIsEssential`), so a `UseCookiePolicy` consent requirement does not
drop it.

Register `UseAntiforgery` after `UseRouting`, and after `UseRateLimiting` and `UseRequestTimeouts`
when the application uses them, so floods are rejected before any body is read and a form read runs
under the endpoint's timeout. A client that sends the token in the header keeps its body unread by
the middleware, which matters for endpoints that stream large uploads.

See `docs/DESIGN.md` for the token flow, the protector selection, the generator integration, the
fail-closed rule, and the non-goals.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.ProblemDetails` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Routing` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.Antiforgery` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.Forms` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.Streaming` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Security.DataProtection` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Antiforgery/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Antiforgery/src/Assimalign.Cohesion.Web.Antiforgery.csproj`.
- **Cookie token attributes** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/docs/DESIGN.md`.
