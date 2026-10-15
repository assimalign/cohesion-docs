# Assimalign.Cohesion.Http.Antiforgery

`Assimalign.Cohesion.Http.Antiforgery` creates and validates signed cookie and
request-token pairs for cross-site request forgery protection. The public sealed
`HttpAntiforgery` implements `IHttpAntiforgery`. Create it with either
`HttpAntiforgery.Create(options)` or `HttpAntiforgery.Create(configure)` and share
it across the application's requests; its constructor is private. Both overloads
return `HttpAntiforgery`, which can be used through `IHttpAntiforgery`.
`GetAndStoreTokens(context)` issues the pair; `IsRequestValidAsync(context)`
checks a submitted pair.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

The package reads the cookie through Http.Cookies and the request token from
the already-parsed Http.Forms collection. The first occurrence of the
configured form-token field is used. If it is absent or empty, the configured
request header supplies the token. An invalid nonempty first form token fails
validation; later form values are not tried. Form parsing must happen before
validation when using the form-token path.

This changes repeated-field behavior from 10.0.0-preview.1: multipart formerly
selected the last value, and urlencoded forms comma-joined the values. Both
encodings now select the first token.

See [Design](design.md) for token protection, dependency direction, and Web
pipeline integration.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Http.Cookies`](../../http/assimalign-cohesion-http-cookies/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Http.Forms`](../../http/assimalign-cohesion-http-forms/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Http.Forwarded`](../../http/assimalign-cohesion-http-forwarded/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `AntiforgeryValidationException` | `src/Exceptions/AntiforgeryValidationException.cs` |
| `HttpAntiforgery` | `src/HttpAntiforgery.cs` |
| `HttpAntiforgeryOptions` | `src/HttpAntiforgeryOptions.cs` |
| `HttpAntiforgeryTokenSet` | `src/HttpAntiforgeryTokenSet.cs` |
| `HttpContextAntiforgeryExtensions` | `src/Extensions/HttpContextAntiforgeryExtensions.cs` |
| `IHttpAntiforgery` | `src/Abstractions/IHttpAntiforgery.cs` |
| `IHttpAntiforgeryFeature` | `src/Abstractions/IHttpAntiforgeryFeature.cs` |
| `IHttpAntiforgeryProtector` | `src/Abstractions/IHttpAntiforgeryProtector.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src/Assimalign.Cohesion.Http.Antiforgery.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/README.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src/Exceptions/AntiforgeryValidationException.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src/HttpAntiforgery.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src/HttpAntiforgeryOptions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src/HttpAntiforgeryTokenSet.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src/Extensions/HttpContextAntiforgeryExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src/Abstractions/IHttpAntiforgery.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src/Abstractions/IHttpAntiforgeryFeature.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src/Abstractions/IHttpAntiforgeryProtector.cs`.
