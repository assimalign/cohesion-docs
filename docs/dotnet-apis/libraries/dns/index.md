# Dns

Domain Name System (DNS) contracts, wire models, resolvers, and transport packages.

[Libraries](../index.md)

## Packages

| Package | Responsibility | Reference |
|---|---|---|
| `Assimalign.Cohesion.Dns` | Defines Domain Name System (DNS) contracts and wire-format types. | [Overview](assimalign-cohesion-dns/index.md) |
| `Assimalign.Cohesion.Dns.Client` | Provides stub and recursive resolution with UDP and TCP DNS transports. | [Overview](assimalign-cohesion-dns-client/index.md) |
| `Assimalign.Cohesion.Dns.Client.Doh` | Reserves the DNS-over-HTTPS transport package. | [Overview](assimalign-cohesion-dns-client-doh/index.md) |
| `Assimalign.Cohesion.Dns.Client.Doq` | Reserves the DNS-over-QUIC transport package. | [Overview](assimalign-cohesion-dns-client-doq/index.md) |
| `Assimalign.Cohesion.Dns.Client.Dot` | Reserves the DNS-over-TLS transport package. | [Overview](assimalign-cohesion-dns-client-dot/index.md) |

## Dependencies and delivery

The repository places this area in delivery wave 4. The wave is a dependency-ordering guide, not a
release or completeness guarantee.

| Assembly | Declared dependencies |
|---|---|
| `Assimalign.Cohesion.Dns` | `Assimalign.Cohesion.Core` (CohesionProjectReference) |
| `Assimalign.Cohesion.Dns.Client` | `Assimalign.Cohesion.Dns` (CohesionProjectReference) |
| `Assimalign.Cohesion.Dns.Client.Doh` | `Assimalign.Cohesion.Dns.Client` (CohesionProjectReference) |
| `Assimalign.Cohesion.Dns.Client.Doq` | `Assimalign.Cohesion.Dns.Client` (CohesionProjectReference) |
| `Assimalign.Cohesion.Dns.Client.Dot` | `Assimalign.Cohesion.Dns.Client` (CohesionProjectReference) |

## Sources

- **Source** — `cohesion/libraries/README.md`.

- **Source** — `cohesion/README.md`.

- **Source** — `cohesion/docs/programs/DELIVERY_ROADMAP.md`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Dns/README.md`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/src/Assimalign.Cohesion.Dns.csproj`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/src`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client/src/Assimalign.Cohesion.Dns.Client.csproj`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client/src`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client.Doh/src/Assimalign.Cohesion.Dns.Client.Doh.csproj`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client.Doh/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client.Doh/src`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client.Doq/src/Assimalign.Cohesion.Dns.Client.Doq.csproj`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client.Doq/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client.Doq/src`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client.Dot/src/Assimalign.Cohesion.Dns.Client.Dot.csproj`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client.Dot/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client.Dot/src`.
