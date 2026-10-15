# Assimalign.Cohesion.IdentityModel.Token.JsonWebToken design

Design decisions and ownership boundaries for `Assimalign.Cohesion.IdentityModel.Token.JsonWebToken`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Compact parsing preserves authoritative header and claim data and rejects duplicate-member
ambiguity. Writing supports ES256, while reusable verifiers handle supported RSA and ECDSA
algorithms. Callers own keys and trust policy; structural validation alone is not signature
verification.

## The cryptographic boundary is format execution versus key management

Design item 25b (`#970`) places the compact-JWS primitives here so headless resources can mint
or verify Cohesion bootstrap credentials without depending on the Web area. This supersedes the
older plan in `#830` that assigned all keyed token cryptography to a future Security package.
The boundary is now:

- **This package executes format-specific BCL primitives.** `JsonWebTokenWriter.CreateEs256`
  returns a public sealed `JsonWebTokenWriter` implementing `IJsonWebTokenWriter` that writes compact JWS
  and calls `ECDsa.SignData` with SHA-256 and the IEEE P1363 fixed-field `r||s` format.
  `EcdsaJsonWebTokenSignatureVerifier.Create` and `RsaJsonWebTokenSignatureVerifier.Create`
  return their public sealed implementations of `IJsonWebTokenSignatureVerifier`.
- **Callers own keys and trust policy.** Writers and verifiers borrow key instances; they never
  generate, persist, rotate, dispose, or publish keys. Public JWK parsing, RFC 7638 thumbprints,
  and the ES256 validator (below) are format execution and live here; JWKS retrieval,
  trusted-issuer storage, and revocation remain outside this package. The validator resolves an
  issuer's keys through a caller-supplied delegate, so the trust store stays with the caller.
- **`JsonWebToken.Validate` remains document-only.** It composes issuer, audience, temporal,
  algorithm, critical-header, and hash rules, but never invokes a signature verifier. A caller
  verifies the exact received `header.payload` and decoded signature before trusting claims.
  A successful `Validate` still means “data and hash rules passed,” not “signature verified.”

HMAC verification remains in `Web.Authentication.Bearer`; item 25b moved only the reusable
asymmetric implementations. The broader Security work in `#830` can still own JWE, SAML
XML-DSig/XML encryption, and key-resolution suites, but not these moved RSA/ECDSA primitives.

## Compact writing

The public sealed `JsonWebTokenWriter` implements `IJsonWebTokenWriter`; its private constructor
is reached through `CreateEs256`, which returns the concrete writer. Creation requires a
NIST P-256 ECDSA key and a non-empty `kid`; the writer owns both `alg=ES256` and `kid`, rejects those
parameters if the descriptor also supplies them, and never mutates the descriptor.

Payloads use the existing `JsonWebTokenDescriptor` model rather than an object-valued or
serializer-specific claim bag:

- `Issuer`, `Subject`, `Audiences`, `ExpiresAt`, `NotBefore`, `IssuedAt`, and `Id` project to
  `iss`, `sub`, `aud`, `exp`, `nbf`, `iat`, and `jti`.
- NumericDate helpers are emitted as integer Unix seconds; one audience is a string and multiple
  audiences are an array, matching RFC 7519.
- Remaining `IIdentityClaim` values are written through the closed `IdentityClaimValue` kind
  switch. Repeated non-singleton claim types become one JSON array, because duplicate JSON object
  members are forbidden. A registered claim supplied through both a typed helper and `Claims` is
  rejected instead of silently choosing precedence.
- Header and payload JSON are written directly with `Utf8JsonWriter`; there is no runtime type
  discovery or reflection-based serialization. The unencoded-payload (`b64:false`) variant is
  rejected.

JSON property order is deterministic for the typed registered members and preserves input order
for remaining descriptor members, but it is not a canonicalization contract. Verification always
uses the exact compact segments received; claim `Canonicalize` is name normalization and is
unrelated to JWS signing.

## Public keys and the ES256 validator

Seven Cohesion call sites — the resource bootstrap verifiers in the Web, IdentityHub, and Scheduler
hosting modules, the LogSpace sink verifier, the SecretStore and ConfigurationStore trusted-issuer
verifiers, and the gateway control plane — once carried their own copy of the same JWK parsing,
thumbprint, ECDSA import, and claim rules. Those copies now configure three shared types:

- **`JsonWebKey`** — a structural parse of one public JWK (`TryParse` from UTF-8 JSON or a
  `JsonElement`). It captures the string-valued `kty`, `crv`, `x`, `y`, `n`, `e`, `kid`, `alg`, and
  `use` members and never captures private members. `ComputeThumbprint` implements RFC 7638 for EC
  (`crv`, `kty`, `x`, `y`) and RSA (`e`, `kty`, `n`) keys; `CreateECDsa` returns a caller-owned
  public ECDSA key for P-256/384/521. Policy is a separate call with two strengths:
  `TryValidateEcdsaVerificationKey(curve)` is the lenient rule the application-trust-key readers
  apply (`kty=EC`, the named `crv`, a string `kid`, and a usable public point; other members,
  `alg`, `use`, and the `kid`/thumbprint relation are not examined), and `TryValidateEs256SigningKey`
  applies the strict profile Cohesion trusted issuers publish (exactly `kty=EC`, `crv=P-256`,
  `alg=ES256`, `use=sig`, `x`, `y`, `kid`; 32-byte coordinates on the curve; `kid` equal to the
  thumbprint). Keeping parse and policy apart is what lets the lenient application-trust-key
  readers and the strict trusted-issuer stores share one type without either changing which keys
  it accepts.
- **`JsonWebKeySet`** — an issuer's keys; `Find(kid)` selects by exact `kid` and never matches a
  missing `kid`, so every accepted token names its key.
- **`IJsonWebTokenValidator`** from `JsonWebTokenValidator.CreateEs256(profile)` — verifies and
  validates in one call. `JsonWebTokenValidationProfile` supplies the issuer-to-key-set resolver,
  the lifetime ceiling, the clock skew (default five minutes), and the subject rule (an exact
  expected `sub`, a non-empty `sub`, or none when the caller applies a scope-dependent rule).

```mermaid
flowchart TD
    Parse["Parse compact JWS"] --> Issuer["Resolve iss through the profile"]
    Issuer --> Key["Select key by kid"]
    Key --> Signature["Verify ES256 signature"]
    Signature --> Document["Document rules: alg, required claims, exp and nbf with skew"]
    Document --> Profile["Profile rules: subject, jti, iat skew, lifetime ceiling"]
    Profile --> Caller["Caller: audience and scope decisions"]
```

The diagram shows the order: parse, issuer resolution, `kid` selection, signature, document rules,
and profile rules all yield a single `false`, so a caller cannot probe which rule failed. Every
profile requires `iss`, `sub`, `aud`, `exp`, `nbf`, `iat`, and `jti`, a non-blank `jti`, `iat` no
later than now plus the skew, `exp` after both `iat` and `nbf`, and `exp - iat` within the ceiling.
Audience is deliberately **not** part of the profile: resources distinguish an unauthenticated token
(401) from an authentic token for another audience (403), so they call
`token.HasAudience(...)` from IdentityModel.Token after validation, and LogSpace applies its `scope=telemetry`
rules between the two. The ECDSA key is created per validation and disposed before the call
returns, so `JsonWebKey` and the validator hold no disposable state; an unusable trusted key
rejects the token instead of throwing.

The stateless `Es256JsonWebTokenValidation` helper in this project's `shared/` folder holds these
rules in one source location (#1412). The public validator compiles that helper with its existing
per-call key ownership. Web.Hosting also compiles it and supplies an
`IJsonWebTokenSignatureVerifier` over a middleware-owned ECDSA handle while an explicit lifetime
owner is present. The helper still resolves the issuer, selects the trusted `kid`, enforces ES256,
and applies the same document and profile rules before accepting a token. Its caller must bind a
borrowed verifier to the selected trusted key and coordinate verification with disposal. The helper
has no mutable static state, no handle ownership, and no public API. Public validators retain their
existing contract and token acceptance relative to 10.0.0-preview.1.

The Cohesion claim names and ceilings (`cohesion_token_use`, `scope=telemetry`, `cohesion-export`,
24 hours, 8 hours) are not here: they belong to the resource credential seam, so they live in
`Assimalign.Cohesion.Hosting.Resources.ResourceCredentialProfile`, which this package does not
reference, and each call site passes them into its profile.

## Preview.1 compatibility — public implementations (#1465)

On owner decisions 59 and 15's preview terms, the factory facades shipped in
10.0.0-preview.1 become public sealed implementations with private constructors:

- `JsonWebTokenWriter.CreateEs256(key, kid)` returns `JsonWebTokenWriter` and
  `JsonWebTokenValidator.CreateEs256(profile)` returns `JsonWebTokenValidator`. These calls keep
  their spelling and are source-compatible, including Database's unchanged writer calls.
- `JsonWebTokenSignatureVerifier` is removed. Replace `CreateRsa` and `CreateEcdsa` calls with
  `RsaJsonWebTokenSignatureVerifier.Create` and `EcdsaJsonWebTokenSignatureVerifier.Create`.
- `JsonWebTokenValidator.HasAudience` is removed. Import IdentityModel.Token and use
  `token.HasAudience(audience)`. That extension rejects null and whitespace audiences; callers
  that previously accepted them retain ordinal matching locally before using the extension for
  nonblank values. The Web, IdentityHub, Scheduler and LogSpace verifiers use an ordinal audience
  list match for null/blank inputs, preserving false for null and exact blank-string matches.
  SecretStore and ConfigurationStore already reject blank inputs; the gateway uses a nonblank
  constant.

All these conversions are binary-breaking: creation return types change and the old signature
verifier facade and audience method disappear. Recompile consumers, applying the renamed calls
above. Existing interface contracts, key ownership, argument validation and token acceptance are
unchanged. No compatibility facade remains. Advisory: not applicable: API shape.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.IdentityModel.Token` and this package's
`shared/Es256JsonWebTokenValidation.cs` through `CohesionSharedSource`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/IdentityModel/README.md`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/src`.

- **Source** — `cohesion/libraries/IdentityModel/Assimalign.Cohesion.IdentityModel.Token.JsonWebToken/shared/Es256JsonWebTokenValidation.cs`.
