# Assimalign.Cohesion.Http.Forms design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.Forms`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Form parsing is an opt-in application concern above the raw protocol body. The feature owns parsing,
limits, and temporary-file spill behavior. Keeping these operations out of the protocol core avoids
imposing form lifecycle costs on clients and proxies.

## Multiple files under one name

`HttpFormFileCollection` keeps every file part, in arrival order. RFC 7578 §4.3 sends the files of a
multiple-file field (`<input type="file" multiple>`) as separate parts with the same `name`, so
names are not unique: enumerating the collection yields them all, and `TryGetValue(name, ...)`
returns the first with that name (names compare case-insensitively). Before #1061 the collection was
keyed by name and each part replaced the previous one, so only the last file of such a field
survived. Repeated scalar values are unaffected.

## An empty file input is a value

The multipart parser reads value parts as text, and file parts flow through `ReadFileSectionAsync`.
A part is a file part only when its `filename` is non-empty, ASP.NET Core's `IsFileDisposition`
rule: a browser sends an optional `<input type="file">` left empty as `filename=""` with no content,
and that part is read as an empty value. Before the #1210 review it reached the `HttpFormFile`
constructor, whose `ArgumentException` no caller catches, so the ordinary submission failed as a
`500`.

## Error model

Every parse failure surfaces as `System.IO.InvalidDataException` mid-parse — the same type the
underlying readers throw — so callers catch one thing regardless of what went wrong. When the
failure is a limit violation, that exception's `InnerException` is an
`HttpFormLimitExceededException` (an `HttpException` with `Code = ReadingError`) carrying the same
message; a malformed body has no inner exception. That is the one distinction a caller answering the
request needs, made by type rather than by message: a body over a limit is content the server is
unwilling to process, `413 Content Too Large` (RFC 9110 §15.5.14), and a malformed body is a `400`.
The source-generated typed-endpoint binding and `UseAntiforgery` answer exactly that way (#1061).
So does `UseForms()` (`Assimalign.Cohesion.Web.Forms`): a form over a limit is answered `413` and a
malformed one `400` there, as problem+json, and the rest of the pipeline does not run (#1210).
`InvalidDataException` is sealed, so the limit cannot be a subtype of it; the inner exception keeps
the established contract — existing `catch (InvalidDataException)` sites, such as IdentityHub's token
endpoint, are unaffected — while making the cause explicit.

| Limit (`HttpFormOptions`) | Guards against | Thrown by |
|---|---|---|
| `ValueCountLimit` | Too many form entries | `HttpFormReader` |
| `KeyLengthLimit` / `ValueLengthLimit` | Oversized urlencoded key/value | `HttpFormReader` |
| `MultipartBoundaryLengthLimit` | Unbounded boundary look-ahead | `HttpFormFeature` (pre-flight) |
| `MultipartHeadersCountLimit` / `MultipartHeadersLengthLimit` | Header floods per section (and an overlong preamble) | `HttpMultipartFormReader`, `BufferedReadStream` |
| `MultipartBodyLengthLimit` | Oversized section body | `HttpMultipartFormReaderStream` |
| `MemoryBufferThreshold` | Peak memory on large uploads (spills, does not throw) | `HttpFormFeature` |

The headers-length budget counts each line's CRLF, so a header block can end two bytes past the
limit; the reader then still accepts the blank line that closes the block and rejects any further
header line as over the limit. Before #1061 the negative remainder reached the pooled line buffer as a
negative size and surfaced as an `ArgumentOutOfRangeException`.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Http`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src/Assimalign.Cohesion.Http.Forms.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Forms/README.md`.
