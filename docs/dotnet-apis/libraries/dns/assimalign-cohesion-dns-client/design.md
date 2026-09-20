# Assimalign.Cohesion.Dns.Client design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Dns.Client`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

`StubDnsClient`, forwarding and iterative resolvers, and UDP/TCP transports implement the root
abstract contracts. The project has no separate overview or design record; its implementation and
tests define the current resolver behavior. Encrypted transports are separate placeholder projects.

There is no separate project DESIGN.md in this checkout. The project file, source, and area
documentation are the available design evidence.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Dns`. The [overview](index.md#dependencies)
distinguishes project references, package references, shared source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client/src/Assimalign.Cohesion.Dns.Client.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Dns/README.md`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client/src`.
