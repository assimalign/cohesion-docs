# Assimalign.Cohesion.Http.Forms

Parses URL-encoded and multipart form bodies into typed collections.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

Form parsing is an opt-in application concern above the raw protocol body. The feature owns parsing,
limits, and temporary-file spill behavior. Keeping these operations out of the protocol core avoids
imposing form lifecycle costs on clients and proxies.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `HttpContextFormExtensions` | `src/Extensions/HttpContextFormExtensions.cs` |
| `HttpFormCollection` | `src/HttpFormCollection.cs` |
| `HttpFormFeature` | `src/HttpFormFeature.cs` |
| `HttpFormFile` | `src/HttpFormFile.cs` |
| `HttpFormFileCollection` | `src/HttpFormFileCollection.cs` |
| `HttpFormOptions` | `src/HttpFormOptions.cs` |
| `IHttpFormCollection` | `src/Abstractions/IHttpFormCollection.cs` |
| `IHttpFormFeature` | `src/Abstractions/IHttpFormFeature.cs` |
| `IHttpFormFile` | `src/Abstractions/IHttpFormFile.cs` |
| `IHttpFormFileCollection` | `src/Abstractions/IHttpFormFileCollection.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/Assimalign.Cohesion.Http.Forms.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/Extensions/HttpContextFormExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/HttpFormCollection.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/HttpFormFeature.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/HttpFormFile.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/HttpFormFileCollection.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/HttpFormOptions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/Abstractions/IHttpFormCollection.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/Abstractions/IHttpFormFeature.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/Abstractions/IHttpFormFile.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/Abstractions/IHttpFormFileCollection.cs`.
