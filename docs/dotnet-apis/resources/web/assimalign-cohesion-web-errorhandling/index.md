# Assimalign.Cohesion.Web.ErrorHandling

The `OnError` hook for the Cohesion Web pipeline: the fault seam through which an application owns its error responses.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

The `OnError` hook for the Cohesion Web pipeline: the fault seam through which an application owns
its error responses. Feature libraries throw their area-scoped exceptions; the hook decides what the
client sees, with an overridable default that renders the RFC 9457 `Web.ProblemDetails` payload.

## What it provides

- **The hook contract** — `IErrorHandler` / the `HttpErrorHandler` delegate: inspect a fault, own
  the response for it (return `true`) or pass (`false`).
- **Builder-time registration** — `builder.AddErrorHandling().OnError(...)`; handlers are
  consulted in registration order.
- **The exchange feature** — `IErrorHandlingFeature`, seeded onto every exchange; a pipeline
  exception boundary invokes `HandleAsync(context, exception)` to turn a caught fault into the
  application's response.
- **The terminal default** — when no registration owns a fault, the response is
  `500` + `application/problem+json` (`about:blank`, status phrase, no exception detail).
- **The pipeline exception boundary** — `UseErrorHandling()` installs the middleware that catches
  faults escaping downstream, publishes the caught exception as an `IHttpExceptionFeature`, resets
  an unstarted response (aborting the exchange when it has already started), and dispatches through
  the `OnError` chain. A developer-detail toggle enriches the terminal payload; a diagnostics
  observer (`OnException`) and its suppression predicate provide the fault-observation seam.
- **Status-code pages** — `UseStatusCodePages()` upgrades a bodyless `4xx`/`5xx` terminal response
  (such as the pipeline's bodyless 404) into problem+json, or a custom responder body.

## Usage

See the [source-backed usage examples](examples/index.md).

Install the boundary (and, optionally, status-code pages) on the pipeline. `Register`
`UseErrorHandling()` first so it wraps everything downstream:

See the [source-backed usage examples](examples/index.md).

The boundary resolves the hook from the exchange and delegates to it — the manual equivalent (an
inline `try/catch` around `next(context)`) is:

See the [source-backed usage examples](examples/index.md).

## Scope boundaries

- **Faults only.** Expected protocol outcomes — an authentication challenge's `401`, a router's
  `404`, an unsupported media type's `415` — are each feature's normal response path and must
  never arrive here as exceptions.
- **The bodyless 404 terminal lives in `Web.Hosting`.** The pipeline's unhandled-request terminal
  can only set a payload-free `404` (the hosting-isolation rule keeps `Web.ProblemDetails` out of
  the runtime module); `UseStatusCodePages()` here is what upgrades it to problem+json.
- **The payload** is `Web.ProblemDetails`' scope; this package renders it.

Design rationale lives in [DESIGN.md](design.md) .

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Http` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.Streaming` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.ProblemDetails` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.ErrorHandling/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.ErrorHandling/src/Assimalign.Cohesion.Web.ErrorHandling.csproj`.
