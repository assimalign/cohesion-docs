# Assimalign.Cohesion.Dns.Client

Provides stub and recursive resolution with UDP and TCP DNS transports.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Dns](../index.md)

## Scope

`StubDnsClient`, forwarding and iterative resolvers, and UDP/TCP transports implement the root
abstract contracts. The project has no separate overview or design record; its implementation and
tests define the current resolver behavior. Encrypted transports are separate placeholder projects.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Dns`](../../dns/assimalign-cohesion-dns/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `DnsRootHints` | `src/DnsRootHints.cs` |
| `ForwardingDnsResolver` | `src/ForwardingDnsResolver.cs` |
| `ForwardingDnsResolverOptions` | `src/ForwardingDnsResolverOptions.cs` |
| `IterativeDnsResolver` | `src/IterativeDnsResolver.cs` |
| `IterativeDnsResolverOptions` | `src/IterativeDnsResolverOptions.cs` |
| `StubDnsClient` | `src/StubDnsClient.cs` |
| `StubDnsClientOptions` | `src/StubDnsClientOptions.cs` |
| `TcpDnsTransport` | `src/TcpDnsTransport.cs` |
| `TcpDnsTransportOptions` | `src/TcpDnsTransportOptions.cs` |
| `UdpDnsTransport` | `src/UdpDnsTransport.cs` |
| `UdpDnsTransportOptions` | `src/UdpDnsTransportOptions.cs` |

## Sources

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client/src/Assimalign.Cohesion.Dns.Client.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Dns/README.md`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client/src`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client/src/DnsRootHints.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client/src/ForwardingDnsResolver.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client/src/ForwardingDnsResolverOptions.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client/src/IterativeDnsResolver.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client/src/IterativeDnsResolverOptions.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client/src/StubDnsClient.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client/src/StubDnsClientOptions.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client/src/TcpDnsTransport.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client/src/TcpDnsTransportOptions.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client/src/UdpDnsTransport.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns.Client/src/UdpDnsTransportOptions.cs`.
