# Assimalign.Cohesion.Dns.Client.Doh design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Dns.Client.Doh`.

> **Status:** Not yet implemented.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The overview describes a future adapter onto the HTTP client factory, but the package has no
implementation. Its requirements document is a proposed contract and must not be treated as a
callable API.

There is no separate project DESIGN.md in this checkout. The overview records the package intent;
the project and source define its current shape.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Dns.Client`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client.Doh/src/Assimalign.Cohesion.Dns.Client.Doh.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client.Doh/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Dns/README.md`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client.Doh/src`.
