# Assimalign.Cohesion.Security

Loads X.509 certificates through a platform-neutral contract.

> **Status:** Partial.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Security](../index.md)

## Scope

The implementation delegates PKCS#12 and PEM loading to base-class-library certificate APIs.
Argument errors remain argument errors; load failures use `CertificateException`. Operating-system
certificate stores, generation, and thumbprint lookup are explicitly outside this iteration.

## Dependencies

The project file declares no explicit Cohesion project or external package references.

## Principal public types

| Type | Source file |
|---|---|
| `CertificateException` | `src/Exceptions/CertificateException.cs` |
| `CertificateManagerFactory` | `src/CertificateManagerFactory.cs` |
| `ICertificateManager` | `src/Abstractions/ICertificateManager.cs` |

## Sources

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security/src/Assimalign.Cohesion.Security.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Security/README.md`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security/src`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security/src/Exceptions/CertificateException.cs`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security/src/CertificateManagerFactory.cs`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security/src/Abstractions/ICertificateManager.cs`.
