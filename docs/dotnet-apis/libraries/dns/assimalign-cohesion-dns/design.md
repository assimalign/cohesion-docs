# Assimalign.Cohesion.Dns design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Dns`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

Abstract client, resolver, authority, and transport bases centralize synchronous and asynchronous
disposal. Protocol values retain their wire identities, while `DnsException` supplies the family
error model. Concrete client and transport implementations live in descendant packages.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Core`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/src/Assimalign.Cohesion.Dns.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Dns/README.md`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/src`.
