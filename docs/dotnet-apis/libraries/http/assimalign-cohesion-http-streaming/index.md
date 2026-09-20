# Assimalign.Cohesion.Http.Streaming

Writes HTTP response bodies incrementally through an optional feature.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

The host registers an exchange interceptor to attach the streaming API. Starting a response,
writing, flushing, and completing are explicit lifecycle steps. The feature depends on core response
seams rather than the concrete HTTP transport assembly.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `HttpResponseStreaming` | `src/HttpResponseStreaming.cs` |
| `IHttpResponseStreamingFeature` | `src/IHttpResponseStreamingFeature.cs` |
| `HttpResponseStreamingExtensions` | `src/Extensions/HttpResponseStreamingExtensions.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Streaming/src/Assimalign.Cohesion.Http.Streaming.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Streaming/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Streaming/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Streaming/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Streaming/src/HttpResponseStreaming.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Streaming/src/IHttpResponseStreamingFeature.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Streaming/src/Extensions/HttpResponseStreamingExtensions.cs`.
