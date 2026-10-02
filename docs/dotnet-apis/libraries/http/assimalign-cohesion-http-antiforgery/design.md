# Assimalign.Cohesion.Http.Antiforgery design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.Antiforgery`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The signed double-submit model protects both halves so an injected cookie alone cannot authenticate
a request token. Antiforgery composes protocol, cookie, form, and forwarded-identity contracts
without adding these concerns to the HTTP root. Validation is an explicit server-side operation.
Pipeline enforcement is the Web layer's: `Assimalign.Cohesion.Web.Antiforgery` validates the unsafe
requests of endpoints that declare a requirement, reads the form for the form-token flow, and ships
the adapter from a `Security.DataProtection` protector to `IHttpAntiforgeryProtector`. This package
takes no data-protection dependency.

The cookie token is `Secure` whenever the request's effective scheme is HTTPS, so the
forwarded-headers middleware has to run before a handler stores the token behind a TLS-terminating
proxy; `CookieSecure` forces the flag on plaintext requests too. Until 2026-10-01 the flag followed
`CookieSecure` alone, which defaulted to off. A three-way policy was not added: that is a default
for every cookie, which `Web.CookiePolicy` owns as `CookiePolicyOptions.Secure`. The cookie token is
also essential by default (`CookieIsEssential`), because without it no unsafe request from a user
who has not answered a consent prompt could pass validation.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Http`, `Assimalign.Cohesion.Http.Cookies`,
`Assimalign.Cohesion.Http.Forms`, `Assimalign.Cohesion.Http.Forwarded`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src/Assimalign.Cohesion.Http.Antiforgery.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/README.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src`.
