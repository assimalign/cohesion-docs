# Assimalign.Cohesion.Web.Cors

Cross-origin resource sharing (CORS) for the Cohesion Web pipeline, following the Fetch standard's CORS protocol.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

Cross-origin resource sharing (CORS) for the Cohesion Web pipeline, following the Fetch standard's
CORS protocol. `UseCors` answers CORS preflights and adds the CORS headers to the responses its
policy governs, so a browser lets an allowed origin's script read them. Policies are immutable,
validated when they are built, and selected per endpoint from routing metadata.

## Scope

- **A policy model** (`CorsPolicy`, built with `CorsPolicyBuilder`): allowed origins (an exact list,
  a predicate, or any origin), methods, request headers, exposed response headers, credentials, and
  the preflight max age. Configured origins must be origins (`scheme://host[:port]`) and are
  normalized to the form a browser sends; any origin cannot be combined with credentials.
- **The middleware** (`UseCors`): a preflight is answered `204` with the policy's grant (origin,
  credentials, methods, headers, max age) or with no CORS header at all when the policy denies it,
  and never reaches the endpoint. An actual request gets the allowed origin (echoed, or `*`),
  credentials and exposed headers, plus `Vary: Origin` whenever the answer depends on the origin.
- **Fetch matching**: methods compare byte for byte, with the CORS-safelisted `GET`, `HEAD` and
  `POST` always allowed; request-header names compare case-insensitively.
- **Per-endpoint policy selection**: a named policy, an inline policy, or `DisableCors`, attached
  with convention verbs to a route or a group and read last-wins from the endpoint `UseRouting`
  publishes; the default policy covers everything else.

## Dependencies

- **`Assimalign.Cohesion.Web`** — the pipeline builder and middleware abstractions.
- **`Assimalign.Cohesion.Web.Routing`** — the published endpoint and its metadata, the preflight's
  candidate endpoint, the endpoint convention builders, and the acknowledgement routing checks
  before it dispatches.
- **`Assimalign.Cohesion.Http`** — the HTTP context, header keys and methods.
- **`Assimalign.Cohesion.Http.Streaming`** — the response-streaming feature that tells the
  middleware whether the response head has been sent.

It never references `Assimalign.Cohesion.Web.Hosting` or any `Assimalign.Cohesion.Hosting*` library
(`COHRES001`, `COHRES004`).

## Usage

See the [source-backed usage examples](examples/index.md).

Register `UseCors` after `UseRouting`, so the endpoint's policy is known, and ahead of any
middleware that can reject a preflight. An endpoint that declares CORS fails with
`InvalidOperationException` when it is dispatched without `UseCors` having processed it (missing, or
registered ahead of `UseRouting`), instead of running under the wrong policy. A preflight for a
method no route serves is answered by the default policy, or left to routing's `405` when there is
none.

See [`DESIGN.md`](design.md) for the policy model and its validation, policy selection, the
preflight's policy source, the `Vary: Origin` and cache rules, the fail-closed decision, ordering,
and the non-goals.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web.Routing` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Http.Streaming` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Cors/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Cors/src/Assimalign.Cohesion.Web.Cors.csproj`.
