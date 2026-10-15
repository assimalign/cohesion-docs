# Assimalign.Cohesion.Dns

Defines Domain Name System (DNS) contracts and wire-format types.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Dns](../index.md)

## Scope

Abstract client, resolver, authority, and transport bases centralize synchronous and asynchronous
disposal. Protocol values retain their wire identities, while `DnsException` supplies the family
error model. Concrete client and transport implementations live in descendant packages.

## Dependencies

| Reference | Build item |
|---|---|
| [`Assimalign.Cohesion.Core`](../../core/assimalign-cohesion-core/index.md) | `CohesionProjectReference` |

## Principal public types

| Type | Source file |
|---|---|
| `DnsAaaaRecord` | `src/Records/DnsAaaaRecord.cs` |
| `DnsARecord` | `src/Records/DnsARecord.cs` |
| `DnsAuthority` | `src/DnsAuthority.cs` |
| `DnsClass` | `src/DnsClass.cs` |
| `DnsClient` | `src/DnsClient.cs` |
| `DnsCnameRecord` | `src/Records/DnsCnameRecord.cs` |
| `DnsEdnsClientSubnetOption` | `src/Edns/DnsEdnsClientSubnetOption.cs` |
| `DnsEdnsCookieOption` | `src/Edns/DnsEdnsCookieOption.cs` |
| `DnsEdnsExtendedErrorOption` | `src/Edns/DnsEdnsExtendedErrorOption.cs` |
| `DnsEdnsFlags` | `src/Edns/DnsEdnsFlags.cs` |
| `DnsEdnsOption` | `src/Edns/DnsEdnsOption.cs` |
| `DnsEdnsOptionCode` | `src/Edns/DnsEdnsOptionCode.cs` |
| `DnsEdnsUnknownOption` | `src/Edns/DnsEdnsUnknownOption.cs` |
| `DnsErrorCode` | `src/Exceptions/DnsErrorCode.cs` |
| `DnsException` | `src/Exceptions/DnsException.cs` |
| `DnsHeader` | `src/DnsHeader.cs` |

## Sources

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/src/Assimalign.Cohesion.Dns.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Dns/README.md`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/src`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/src/Records/DnsAaaaRecord.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/src/Records/DnsARecord.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/src/DnsAuthority.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/src/DnsClass.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/src/DnsClient.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/src/Records/DnsCnameRecord.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/src/Edns/DnsEdnsClientSubnetOption.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/src/Edns/DnsEdnsCookieOption.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/src/Edns/DnsEdnsExtendedErrorOption.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/src/Edns/DnsEdnsFlags.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/src/Edns/DnsEdnsOption.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/src/Edns/DnsEdnsOptionCode.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/src/Edns/DnsEdnsUnknownOption.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/src/Exceptions/DnsErrorCode.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/src/Exceptions/DnsException.cs`.

- **Source** — `cohesion/libraries/Dns/Assimalign.Cohesion.Dns/src/DnsHeader.cs`.
