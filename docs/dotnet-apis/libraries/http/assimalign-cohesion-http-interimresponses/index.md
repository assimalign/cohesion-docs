# Assimalign.Cohesion.Http.InterimResponses

Sends interim HTTP responses through the exchange interceptor seam.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

A host registers the interceptor to attach the feature to exchanges. Interim responses precede the
final response and use the core response seam, keeping the transport independent of this package.
Convenience members cover Continue and Early Hints.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `HttpInterimResponses` | `src/HttpInterimResponses.cs` |
| `IHttpInterimResponseFeature` | `src/IHttpInterimResponseFeature.cs` |
| `HttpInterimResponseExtensions` | `src/Extensions/HttpInterimResponseExtensions.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/src/Assimalign.Cohesion.Http.InterimResponses.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/src/HttpInterimResponses.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/src/IHttpInterimResponseFeature.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.InterimResponses/src/Extensions/HttpInterimResponseExtensions.cs`.
