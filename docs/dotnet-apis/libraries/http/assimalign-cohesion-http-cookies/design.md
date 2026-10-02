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
package; the one serializer that reads `IHttpResponseCookieFeature` directly is the HTTP/1.1
upgrade writer in `Http.ProtocolUpgrade`.

A replacement response feature (signed cookies, encrypted cookies, a cookie policy) removes the
existing feature's slot before installing itself, because the feature collection is keyed by
`IHttpFeature.Name` and `Get<T>()` returns the first match. It must also keep `Set-Cookie` in sync,
typically by queuing into the collection of the feature it replaces, as `Web.CookiePolicy` does.
Cookie-aware packages depend on this one, and this one depends only on the protocol core.

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
