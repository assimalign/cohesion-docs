# Assimalign.Cohesion.Http.Antiforgery

Protects HTTP requests with paired antiforgery cookie and request tokens.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

The signed double-submit model protects both halves so an injected cookie alone cannot authenticate
a request token. Antiforgery composes protocol, cookie, and form contracts without adding these
concerns to the HTTP root. Validation is an explicit server-side operation.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Http.Cookies`](../../http/assimalign-cohesion-http-cookies/index.md) | `CohesionProjectReference` |
| [`Assimalign.Cohesion.Http.Forms`](../../http/assimalign-cohesion-http-forms/index.md) | `CohesionProjectReference` |

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

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src/Assimalign.Cohesion.Http.Antiforgery.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/docs/DESIGN.md`.

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
