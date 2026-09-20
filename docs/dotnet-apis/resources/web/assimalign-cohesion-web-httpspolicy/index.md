# Assimalign.Cohesion.Web.HttpsPolicy

HTTPS policy for the Cohesion Web pipeline: the natural pair of HTTP-to-HTTPS redirection and HTTP Strict Transport Security (HSTS), delivered as one lean feature package.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

HTTPS policy for the Cohesion Web pipeline: the natural pair of HTTP-to-HTTPS redirection and HTTP
Strict Transport Security (HSTS), delivered as one lean feature package. Both are table-stakes for
an enterprise-facing web server, and both hinge on the same question — *is this connection secure?*
— answered from the transport-derived typed scheme the Web TLS surface (#763) resolves onto every
request.

## What it provides

- **`UseHttpsRedirection(Action<HttpsRedirectionOptions>?)`** — a pipeline verb on
  `IWebApplicationPipelineBuilder`. An insecure request is answered with a
  bodyless, method-preserving redirect (`307` by default, `308` when configured)
  whose `Location` is the same request re-addressed to `https`, the request host
  with its port replaced by the configured HTTPS port, and the path and query
  preserved. An already-secure request passes straight through. **`Register` it
  early** so an insecure request is discarded before downstream middleware works
  on a response that is about to be thrown away.
- **`UseHsts(Action<HstsOptions>?)`** — a pipeline verb that emits the
  `Strict-Transport-Security` field, composed once at registration, on secure
  responses **only** (RFC 6797 §7.2), skipping the excluded hosts (`localhost`,
  `127.0.0.1`, `[::1]` by default).

Both verbs take an optional configuration callback (the defaults are sensible) and validate their
options at builder time — a bad status, port, `max-age`, or excluded-host pattern throws at
registration, never per request.

## Usage

See the [source-backed usage examples](examples/index.md).

## Dependencies

- **`Assimalign.Cohesion.Web`** — the pipeline abstractions the verbs extend.
- **`Assimalign.Cohesion.Http`** — `HttpScheme` (the transport-derived security
  signal), `HttpHost`/`HttpHostMatcher` (excluded-host matching), `HttpHeaderKey`
  (the RFC 6797 `Strict-Transport-Security` key), and the redirect status codes.

No DI, configuration, or logging dependency — the verbs capture values at builder time and the
middleware resolves nothing per request. Delivered to applications through the `App.Web` shared
framework (via `Sdk.Web`); no project wiring required. See `docs/DESIGN.md` for the
security-detection, port-resolution, and HSTS emission-point decisions, and the non-goals.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.HttpsPolicy/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.HttpsPolicy/src/Assimalign.Cohesion.Web.HttpsPolicy.csproj`.
