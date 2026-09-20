# Assimalign.Cohesion.Dns.Client.Doq design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Dns.Client.Doq`.

> **Status:** Not yet implemented.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The project fixes the assembly's place in the DNS family but contains no transport code.
Requirements describe future protocol and authentication integration; no delivered QUIC DNS behavior
follows from the package's existence.

There is no separate project DESIGN.md in this checkout. The overview records the package intent;
the project and source define its current shape.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Dns.Client`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client.Doq/src/Assimalign.Cohesion.Dns.Client.Doq.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client.Doq/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Dns/README.md`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client.Doq/src`.
