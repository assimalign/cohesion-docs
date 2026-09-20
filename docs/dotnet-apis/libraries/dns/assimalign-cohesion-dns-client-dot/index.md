# Assimalign.Cohesion.Dns.Client.Dot

Reserves the DNS-over-TLS transport package.

> **Status:** Not yet implemented.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Dns](../index.md)

## Scope

The package has no transport implementation. The overview and requirements reserve a future TLS DNS
adapter, so callers cannot infer a working encrypted transport from the assembly name.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Dns.Client`](../../dns/assimalign-cohesion-dns-client/index.md) | `CohesionProjectReference` |

## Principal public types

No implemented public type declarations were found in the project's retained source files.

## Sources

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client.Dot/src/Assimalign.Cohesion.Dns.Client.Dot.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client.Dot/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Dns/README.md`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client.Dot/src`.
