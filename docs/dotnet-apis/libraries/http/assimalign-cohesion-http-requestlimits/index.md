# Assimalign.Cohesion.Http.RequestLimits

Exposes a per-request view of the maximum request body size.

> **Status:** Partial.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

The feature is attached on all three protocol parse paths and writes through to the transport's
limit state. It becomes read-only when body reading starts. The documented wire-level enforcement
remains HTTP/1.1-only; feature presence on HTTP/2 or HTTP/3 does not imply enforcement.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `HttpRequestLimits` | `src/HttpRequestLimits.cs` |
| `IHttpMaxRequestBodySizeFeature` | `src/IHttpMaxRequestBodySizeFeature.cs` |
| `HttpRequestLimitsExtensions` | `src/Extensions/HttpRequestLimitsExtensions.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/src/Assimalign.Cohesion.Http.RequestLimits.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/src/HttpRequestLimits.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/src/IHttpMaxRequestBodySizeFeature.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.RequestLimits/src/Extensions/HttpRequestLimitsExtensions.cs`.
