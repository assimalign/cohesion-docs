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

Every parse failure surfaces as `System.IO.InvalidDataException`. When the body exceeds one of the
configured `HttpFormOptions` limits, that exception's `InnerException` is an
`HttpFormLimitExceededException` carrying the same message; a malformed body has no inner
exception. A caller answering the request tells the two apart by type: a body over a limit is
`413 Content Too Large`, and a malformed one `400 Bad Request`. The source-generated typed-endpoint
binding, `UseAntiforgery`, and `UseForms()` (`Assimalign.Cohesion.Web.Forms`) answer that way;
`UseForms()` answers as problem+json, and the rest of the pipeline does not run (#1210).

`HttpFormFileCollection` keeps every uploaded file part, in arrival order. The files of a
multiple-file field (`<input type="file" multiple>`) arrive as separate parts with the same name
(RFC 7578 §4.3), so enumerating the collection yields them all, and `TryGetValue(name, ...)` returns
the first with that name (names compare case-insensitively). A part is a file part only when its
`filename` is non-empty: a browser sends an optional `<input type="file">` left empty as
`filename=""` with no content, and that part is read as an empty value. See the
[design](design.md#an-empty-file-input-is-a-value).

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
| `HttpFormLimitExceededException` | `src/Exceptions/HttpFormLimitExceededException.cs` |
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

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/Exceptions/HttpFormLimitExceededException.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/HttpFormOptions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/Abstractions/IHttpFormCollection.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/Abstractions/IHttpFormFeature.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/Abstractions/IHttpFormFile.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/Abstractions/IHttpFormFileCollection.cs`.
