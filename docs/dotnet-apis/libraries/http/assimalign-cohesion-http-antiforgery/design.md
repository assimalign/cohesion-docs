# Assimalign.Cohesion.Http.Antiforgery design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Http.Antiforgery`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The signed double-submit model protects both halves so an injected cookie alone cannot authenticate
a request token. Antiforgery composes protocol, cookie, and form contracts without adding these
concerns to the HTTP root. Validation is an explicit server-side operation.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Http`, `Assimalign.Cohesion.Http.Cookies`,
`Assimalign.Cohesion.Http.Forms`. The [overview](index.md#dependencies) distinguishes project
references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src/Assimalign.Cohesion.Http.Antiforgery.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Http/README.md`.

- **Source** — `cohesion/libraries/Http/Assimalign.Cohesion.Http.Antiforgery/src`.
