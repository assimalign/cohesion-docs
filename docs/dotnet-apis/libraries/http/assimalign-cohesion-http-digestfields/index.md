# Assimalign.Cohesion.Http.DigestFields

Parses and verifies HTTP integrity digest fields.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Http](../index.md)

## Scope

Digest values use structured fields and support SHA-256 and SHA-512 computation. The verifier
rejects malformed fields before dispatch; HTTP/2 body verification is lazy and reports mismatches on
terminal reads. Recognized deprecated algorithms are not enabled for computation.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Http`](../../http/assimalign-cohesion-http/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `HttpContentDigester` | `src/HttpContentDigester.cs` |
| `HttpContentDigestMismatchException` | `src/HttpContentDigestMismatchException.cs` |
| `HttpDigestAlgorithm` | `src/HttpDigestAlgorithm.cs` |
| `HttpDigestField` | `src/HttpDigestField.cs` |
| `HttpDigestFields` | `src/HttpDigestFields.cs` |
| `HttpDigestVerificationResult` | `src/HttpDigestVerificationResult.cs` |
| `HttpWantDigestField` | `src/HttpWantDigestField.cs` |
| `HttpDigestEntry` | `src/HttpDigestEntry.cs` |
| `HttpDigestException` | `src/HttpDigestException.cs` |
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

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/HttpContentDigestMismatchException.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/HttpDigestAlgorithm.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/HttpDigestField.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/HttpDigestFields.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/HttpDigestVerificationResult.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/HttpWantDigestField.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/HttpDigestEntry.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/HttpDigestException.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/Extensions/HttpDigestFieldsExtensions.cs`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.DigestFields/src/HttpWantDigestPreference.cs`.
