# Security

Certificate loading and purpose-bound data protection.

[Libraries](../index.md)

## Packages

| Package | Responsibility | Reference |
|---|---|---|
| `Assimalign.Cohesion.Security` | Loads X.509 certificates through a platform-neutral contract. | [Overview](assimalign-cohesion-security/index.md) |
| `Assimalign.Cohesion.Security.DataProtection` | Protects purpose-scoped payloads with authenticated encryption and a rotating key ring. | [Overview](assimalign-cohesion-security-dataprotection/index.md) |

## Dependencies and delivery

The repository places this area in delivery wave 1. The wave is a dependency-ordering guide, not a
release or completeness guarantee.

The current projects are certificate loading and data protection. TLS connection transformation now
lives in the Connections area; the older area README still describes the previous layout.

| Assembly | Declared dependencies |
|---|---|
| `Assimalign.Cohesion.Security` | No explicit Cohesion or package reference in the project file |
| `Assimalign.Cohesion.Security.DataProtection` | No explicit Cohesion or package reference in the project file |

## Sources

- **Source** — `cohesion/libraries/README.md`.

- **Source** — `cohesion/README.md`.

- **Source** — `cohesion/docs/programs/DELIVERY_ROADMAP.md`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Security/README.md`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security/src/Assimalign.Cohesion.Security.csproj`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security/src`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/src/Assimalign.Cohesion.Security.DataProtection.csproj`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/src`.
