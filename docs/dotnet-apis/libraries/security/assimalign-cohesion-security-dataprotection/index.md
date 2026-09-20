# Assimalign.Cohesion.Security.DataProtection

Protects purpose-scoped payloads with authenticated encryption and a rotating key ring.

## Reference

- **[Overview](index.md)** — purpose, dependencies, and principal types.

- **[Design](design.md)** — decisions and extension boundaries.

- **[Examples](examples/index.md)** — source-backed usage and tests.

[Security](../index.md)

## Scope

AES-256-GCM protects versioned payloads and HKDF-SHA256 derives purpose-specific subkeys. Lazy
rotation keeps retired keys available during a grace window. `IKeyRepository` separates persistence
from protection, allowing nodes to share a configured key directory.

## Dependencies

The project file declares no explicit Cohesion project or external package references.

## Principal public types

| Type | Source file |
|---|---|
| `DataProtectionException` | `src/Exceptions/DataProtectionException.cs` |
| `DataProtectionProvider` | `src/DataProtectionProvider.cs` |
| `IDataProtectionProvider` | `src/Abstractions/IDataProtectionProvider.cs` |
| `IDataProtector` | `src/Abstractions/IDataProtector.cs` |
| `IKey` | `src/Abstractions/IKey.cs` |
| `IKeyRepository` | `src/Abstractions/IKeyRepository.cs` |
| `KeyRepository` | `src/KeyRepository.cs` |
| `DataProtectionOptions` | `src/DataProtectionOptions.cs` |
| `DataProtectionProviderExtensions` | `src/Extensions/DataProtectionProviderExtensions.cs` |
| `KeyDocument` | `src/KeyDocument.cs` |

## Sources

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/src/Assimalign.Cohesion.Security.DataProtection.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Security/README.md`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/src`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/src/Exceptions/DataProtectionException.cs`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/src/DataProtectionProvider.cs`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/src/Abstractions/IDataProtectionProvider.cs`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/src/Abstractions/IDataProtector.cs`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/src/Abstractions/IKey.cs`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/src/Abstractions/IKeyRepository.cs`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/src/KeyRepository.cs`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/src/DataProtectionOptions.cs`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/src/Extensions/DataProtectionProviderExtensions.cs`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/src/KeyDocument.cs`.
