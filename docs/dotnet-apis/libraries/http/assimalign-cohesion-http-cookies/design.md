# Assimalign.Cohesion.Http.Cookies design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.Cookies`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Cookies are opt-in features over the wire-level `Cookie` and `Set-Cookie` fields. Extension members
retain property-style access while keeping cookie types out of the protocol root. The request side
tokenizes the `Cookie` header the first time `request.Cookies` is read. The response collection
writes every change through to `response.Headers[Set-Cookie]`, one value per cookie, and the
transports serialize that header like any other field, honoring the RFC 6265 rule that each
`Set-Cookie` value goes on its own line. `Http.Connections` therefore takes no dependency on this
package, and neither does the HTTP/1.1 upgrade writer in `Http.ProtocolUpgrade`, which writes the
`101` head's `Set-Cookie` header the same way. No serializer reads `IHttpResponseCookieFeature`. The
upgrade writer was the last one: it wrote the header's values comma-folded on one line and then
each cookie of the feature again, so a `101` sent every cookie twice. Since #1154 it writes the
header the way `Http1MessageWriter` does and takes no dependency on this package.

A replacement response feature (signed cookies, encrypted cookies, a cookie policy) removes the
existing feature's slot before installing itself, because the feature collection is keyed by
`IHttpFeature.Name` and `Get<T>()` returns the first match. It must also keep `Set-Cookie` in sync,
typically by queuing into the collection of the feature it replaces, as `Web.CookiePolicy` does;
whatever the header holds is what every response, the `101` included, sends.
Cookie-aware packages depend on this one, and this one depends only on the protocol core.

## Parse-side size limits and lifetime cap

**Configurable parse-side size limits (`HttpCookieLimits`).**
`HttpCookieCollection` parsing enforces RFC 6265bis-aligned defaults:
name+value ≤ 4096 octets, per-attribute value ≤ 1024 octets, and a
bounded retained-attribute count (default 50, a DoS backstop that
keeps a hostile `Set-Cookie` from growing the attribute/extension list
without bound). Oversized cookies and attributes are **ignored**
(dropped) rather than throwing, matching the RFC's parsing-robustness
posture — a malformed inbound header must not crash the server. The
limits are a constructor parameter (`HttpCookieLimits.Default` when
omitted), so a host can tune or effectively disable a dimension
(`int.MaxValue`). The outer bound on total header length stays with
the transport's request/response header-size limits; these are the
per-cookie inner defense.

**Lifetime cap mechanism (`HttpCookie.ClampLifetime`).** The model
supplies the RFC 6265bis §5.5 400-day constant
(`HttpCookieLimits.DefaultMaxLifetime`) and the *pure* clamping math:
`ClampLifetime(referenceTime[, maxLifetime])` returns a cookie whose
`Max-Age` is reduced to the cap when longer and whose `Expires` is
pulled back to `referenceTime + cap` when further out, while a zero or
negative `Max-Age` (a deletion signal, RFC 6265 §5.2.2) and a
past/near `Expires` round-trip untouched. Clamping is a pure function
with no hidden clock: **deciding when to apply it** — at emission,
against the current time — is an emission/policy responsibility, so
the mechanism lives here and the "apply the cap to outbound cookies"
trigger is left to the emitter / `Web.CookiePolicy`. Keeping the clock
out of the model is what makes the cap deterministically testable.

## Attribute grammar

`HttpCookie` validates its name against the RFC 6265 §4.1.1 `token` grammar and its value against
`*cookie-octet` in the constructor, so a hostile value can never split or corrupt the emitted
`Set-Cookie` line. The `Path` and `Domain` options are held to the same standard (#1153).
`HttpCookie.ToString()` appends both values verbatim, so before #1153 a path of
`/x; SameSite=None` added an attribute to the line, and a control character corrupted it.
Applications build these values from untrusted input (a tenant segment, a forwarded host), and
`Web.CookiePolicy` trusts them, so the model has to refuse them.

- **`Path`** must be an RFC 6265 §4.1.1 `path-value`: US-ASCII with no control character
  (`0x00`–`0x1F`, `0x7F`) and no `;`. RFC 6265bis writes the same set as
  `av-octet = %x20-3A / %x3C-7E`. The empty string is in the grammar.
- **`Domain`** must be a `domain-value`: an RFC 1034 §3.5 `subdomain` as enhanced by RFC 1123 §2.1.
  That is dot-separated labels of 1–63 letters, digits and hyphens that neither start nor end with a
  hyphen, at most 253 characters in all. One leading dot is tolerated because a user agent ignores
  it (RFC 6265 §5.2.3), and it does not count toward the 253. An empty label, a trailing dot, an
  underscore, a port, an IPv6 literal and a non-ASCII name are refused; an internationalized name is
  written as its `xn--` A-label.
- **`null` stays legal for both**, because `CookiePolicyRules` repairs a `__Host-` cookie with
  `Domain = null` and the collection parser starts from `Path = null`. An empty `Domain` also means
  "no attribute", as `ToString()` has always read it, and as RFC 6265 §5.2.3 tells a user agent to
  treat an empty `Domain=`.
- **The setters throw `ArgumentException`** whose `ParamName` and message name the attribute. The
  message does not echo the value, which may hold the control character that made it invalid. Like
  the name and value checks, this is a server bug surfacing, not a request rejection.
- **Validation is in the setter**, so every reader sees only valid values: the copy constructor
  (which copies the checked fields without checking them again), `HttpCookie`'s copy of its
  options, and a later write to `cookie.Options`.
- **The parser drops a cookie** whose `Path` or `Domain` attribute is outside the grammar, as it
  drops an out-of-grammar name or value, instead of throwing on wire input. It does not keep the
  cookie without the attribute: re-emitted that way, the cookie would have a different scope from
  the one its sender wrote. An empty `Domain=` is the exception; it is ignored, per §5.2.3.
- **What it does not cover.** `Extensions` are still appended verbatim; each one is an attribute by
  design. The transports' head writers refuse CR, LF and the other control characters in any field
  value at encode time (#1183), whatever builds the value.

The grammar guard (`HttpCookieGrammar`) classifies characters with `SearchValues<char>` membership
scans and character-range scans: no regex, no reflection.

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

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/src/Assimalign.Cohesion.Http.Cookies.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/README.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Cookies/src`.
