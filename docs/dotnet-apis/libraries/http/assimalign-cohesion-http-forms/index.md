# Assimalign.Cohesion.Http.Forms

Streaming parsers and collections for `application/x-www-form-urlencoded`
and `multipart/form-data`, layered on the core Http contracts. Form feature
state lives on `IHttpContext.Features`. Read it with
`await context.ReadFormAsync(cancellationToken)`; a parsed or pre-attached
form is available through `request.Form`. Web.Forms provides the pipeline
middleware that parses eagerly and maps parsing failures to HTTP responses.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Repeated fields

For `tag=first&tag=&tag=last`, or three multipart scalar parts named `tag`,
`form["tag"]` contains `"first"`, `""`, and `"last"` in that order.
Enumerate it or use its integer indexer and `Count` for every occurrence.
`form["tag"].Value` reads `"first"`. A comma in one value is not a separator.
Collection `Count` counts distinct scalar keys, and `Files` is separate.
`HttpFormCollection.Add` appends all supplied values to the named field.

Both encodings now follow the same first-scalar and multi-value rules
(decision 31). This changes preview.1's multipart last-value and urlencoded
comma-joined scalar behavior. See [Design](design.md) for compatibility,
consumer effects, parsing limits, uploaded file ownership, and AOT posture.

## Form limits

Set `HttpFormOptions` when constructing `HttpFormFeature`. `ValueCountLimit`
defaults to 1024 and limits urlencoded values or multipart sections. Every
multipart section counts, including files, repeated fields, and unnamed
sections. The first excess section is rejected before its body is consumed.
This multipart enforcement changes preview.1 behavior, which applied that
limit only to urlencoded forms; raise the option for intentionally larger forms.

Continue catching `InvalidDataException` for parsing failures. A limit failure
has an inner `HttpFormLimitExceededException`; its read-only `Limit` property
returns the `HttpFormLimit` category without request input or message parsing.
Use that category for diagnostics. Malformed form messages can contain request
input and should not be logged as a limit name. The existing message-only
exception constructor reports `Unspecified`; parsers always report a specific
category. See [the error model](design.md#error-model) for every category and its
corresponding option.

Transport integration tests consume duplex byte pipes through `IDuplexConnection` and HTTP
connections through `IHttpConnection`. Their listeners use `AcceptAsync`; HTTP/3 stream fixtures
use `IMultiplexedConnectionStream`. These contract names do not alter this package's
request/response behavior.

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
| `HttpFormLimit` | `src/HttpFormLimit.cs` |
| `HttpFormLimitExceededException` | `src/Exceptions/HttpFormLimitExceededException.cs` |
| `HttpFormOptions` | `src/HttpFormOptions.cs` |
| `IHttpFormCollection` | `src/Abstractions/IHttpFormCollection.cs` |
| `IHttpFormFeature` | `src/Abstractions/IHttpFormFeature.cs` |
| `IHttpFormFile` | `src/Abstractions/IHttpFormFile.cs` |
| `IHttpFormFileCollection` | `src/Abstractions/IHttpFormFileCollection.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/docs/OVERVIEW.md`.

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

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/HttpFormLimit.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/Exceptions/HttpFormLimitExceededException.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/HttpFormOptions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/Abstractions/IHttpFormCollection.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/Abstractions/IHttpFormFeature.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/Abstractions/IHttpFormFile.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/Abstractions/IHttpFormFileCollection.cs`.
