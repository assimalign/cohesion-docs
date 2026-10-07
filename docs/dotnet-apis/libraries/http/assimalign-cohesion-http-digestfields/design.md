# Assimalign.Cohesion.Http.DigestFields design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.DigestFields`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Digest values use structured fields and support SHA-256 and SHA-512 computation. The verifier
rejects malformed fields before dispatch; HTTP/2 and HTTP/3 body verification is lazy and reports
mismatches on terminal reads. Recognized deprecated algorithms are not enabled for computation.

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
