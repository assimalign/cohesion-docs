# Assimalign.Cohesion.Security design

Design decisions and ownership boundaries for `Assimalign.Cohesion.Security`.

> **Status:** Partial.

[Overview](index.md) · [Examples](examples/index.md)

## Design and boundaries

The implementation delegates PKCS#12 and PEM loading to base-class-library certificate APIs.
Argument errors remain argument errors; load failures use `CertificateException`. Operating-system
certificate stores, generation, and thumbprint lookup are explicitly outside this iteration.

## Dependency boundary

The project declares no explicit Cohesion or external package references. Shared repository build
properties still apply.

## Sources

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security/src/Assimalign.Cohesion.Security.csproj`.

- **Source** — `cohesion/libraries/Directory.Build.props`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security/docs/OVERVIEW.md`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security/docs/DESIGN.md`.

- **Source** — `cohesion/libraries/Security/README.md`.

- **Source** — `cohesion/libraries/Security/Assimalign.Cohesion.Security/src`.
