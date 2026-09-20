# Assimalign.Cohesion.Web.Serialization

The content-serialization registry for the Cohesion Web pipeline: how request and response bodies are formatted.

> **Status:** Partial.

## Reference pages

- **[Design](design.md)** — Dependency boundaries and implementation decisions.
- **[Examples](examples/index.md)** — Source-backed usage and expected behavior.

The content-serialization registry for the Cohesion Web pipeline: how request and response bodies
are formatted. Applications register serialization once at builder time, keyed by media type, and
middleware/handlers read and write typed bodies with no per-call-site serializer ceremony —
reflection-free under NativeAOT.

## What it provides

- **The registry** — `IHttpContentSerializationFeature`, a typed feature on every exchange with
  distinct request-deserialization (`IHttpContentReader`) and response-serialization
  (`IHttpContentWriter`) halves, keyed by `HttpMediaType`.
- **Builder-time registration** — `AddJsonSerialization(AppJsonContext.Default)` registers the
  built-in JSON pair over a source-generated `IJsonTypeInfoResolver`;
  `AddContentSerialization()` + `ContentSerializationBuilder` register custom formats.
- **Typed call sites** — `request.ReadContentAsync<T>()` and
  `response.WriteContentAsync(value)` extensions that dispatch through the registry.
- **Content negotiation** — `context.WriteNegotiatedContentAsync(value)` selects the response
  format from the request's `Accept` header (over the same registry, reusing the #771 negotiation
  primitive), stamps `Vary: Accept`, and composes a bodyless `406` when nothing is acceptable;
  `feature.TryNegotiate(acceptHeader, out mediaType)` is the underlying non-throwing seam.

## Usage

See the [source-backed usage examples](examples/index.md).

## Scope boundaries

- **Content negotiation** (selecting the response format from `Accept`) is delivered here as a
  thin layer *over* the registry (#149), reusing the shared `HttpContentNegotiation` primitive; it
  adds no matching rules of its own beyond a narrow structured-suffix fallback. `Accept-Charset` /
  `Accept-Language` and the client-side half stay out of scope (see DESIGN non-goals).
- **Source-generated binding and validation** (#796) consume the registry for body IO;
  validation sits between deserialization and the handler, outside this package.
- **Error surfacing** is the `Web.ErrorHandling` `OnError` hook's scope; this package only
  *throws* well-defined faults (`HttpContentSerializationException`) for it to handle.

Design rationale lives in [DESIGN.md](design.md) .

## Project references

| Reference | Kind |
|---|---|
| `Assimalign.Cohesion.Http` | `CohesionProjectReference` |
| `Assimalign.Cohesion.Web` | `CohesionProjectReference` |

[Parent: Web](../index.md)

## Sources

- **Primary source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Serialization/docs/OVERVIEW.md`.
- **Source** — `cohesion/resources/Web/Assimalign.Cohesion.Web.Serialization/src/Assimalign.Cohesion.Web.Serialization.csproj`.
