# Assimalign.Cohesion.Security.DataProtection design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Security.DataProtection`.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

AES-256-GCM protects versioned payloads and HKDF-SHA256 derives purpose-specific subkeys. Lazy
rotation keeps retired keys available during a grace window. `IKeyRepository` separates persistence
from protection, allowing nodes to share a configured key directory.

## Dependency boundary

The project declares no explicit Cohesion or external package references. Shared repository build
properties still apply.

## Sources

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/src/Assimalign.Cohesion.Security.DataProtection.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Security/README.md`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security.DataProtection/src`.
