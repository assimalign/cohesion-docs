# Assimalign.Cohesion.Http

Defines Hypertext Transfer Protocol (HTTP) messages, fields, features, and exchange contracts.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

Headers and trailers are distinct ordered field sections using compatible collection primitives.
Optional concerns attach through feature and interceptor seams rather than widening the protocol
root. The core remains independent of hosting and resource platforms.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `HttpAcceptParser` | `src/HttpAcceptParser.cs` |
| `HttpAcceptQuery` | `src/HttpAcceptQuery.cs` |
| `HttpAltService` | `src/HttpAltService.cs` |
| `HttpCacheControl` | `src/HttpCacheControl.cs` |
| `HttpCacheControlExtension` | `src/HttpCacheControlExtension.cs` |
| `HttpConditionalRequest` | `src/HttpConditionalRequest.cs` |
| `HttpConditionalRequestContext` | `src/HttpConditionalRequestContext.cs` |
| `HttpConnectionInfo` | `src/HttpConnectionInfo.cs` |
| `HttpContentNegotiation` | `src/HttpContentNegotiation.cs` |
| `HttpContentRange` | `src/HttpContentRange.cs` |
| `HttpContentTypes` | `src/HttpContentTypes.cs` |
| `HttpContext` | `src/HttpContext.cs` |
| `HttpContextExtensions` | `src/Extensions/HttpContextExtensions.cs` |
| `HttpDate` | `src/HttpDate.cs` |
| `HttpEntityTag` | `src/HttpEntityTag.cs` |
| `HttpEntityTagCondition` | `src/HttpEntityTagCondition.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Assimalign.Cohesion.Http.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpAcceptParser.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpAcceptQuery.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpAltService.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpCacheControl.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpCacheControlExtension.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpConditionalRequest.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpConditionalRequestContext.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpConnectionInfo.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpContentNegotiation.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpContentRange.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpContentTypes.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpContext.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/Extensions/HttpContextExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpDate.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpEntityTag.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http/src/HttpEntityTagCondition.cs`.
