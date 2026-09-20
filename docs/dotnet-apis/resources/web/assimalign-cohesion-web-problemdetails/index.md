# Assimalign.Cohesion.Web.ProblemDetails

The RFC 9457 Problem Details for HTTP APIs payload for the Cohesion Web pipeline: a machine- readable error body (`application/problem+json`) with an AOT-safe, zero-reflection writer.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

The RFC 9457 *Problem Details for HTTP APIs* payload for the Cohesion Web pipeline: a machine-
readable error body (`application/problem+json`) with an AOT-safe, zero-reflection writer.

## What it provides

- **`ProblemDetails`** — the RFC 9457 model: `Type`, `Title`, `Status`, `Detail`, `Instance`,
  plus a constrained `Extensions` bag; `ProblemDetails.FromStatus(statusCode, detail)` for the
  common status-derived shape.
- **`ProblemDetailsWriter.Default`** — the single problem+json serializer in the framework
  (`IProblemDetailsWriter`), hand-rolled on `Utf8JsonWriter` so it is NativeAOT- and
  trimming-safe and safe to call from last-chance error handling.
- **`response.WriteProblemDetailsAsync(problem)`** — the imperative write path for middleware
  and error handlers.

## Usage

See the [source-backed usage examples](examples/index.md).

Types surface in the `Assimalign.Cohesion.Web` namespace (documented deviation — see
[DESIGN.md](design.md) ).

## Scope boundaries

This is the payload only. Error *policy* — when a fault becomes a response and in what shape — is
the application's, composed through the `OnError` hook + content-serialization registry design that
issue #864 owns; its default handler renders this payload. Dependencies: `Assimalign.Cohesion.Http`
only.

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Http` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.ProblemDetails/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.ProblemDetails/src/Assimalign.Cohesion.Web.ProblemDetails.csproj`.
