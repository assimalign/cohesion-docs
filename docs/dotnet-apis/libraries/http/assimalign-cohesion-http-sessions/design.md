# Assimalign.Cohesion.Http.Sessions design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.Sessions`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

`IHttpSession` exposes byte-array values, keys, availability, and explicit load/commit operations.
Typed string and integer helpers are extension members over that binary contract. An attached
feature supplies the session without adding application state to the protocol core.

`HttpSessionOptions.CookieIsEssential` defaults to `false` because a session usually holds state
that needs consent; under a consent requirement the session cookie, and so the session, starts only
after the user consents unless the application marks it essential.

`HttpSessionOptions.CookiePath` must be an RFC 6265 `path-value`: US-ASCII with no `;` and no
control characters. The session cookie is written through the cookie model, whose
`HttpCookieOptions.Path` refuses any other path (#1153), and the cookie is built each time a session
is established, so `UseSessions` in `Assimalign.Cohesion.Web.Sessions` checks the path when it
registers the middleware: a path such as `/tenant;x` or `/café` fails the application at
composition instead of answering every session establishment with a 500.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Http`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/src/Assimalign.Cohesion.Http.Sessions.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Sessions/src`.
