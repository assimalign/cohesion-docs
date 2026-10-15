# Assimalign.Cohesion.Dns.Client.Dot design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Dns.Client.Dot`.

> **Status:** Not yet implemented.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The package has no transport implementation. The overview and requirements reserve a future TLS DNS
adapter, so callers cannot infer a working encrypted transport from the assembly name.

There is no separate project DESIGN.md in this checkout. The overview records the package intent;
the project and source define its current shape.

## Dependency boundary

The declared build inputs are `Assimalign.Cohesion.Dns.Client`. The
[overview](index.md#dependencies) distinguishes project references, package references, shared
source, and analyzer inputs.

## Sources

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client.Dot/src/Assimalign.Cohesion.Dns.Client.Dot.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client.Dot/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Dns/README.md`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client.Dot/src`.
