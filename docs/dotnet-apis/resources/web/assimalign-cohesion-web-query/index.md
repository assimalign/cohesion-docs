# Assimalign.Cohesion.Web.Query

Server-side handling for the RFC 10008 HTTP QUERY method in the Cohesion Web pipeline.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

Server-side handling for the RFC 10008 HTTP **QUERY** method in the Cohesion Web pipeline. The core
value-object layer (`HttpMethod.Query`, its safe/idempotent/cacheable classification, and the
`Accept-Query` field model `HttpAcceptQuery`) ships in `Assimalign.Cohesion.Http`; this package
adds the *application decisions* the RFC's MUST-level server rules call for — middleware-first,
every response written imperatively.

## Scope

- **Request-content validation (RFC 10008 § 2.1 / § 2.3)** — `UseQueryValidation(...)`:
  query content must declare a parseable `Content-Type` (else `400`, or `415` per policy); the
  declared type must fall within the resource's advertised `Accept-Query` set (else `415`, with
  the accepted set advertised on the rejection); the request's `Accept` field must be
  satisfiable against the resource's producible representations (else `406`). Configured
  through `WebQueryValidationOptions`.
- **Method-preserving redirects (RFC 10008 § 2.5)** — `response.``RedirectQuery``(location,
  permanent)` emits `307`/`308` (never `301`/`302`, whose legacy GET rewrite would drop the
  query content), and `response.RedirectQueryToGet(location)` emits the one sanctioned method
  switch, `303 See Other`. The client half — a redirect-following client that re-issues QUERY
  with its content — lives in `Assimalign.Cohesion.Http.ClientFactory`.
- **Conditional QUERY (RFC 10008 § 2.6)** — `UseQueryConditionals(provider)` and the
  `EvaluateQueryPreconditions` / `TryHandleQueryPreconditions` context helpers evaluate the
  `If-*` precondition fields exactly as for the equivalent conditional GET — reusing the core
  `HttpConditionalRequest` evaluator — and answer `304` / `412` without executing the query.

## Dependencies

`Assimalign.Cohesion.Web` (the pipeline seams) and, through it, `Assimalign.Cohesion.Http` (the
QUERY/media-type/conditional primitives). No DI, no configuration binding, no reference to
`Web.Hosting` — the package follows the Web-area dependency rule.

## Usage

See the [source-backed usage examples](examples/index.md).

Design rationale (why middleware and not a transport interceptor, snapshot semantics, status
choices, non-goals) lives in [DESIGN.md](design.md) .

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Query/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Query/src/Assimalign.Cohesion.Web.Query.csproj`.
