# Assimalign.Cohesion.Http.DigestFields design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.DigestFields`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Digest values use structured fields and support SHA-256 and SHA-512 computation. The verifier
rejects malformed fields before dispatch; HTTP/2 and HTTP/3 body verification is lazy and reports
mismatches on terminal reads. Recognized deprecated algorithms are not enabled for computation.

## Server-side verification (the interceptor)

`HttpContentDigestInterceptor.Create()` returns a stateless `IHttpExchangeInterceptor` — the
#818 request-parse seam is the natural hook for request-digest validation. It hooks
`AfterRequestBody`:

1. No `Content-Digest`, or only deprecated/unregistered algorithms → pass the body through
   untouched (nothing this library can verify).
2. A malformed `Content-Digest` → **fail closed**: throw `HttpRequestRejectedException(400)`. RFC
   9530 §2 lets a recipient either ignore or reject; an *operator-installed verifier* should not
   silently accept a field it cannot parse. Parsing is CPU-only, so this pre-dispatch rejection
   holds on **every** protocol.
3. Otherwise verify — in one of two modes, chosen by `context.Version`.

### Two verification modes, chosen by where the verdict is affordable

**Eager buffer-and-replay (HTTP/1.1).** The hook reads the body in full, verifies against every
supported digest with constant-time comparison (`CryptographicOperations.FixedTimeEquals`), and
either rejects a mismatch with `HttpRequestRejectedException(400)` or returns a replay stream
(`HttpDigestReplayStream`) so the application still observes the body. The rejection happens
**before the application runs**, so the transport answers a real, deterministic `400` with no
application involvement. The in-hook full read is free there: the h1 parse path is its own body
reader — the post-#810 streamed body is pulled from the wire by the same logical flow that runs
the hook, the peer needs nothing from the server to keep sending (the `Expect: 100-continue`
solicitation is emitted by the body stream's own first read), and the minimum request-body data
rate bounds how long a slow sender can hold the read. The cost is one transient extra copy of the
body, bounded by the request-limits cap.

HTTP/3 was eager until #1066. Its transport used to drain each request stream to FIN before
decoding the header section, so the hook was handed a fully received body. Since #1066 the h3
body is read lazily off the QUIC stream after the request's HEADERS frame, and HTTP/3 has no
minimum data rate. An in-hook full read would hold a thread-pool thread for as long as the peer
takes to send, so a slow sender could exhaust the pool. HTTP/3 therefore moved to the lazy path.

**Lazy verify-on-read (HTTP/2, HTTP/3, and, defensively, any version not proven eager-safe).** On
h2 the parse hooks run on the connection's **single frame pump** and the body is a live
`Http2RequestBodyStream` that may still be arriving under flow-control backpressure (#750): an
in-hook read would wait for DATA frames only the blocked pump can deliver, deadlocking every
multiplexed stream on the connection. On h3 the body is a live `Http3RequestBodyStream` reading
off the QUIC stream (above). So the hook stays CPU-only — it wraps the body in
`HttpDigestVerifyingStream` and reads nothing. The wrapper feeds every octet the application reads
into one BCL `IncrementalHash` per supported digest entry (zero double-buffering; the only copy of
the body is the flow-control-bounded pipe the transport already owns) and resolves the verdict on
the **terminal read** — the read that observes end-of-body:

- **Match** → the terminal read returns `0`; the application saw a normal body end-to-end.
- **Mismatch** → the terminal read throws `HttpContentDigestMismatchException` (naming the first
  failing algorithm in field order), and every subsequent read rethrows it — the failure is
  sticky, so a consumer that swallows it once cannot go on treating the stream as verified.

The mode switch is deliberately an allow-list (`Http11` eager, everything else lazy):
eager is an *optimization* that must be proven safe per protocol, while lazy is safe everywhere
because the hook performs no I/O. An unknown future version therefore degrades to correctness,
not to a deadlock.

### Mid-stream rejection semantics (the lazy path's contract)

Once the application has begun consuming a streamed body, a digest mismatch can no longer become
a clean pre-dispatch `400` — the content was already observed and the exchange is running. The
contract, per the seam's layering rule (interceptors are wiring; aborting is an application-layer
act):

- The typed failure surfaces **on the terminal body read** (`HttpContentDigestMismatchException`,
  `HttpErrorCode.ReadingError`), never earlier — the verdict does not exist until every content
  octet has been hashed.
- The application (or the hosting layer that installed the verifier) must treat the body as
  corrupt, discard anything derived from it, and abort the exchange via `IHttpContext.Cancel`.
  The transport answers the abort with its per-exchange reset — on h2, `RST_STREAM(CANCEL)` from
  the exchange abort path; on h3, a request-stream reset with `H3_REQUEST_CANCELLED` — instead of
  writing a response. There is deliberately no
  transport-automatic abort: the seam has no abort verb, and the application may prefer to answer
  (e.g. `422`) rather than reset.
- **A body the application never drains is never verified.** Lazy verification can only cover
  what is consumed; a handler that responds without reading to end-of-body has waived
  verification for the unread remainder. Operators who need an unconditional verdict must drain
  the body (h2, h3) or rely on the eager protocol (h1).

Both wrapper streams own the body they replace and dispose it when the exchange disposes the
stream chain, honoring the seam's "a wrapper owns the stream it wraps" contract
(`HttpDigestVerifyingStream` also disposes its running hashes at verdict or disposal, whichever
comes first).

One ordering note applies to both modes: **register the verifier first**, before any
content-decoding interceptor. `Content-Digest` is taken over the message content *as received*
(post-content-coding, on the wire), so it must be verified against the raw body before a
decompression wrapper transforms it.

## Response stamping and Want-* honoring

`HttpDigestFieldsExtensions` adds the response-side surface:

- `IHttpResponse.SetContentDigest(content, algorithm)` / `SetContentDigest(field)` stamp an explicit
  or precomputed digest.
- `IHttpResponse.SetContentDigest(content)` honors the request's `Want-Content-Digest`:
  `HttpWantDigestField.TrySelectPreferred` picks the supported algorithm the client most prefers
  (highest integer preference, ties broken toward the stronger algorithm, `0`/deprecated skipped),
  falling back to `sha-256` when the request expressed no usable preference.
- `IHttpRequest.TryGetContentDigest` / `TryGetWantContentDigest` read the parsed request fields for
  application code that wants them directly.

## Content-Digest vs Repr-Digest scope

`Content-Digest` is implemented end-to-end: header key, value model, computation, and server-side
verification. `Repr-Digest` is **modeled** — the header key exists and `HttpDigestField` parses,
serializes, and computes it identically — but the verifier does not enforce it, because
representation-data reconstruction depends on content-coding and range layers not yet present.
When those land, `Repr-Digest` verification is a drop-in on the same value model.

## Digests as trailers

For a streamed body the digest is not known until the body is written, so it belongs in the trailer
section. RFC 9530 permits digest fields as trailers, and `HttpFieldRules.IsProhibitedInTrailers`
does not list them (a guard test locks this). `HttpContentDigester` is the incremental "hash as you
write" primitive for that case. The transports send response trailers on HTTP/2 and HTTP/3 (decision
18, the Http area's ADR 2): a caller stages `ToField().Serialize()` under `Content-Digest` on
`Response.Trailers` before the response completes, after checking `Trailers.IsSupported`. On HTTP/1.1
the collection is unsupported, so the digest has to go in the header section instead.

`SetContentDigest` stamps the header section only. Sending the digest as a trailer needs no package
support any more; a helper that hashes a streamed body and stages the trailer itself is a possible
follow-up.

## Public interceptor and breaking changes since 10.0.0-preview.1 (#1465)

`HttpContentDigestInterceptor` is the public sealed implementation of the existing
`IHttpExchangeInterceptor` contract through `HttpExchangeInterceptor`. Its constructor is private;
`HttpContentDigestInterceptor.Create()` creates a new stateless instance and returns its concrete type.
The interface remains the contract for listener registration and custom interceptors. Feature
implementations remain internal. This is the factory-to-implementation convention approved by
owner decision 59.

The `v10.0.0-preview.1` tag shipped `HttpDigestFields.CreateContentDigestVerifier()` returning
`IHttpExchangeInterceptor`. That static factory class is removed. Consumers must replace the call
with `HttpContentDigestInterceptor.Create()` and rebuild. Removing the old type and method is a
**source- and binary-breaking change**, accepted during previews on decision 15's terms.
Assignments to `IHttpExchangeInterceptor` continue to work after the call is renamed. The
interceptor's registration order, scopes, feature installation, and request/response behavior
are unchanged.

## Exception message resources (#1467)

Resource keys name the failure with a short PascalCase noun phrase, at most 40 characters
(#1471). These internal names are independent of the preserved message text and placeholders.

Exception diagnostics live in `src/Properties/ErrorMessages.resx`. The shared build targets
use MSBuild strongly typed resource generation to produce an internal `ErrorMessages` class
in this assembly's `RootNamespace`, under the intermediate output directory. No designer source
is checked in, and a command-line or NativeAOT build uses the same embedded resources.
Formatted diagnostics use `string.Format(CultureInfo.CurrentCulture, ErrorMessages.<Name>, ...)`.
The invariant messages, exception types, error codes and existing input-redaction rules stay
unchanged; this introduces no public API or breaking behavior against 10.0.0-preview.1.
EventSource templates, log messages and HTTP response bodies keep their existing owners.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Http`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/Assimalign.Cohesion.Http.DigestFields.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src`.
