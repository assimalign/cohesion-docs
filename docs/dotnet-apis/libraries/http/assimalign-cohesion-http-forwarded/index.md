# Assimalign.Cohesion.Http.Forwarded

Defines effective request identity after a trusted proxy has forwarded an exchange.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

The feature records effective values, original wire values, and trusted hop count. Extension members
read the feature first and fall back to wire values. Trust policy and header interpretation belong
to producers of the feature, not to consumers repeating their own parsing.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `HttpContextForwardedExtensions` | `src/Extensions/HttpContextForwardedExtensions.cs` |
| `IHttpForwardedFeature` | `src/Abstractions/IHttpForwardedFeature.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forwarded/src/Assimalign.Cohesion.Http.Forwarded.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forwarded/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forwarded/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forwarded/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forwarded/src/Extensions/HttpContextForwardedExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forwarded/src/Abstractions/IHttpForwardedFeature.cs`.
