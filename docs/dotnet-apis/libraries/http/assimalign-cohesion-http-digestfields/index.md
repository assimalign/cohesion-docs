# Assimalign.Cohesion.Http.DigestFields

Parses and verifies HTTP integrity digest fields.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

RFC 9530 integrity digest fields for the Cohesion HTTP stack: `Content-Digest`, `Repr-Digest`,
`Want-Content-Digest`, and `Want-Repr-Digest`. Provides a structured-fields value model, `sha-256`
/ `sha-512` computation and verification over BCL incremental hashing, and an opt-in server-side
request verifier.

- **Depends on:** `Assimalign.Cohesion.Http` (core) only — for the four `HttpHeaderKey` constants,
  the `StructuredFieldDictionary` toolkit the field values are built on, and the
  `IHttpExchangeInterceptor` request-parse seam. The server transport
  (`Assimalign.Cohesion.Http.Connections`) never references this package.
- **Value model:** `HttpDigestField` (a `Content-Digest` / `Repr-Digest` value — an ordered map of
  algorithm → digest bytes) and `HttpWantDigestField` (a `Want-*` value — algorithm → integer
  preference). Both offer span-based `TryParse`/`Serialize` and are AOT-safe.
- **Algorithms:** `HttpDigestAlgorithm` — `sha-256` and `sha-512` are supported for compute and
  verify; the deprecated registry entries (`md5`, `sha`, `unixsum`, `unixcksum`) are recognized on
  parse but never used (RFC 9530 §5).
- **Server verification:** `HttpContentDigestInterceptor.Create()` returns an
  `IHttpExchangeInterceptor` the composition root registers on its listener; it verifies an inbound
  `Content-Digest` against the request body. On HTTP/1.1 verification is eager and a mismatch is
  rejected with `400 Bad Request` before dispatch; on HTTP/2 and HTTP/3 (whose bodies stream in
  after dispatch) it is lazy — the body is hashed incrementally as the application reads, and a
  mismatch surfaces as `HttpContentDigestMismatchException` on the terminal body read, after which
  the application aborts the exchange (`IHttpContext.Cancel` → `RST_STREAM` on HTTP/2, a stream
  reset on HTTP/3). A malformed field is a pre-dispatch `400` on every protocol.
- **Response stamping:** `IHttpResponse.SetContentDigest(...)` computes and stamps `Content-Digest`,
  honoring the request's `Want-Content-Digest` preference ordering; `HttpContentDigester` is the
  incremental "hash as you write" primitive for the trailer-borne streamed case: its field can be
  staged on `Response.Trailers`, which HTTP/2 and HTTP/3 send (HTTP/1.1 has no response trailers).

## Usage

```csharp
// Server: verify inbound Content-Digest (register before any content-decoding interceptor).
listenerOptions.Interceptors.Add(HttpContentDigestInterceptor.Create());

// Response: stamp Content-Digest, honoring the client's Want-Content-Digest.
context.Response.SetContentDigest(body);

// Value model: parse and verify directly.
if (HttpDigestField.TryParse(headerValue, out var field) &&
    field.VerifyContent(content) == HttpDigestVerificationResult.Matched) { /* trusted */ }
```

See [design](design.md) for the placement rationale (consumer of the structured-fields toolkit), the
two verification modes (eager buffer-and-replay vs. lazy verify-on-read) and the mid-stream
rejection semantics, and the Content-Digest vs Repr-Digest scope.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `HttpContentDigester` | `src/HttpContentDigester.cs` |
| `HttpContentDigestMismatchException` | `src/Exceptions/HttpContentDigestMismatchException.cs` |
| `HttpDigestAlgorithm` | `src/ValueObjects/HttpDigestAlgorithm.cs` |
| `HttpDigestField` | `src/HttpDigestField.cs` |
| `HttpContentDigestInterceptor` | `src/HttpContentDigestInterceptor.cs` |
| `HttpDigestVerificationResult` | `src/HttpDigestVerificationResult.cs` |
| `HttpWantDigestField` | `src/HttpWantDigestField.cs` |
| `HttpDigestEntry` | `src/HttpDigestEntry.cs` |
| `HttpDigestException` | `src/Exceptions/HttpDigestException.cs` |
| `HttpDigestFieldsExtensions` | `src/Extensions/HttpDigestFieldsExtensions.cs` |
| `HttpWantDigestPreference` | `src/HttpWantDigestPreference.cs` |

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/Assimalign.Cohesion.Http.DigestFields.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/HttpContentDigester.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/Exceptions/HttpContentDigestMismatchException.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/ValueObjects/HttpDigestAlgorithm.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/HttpDigestField.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/HttpContentDigestInterceptor.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/HttpDigestVerificationResult.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/HttpWantDigestField.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/HttpDigestEntry.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/Exceptions/HttpDigestException.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/Extensions/HttpDigestFieldsExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/HttpWantDigestPreference.cs`.
